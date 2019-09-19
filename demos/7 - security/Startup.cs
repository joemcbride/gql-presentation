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
            services.AddTransient<QueryType>();
            services.AddTransient<MutationType>();
            services.AddTransient<DroidType>();

            services.AddSingleton<DataStore>();

            // this is an extension method written in this project
            services.AddGraphQLAuth(_ =>
            {
                _.AddPolicy("AdminPolicy", p => p.RequireClaim("role", "admin"));
            });

            services.AddTransient<ISchema>(s =>
            {
                return Schema.For(@"
                    type Droid {
                        id: ID!
                        name: String!
                        primaryFunction: String
                        description: String
                        appearsIn: [Episode!]!
                    }

                    enum Episode {
                        NEWHOPE
                        EMPIRE
                        JEDI
                    }

                    type Query {
                        hero: Droid
                        droid(id: ID!): Droid
                    }

                    input UpdateDroid {
                        id: ID!
                        name: String!
                        primaryFunction: String
                    }

                    type Mutation {
                        updateDroid(droid: UpdateDroid!): Droid
                    }
                ", _ =>
                {
                    _.ServiceProvider = s;
                    _.Types.Include<QueryType>();
                    _.Types.Include<MutationType>();
                    _.Types.Include<DroidType>();
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
