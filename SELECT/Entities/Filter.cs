using Newtonsoft.Json;

namespace SELECT.Entities
{
    public class Filter
    {
        [JsonConstructor]
        public Filter(Operator op, string fieldName, object value)
        {
            Op = op;
            FieldName = fieldName;
            Value = value;
        }
        public Operator Op { get; set; }
        public string FieldName { get; set; }
        public object Value { get; set; }
    }
}
