using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEditor.Memento;
using TextEditor.Model;
using TextEditor.State;

namespace TextEditor.Command
{
    internal class PasteCommand : ICommand
    {
        private TextEditorModel _textEditor;
        private TextEditorMemento? _before;
        private readonly HistoryManager _history;
        public PasteCommand(TextEditorModel textEditor, HistoryManager history)
        {
            _textEditor = textEditor;
            _history = history;
        }
        public void Execute()
        {
            _before = _textEditor.Save();
            _textEditor.Paste();
            if (_textEditor.getState() is not ReadOnlyState)
            {
                _history.Save(_before);
            }
        }
        public void Undo()
        {
            if(_history.CanUndo)
            {
                _before = _history.Undo(_before!);
                _textEditor.Restore(_before!);
            }
        }

        public void Redo()
        {
            if(_history.CanRedo)
            {
                _before = _history.Redo(_before!);
                _textEditor.Restore(_before!);
            }
        }
    }
}
