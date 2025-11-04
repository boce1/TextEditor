using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TextEditor.Command;
using TextEditor.Memento;
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
        private Label _fileLabel;
        private HistoryManager _historyManager;
        private bool areLeftRightKeysPressed = false;

        private int HighlighPosition;
        public TextEditorController(TextBox editorBox, Label stateLabel, Label caretLabel, Label fileLabel)
        {
            _model = new TextEditorModel(new InsertState());
            _editorBox = editorBox;
            _stateLabel = stateLabel;
            _caretLabel = caretLabel;
            _fileLabel = fileLabel;
            _historyManager = new HistoryManager();
            _command = new AddTextCommand(_model, _model.Text, _historyManager);
            _editorBox.Focus();
        }

        /*--------------------------------------------------*/
        /*Keyboard events*/
        // Handles regular text characters (letters, numbers, symbols)
        public void EditorBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int caret = _editorBox.CaretIndex;
            UpdateCaretPossition();
            _command = new AddTextCommand(_model, e.Text, _historyManager);
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

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                HandleCtrlShortcuts(e, caret);
            }
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                HandleShiftControls(e);
            }
            HandleSpecialKeys(e, caret);
        }

        private void HandleShiftControls(KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Down || e.Key == Key.Up) // check if these keys are pressed, because pressing other keys move the caret
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
                        areLeftRightKeysPressed = true;
                        break;
                    case Key.Right:
                        UpdateCaretAddedChar(_editorBox.CaretIndex);
                        _model.SelectionEnd = _editorBox.CaretIndex;
                        HighlighPosition = _model.SelectionStart;
                        areLeftRightKeysPressed = true;
                        break;
                    case Key.Down:
                    case Key.Up:
                        e.Handled = true;
                        UpdateCaretVertically(e.Key == Key.Up);
                        _model.SelectionEnd = _editorBox.CaretIndex;
                        HighlighPosition = _model.SelectionStart;
                        break;
                }

                e.Handled = true;
                return;
            }
        }

        private void HandleCtrlShortcuts(KeyEventArgs e, int caret)
        {
            switch (e.Key)
            {
                case Key.C: // ctrl + C
                    e.Handled = true; // Disable default copy
                    Copy();
                    break;

                case Key.X: // ctrl + X
                    e.Handled = true; // Disable default cut
                    Cut(caret);
                    break;

                case Key.V: // ctrl + V
                    e.Handled = true; // Disable default paste
                    Paste(caret);
                    break;
                case Key.Z: // ctrl + Z
                    e.Handled = true; // Disable default undo
                    Undo();
                    break;
                case Key.Y: // ctrl + Y
                    e.Handled = true; // Disable default redo
                    Redo();
                    break;
                case Key.R: // ctrl + R
                    e.Handled = true; // Disable default
                    ChangeToReadOnly();
                    break;
                case Key.I: // ctrl + I
                    e.Handled = true; // Disable default
                    ChangeToInsert();
                    break;
                case Key.O: // ctrl + O
                    e.Handled = true; // Disable default
                    ChangeToOverwrite();
                    break;
                case Key.S: // ctrl + S
                    e.Handled = true; // Disable default
                    _command = new SaveCommand(_model);
                    _command.Execute();
                    break;
                default:
                    // Other Ctrl+key combos disabled
                    e.Handled = true;
                    break;
            }
            return;
        }

        private void HandleSpecialKeys(KeyEventArgs e, int caret)
        {
            switch (e.Key)
            {
                case Key.Back:
                    _command = new DeleteTextCommand(_model, _historyManager);
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
                        UpdateCaretDeletedChar(caret);
                    }
                    break;

                case Key.Enter:
                    _command = new AddTextCommand(_model, "\n", _historyManager);
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

                    break;

                case Key.Space:
                    _command = new AddTextCommand(_model, " ", _historyManager);
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
                    break;
                case Key.Delete:
                    e.Handled = true;
                    if (_model.CaretPosition < _model.Text.Length)
                    {
                        UpdateCaretAddedChar(caret);
                        _command = new DeleteTextCommand(_model, _historyManager);
                        _command.Execute();
                        UpdateEditorBox();
                        if (_model.getState() is HighlightState)
                        {
                            _editorBox.CaretIndex = HighlighPosition;
                            UpdateCaretPossition();
                            UpdateStatusBar();
                            _model.changeState(new InsertState());
                        }
                        else
                        {
                            _editorBox.CaretIndex = caret;
                            UpdateCaretPossition();
                        }
                    }
                    break;
                case Key.Left:
                    if (!areLeftRightKeysPressed)
                    {
                        UpdateCaretDeletedChar(_editorBox.CaretIndex);
                        UpdateStatusBar();
                    }
                    break;
                case Key.Right:
                    if (!areLeftRightKeysPressed)
                    {
                        UpdateCaretAddedChar(_editorBox.CaretIndex);
                        UpdateStatusBar();
                    }
                    break;
                case Key.Up:
                case Key.Down:
                    e.Handled = true;
                    UpdateCaretVertically(e.Key == Key.Up);
                    break;
            }
            if ((e.Key == Key.Left || e.Key == Key.Right) && !areLeftRightKeysPressed)
            {
                _model.SelectionStart = _model.SelectionEnd = _model.CaretPosition;
                //if((_model.getState() is not InsertState) && (_model.getState() is not OverwriteState))
                if (_model.getState() is HighlightState)
                {
                    _model.changeState(new InsertState());
                    UpdateStatusBar();
                }
                e.Handled = true;
                return;
            }
            areLeftRightKeysPressed = false;
        }

        /*--------------------------------------------------*/
        /*Mouse events*/
        // When mouse is pressed — begin selection
        public void EditorBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_model.getState() is HighlightState) // if user choose not to highlight but to move the caret
            {
                _model.changeState(new InsertState());
                UpdateStatusBar();
            }

            //_editorBox.Text += " ";

            int clickPos = _editorBox.GetCharacterIndexFromPoint(e.GetPosition(_editorBox), true);
            if (clickPos >= 0 && clickPos < _editorBox.Text.Length && _editorBox.Text[clickPos] != '\n')
            {
                clickPos++;
            }
            HighlighPosition = clickPos;

            _model.SelectionStart = clickPos;
            _model.SelectionEnd = clickPos;

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
            if (movePos >= 0 && movePos < _editorBox.Text.Length && _editorBox.Text[movePos] != '\n')
            {
                movePos++;
            }
            HighlighPosition = Math.Min(movePos, HighlighPosition);
            if (movePos < 0) return;

            _model.SelectionEnd = movePos;
            if(_model.SelectionStart > _model.SelectionEnd)
            {
                _model.SelectionEnd--;
                HighlighPosition = Math.Max(0, _model.SelectionEnd);
            }
            _model.SelectionStart = Math.Min(_model.SelectionStart, _model.Text.Length);
            _model.SelectionEnd = Math.Min(_model.SelectionEnd, _model.Text.Length);

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
            _editorBox.SelectionStart = _editorBox.SelectionStart;
            _editorBox.SelectionLength = _model.SelectedText.Length;

            if (_model.getState() is not HighlightState) // if user choose not to highlight but to move the caret
            {
                //_model.changeState(new InsertState());
                UpdateStatusBar();
            }
            e.Handled = true;
        }
        /*--------------------------------------------------*/

        /*--------------------------------------------------*/
        /*Update view and caret functions*/
        private void UpdateEditorBox()
        {
            _editorBox.Text = _model.Text;
            _editorBox.Focus();
        }

        private void UpdateCaretPossition()
        {
            _editorBox.CaretIndex = Math.Min(_editorBox.CaretIndex, _editorBox.Text.Length);
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
        private void UpdateCaretVertically(bool moveUp)
        {
            int oldCaret = _editorBox.CaretIndex;
            _editorBox.Text += " ";
            _editorBox.CaretIndex = oldCaret;
            int currentLine = _editorBox.GetLineIndexFromCharacterIndex(_editorBox.CaretIndex);
            int targetLine = moveUp ? currentLine - 1 : currentLine + 1;

            // Ensure it stays within range
            if (targetLine < 0 || targetLine >= _editorBox.LineCount)
                return;

            // How many characters from the start of this line to where the caret currently is
            int column = _editorBox.CaretIndex - _editorBox.GetCharacterIndexFromLineIndex(currentLine);

            // Move to the same column on the target line (clamped to its length)
            int targetLineStart = _editorBox.GetCharacterIndexFromLineIndex(targetLine);
            int targetLineLength = _editorBox.GetLineLength(targetLine) - 1;

            int newIndex = targetLineStart + Math.Min(column, targetLineLength);

            // Update model + view
            UpdateEditorBox();
            _editorBox.CaretIndex = newIndex;


            UpdateCaretPossition();
            UpdateStatusBar();

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
                    stateString += $" [{_model.SelectionStart}-{_model.SelectionEnd}]";
                } else
                {
                    stateString += $" []";
                }
            }
            _stateLabel.Content = stateString;
            _stateLabel.Foreground = textColor;

            if(_model.FilePath != null)
            {
                _fileLabel.Content = $"File: {_model.FilePath}";
            } else
            {
                _fileLabel.Content = "File: Untitled";
            }

            _caretLabel.Content = $"Caret: {_editorBox.CaretIndex}";
            
        }
        /*--------------------------------------------------*/

        /*--------------------------------------------------*/
        /* Menu commands */
        public void OpenFileMenu_Click(object sender, RoutedEventArgs e)
        {
            _command = new OpenCommand(_model, _historyManager);
            _command.Execute();
            UpdateEditorBox();
            UpdateStatusBar();
        }

        public void SaveFileMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_model.FilePath != null)
            {
                _command = new SaveCommand(_model, false);
                _command.Execute();
                UpdateStatusBar();
            }
        }

        public void SaveAsFileMenu_Click(object sender, RoutedEventArgs e)
        {
            _command = new SaveCommand(_model, true);
            _command.Execute();
            UpdateStatusBar();
        }

        public void CopyMenu_Click(object sender, RoutedEventArgs e)
        {
            Copy();
        }

        public void PasteMenu_Click(object sender, RoutedEventArgs e)
        {
            Paste(_editorBox.CaretIndex);
        }

        public void CutMenu_Click(object sender, RoutedEventArgs e)
        {
            Cut(_editorBox.CaretIndex);
        }

        public void ReadOnlyMenu_Click(object sender, RoutedEventArgs e)
        {
            ChangeToReadOnly();
        }

        public void InsertMenu_Click(object sender, RoutedEventArgs e)
        {
            ChangeToInsert();
        }

        public void OverwriteMenu_Click(object sender, RoutedEventArgs e)
        {
            ChangeToOverwrite();
        }

        internal void UndoMenu_Click(object sender, RoutedEventArgs e)
        {
            _command = new UndoCommand(_model, _historyManager);
            _command.Execute();
            UpdateEditorBox();
            UpdateStatusBar();
            _editorBox.CaretIndex = _model.CaretPosition;
        }

        internal void RedoMenu_Click(object sender, RoutedEventArgs e)
        {
            _command = new RedoCommand(_model, _historyManager);
            _command.Execute();
            UpdateEditorBox();
            UpdateStatusBar();
            _editorBox.CaretIndex = _model.CaretPosition;
        }
        /*--------------------------------------------------*/

        /*--------------------------------------------------*/
        /* Copy paste cut undo redo funtions */
        private void Copy()
        {
            _command = new CopyCommand(_model, _historyManager);
            _command.Execute();
            if (_model.getState() is HighlightState)
            {
                _model.changeState(new InsertState());
            }
            _editorBox.Focus();
        }

        private void Cut(int caret)
        {
            _command = new CutCommand(_model, _historyManager);
            _command.Execute();
            UpdateEditorBox();
            _editorBox.CaretIndex = caret;
            UpdateCaretPossition();
            if (_model.getState() is HighlightState)
            {
                _model.changeState(new InsertState());
            }
            _editorBox.Focus();
        }

        private void Paste(int caret)
        {
            _command = new PasteCommand(_model, _historyManager);
            _command.Execute();
            UpdateEditorBox();
            _editorBox.CaretIndex = caret + Clipboard.GetText().Length;
            UpdateCaretPossition();
            if (_model.getState() is HighlightState)
            {
                _model.changeState(new InsertState());
            }
            UpdateStatusBar();
            _editorBox.Focus();
        }

        private void Undo()
        {
            _command.Undo();
            UpdateEditorBox();
            _editorBox.CaretIndex = _model.CaretPosition;
            isSelecting = false;
            UpdateStatusBar();
            _editorBox.Focus();
        }

        private void Redo()
        {
            _command.Redo();
            UpdateEditorBox();
            _editorBox.CaretIndex = _model.CaretPosition;
            isSelecting = false;
            UpdateStatusBar();
            UpdateStatusBar();
            _editorBox.Focus();
        }

        private void ChangeToReadOnly()
        {
            _model.changeState(new ReadOnlyState());
            UpdateStatusBar();
            _editorBox.Focus();
        }

        private void ChangeToInsert()
        {
            _model.changeState(new InsertState());
            UpdateStatusBar();
            _editorBox.Focus();
        }

        private void ChangeToOverwrite()
        {
            _model.changeState(new OverwriteState());
            UpdateStatusBar();
            _editorBox.Focus();
        }
        /*--------------------------------------------------*/
    }
}
