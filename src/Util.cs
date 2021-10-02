using System.Collections.Generic;
using System.Collections;
using System.Linq;
namespace LDAPSyncTool
{
    public static class ExtensionUtilities
    {
        public static string StringJoin(this IEnumerable<string> source, string delim)
        {
            return string.Join(delim, source.ToArray());
        }
        public static string ToJson(this IDictionary<string,object> source)
        {
            return System.Text.Json.JsonSerializer.Serialize(source);
        }
    }
}