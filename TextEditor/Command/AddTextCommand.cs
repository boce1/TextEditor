using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TextEditor.Memento;
using TextEditor.Model;
using TextEditor.State;

namespace TextEditor.Command
{
    internal class AddTextCommand : ICommand
    {
        private TextEditorModel _textEditor;
        private string _text;
        private TextEditorMemento? _before;
        private readonly HistoryManager _history;

        public AddTextCommand(TextEditorModel textEditor, string text, HistoryManager history)
        {
            _textEditor = textEditor;
            _text = text;
            _history = history;
        }

        public void Execute()
        {
            _before = _textEditor.Save();
            _textEditor.Type(_text);
            if(_textEditor.getState() is not ReadOnlyState)
            {
                _history.Save(_before);
            }
        }
        public void Undo()
        {
            UndoRedoHelper.Undo(_textEditor, _history);
        }

        public void Redo()
        {
            UndoRedoHelper.Redo(_textEditor, _history);
        }
    }
}
