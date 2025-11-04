using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEditor.Memento;
using TextEditor.State;

namespace TextEditor.Model
{
    internal class TextEditorModel
    {
        private IState _state;
        public string Text { get; set; } = "";
        public int CaretPosition { get; set; } = 0;
        public int SelectionStart { get; set; } = 0;
        public int SelectionEnd { get; set; } = 0;
        public bool HasSelection => SelectionStart != SelectionEnd;
        public string SelectedText => HasSelection // readonly property is calculated everytime its accessed
            ? Text.Substring(Math.Min(SelectionStart, SelectionEnd),
                             Math.Abs(SelectionEnd - SelectionStart))
            : string.Empty;
        public string? FilePath { get; set; } = null;

        public TextEditorModel(IState state)
        {
            _state = state;
            _state.setContext(this);
        }

        public IState getState()
        {
            return _state;
        }

        public void changeState(IState newState)
        {
            _state = newState;
            _state.setContext(this);
        }

        public void Type(string input)
        {
            _state.Type(input);
        }

        public void Delete()
        {
            _state.Delete();
        }

        public void Copy()
        {
            _state.Copy();
        }

        public void Paste()
        {
            _state.Paste();
        }

        public void Cut()
        {
            _state.Cut();
        }

        public TextEditorMemento Save()
        {
            return new TextEditorMemento(Text, CaretPosition, SelectionStart, SelectionEnd, _state, FilePath);
        }

        public void Restore(TextEditorMemento memento)
        {
            Text = memento.Text;
            CaretPosition = memento.CaretPosition;
            SelectionStart = memento.SelectionStart;
            SelectionEnd = memento.SelectionEnd;
            _state = memento.State;
            FilePath = memento.FilePath;
        }
    }
}
