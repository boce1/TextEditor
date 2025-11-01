using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEditor.Model;


namespace TextEditor.Command
{
    internal class DeleteTextCommand : ICommand
    {
        private TextEditorModel _textEditor;
        private int _caret;

        public DeleteTextCommand(TextEditorModel textEditor, int caretPosition)
        {
            _textEditor = textEditor;
            _caret = caretPosition;
        }

        public void Execute()
        {
            _textEditor.Delete();
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
