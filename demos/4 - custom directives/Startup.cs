using GraphQL;
using GraphQL.Http;
using GraphQL.Server.Ui.Playground;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace example
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            // GraphQL types
            services.AddSingleton<IDocumentWriter, DocumentWriter>();
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();

            // custom types
            services.AddTransient<RestDirectiveVisitor>();

            services.AddTransient<ISchema>(s =>
            {
                return Schema.For(@"
                    directive @rest(url: String) on FIELD_DEFINITION

                    type Droid {
                      id: ID
                      name: String
                    }

                    type Query {
                      hero: Droid @rest(url: ""http://localhost:3030/api/droid/3"")
                    }
                ", _ =>
                {
                    _.ServiceProvider = s;
                    _.RegisterDirectiveVisitor<RestDirectiveVisitor>("rest");
                    _.ResolveAsRestType("Droid", "id", "name");
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
