using System.Threading;

namespace MessagesServer
{
    public class Loader
    {
        public static void Main()
        {
            for (int i = 0; i < Convert.ToInt32(Environment.GetEnvironmentVariable("RETRIES_COUNT") ?? "1"); ++i)
            {
                try
                {
                    using (var databaseController = new Controllers.MySqlDatabaseController())
                    {
                        Controllers.RabbitController rabbitController = new Controllers.RabbitController(databaseController);
                        rabbitController.GetQueryFromTheQueue();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Thread.Sleep(5000);
                }
            }                                
        }
    }
}