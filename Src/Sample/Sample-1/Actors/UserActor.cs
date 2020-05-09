using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TinyService.Core;

namespace Sample_1.Actors
{
    public class UserActor: ProcessActor
    {
        readonly IActorFactory actorFactory;

        readonly ILogger<UserActor> logger;

        public UserActor(IActorFactory actorFactory, ILoggerFactory loggerFactory)
        {
            this.actorFactory = actorFactory;

            this.logger = loggerFactory.CreateLogger<UserActor>();
        }

        public override void Handle(Started message)
        {
            logger.LogInformation($"Actor:{Self.ToShortString()} is Start!!");
        }

        public void Handle(string message)
        {
            logger.LogInformation($"Receied:{message}");
        }
    }
 }
