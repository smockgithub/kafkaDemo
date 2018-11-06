using Confluent.Kafka;
using System;
using System.Threading.Tasks;

namespace KafkaDemo_Producer
{
    class Program
    {
        public static void Main(string[] args)
        {
            var config = new ProducerConfig { BootstrapServers = "119.23.104.248:9092" };

            Action<DeliveryReportResult<Null, string>> handler = r =>
            Console.WriteLine(!r.Error.IsError
                ? $"Delivered message to {r.TopicPartitionOffset}"
                : $"Delivery Error: {r.Error.Reason}");

            using (var p = new Producer<Null, string>(config))
            {
                for (int i = 0; i < 100; ++i)
                {
                    p.BeginProduce("my-topic", new Message<Null, string> { Value = i.ToString() }, handler);
                }

                // wait for up to 10 seconds for any inflight messages to be delivered.
                p.Flush(TimeSpan.FromSeconds(10));
            }



            Console.WriteLine("Down!");

            Console.ReadKey();
        }
    }
}
