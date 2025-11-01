using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEditor.Model;

namespace TextEditor.Command
{
    internal class PasteCommand : ICommand
    {
        private TextEditorModel _textEditor;
        public PasteCommand(TextEditorModel textEditor)
        {
            _textEditor = textEditor;
        }
        public void Execute()
        {
            _textEditor.Paste();
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
