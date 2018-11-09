using Confluent.Kafka;
using System;

namespace Consumer1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("请输入GroupId：如果不输入，默认为test-consumer-group");
            var groupId = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(groupId))
            {
                groupId = "test-consumer-group";
            }

            Console.WriteLine("请输入TopicName：如果不输入，默认为my-topic");
            var topicName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(topicName))
            {
                topicName = "my-topic";
            }

            var conf = new ConsumerConfig
            {
                GroupId = groupId,
                BootstrapServers = "119.23.104.248:9092",
                // Note: The AutoOffsetReset property determines the start offset in the event
                // there are not yet any committed offsets for the consumer group for the
                // topic/partitions of interest. By default, offsets are committed
                // automatically, so in this example, consumption will only start from the
                // eariest message in the topic 'my-topic' the first time you run the program.
                AutoOffsetReset = AutoOffsetResetType.Earliest
            };

            using (var c = new Consumer<Ignore, string>(conf))
            {
                c.Subscribe(topicName);//订阅模式

                bool consuming = true;
                // The client will automatically recover from non-fatal errors. You typically
                // don't need to take any action unless an error is marked as fatal.
                c.OnError += (_, e) => consuming = !e.IsFatal;

                while (consuming)
                {
                    try
                    {
                        var cr = c.Consume();
                        Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occured: {e.Error.Reason}");
                    }
                }

                // Ensure the consumer leaves the group cleanly and final offsets are committed.
                c.Close();
            }
        }
    }
}
