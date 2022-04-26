using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Services.Balance.Domain.AggregateModels.KafkaOffsetAggregate
{
    public class KafkaOffset
    {
        public static long CommandOffset { get; set; } = -1;
        public static long PersistentOffset { get; set; } = -1;
    }
}
