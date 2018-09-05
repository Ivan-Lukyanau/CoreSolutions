using Newtonsoft.Json;

namespace CoreFireAPI.Models
{
    public class Customer
    {
        [JsonIgnore]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
    }
}
