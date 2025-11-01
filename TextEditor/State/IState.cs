using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEditor.Model;

namespace TextEditor.State
{
    internal interface IState
    {
        public void setContext(TextEditorModel textEditor);
        public void Type(string input);
        public void Delete();
        public void Copy();
        public void Paste();
        public void Cut();

    }
}
