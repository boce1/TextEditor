using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TextEditor.Command;
using TextEditor.Model;
using TextEditor.State;
using ICommand = TextEditor.Command.ICommand;

namespace TextEditor.Controler
{
    internal class TextEditorController
    {
        private TextEditorModel _model;
        private TextBox _editorBox;
        private ICommand _command;
        private bool isSelecting = false;
        private Label _stateLabel;
        private Label _caretLabel;

        private int HighlighPosition;
        public TextEditorController(TextBox editorBox, Label stateLabel, Label caretLabel)
        {
            _model = new TextEditorModel(new InsertState());
            _editorBox = editorBox;
            _stateLabel = stateLabel;
            _caretLabel = caretLabel;
            _command = new AddTextCommand(_model, _model.Text);
            _editorBox.Focus();
        }
        // Handles regular text characters (letters, numbers, symbols)
        public void EditorBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int caret = _editorBox.CaretIndex;
            UpdateCaretPossition();
            _command = new AddTextCommand(_model, e.Text);
            _command.Execute();
            UpdateEditorBox();
            e.Handled = true;
            if (_model.getState() is HighlightState)
            {
                _editorBox.CaretIndex = HighlighPosition + 1;
                UpdateCaretPossition();
                UpdateStatusBar();
                _model.changeState(new InsertState());
            }
            else
            {
                UpdateCaretAddedChar(caret);
            }
            UpdateStatusBar();
        }

        // Handles special keys like Backspace or Enter
        public void EditorBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            int caret = _editorBox.CaretIndex;
            UpdateCaretPossition();
            // To do add swich and return at the end 
            switch (e.Key)
            {
                case Key.Back:
                    _command = new DeleteTextCommand(_model, _editorBox.CaretIndex);
                    _command.Execute();
                    UpdateEditorBox();
                    e.Handled = true;
                    if(_model.getState() is HighlightState)
                    {
                        _editorBox.CaretIndex = HighlighPosition;
                        UpdateCaretPossition();
                        UpdateStatusBar();
                        _model.changeState(new InsertState());
                    } else
                    {
                        UpdateCaretDeletedChar(caret);
                    }
                    return;

                case Key.Enter:
                    _command = new AddTextCommand(_model, "\n");
                    _command.Execute();
                    UpdateEditorBox();
                    e.Handled = true;
                    if (_model.getState() is HighlightState)
                    {
                        _editorBox.CaretIndex = HighlighPosition;
                        UpdateCaretPossition();
                        UpdateStatusBar();
                        _model.changeState(new InsertState());
                    }
                    else
                    {
                        UpdateCaretAddedChar(caret);
                    }
                    
                    return;

                case Key.Space:
                    _command = new AddTextCommand(_model, " ");
                    _command.Execute();
                    UpdateEditorBox();
                    e.Handled = true;
                    if (_model.getState() is HighlightState)
                    {
                        _editorBox.CaretIndex = HighlighPosition + 1;
                        UpdateCaretPossition();
                        UpdateStatusBar();
                    }
                    else
                    {
                        UpdateCaretAddedChar(caret);
                    }
                    return;
            }

            // Disable Default Copy and Paste commands and Handle them in controller
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.C:
                        e.Handled = true; // Disable default copy
                        _command = new CopyCommand(_model);
                        _command.Execute();
                        if(_model.getState() is HighlightState)
                        {
                            _model.changeState(new InsertState());
                        }
                        break;

                    case Key.X:
                        e.Handled = true; // Disable default cut
                        _command = new CutCommand(_model);
                        _command.Execute();
                        UpdateEditorBox();
                        _editorBox.CaretIndex = caret;
                        UpdateCaretPossition();
                        if (_model.getState() is HighlightState)
                        {
                            _model.changeState(new InsertState());
                        }
                        break;

                    case Key.V:
                        e.Handled = true; // Disable default paste
                        _command = new PasteCommand(_model);
                        _command.Execute();
                        UpdateEditorBox();
                        _editorBox.CaretIndex = caret + Clipboard.GetText().Length;
                        UpdateCaretPossition();
                        if (_model.getState() is HighlightState)
                        {
                            _model.changeState(new InsertState());
                        }
                        UpdateStatusBar();
                        break;

