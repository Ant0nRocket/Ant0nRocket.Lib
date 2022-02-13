using Newtonsoft.Json;

namespace Ant0nRocket.Lib.Std20.Extensions
{
    public static class ObjectExtensions
    {
        public static string AsJson(this object obj, bool pretty = false)
        {
            var formatting = pretty ? Formatting.Indented : Formatting.None;
            return JsonConvert.SerializeObject(obj, formatting);
        }
    }
}
