using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.DataLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace example
{
    public abstract class StarWarsCharacter
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Friends { get; set; }
    }

    public class Human : StarWarsCharacter
    {
        public string HomePlanet { get; set; }
    }

    public class Droid : StarWarsCharacter
    {
        public string PrimaryFunction { get; set; }
    }

    public class Query
    {
        private readonly CharacterService _service;

        public Query(CharacterService service)
        {
            _service = service;
        }

        public Task<Droid> Hero()
        {
            return _service.FetchCharacterByIdAsync<Droid>("3");
        }
    }

    [GraphQLMetadata("Query")]
    public class QueryTypeDataLoader
    {
        private readonly CharacterDataLoaderService _service;
        private readonly IDataLoaderContextAccessor _accessor;

        public QueryTypeDataLoader(CharacterDataLoaderService service, IDataLoaderContextAccessor accessor)
        {
            _service = service;
            _accessor = accessor;
        }

        public Task<StarWarsCharacter> Hero()
        {
            // Get or add a batch loader with the key "GetCharacterById"
            // The loader will call FetchCharacterByIdAsync for each batch of keys
            var loader = _accessor.Context.GetOrAddBatchLoader<string, StarWarsCharacter>("GetCharacterById", _service.FetchCharacterByIdAsync);

            // Add the list of friend ids to the pending keys to fetch
            // The task will complete once all of the FetchCharacterByIdAsync() returns with the batched results
            return loader.LoadAsync("3");
        }
    }

    [GraphQLMetadata("Droid", IsTypeOf=typeof(Droid))]
    public class DroidTypeDataLoader
    {
        private readonly CharacterDataLoaderService _service;
        private readonly IDataLoaderContextAccessor _accessor;

        public DroidTypeDataLoader(CharacterDataLoaderService service, IDataLoaderContextAccessor accessor)
        {
            _service = service;
            _accessor = accessor;
        }

        public Task<StarWarsCharacter[]> Friends(Droid source)
        {
            var friends = source.Friends ?? new string[0];

            // Get or add a batch loader with the key "GetCharacterById"
            // The loader will call FetchCharacterByIdAsync for each batch of keys
            var loader = _accessor.Context.GetOrAddBatchLoader<string, StarWarsCharacter>("GetCharacterById", _service.FetchCharacterByIdAsync);

            // Add the list of friend ids to the pending keys to fetch
            // The task will complete once all of the FetchCharacterByIdAsync() returns with the batched results
            return loader.LoadAsync(friends);
        }
    }

    [GraphQLMetadata("Human", IsTypeOf=typeof(Human))]
    public class HumanTypeDataLoader
    {
        private readonly CharacterDataLoaderService _service;
        private readonly IDataLoaderContextAccessor _accessor;

        public HumanTypeDataLoader(CharacterDataLoaderService service, IDataLoaderContextAccessor accessor)
        {
            _service = service;
            _accessor = accessor;
        }

        public Task<StarWarsCharacter[]> Friends(Human source)
        {
            var friends = source.Friends ?? new string[0];

            // Get or add a batch loader with the key "GetCharacterById"
            // The loader will call FetchCharacterByIdAsync for each batch of keys
            var loader = _accessor.Context.GetOrAddBatchLoader<string, StarWarsCharacter>("GetCharacterById", _service.FetchCharacterByIdAsync);

            // Add the list of friend ids to the pending keys to fetch
            // The task will complete once all of the FetchCharacterByIdAsync() returns with the batched results
            return loader.LoadAsync(friends);
        }
    }

    [GraphQLMetadata("Droid", IsTypeOf=typeof(Droid))]
    public class DroidType
    {
        private readonly CharacterService _service;

        public DroidType(CharacterService service)
        {
            _service = service;
        }

        public Task<StarWarsCharacter[]> Friends(Droid source)
        {
            var friends = source.Friends ?? new string[0];
            return Task.WhenAll(friends.Select(id => _service.FetchCharacterByIdAsync<StarWarsCharacter>(id)));
        }
    }

    [GraphQLMetadata("Human", IsTypeOf=typeof(Human))]
    public class HumanType
    {
        private readonly CharacterService _service;

        public HumanType(CharacterService service)
        {
            _service = service;
        }

        public Task<StarWarsCharacter[]> Friends(Human source)
        {
            var friends = source.Friends ?? new string[0];
            return Task.WhenAll(friends.Select(id => _service.FetchCharacterByIdAsync<StarWarsCharacter>(id)));
        }
    }

    public class CharacterService
    {
        private const string BASE_URL = "http://localhost:3030/api";
        private readonly JsonSerializerSettings _serializerSettings;

        public CharacterService()
        {
            _serializerSettings = new JsonSerializerSettings();
            _serializerSettings.Converters.Add(new StarWarsCharacterJsonConverter());
        }

        public Task<T> FetchCharacterByIdAsync<T>(string id) where T : StarWarsCharacter
        {
            return RestRequest.Fetch<T>($"{BASE_URL}/character/{id}", _serializerSettings);
        }
    }

    public class CharacterDataLoaderService
    {
        private readonly CharacterService _service;

        public CharacterDataLoaderService(CharacterService service)
        {
            _service = service;
        }

        public async Task<IDictionary<string, StarWarsCharacter>> FetchCharacterByIdAsync(IEnumerable<string> ids)
        {
            var characters = await Task.WhenAll(ids.Select(id => _service.FetchCharacterByIdAsync<StarWarsCharacter>(id)));
            return characters.ToDictionary(x => x.Id);
        }
    }

    public class StarWarsCharacterJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(StarWarsCharacter).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            StarWarsCharacter character;

            if(obj.ContainsKey("homePlanet"))
            {
                character = new Human();
            }
            else
            {
                character = new Droid();
            }

            serializer.Populate(obj.CreateReader(), character);

            return character;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }
}
