using MessagesServer.Controllers;

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
                    using (var databaseController = new MySqlDatabaseController())
                    {
                        using (var cacheController = new RedisController())
                        {
                            RabbitController rabbitController = new RabbitController(databaseController, cacheController);
                            rabbitController.GetQueryFromTheQueue();
                        }
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