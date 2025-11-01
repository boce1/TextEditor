using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditor.Command
{
    internal class CommandManager
    {
        private readonly Stack<ICommand> _history = new();

        public void Execute(ICommand command)
        {
            command.Execute();
            _history.Push(command);
        }

        public void Undo()
        {
            if (_history.Count > 0)
                _history.Pop().Undo();
        }
    }
}
