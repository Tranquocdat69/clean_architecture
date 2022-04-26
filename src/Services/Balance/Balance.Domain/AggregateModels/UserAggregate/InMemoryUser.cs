using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Services.Balance.Domain.AggregateModels.UserAggregate
{
    public class InMemoryUser : IAggregateRoot
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public decimal CreditLimit { get; set; }
        public int LogHandlerId { get; set; }
    }
}
