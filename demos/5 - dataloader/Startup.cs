using GraphQL;
using GraphQL.DataLoader;
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

            services.AddGraphQLSchema();
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

    public static class GraphQLServiceExtensions
    {
        public static IServiceCollection AddGraphQLSchema(this IServiceCollection services)
        {
            // custom types
            services.AddTransient<Query>();
            services.AddTransient<DroidType>();
            services.AddTransient<HumanType>();
            services.AddTransient<CharacterService>();

            services.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>();
            services.AddSingleton<DataLoaderDocumentListener>();

            services.AddTransient<ISchema>(s =>
            {
                return Schema.For(@"
                    interface Character {
                      id: ID
                      name: String
                      friends: [Character]
                    }

                    type Droid implements Character {
                      id: ID
                      name: String
                      friends: [Character]
                      primaryFunction: String
                    }

                    type Human implements Character {
                      id: ID
                      name: String
                      friends: [Character]
                      homePlanet: String
                    }

                    type Query {
                      hero: Droid
                    }
                ", _ =>
                {
                    _.ServiceProvider = s;
                    _.Types.Include<Query>();
                    _.Types.Include<DroidType>();
                    _.Types.Include<HumanType>();
                });
            });

            return services;
        }

        public static IServiceCollection AddGraphQLSchemaWithDataLoader(this IServiceCollection services)
        {
            // custom types
            services.AddTransient<QueryTypeDataLoader>();
            services.AddTransient<DroidTypeDataLoader>();
            services.AddTransient<HumanTypeDataLoader>();
            services.AddTransient<CharacterService>();
            services.AddTransient<CharacterDataLoaderService>();

            services.AddSingleton<IDataLoaderContextAccessor, DataLoaderContextAccessor>();
            services.AddSingleton<DataLoaderDocumentListener>();

            services.AddTransient<ISchema>(s =>
            {
                return Schema.For(@"
                    interface Character {
                      id: ID
                      name: String
                      friends: [Character]
                    }

                    type Droid implements Character {
                      id: ID
                      name: String
                      friends: [Character]
                      primaryFunction: String
                    }

                    type Human implements Character {
                      id: ID
                      name: String
                      friends: [Character]
                      homePlanet: String
                    }

                    type Query {
                      hero: Droid
                    }
                ", _ =>
                {
                    _.ServiceProvider = s;
                    _.Types.Include<QueryTypeDataLoader>();
                    _.Types.Include<DroidTypeDataLoader>();
                    _.Types.Include<HumanTypeDataLoader>();
                });
            });

            return services;
        }
    }
}
