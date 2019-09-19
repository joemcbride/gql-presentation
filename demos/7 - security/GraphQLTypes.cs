using System.Collections.Generic;
using System.Security.Claims;
using GraphQL;
using GraphQL.Authorization;

namespace example
{
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
        private readonly DataStore _store;

        public QueryType(DataStore store)
        {
            _store = store;
        }

        public Droid Hero()
        {
            return _store.GetHero();
        }

        public Droid Droid(int id)
        {
            return _store.GetCharacterById(id);
        }
    }

    [GraphQLMetadata("Mutation")]
    public class MutationType
    {
        private readonly DataStore _store;

        public MutationType(DataStore store)
        {
            _store = store;
        }

        [GraphQLAuthorize(Policy = "AdminPolicy")]
        public Droid UpdateDroid(DroidInput droid)
        {
            return _store.Update(droid);
        }
    }

    public class GraphQLUserContext : Dictionary<string, object>, IProvideClaimsPrincipal
    {
        public ClaimsPrincipal User { get; set; }
    }
}
