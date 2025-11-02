using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEditor.State;

namespace TextEditor.Memento
{
    internal class TextEditorMemento
    {
        public string Text { get; }
        public int CaretPosition { get; }
        public int SelectionStart { get; }
        public int SelectionEnd { get; }
        public IState State { get; }

        public TextEditorMemento(string text, int caret, int selStart, int selEnd, IState state)
        {
            Text = text;
            CaretPosition = caret;
            SelectionStart = selStart;
            SelectionEnd = selEnd;
            State = state;
        }
    }
}
