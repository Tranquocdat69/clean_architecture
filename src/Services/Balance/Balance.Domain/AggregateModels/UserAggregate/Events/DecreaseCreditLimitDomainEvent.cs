using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Services.Balance.Domain.AggregateModels.UserAggregate.Events
{
    public class DecreaseCreditLimitDomainEvent : BaseDomainEvent
    {
        public long Offset { get; set; }
        public int UserId { get; set; }
        public decimal CreditLimit { get; set; }
        public int SerializeHandlerId { get; set; }

        public DecreaseCreditLimitDomainEvent(long offset, int userId, decimal creditLimit, int serializeHandlerId)
        {
            Offset = offset;
            UserId = userId;
            CreditLimit = creditLimit;
            SerializeHandlerId = serializeHandlerId;
        }
    }
}
