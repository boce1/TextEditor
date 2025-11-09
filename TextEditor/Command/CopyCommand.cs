using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEditor.Controler;
using TextEditor.Memento;
using TextEditor.Model;
using TextEditor.State;

namespace TextEditor.Command
{
    internal class CopyCommand : ICommand
    {
        private TextEditorModel _textEditor;
        private TextEditorMemento? _before;
        private readonly HistoryManager _history;
        public CopyCommand(TextEditorModel textEditor, HistoryManager history)
        {
            _textEditor = textEditor;
            _history = history;
        }
        public void Execute()
        {
            _before = _textEditor.Save();
            _textEditor.Copy();
            if (_textEditor.SelectedText != string.Empty)
            {
                _history.Save(_before);
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
