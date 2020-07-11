using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sample_1.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyService.Cqrs.Events;

namespace Sample_1.EventHandler
{
   

    public class ProductEventHandler : IEventHandler<CreateProductEvent>,
                                       IEventHandler<ProductActivateEvent>
    {
        readonly ILogger<ProductEventHandler> logger;

        readonly IInMemeoryRepository _inMemeoryRepository;

        public ProductEventHandler(ILogger<ProductEventHandler> logger, IInMemeoryRepository inMemeoryRepository)
        {
            this.logger = logger;

            this._inMemeoryRepository = inMemeoryRepository;
        }

        public Task HandleAsync(CreateProductEvent @event)
        {
            
            var count=this._inMemeoryRepository.GetAll().Count();
            logger.LogInformation("============新建商品=======================");
            this.logger.LogInformation($"创建:{@event.Name}");
            logger.LogInformation("count:{0}", count);
            logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(@event));
            logger.LogInformation("===================================");
            return Task.CompletedTask;
        }

        public Task HandleAsync(ProductActivateEvent @event)
        {
            var product = this._inMemeoryRepository.Get(@event.Id);

            logger.LogInformation($"============商品:{product.Id}已激活状态=======================");
            logger.LogInformation($"激活商品:{product.Name}--status:{product.Status}");
            logger.LogInformation(JsonConvert.SerializeObject(product));
            logger.LogInformation("===================================");

            return Task.CompletedTask;
        }
    }
}
