using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TextEditor.Model;

namespace TextEditor.Command
{
    internal class SaveCommand : ICommand
    {
        private readonly TextEditorModel _textEditor;
        private readonly bool _isSaveAs;

        public SaveCommand(TextEditorModel textEditor, bool isSaveAs = false)
        {
            _textEditor = textEditor;
            _isSaveAs = isSaveAs;
        }

        public void Execute()
        {
            try
            {
                if (_isSaveAs || string.IsNullOrEmpty(_textEditor.FilePath))
                {
                    SaveFileDialog saveDialog = new SaveFileDialog
                    {
                        Title = "Save Text File",
                        Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                        DefaultExt = ".txt",
                        AddExtension = true,
                        FileName = "NewFile"
                    };

                    bool? result = saveDialog.ShowDialog();

                    if (result == true)
                    {
                        _textEditor.FilePath = saveDialog.FileName;
                    }
                    else
                    {
                        return; // user canceled
                    }
                }

                File.WriteAllText(_textEditor.FilePath, _textEditor.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file:\n{ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Save doesn’t change editor state so these are intentionally left empty
        public void Undo() { }

        public void Redo() { }
    }
}
