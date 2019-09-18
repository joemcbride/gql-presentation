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
              type Query {
                  hello: String
              }
            ");

            var json = schema.Execute(_ =>
            {
                _.Query = @"{ hello }";
                _.Root = new { hello = "World" };
            });

            Console.WriteLine(json);
        }
    }
}
