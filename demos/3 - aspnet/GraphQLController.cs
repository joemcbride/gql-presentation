using System.Net;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Http;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace example
{
    public class GraphQLRequest
    {
        public string Query { get; set; }
        public string OperationName { get; set; }
        public JObject Variables { get; set; }
    }

    [Route("[controller]")]
    public class GraphQLController : Controller
    {
        private readonly IDocumentExecuter _executer;
        private readonly IDocumentWriter _writer;

        public GraphQLController(IDocumentExecuter executer, IDocumentWriter writer)
        {
            _executer = executer;
            _writer = writer;
        }

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromBody]GraphQLRequest request,
            [FromServices]ISchema schema)
        {
            var result = await _executer.ExecuteAsync(_ =>
            {
                _.Schema = schema;
                _.OperationName = request.OperationName;
                _.Query = request.Query;
                _.Inputs = request.Variables.ToInputs();
                _.ExposeExceptions = true;
            });

            var json = await _writer.WriteToStringAsync(result);

            var httpResult = result.Errors?.Count > 0
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.OK;

            return Content(json, "application/json");
        }
    }
}
