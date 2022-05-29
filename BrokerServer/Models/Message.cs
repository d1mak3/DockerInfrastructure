namespace BrokerServer.Models
{
    [Serializable]
    public class Message
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Content { get; set; }

        public override string ToString()
        {
            return "'" + Convert.ToString(Id) + "'" + ", " + "'" + Sender + "'" + ", " + "'" + Content + "'";
        }
    }
}
