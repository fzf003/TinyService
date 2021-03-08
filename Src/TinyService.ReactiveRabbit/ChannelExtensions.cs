using RabbitMQ.Client;
using System.Collections.Generic;

namespace TinyService.ReactiveRabbit
{
    public static class ChannelExtensions
    {
        /// <summary>
        /// 定义延迟队列Exchange
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="exchangetype"></param>
        public static void DefineDelayExchange(this IModel channel, string exchangetype = "direct", string exchangename = "", bool durable = true)
        {
            ///声明交换机
            /////ExchangeType.Direct
            var exchangeargs = new Dictionary<string, object>
                            {
                                {"x-delayed-type", exchangetype}
                            };

            channel.ExchangeDeclare(exchange: exchangename, type: "x-delayed-message", durable: true, arguments: exchangeargs);
        }
        /// <summary>
        /// 发送延迟消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="exchangename">交换机</param>
        /// <param name="delayseconds">延迟时间单位(秒)</param>
        /// <param name="message">消息</param>
        public static void SendDelayMessage(this IModel channel,byte[] message, string exchangename = "", int delayseconds = 5)
        {
            var props = channel.CreateBasicProperties();

            props.Headers = new Dictionary<string, object>
                {
                    {"x-delay", delayseconds*1000}
                };

            channel.BasicPublish(exchange: exchangename, routingKey: string.Empty, basicProperties: props, body: message);
        }
    }
}
