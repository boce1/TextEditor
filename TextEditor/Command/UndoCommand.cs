using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEditor.Memento;
using TextEditor.Model;

namespace TextEditor.Command
{
    internal class UndoCommand : ICommand
    {
        private TextEditorModel _textEditor;
        private readonly HistoryManager _history;

        public UndoCommand(TextEditorModel textEditor, HistoryManager history)
        {
            _textEditor = textEditor;
            _history = history;
        }
        public void Execute()
        {
            Undo();
        }

        public void Redo()
        {
            UndoRedoHelper.Redo(_textEditor, _history);
        }

        public void Undo()
        {
            UndoRedoHelper.Undo(_textEditor, _history);
        }
    }
}
