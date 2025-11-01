using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEditor.Controler;
using TextEditor.Model;

namespace TextEditor.Command
{
    internal class CopyCommand : ICommand
    {
        private TextEditorModel _textEditor;
        public CopyCommand(TextEditorModel textEditor)
        {
            _textEditor = textEditor;
        }
        public void Execute()
        {
            _textEditor.Copy();
        }

        public void Undo()
        {
            throw new NotImplementedException();
        }
    }
}
