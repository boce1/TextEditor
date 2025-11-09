using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TextEditor.Model;

namespace TextEditor.State
{
    internal class HighlightState : IState
    {
        TextEditorModel _textEditor;

        public void setContext(TextEditorModel textEditor)
        {
            _textEditor = textEditor;
        }

        public void Type(string input)
        {
            if (!_textEditor.HasSelection)
                return;

            int start = Math.Min(_textEditor.SelectionStart, _textEditor.SelectionEnd);
            int end = Math.Max(_textEditor.SelectionStart, _textEditor.SelectionEnd);

            // Replace selected text with input
            string before = _textEditor.Text.Substring(0, start);
            string after = _textEditor.Text.Substring(Math.Min(end, _textEditor.Text.Length));

            _textEditor.Text = before + input + after;

            // Move caret to end of inserted text
            _textEditor.CaretPosition = start + input.Length;

            // Clear selection after typing
            _textEditor.SelectionStart = _textEditor.CaretPosition = _textEditor.SelectionEnd;
            _textEditor.changeState(new InsertState());
        }

        public void Delete()
        {
            if (!_textEditor.HasSelection)
                return;

            int start = Math.Min(_textEditor.SelectionStart, _textEditor.SelectionEnd);
            int end = Math.Max(_textEditor.SelectionStart, _textEditor.SelectionEnd);

            int count = Math.Min(end - start, _textEditor.Text.Length - start);
            _textEditor.Text = _textEditor.Text.Remove(start, count);
            _textEditor.CaretPosition = start;
            _textEditor.SelectionStart = _textEditor.SelectionEnd = start;

            _textEditor.changeState(new InsertState());
        }

        public void Copy()
        {
            if (!string.IsNullOrEmpty(_textEditor.SelectedText))
            {
                Clipboard.SetText(_textEditor.SelectedText);
            }
            _textEditor.changeState(new InsertState());
        }

        public void Paste()
        {
            Delete();
            _textEditor.changeState(new InsertState());
            _textEditor.Type(Clipboard.GetText());
        }

        public void Cut()
        {
            if (!_textEditor.HasSelection)
                return;

            Copy();
            Delete();
            _textEditor.changeState(new InsertState());
        }
    }
}
