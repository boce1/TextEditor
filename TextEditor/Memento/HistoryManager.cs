using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.Memento
{
    internal class HistoryManager
    {
        private readonly Stack<TextEditorMemento> _undoStack = new();
        private readonly Stack<TextEditorMemento> _redoStack = new();

        public void Save(TextEditorMemento memento)
        {
            _undoStack.Push(memento);
            _redoStack.Clear(); // clear redo when new action happens
        }

        public TextEditorMemento? Undo(TextEditorMemento current)
        {
            if (CanUndo)
            {
                var last = _undoStack.Pop();
                _redoStack.Push(current);
                return last;
            }
            return null;
        }

        public TextEditorMemento? Redo(TextEditorMemento current)
        {
            if (CanRedo)
            {
                var next = _redoStack.Pop();
                _undoStack.Push(current);
                return next;
            }
            return null;
        }

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;
    }
}
