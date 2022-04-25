using Microsoft.Extensions.Hosting;

namespace ECom.Services.Balance.Infrastructure
{
    public class UserDbContextSeed
    {
        public async Task SeedAsync(UserDbContext userDbContext, IUserRepository userRepository, IHostEnvironment env, int numberOfLogHandlers, int numberOfReplyHandlers)
        {
            string contentRootPath = env.ContentRootPath.Replace("Balance.App", "Balance.Infrastructure");
            
            if (userDbContext.Users.Any())
            {
                IEnumerable<User> users = await userDbContext.Users.ToListAsync();
                InitInMemoryUsers(userRepository, users, numberOfLogHandlers, numberOfReplyHandlers);
            }
            else
            {
                IEnumerable<User> users = GetCustomerBalances(contentRootPath);
                userDbContext.Users.AddRange(users);
                userDbContext.SaveChanges();
                InitInMemoryUsers(userRepository, users, numberOfLogHandlers, numberOfReplyHandlers);
            }
        }

        private void InitInMemoryUsers(IUserRepository userRepository, IEnumerable<User> users, int numberOfLogHandlers, int numberOfReplyHandlers)
        {
            int currentLogHandler = 1;
            int currentReplyHandler = 1;

            foreach (var u in users)
            {
                userRepository.Add(u.Id, new InMemoryUser()
                {
                    UserId = u.Id,
                    Username = u.Name,
                    CreditLimit = u.CreditLimit,
                    LogHandlerId = currentLogHandler,
                    ReplyHandlerId = currentReplyHandler,
                });

                currentLogHandler++;
                currentReplyHandler++;

                if (currentLogHandler > numberOfLogHandlers)
                {
                    currentLogHandler = 1;
                }

                if (currentReplyHandler > numberOfReplyHandlers)
                {
                    currentReplyHandler = 1;
                }
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
