using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEditor.Memento;
using TextEditor.Model;

namespace TextEditor.Command
{
    internal static class UndoRedoHelper
    {
        public static void Redo(TextEditorModel textEditor, HistoryManager history)
        {
            var current = textEditor.Save();
            var next = history.Redo(current);
            if (next != null)
                textEditor.Restore(next);
        }

        public static void Undo(TextEditorModel textEditor, HistoryManager history)
        {
            var current = textEditor.Save();
            var previous = history.Undo(current);
            if (previous != null)
                textEditor.Restore(previous);
        }
    }
}