                    default:
                        // Other Ctrl+key combos can go here if needed
                        break;
                }

                // Disable Ctrl + Arrow keys
                if (e.Key == Key.Up || e.Key == Key.Down)
                {
                    e.Handled = true;
                }
                return;
            }
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                if (e.Key == Key.Left || e.Key == Key.Right)
                {
                    if (_model.getState().GetType() != typeof(HighlightState))
                    {
                        _model.SelectionStart = _model.SelectionEnd = _editorBox.CaretIndex;
                        _model.changeState(new HighlightState());
                        HighlighPosition = _editorBox.CaretIndex;
                    }

                    switch (e.Key)
                    {
                        case Key.Left:
                            UpdateCaretDeletedChar(_editorBox.CaretIndex);
                            _model.SelectionEnd = _editorBox.CaretIndex;
                            HighlighPosition = _model.SelectionEnd;
                            break;

                        case Key.Right:
                            UpdateCaretAddedChar(_editorBox.CaretIndex);
                            _model.SelectionEnd = _editorBox.CaretIndex;
                            HighlighPosition = _model.SelectionStart;
                            break;
                    }
                    
                    UpdateStatusBar();
                   
                    
                    UpdateEditorBox();
                    //_editorBox.SelectionStart = Math.Min(_model.SelectionStart, _model.SelectionEnd);
                    //_editorBox.SelectionLength = Math.Abs(_model.SelectionEnd - _model.SelectionStart);
                    e.Handled = true;
                    return;
                }
            }

            // If user presses arrow without shift, clear selection
            switch (e.Key) 
            {
                case Key.Left:
                    UpdateCaretDeletedChar(_editorBox.CaretIndex);
                    UpdateStatusBar();
                    break;
                case Key.Right:
                    UpdateCaretAddedChar(_editorBox.CaretIndex);
                    UpdateStatusBar();
                    break;
            }
            if (e.Key == Key.Left || e.Key == Key.Right)
            {
                _model.SelectionStart = _model.SelectionEnd = _model.CaretPosition;
                //if((_model.getState() is not InsertState) && (_model.getState() is not OverwriteState))
                if(_model.getState() is HighlightState)
                {
                    _model.changeState(new InsertState());
                    UpdateStatusBar();
                }
                e.Handled = true;
                return;
            }
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                e.Handled = true;
            }

        }

        // When mouse is pressed — begin selection
        public void EditorBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            if(_model.getState() is HighlightState) // if user choose not to highlight but to move the caret
            {
                _model.changeState(new InsertState());
                UpdateStatusBar();
            }
            _editorBox.Text += " "; // to be able to select the last char
            int clickPos = _editorBox.GetCharacterIndexFromPoint(e.GetPosition(_editorBox), true);
            HighlighPosition = clickPos;

            _model.SelectionStart = clickPos;
            _model.SelectionEnd = clickPos;
            _model.CaretPosition = clickPos;

            isSelecting = true;

            _editorBox.CaretIndex = clickPos;
            UpdateCaretPossition();
            e.Handled = true;
        }

        // When mouse moves — extend selection
        public void EditorBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!isSelecting) return;
            _model.changeState(new HighlightState());
            int movePos = _editorBox.GetCharacterIndexFromPoint(e.GetPosition(_editorBox), true);
            HighlighPosition = Math.Min(movePos, HighlighPosition);
            if (movePos < 0) return;

            _model.SelectionEnd = movePos;

            
            // Visually show selection in the TextBox
            _editorBox.SelectionStart = Math.Min(_model.SelectionStart, _model.SelectionEnd);
            _editorBox.SelectionLength = Math.Abs(_model.SelectionEnd - _model.SelectionStart);
            UpdateStatusBar();
            e.Handled = true;

        }

        // When mouse released — stop selecting
        public void EditorBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            isSelecting = false;
            int oldCaret = _editorBox.CaretIndex;
            
            UpdateEditorBox(); // text in tex box is now without extra space at the end
            
            _editorBox.CaretIndex = oldCaret;
            UpdateCaretPossition();
            _editorBox.SelectionStart = HighlighPosition;
            _editorBox.SelectionLength = _model.SelectedText.Length;

            if (_model.getState() is not HighlightState) // if user choose not to highlight but to move the caret
            {
                //_model.changeState(new InsertState());
                UpdateStatusBar();
            }
        }

        public void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            _model.changeState(new InsertState());
            UpdateStatusBar();
            _editorBox.Focus();
        }

        public void ReadOnlyButton_Click(object sender, RoutedEventArgs e)
        {
            _model.changeState(new ReadOnlyState());
            UpdateStatusBar();
            _editorBox.Focus();
        }

        public void OverwriteButton_Click(object sender, RoutedEventArgs e)
        {
            _model.changeState(new OverwriteState());
            UpdateStatusBar();
            _editorBox.Focus();
        }

        private void UpdateEditorBox()
        { 
            _editorBox.Text = _model.Text;
            _editorBox.Focus();
        }

        private void UpdateCaretPossition()
        {
            _model.CaretPosition = _editorBox.CaretIndex;
        }

        private void UpdateCaretAddedChar(int caret) // needs the parameter in case TextBox gets updated
        {
            _editorBox.CaretIndex = caret + 1;
            UpdateCaretPossition();
            UpdateStatusBar();
        }

        private void UpdateCaretDeletedChar(int caret)
        {
            if (caret > 0)
            {
                _editorBox.CaretIndex = caret - 1;
                UpdateCaretPossition();
                UpdateStatusBar();
            }
        }

        private void UpdateStatusBar()
        {
            Brush textColor = Brushes.Black;
            string stateString = $"State: {_model.getState().GetType().Name.Replace("State", "")}";
            if (_model.getState() is HighlightState)
            {
                textColor = Brushes.Red;
                if (_model.SelectionStart != _model.SelectionEnd)
                {
                    stateString += $"\t[{_model.SelectionStart} - {_model.SelectionEnd}]";
                } else
                {
                    stateString += $"\t[]";
                }
            }
            _stateLabel.Content = stateString;
            _stateLabel.Foreground = textColor;
            _caretLabel.Content = $"Caret: {_editorBox.CaretIndex}";
        }
    }
}
