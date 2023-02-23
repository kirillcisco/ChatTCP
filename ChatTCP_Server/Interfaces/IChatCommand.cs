using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatTCP
{
    // depricated now, target to delete
    internal interface IChatCommand
    {
        bool Executable();
        void Execute();
    }
}
