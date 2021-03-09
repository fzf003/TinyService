using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TinyService.Logging
{
    public static class Log
    {
        private static ILoggerFactory _loggerFactory;

        public static void SetLoggerFactory(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public static ILogger CreateLogger(string categoryName)
        {
            return _loggerFactory.CreateLogger(categoryName);
        }

        public static ILogger CreateLogger<T>()
        {
            return _loggerFactory.CreateLogger(typeof(T));
        }
    }

}
