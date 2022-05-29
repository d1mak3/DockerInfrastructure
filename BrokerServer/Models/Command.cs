namespace BrokerServer.Models
{
    [Serializable]
    public class Command
    {
        public string Option { get; set; } // POST or GET
        public string Data { get; set; }
    }
}
