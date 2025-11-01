using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEditor.Model;

namespace TextEditor.State
{
    internal class ReadOnlyState : IState
    {
        private TextEditorModel _textEditor;
        public void setContext(TextEditorModel textEditor)
        {
            _textEditor = textEditor;
        }

        public void Type(string input)
        {
            // ignore typing
        }

        public void Delete()
        {
            // ignore deleting
        }

        public void Copy()
        {
            // copy is only handled in highlight state
        }

        public void Paste()
        {
            // pasting is only handled in insert and overwrite states
        }

        public void Cut()
        {
            // cut is only handled in highlight state
        }
    }
}
