using Newtonsoft.Json;

namespace JPlayer.Lib.Object
{
    public static class ObjectExtension
    {
        public static string ToJson(this object value)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return JsonConvert.SerializeObject(value, Formatting.Indented, settings);
        }
    }
}