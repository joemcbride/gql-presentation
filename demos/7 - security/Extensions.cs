using System;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Authorization;
using GraphQL.Introspection;
using GraphQL.Types;
using GraphQL.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace example
{
    public interface IBuildUserContext
    {
        GraphQLUserContext Build(HttpContext context);
    }

    public class DefaultBuildUserContext : IBuildUserContext
    {
        public GraphQLUserContext Build(HttpContext context)
        {
            return new GraphQLUserContext { User = context.User };
        }
    }

    public class MySchemaFilter : DefaultSchemaFilter
    {
        private string[] hiddenTypes = new string[] { "Mutation", "UpdateDroid" };

        public override Task<bool> AllowType(IGraphType type)
        {
            if (hiddenTypes.Contains(type?.Name))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }
    }

    public static class GraphQLAuthExtensions
    {
        public static void AddGraphQLAuth(this IServiceCollection services, Action<AuthorizationSettings> configure)
        {
            services.AddHttpContextAccessor();
            services.TryAddSingleton<IAuthorizationEvaluator, AuthorizationEvaluator>();
            services.AddTransient<IValidationRule, AuthorizationValidationRule>();
            services.AddTransient<IBuildUserContext, DefaultBuildUserContext>();
            services.AddTransient<ISchemaFilter, MySchemaFilter>();

            services.TryAddTransient(s =>
            {
                var authSettings = new AuthorizationSettings();
                configure(authSettings);
                return authSettings;
            });
        }
    }
}
