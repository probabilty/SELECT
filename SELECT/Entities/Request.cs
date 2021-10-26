
namespace SELECT.Entities
{
    public class Request
    {
        public string Items { get; set; }
        public Filter[] filters { get; set; }
        public Order order { get; set; }
    }
}
