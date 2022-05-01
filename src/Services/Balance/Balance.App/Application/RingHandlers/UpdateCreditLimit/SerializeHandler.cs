using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECom.Services.Balance.App.Application.RingHandlers.UpdateCreditLimit
{
    public class SerializeHandler : IRingHandler<UpdateCreditLimitPersistentEvent>
    {
        private readonly int _handlerId;
        private JsonSerializerOptions options;

        public SerializeHandler(int handlerId)
        {
            _handlerId = handlerId;
            options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }
        public void OnEvent(UpdateCreditLimitPersistentEvent data, long sequence, bool endOfBatch)
        {
            if (data.SerializeHandlerId == _handlerId)
            {
                string jsonData = JsonSerializer.Serialize(data, options);
                data.UpdateCreditLimitPersistentEventString = jsonData;
            }
        }
    }
}
