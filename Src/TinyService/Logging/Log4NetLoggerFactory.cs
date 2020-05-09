using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Config;
using log4net.Appender;
using System.IO;
using log4net.Layout;
using log4net;

namespace TinyService.Logging
{
    public class Log4NetLoggerFactory : ILoggerFactory
    {
       private static readonly System.Reflection.Assembly _callingAssembly = typeof(Log4NetLoggerFactory).Assembly;

        public Log4NetLoggerFactory()
            : this("log4net.config")
        {

        }

        public Log4NetLoggerFactory(string configFile = "log4net.config")
        {
            var file = new FileInfo(configFile);
            if (!file.Exists)
            {
                file = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile));
            }

            

           if (file.Exists)
            {
                XmlConfigurator.ConfigureAndWatch(LogManager.GetRepository(_callingAssembly),file);
                 
            }
            else
            {
                 XmlConfigurator.Configure(LogManager.GetRepository(_callingAssembly), file);
            }
        }

        public ILogger Create(string name)
        {
        
            return new Log4NetLogger(LogManager.GetLogger(_callingAssembly,name));
        }

        public ILogger Create(Type type)
        {
         
            return new Log4NetLogger(LogManager.GetLogger(type));
        }
    }
}
