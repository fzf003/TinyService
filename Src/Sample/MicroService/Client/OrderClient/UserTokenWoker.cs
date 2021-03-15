using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderClient
{
    public class UserTokenWoker : BackgroundService
    {
        readonly ILogger<UserTokenWoker> logger;

        readonly OrderClientService orderClientService;

        readonly IDistributedCache distributedCache;

        public UserTokenWoker(OrderClientService orderClientService, ILogger<UserTokenWoker> logger, IDistributedCache distributedCache)
        {
            this.orderClientService = orderClientService;

            this.logger = logger;

            this.distributedCache = distributedCache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var token = await this.orderClientService.GettokenAsync("fzf003", "admin");

                    this.logger.LogInformation("获取新Token:{0}--{1}", token, DateTime.Now.ToString());

                    await this.distributedCache.SetStringAsync("fzf003", token.Token, new DistributedCacheEntryOptions
                    {

                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)

                    });

                   

                }catch(Exception ex)
                {
                    logger.LogError(ex.Message);
                }

                await Task.Delay(1000 * 30);

            }

           
        }
    }
}
