using StackExchange.Redis;
using MessagesServer.Interfaces;

namespace MessagesServer.Controllers
{
    public class RedisController : ICacheController
    {
        private ConnectionMultiplexer redis;
        private IDatabase redisDatabase;

        public RedisController()
        {
            if (!OpenConnection())
            {
                throw new Exception("Couldn't connect to the database!");
            }
        }

        public bool CloseConnection()
        {
            try
            {
                redis.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public void Dispose()
        {
            redis.Dispose();
        }

        public bool OpenConnection()
        {
            try
            {
                redis = ConnectionMultiplexer.Connect(
                    Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? "localhost:6379"
                );

                redisDatabase = redis.GetDatabase();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public string GetValueByKey(string key)
        {            
            return redisDatabase.StringGet(key);
        }

        public bool SetValue(string key, string value)
        {
            try
            {
                redisDatabase.StringSet(key, value);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
