using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyServer.ReactiveSocket;

namespace SocketServer
{
    public class PrintMessageHandle : IMessageHandle<string>
    {
        readonly ILoggerFactory loggerFactory;

        readonly ILogger<PrintMessageHandle> _logger;

        public PrintMessageHandle(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;

            this._logger = loggerFactory.CreateLogger<PrintMessageHandle>();
        }

        public void OnCompleted()
        {
            _logger.LogInformation(" Done...");
        }

        public void OnError(Exception ex)
        {
            _logger.LogInformation(" Error:{0}", ex.Message);
        }

        public void OnMessage(string message)
        {
            _logger.LogInformation(message);
        }

        
    }
}
