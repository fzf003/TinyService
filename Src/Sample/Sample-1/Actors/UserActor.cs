using Microsoft.Extensions.Logging;
using Sample_1.CommandHandler;
using Sample_1.EventHandler;
using System;
using System.Collections.Generic;
using System.Text;
using TinyService.Core;
using TinyService.Cqrs.Commands.Dispatchers;
using TinyService.Cqrs.Events.Dispatchers;

namespace Sample_1.Actors
{
    public class UserActor: ProcessActor
    {
        readonly IActorFactory actorFactory;

        readonly ILogger<UserActor> logger;

        readonly ICommandDispatcher commandDispatcher;

        public UserActor(IActorFactory actorFactory, ILoggerFactory loggerFactory, ICommandDispatcher commandDispatcher)
        {
            this.actorFactory = actorFactory;

            this.logger = loggerFactory.CreateLogger<UserActor>();

            this.commandDispatcher = commandDispatcher;
        }

        public override void Handle(Started message)
        {
            logger.LogInformation($"Actor:{Self.ToShortString()} is Start!!");
        }

        public void Handle(CreateProductCommand message)
        {
            logger.LogInformation($"Receied:{message}");

            this.commandDispatcher.SendAsync(message);
        }

        public void Handle(ProductActivateCommand message)
        {
            this.commandDispatcher.SendAsync(message);
        }
    }
 }
