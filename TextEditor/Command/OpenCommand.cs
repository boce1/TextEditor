using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TextEditor.Memento;
using TextEditor.Model;
using TextEditor.State;

namespace TextEditor.Command
{
    internal class OpenCommand : ICommand
    {
        private readonly TextEditorModel _textEditor;
        private readonly HistoryManager _history;
        private TextEditorMemento? _before;

        public OpenCommand(TextEditorModel textEditor, HistoryManager history)
        {
            _textEditor = textEditor;
            _history = history;
        }

        public void Execute()
        {
            try
            {
                // Save the current state before opening
                _before = _textEditor.Save();

                OpenFileDialog openDialog = new OpenFileDialog
                {
                    Title = "Open Text File",
                    Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
                };

                bool? result = openDialog.ShowDialog();

                if (result == true)
                {
                    string path = openDialog.FileName;
                    string content = File.ReadAllText(path);

                    _textEditor.FilePath = path;
                    _textEditor.Text = content;

                    // Record to history only if not in read-only mode
                    if (_textEditor.getState() is not ReadOnlyState)
                    {
                        _history.Save(_before);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening file:\n{ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Undo()
        {
            UndoRedoHelper.Undo(_textEditor, _history);
        }

        public void Redo()
        {
            UndoRedoHelper.Redo(_textEditor, _history);
        }
    }
}
