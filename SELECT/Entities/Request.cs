
namespace SELECT.Entities
{
    public class Request
    {
        public string Items { get; set; }
        public Filter[] Filters { get; set; }
        public Order Order { get; set; }
    }
}
