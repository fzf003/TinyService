using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Sample_1.Domain
{
    public interface IIdGenerator
    {
        int Next();
    }
    public class IdGenerator: IIdGenerator
    {
        static int Id = 0;

        public int Next()
        {
            return Interlocked.Increment(ref Id);
        }
    }
}
