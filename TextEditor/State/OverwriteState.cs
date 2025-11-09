using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TextEditor.Model;

namespace TextEditor.State
{
    internal class OverwriteState : IState
    {
        TextEditorModel _textEditor;

        public void setContext(TextEditorModel textEditor)
        {
            _textEditor = textEditor;
        }

        public void Type(string input)
        {
            // Split text into two parts
            string before = _textEditor.Text.Substring(0, _textEditor.CaretPosition); // to the caret
            int pos = Math.Min(_textEditor.Text.Length, _textEditor.CaretPosition + input.Length);
            string after = _textEditor.Text.Substring(pos); // rest of text
            _textEditor.Text = before + input + after; 
        }

        public void Delete()
        {
            _textEditor.changeState(new InsertState());
            _textEditor.Delete();
        }

        public void Copy()
        {
            // copy is only handled in highlight state
        }

        public void Paste()
        {
            string pasteText = Clipboard.GetText();
            _textEditor.Type(pasteText);
            _textEditor.changeState(new InsertState());
        }

        public void Cut()
        {
            // cut is only handled in highlight state
        }
    }
}
