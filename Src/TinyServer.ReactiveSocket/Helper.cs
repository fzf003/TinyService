using System;
using System.Collections.Generic;
using System.Text;

namespace TinyServer.ReactiveSocket
{
    public class Helper
    {
        public static void IgnoreException(Action action)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
            }
        }
    }
}
