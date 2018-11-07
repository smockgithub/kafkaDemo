using Confluent.Kafka;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KafkaDemo_Producer
{
    class Program
    {
        public static void Main(string[] args)
        {
            var result = Task.Run(()=>producer2()).GetAwaiter().GetResult();
        }

        public static async Task<string> producer2()
        {
            string brokerList = "119.23.104.248:9092";
            string topicName = "my-topic";

            var config = new ProducerConfig { BootstrapServers = brokerList };

            using (var producer = new Producer<string, string>(config))
            {
                Console.WriteLine("\n-----------------------------------------------------------------------");
                Console.WriteLine($"Producer {producer.Name} producing on topic {topicName}.");
                Console.WriteLine("-----------------------------------------------------------------------");
                Console.WriteLine("To create a kafka message with UTF-8 encoded key and value:");
                Console.WriteLine("> key value<Enter>");
                Console.WriteLine("To create a kafka message with a null key and UTF-8 encoded value:");
                Console.WriteLine("> value<enter>");
                Console.WriteLine("Ctrl-C to quit.\n");

                var cancelled = false;
                Console.CancelKeyPress += (_, e) => {
                    e.Cancel = true; // prevent the process from terminating.
                    cancelled = true;
                };

                while (!cancelled)
                {
                    Console.Write("> ");

                    string text;
                    try
                    {
                        text = Console.ReadLine();
                    }
                    catch (IOException)
                    {
                        // IO exception is thrown when ConsoleCancelEventArgs.Cancel == true.
                        break;
                    }
                    if (text == null)
                    {
                        // Console returned null before 
                        // the CancelKeyPress was treated
                        break;
                    }

                    string key = null;
                    string val = text;

                    // split line if both key and value specified.
                    int index = text.IndexOf(" ");
                    if (index != -1)
                    {
                        key = text.Substring(0, index);
                        val = text.Substring(index + 1);
                    }

                    try
                    {
                        // Awaiting the asynchronous produce request below prevents flow of execution
                        // from proceeding until the acknowledgement from the broker is received.
                        var deliveryReport = await producer.ProduceAsync(topicName, new Message<string, string> { Key = key, Value = val });
                        Console.WriteLine($"delivered to: {deliveryReport.TopicPartitionOffset}");
                    }
                    catch (KafkaException e)
                    {
                        Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                    }
                }

                // Since we are producing synchronously, at this point there will be no messages
                // in-flight and no delivery reports waiting to be acknowledged, so there is no
                // need to call producer.Flush before disposing the producer.
            }

            return "Down";
        }

        public static void producer1()
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
