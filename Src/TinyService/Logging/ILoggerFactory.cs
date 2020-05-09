using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinyService.Logging
{
    public interface ILoggerFactory
    {

        ILogger Create(string name);

        ILogger Create(Type type);
    }
}
