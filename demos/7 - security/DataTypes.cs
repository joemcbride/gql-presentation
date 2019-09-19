using System.Collections.Generic;

namespace example
{
    public class Droid
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Function { get; set; }
        public List<Episode> AppearsIn { get; set; } = new List<Episode>();
    }

    public enum Episode
    {
        NewHope,
        Empire,
        Jedi
    }

    public class DroidInput
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PrimaryFunction { get; set; }
    }

    public class DataStore
    {
        private readonly Droid _droid = new Droid
        {
            Id = 1,
            Name = "R2-D2",
            Function = "astromech",
            AppearsIn = new List<Episode> { Episode.NewHope, Episode.Empire, Episode.Jedi }
        };

        public Droid GetHero()
        {
            return _droid;
        }

        public Droid GetCharacterById(int id)
        {
            return _droid;
        }

        public Droid Update(DroidInput update)
        {
            _droid.Name = update.Name;
            _droid.Function = update.PrimaryFunction;

            return _droid;
        }
    }
}
