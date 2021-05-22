using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JPlayer.Lib.Object
{
    public static class ObjectExtension
    {
        public static string ToJson(this object value)
        {
            JsonSerializerSettings settings = new()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            return JsonConvert.SerializeObject(value, Formatting.Indented, settings);
        }
    }
}