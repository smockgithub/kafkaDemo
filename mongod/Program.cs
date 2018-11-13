using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace mongod
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() => GetMongoDbByPersonName("jack"));

            Console.WriteLine("请输入姓名：");
            while (true)
            {
                string name = Console.ReadLine();

                Task.Run(() => InsertMongodbByPersion(new Person { Name=name})).Wait();

                Task.Run(() => GetMongoDbByPersonName(name)).Wait();

            }


        }

        public static async Task GetMongoDbByPersonName(string name)
        {
            var client = new MongoClient("");
            var database = client.GetDatabase("foo");
            var collection = database.GetCollection<Person>("bar");
            

            var list = await collection.Find(x=>x.Name== name).ToListAsync();

            Console.WriteLine("查询DB:foo Collection:bar里Name为"+name+"的数据");
            foreach (var person in list)
            {
                Console.WriteLine(string.Format("id={0},name={1}",person.Id,person.Name));
            }

        }

        public static async Task InsertMongodbByPersion(Person p)
        {
            var client = new MongoClient("");
            var database = client.GetDatabase("foo");
            var collection = database.GetCollection<Person>("bar");
            await collection.InsertOneAsync(new Person { Name = p.Name });

            Console.WriteLine("Insert DB:foo Collection:bar Person("+p.Name+")");
        }
    }

    public class Person
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
    }
}
