namespace Bps.uCloud.Backend.Settings
{
    public class RabbitMQ
    {
        public string uri { get; set; }
        public string user { get; set; }
        public string pswd { get; set; }
        public Queues queues { get; set; }
    }
}
