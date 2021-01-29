using Newtonsoft.Json;

namespace Omniverse.WinService
{
    public class Todo
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "userId", Required = Required.Always)]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "title", Required = Required.Always)]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "completed", Required = Required.Always)]
        public bool Completed { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
