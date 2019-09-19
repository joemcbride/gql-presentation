using System;
using GraphQL;
using GraphQL.Types;

namespace basic
{
    class Program
    {
        static void Main(string[] args)
        {
            var schema = Schema.For(@"
              type Droid {
                  id: ID
                  name: String
                  primaryFunction: String
                  description: String
              }

              type Query {
                  hero: Droid
                  droid(id: ID!) : Droid
              }
            ", _ =>
            {
                _.Types.Include<QueryType>();
                _.Types.Include<DroidType>();
            });

            var json = schema.Execute(_ =>
            {
                _.Query = @"
                {
                  hero {
                    id
                    name
                    primaryFunction
                    description
                  }
                }";

                // _.Query = @"
                // {
                //   droid(id: 27) {
                //     id
                //     name
                //     primaryFunction
                //     description
                //   }
                // }";
                _.ExposeExceptions = true;
            });

            Console.WriteLine(json);
        }
    }

    public class Droid
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Function { get; set; }
    }

    [GraphQLMetadata("Droid")]
    public class DroidType
    {
        public string PrimaryFunction(Droid source) => source.Function;

        public string Description(Droid source)
        {
            return $"{source.Name} is a {source.Function} droid.";
        }
    }

    [GraphQLMetadata("Query")]
    public class QueryType
    {
        public Droid Hero()
        {
            return new Droid { Id = 1, Name = "R2-D2", Function = "astromech" };
        }

        public Droid Droid(int id)
        {
            return new Droid { Id = id, Name = "R2-D2", Function = "astromech" };
        }
    }
}
