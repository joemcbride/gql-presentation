using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Http;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using GraphQL.Utilities.Federation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace account
{
    public class Query
    {
        private readonly UsersStore _store;

        public Query(UsersStore store)
        {
            _store = store;
        }

        public Task<User> Me()
        {
            return _store.Me();
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // GraphQL types
            services.AddSingleton<IDocumentWriter, DocumentWriter>();
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();

            // Apollo Federation Types
            services.AddSingleton<AnyScalarGraphType>();
            services.AddSingleton<ServiceGraphType>();

            // Custom Types
            services.AddSingleton<UsersStore>();
            services.AddTransient<Query>();

            services.AddTransient<ISchema>(s =>
            {
                var store = s.GetRequiredService<UsersStore>();

                return FederatedSchema.For(@"
                    extend type Query {
                        me: User
                    }

                    type User @key(fields: ""id"") {
                        id: ID!
                        name: String
                        username: String
                    }
                ", _ =>
                {
                    _.ServiceProvider = s;
                    _.Types.Include<Query>();
                    _.Types.For("User").ResolveReferenceAsync(context =>
                    {
                        var id = (string)context.Arguments["id"];
                        return store.GetUserById(id);
                    });
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvcWithDefaultRoute();
            app.UseGraphQLPlayground();
        }
    }
}
