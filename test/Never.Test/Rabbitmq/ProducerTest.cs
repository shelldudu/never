using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Never;
using Never.Messages;
using Never.RabbitMQ;
using ProtoBuf;

namespace Never.TestConsole.Rabbitmq
{
    /// <summary>
    ///
    /// </summary>
    public class ProducerTest
    {
        [Xunit.Fact]
        public void TestSendMessage()
        {
            var action = new MessageRouteProducer(new MessageConnection()
            {
                ConnetctionString = MessageConnection.Sample_Route,
            },
            new MessageRoute()
            {
                Exchange = "hello2",
                ExchangeType = MessageRouteExchangeType.Topic,
                QueueName = "world2",
                RoutingKey = string.Empty,
            });

            action.Start();
            action.Send(new MessagePacket()
            {
                ContentType = "text",
                Body = "hello world2"
            });
        }

        [Xunit.Fact]
        public void TestReceiveMessage()
        {
            var action = new MessageRouteConsumer(new MessageConnection()
            {
                ConnetctionString = MessageConnection.Sample_Route,
            },
            new MessageRoute()
            {
                Exchange = "hello0",
                ExchangeType = MessageRouteExchangeType.Fanout,
                QueueName = "world0",
                RoutingKey = string.Empty,
                Durable = true
            }, new Never.ProtoBuf.ProtoBufSerializer());

            action.Start();
            action.ReceiveAsync((t) =>
            {
                Console.WriteLine(t.Body);
            });
        }

        [Xunit.Fact]
        public void TestSerTestMessage()
        {
            var msg = new TestMessage()
            {
                Id = 9,
                Name = "adg",
                Other = "ete"
            };

            var packet = MessagePacket.UseJson(msg);

            var b = new Never.Serialization.BinarySerializer().Serialize(packet);

            var b2 = new Never.ProtoBuf.MessageSerializer().Serialize(packet);

            var tt = new Never.ProtoBuf.MessageSerializer().Deserialize<MessagePacket>(b2);
        }
    }
}