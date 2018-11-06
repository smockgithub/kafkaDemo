using Confluent.Kafka;
using System;
using System.Threading.Tasks;

namespace KafkaDemo_Producer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ProducerConfig { BootstrapServers = "localhost:9092" };




            Console.WriteLine("Hello World!");

            Console.ReadKey();
        }
    }
}
