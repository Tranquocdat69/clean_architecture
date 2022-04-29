using Microsoft.Extensions.Hosting;

namespace ECom.Services.Balance.Infrastructure
{
    public class UserDbContextSeed
    {
        public static long CurrentCommandTopicOffset = -1;

        public async Task SeedAsync(UserDbContext userDbContext, IUserRepository userRepository, IHostEnvironment env)
        {
            string contentRootPath = env.ContentRootPath.Replace("Balance.App", "Balance.Infrastructure");

            if (userDbContext.KafkaOffsets.Any())
            {
                KafkaOffset kafkaOffset = userDbContext.KafkaOffsets.FirstOrDefault();
                CurrentCommandTopicOffset = kafkaOffset.CommandOffset + 1;
            }
            else
            {
                KafkaOffset kafkaOffset = new()
                {
                    CommandOffset = -1,
                    PersistentOffset = 1
                };
                userDbContext.KafkaOffsets.Add(kafkaOffset);
                userDbContext.SaveChanges();

                CurrentCommandTopicOffset = kafkaOffset.CommandOffset + 1;
            }

            if (userDbContext.Users.Any())
            {
                IEnumerable<User> users = await userDbContext.Users.ToListAsync();
                InitInMemoryUsers(userRepository, users);
            }
            else
            {
                IEnumerable<User> users = GetCustomerBalances(contentRootPath);
                userDbContext.Users.AddRange(users);
                userDbContext.SaveChanges();
                InitInMemoryUsers(userRepository, users);
            }
        }

        private void InitInMemoryUsers(IUserRepository userRepository, IEnumerable<User> users)
        {
            foreach (var u in users)
            {
                userRepository.Add(u.Id, new User(u.Name, u.CreditLimit));
            }
        }

        private IEnumerable<User> GetCustomerBalances(string contentRootPath)
        {
            string pathFile = Path.Combine(contentRootPath, "Setup", "UserBalances.csv");
            if (File.Exists(pathFile))
            {
                return File.ReadAllLines(pathFile)
                    .Skip(1)
                    .Select(line => ConvertStringToUser(line))
                    .ToList();
            }

            return CreateDefaultUsers();
        }

        private IEnumerable<User> CreateDefaultUsers()
        {
            return new List<User>(){
                new User("user 01",100),
                new User("user 02",100),
                new User("user 03",100),
            };
        }

        private User ConvertStringToUser(string line)
        {
            string[] splits = line.Split(",");
            string rawUsername = splits[0];
            string rawCrediteLimit = splits[1];

            //int indexOfPrefixUsername = rawUsername.IndexOf("C");
            //int userId = Int32.Parse(rawUsername.Substring(indexOfPrefixUsername + 1));
            decimal creditLimit = decimal.Parse(rawCrediteLimit);

            return new User(rawUsername, creditLimit);
        }
    }
}
