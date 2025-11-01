using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TextEditor.Model;
using TextEditor.State;

namespace TextEditor.Command
{
    internal class AddTextCommand : ICommand
    {
        private TextEditorModel _textEditor;
        private string _text;

        public AddTextCommand(TextEditorModel textEditor, string text)
        {
            _textEditor = textEditor;
            _text = text;
        }

        public void Execute()
        {
            _textEditor.Type(_text);
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
