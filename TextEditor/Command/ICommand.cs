using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEditor.Model;

namespace TextEditor.Command
{
    internal interface ICommand
    {
        public void Execute();
        public void Undo();
        public void Redo();
    }
}
