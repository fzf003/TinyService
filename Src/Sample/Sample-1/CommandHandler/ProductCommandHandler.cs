using Microsoft.Extensions.Logging;
using Sample_1.Domain;
using Sample_1.EventHandler;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TinyService.Cqrs.Commands;
using TinyService.Cqrs.Events.Dispatchers;

namespace Sample_1.CommandHandler
{
    public class ProductCommandHandler : ICommandHandler<CreateProductCommand>,
                                         ICommandHandler<ProductActivateCommand>
    {
        
        readonly ILogger<ProductCommandHandler> logger;

        readonly IInMemeoryRepository repository;

        readonly IIdGenerator idGenerator;

        readonly IEventDispatcher eventDispatcher;

        readonly IInMemeoryRepository _inMemeoryRepository;

        public ProductCommandHandler(ILogger<ProductCommandHandler> logger, IInMemeoryRepository repository, IIdGenerator idGenerator, IEventDispatcher eventDispatcher, IInMemeoryRepository inMemeoryRepository)
        {
            this.logger = logger;

            this.repository = repository;

            this.idGenerator = idGenerator;

            this.eventDispatcher = eventDispatcher;

            _inMemeoryRepository = inMemeoryRepository;
        }

        public async Task HandleAsync(CreateProductCommand command)
        {
            var Id=this.idGenerator.Next();

            var product= Product.Create(Id, command.Name, command.Category, command.Description, command.ImageFile, command.Price);

            this.repository.Save(product);

            var @event = new CreateProductEvent(id: product.Id, name: product.Name, category: product.Category, summary: product.Summary, description: product.Description, imageFile: product.ImageFile, price: product.Price, status: ProductStatus.Draft);

            await this.eventDispatcher.PublishAsync(@event);

          
        }

        public Task HandleAsync(ProductActivateCommand command)
        {
           var product= _inMemeoryRepository.Get(command.Id);
            product.Activate();
            this._inMemeoryRepository.Save(product);
            this.eventDispatcher.PublishAsync(new ProductActivateEvent(product.Id, product.CreateTime, product.Name));

            return Task.CompletedTask;
        }
    }
}
