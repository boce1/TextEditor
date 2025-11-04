using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEditor.Memento;
using TextEditor.Model;

namespace TextEditor.Command
{
    internal class RedoCommand : ICommand
    {
        private TextEditorModel _textEditor;
        private readonly HistoryManager _history;

        public RedoCommand(TextEditorModel textEditor, HistoryManager history)
        {
            _textEditor = textEditor;
            _history = history;
        }
        public void Execute()
        {
            Redo();
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
