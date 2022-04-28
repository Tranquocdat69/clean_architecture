using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Services.Balance.Infrastructure
{
    public class KafkaOffset
    {
        public int Id { get; set; }
        public long CommandOffset { get; set; } = -1;
        public long PersistentOffset { get; set; } = -1;
    }
}
