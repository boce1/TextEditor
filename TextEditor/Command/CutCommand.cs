using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEditor.Model;

namespace TextEditor.Command
{
    internal class CutCommand : ICommand
    {
        TextEditorModel _textEditor;
        public CutCommand(TextEditorModel textEditor)
        {
            _textEditor = textEditor;
        }
        public void Execute()
        {
            _textEditor.Cut();
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
