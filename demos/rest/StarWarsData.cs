using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace example
{
    public abstract class StarWarsCharacter
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] Friends { get; set; }
        public int[] AppearsIn { get; set; }
    }

    public class Human : StarWarsCharacter
    {
        public string HomePlanet { get; set; }
    }

    public class Droid : StarWarsCharacter
    {
        public string PrimaryFunction { get; set; }
    }

    public class StarWarsData
    {
        private readonly List<StarWarsCharacter> _characters = new List<StarWarsCharacter>();

        public StarWarsData()
        {
            _characters.Add(new Human
            {
                Id = "1",
                Name = "Luke",
                Friends = new[] {"3", "4"},
                AppearsIn = new[] {4, 5, 6},
                HomePlanet = "Tatooine"
            });
            _characters.Add(new Human
            {
                Id = "2",
                Name = "Vader",
                AppearsIn = new[] {4, 5, 6},
                HomePlanet = "Tatooine"
            });

            _characters.Add(new Droid
            {
                Id = "3",
                Name = "R2-D2",
                Friends = new[] {"1", "4"},
                AppearsIn = new[] {4, 5, 6},
                PrimaryFunction = "Astromech"
            });
            _characters.Add(new Droid
            {
                Id = "4",
                Name = "C-3PO",
                Friends = new[] {"1", "3"},
                AppearsIn = new[] {4, 5, 6},
                PrimaryFunction = "Protocol"
            });
        }

        public IEnumerable<StarWarsCharacter> GetFriends(StarWarsCharacter character)
        {
            if (character == null)
            {
                return null;
            }

            var friends = new List<StarWarsCharacter>();
            var lookup = character.Friends;
            if (lookup != null)
            {
                _characters.Where(d => lookup.Contains(d.Id)).Apply(friends.Add);
            }
            return friends;
        }

        public Task<StarWarsCharacter> GetCharacter(string id)
        {
            var result = _characters.FirstOrDefault(h => h.Id == id);
            return Task.FromResult(result);
        }
    }
}
