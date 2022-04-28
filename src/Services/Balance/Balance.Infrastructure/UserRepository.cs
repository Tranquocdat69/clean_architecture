namespace ECom.Services.Balance.Infrastructure
{
    public class UserRepository : IUserRepository
    {
        private static IDictionary<int, User> _users;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ILogger<UserRepository> logger)
        {
            _users = _users ?? new Dictionary<int, User>();
            _logger = logger;
        }

        public void Add(int id, User user)
        {
            if (_users.ContainsKey(id))
            {
                _users[id] = user;
            }
            else
            {
                _users.Add(id, user);
            }
        }
        public bool Exist(int id)
        {
            return _users.ContainsKey(id);
        }

        public User GetT(int id)
        {
            if (_users.ContainsKey(id))
            {
                return _users[id];
            }
            return null;
        }
    }
}
