using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;

namespace account
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
    }

    public class UsersStore
    {
        public static User[] users = new [] {
            new User { Id = "1", Name = "Ada Lovelace", Username = "@ada" },
            new User { Id = "2", Name = "Alan Turing", Username = "@complete" }
        };

        public Task<User> Me()
        {
            return Task.FromResult(users[0]);
        }

        public Task<User> GetUserById(string userId)
        {
            return Task.FromResult(users.FirstOrDefault(x => x.Id == userId));
        }

        public Task<IDictionary<string, User>> GetUsersByIdAsync(IEnumerable<string> userIds, CancellationToken token)
        {
            var list = userIds.ToList();
            IDictionary<string, User> result = users.Where(x => list.Contains(x.Id)).ToDictionary(x => x.Id, x => x);
            return Task.FromResult(result);
        }
    }
}
