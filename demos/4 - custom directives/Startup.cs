using GraphQL;
using GraphQL.Http;
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
            services.AddTransient<LowercaseDirectiveVisitor>();
            services.AddTransient<RestDirectiveVisitor>();

            services.AddTransient<ISchema>(s =>
            {
                return Schema.For(@"
                    directive @lower on FIELD_DEFINITION
                    directive @rest(url: String) on FIELD_DEFINITION

                    type Droid {
                      id: ID
                      name: String @lower
                    }

                    type Query {
                      hero: Droid @rest(url: ""http://localhost:3030/api/character/3"")
                    }
                ", _ =>
                {
                    _.ServiceProvider = s;
                    _.RegisterDirectiveVisitor<RestDirectiveVisitor>("rest");
                    _.RegisterDirectiveVisitor<LowercaseDirectiveVisitor>("lower");
                    _.ResolveAsRestType("Droid", "id", "name");
                });















                const string example = @"
                  directive @auth(
                    requires: Role = ADMIN,
                  ) on OBJECT | FIELD_DEFINITION

                  enum Role {
                    ADMIN
                    REVIEWER
                    USER
                    UNKNOWN
                  }

                  type User @auth(requires: USER) {
                    name: String
                    banned: Boolean @auth(requires: ADMIN)
                    canPost: Boolean @auth(requires: REVIEWER)
                  }
                  ";
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
