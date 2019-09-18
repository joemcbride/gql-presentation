using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace example
{
    public static class RestRequest
    {
        public static async Task<T> Fetch<T>(string url, JsonSerializerSettings settings = null)
        {
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync(new Uri(url));
            return JsonConvert.DeserializeObject<T>(json, settings ?? new JsonSerializerSettings());
        }
    }

    public class JObjectFieldResolver : IFieldResolver
    {
        public static JObjectFieldResolver Instance = new JObjectFieldResolver();

        public object Resolve(ResolveFieldContext context)
        {
            return Resolve(context?.Source as JObject, context?.FieldAst?.Name);
        }

        public static object Resolve(JObject source, string name)
        {
            if (source == null || name == null)
            {
                return null;
            }

            var value = source[name].GetValue();
            return value;
        }
    }
}
