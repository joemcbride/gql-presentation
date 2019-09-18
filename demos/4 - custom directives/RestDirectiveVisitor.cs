using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using GraphQL.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace example
{
    public class RestDirectiveVisitor : SchemaDirectiveVisitor
    {
        public override void VisitField(FieldType field)
        {
            field.Resolver = new AsyncFieldResolver<object>(async context =>
            {
                var url = GetArgument<string>("url");
                var result = await RestRequest.Fetch(url);
                return result;
            });
        }
    }

    public static class RestRequest
    {
        public static async Task<JObject> Fetch(string url)
        {
            var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync(new Uri(url));
            return JObject.Parse(json);
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

    public static class SchemaBuilderExtensions
    {
        public static SchemaBuilder ResolveAsRestType(this SchemaBuilder builder, string type, params string[] fields)
        {
            var typeConfig = builder.Types.For(type);

            foreach(var field in fields)
            {
                typeConfig.FieldFor(field, builder.ServiceProvider).Resolver = JObjectFieldResolver.Instance;
            }

            return builder;
        }
    }
}
