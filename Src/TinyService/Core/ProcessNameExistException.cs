using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinyService.Core
{
    public class ProcessNameExistException : Exception
    {
        public ProcessNameExistException(string name)
            : base(name)
        {
        }
    }
}
