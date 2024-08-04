using System;
using System.Threading.Tasks;
using BridgeCommandPlanner.SQL;
using NLog;

namespace BridgeCommandPlanner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "BridgeCommandPlanner (version 1.0.0)";

            Console.WriteLine("BridgeCommandPlanner is starting..");

            DbConnection dbConnection = new DbConnection("127.0.0.1", "sa", "pass", "PacketFilter");

            CommandPlanner cmdPlanner = new CommandPlanner(dbConnection);

            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    await cmdPlanner.Read();

                    await Task.Delay(1000);
                }
            });

            Console.ReadKey();
        }
    }
}
