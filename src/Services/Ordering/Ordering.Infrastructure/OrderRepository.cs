using ECom.BuildingBlocks.SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;

namespace ECom.Services.Ordering.Infrastructure
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;
        private readonly ILogger<OrderRepository> _logger;

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }
        public OrderRepository(OrderDbContext context, ILogger<OrderRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
        }

        public bool Add(Order t)
        {
            try
            {
                _context.Orders.Add(t);
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return false;
            }
        }
    }
}
