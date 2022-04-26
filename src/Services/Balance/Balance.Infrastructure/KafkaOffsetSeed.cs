using ECom.Services.Balance.Domain.AggregateModels.KafkaOffsetAggregate;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Services.Balance.Infrastructure
{
    public class KafkaOffsetSeed
    {
        public async Task SeedAsync(IHostEnvironment env)
        {
            string contentRootPath = env.ContentRootPath.Replace("Balance.App", "Balance.Infrastructure");
            string path = Path.Combine(contentRootPath, "Setup", "KafkaOffset.csv");

            if (File.Exists(path))
            {
                string value = File.ReadAllLines(path).Skip(1).FirstOrDefault();
                if (!String.IsNullOrEmpty(value))
                {
                    InitInMemoryKafkaOffset(value);
                }
                else
                {
                    await CreateKafkaOffsetFileAsync(path);
                }
            }
            else
            {
                await CreateKafkaOffsetFileAsync(path);
            }
        }

        private async Task CreateKafkaOffsetFileAsync(string path)
        {
            IEnumerable<string> lines = new List<string>()
                {
                    "CommandOffset, PersistentOffset",
                    "-1, -1"
                };
            await File.WriteAllLinesAsync(path, lines);

            string value = File.ReadAllLines(path).Skip(1).FirstOrDefault();
            InitInMemoryKafkaOffset(value);
        }

        private void InitInMemoryKafkaOffset(string value)
        {
            var splits = value.Split(",");
            string commandOffsetStr = splits[0];
            string persistentOffsetStr = splits[0];

            KafkaOffset.CommandOffset    = KafkaOffset.CommandOffset > -1 ? (long.Parse(commandOffsetStr) + 1) : -1;
            KafkaOffset.PersistentOffset = KafkaOffset.PersistentOffset > -1 ? (long.Parse(persistentOffsetStr) + 1) : -1;
        }
    }
}
