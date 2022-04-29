using System.Text.Json;
using System.Text.Json.Serialization;

namespace ECom.Services.Balance.App.Application.RingHandlers.UpdateCreditLimit
{
    public class SerializeHandler : IRingHandler<UpdateCreditLimitPersistentEvent>
    {
        private readonly int _handlerId;

        public SerializeHandler(int handlerId)
        {
            _handlerId = handlerId;
        }
        public void OnEvent(UpdateCreditLimitPersistentEvent data, long sequence, bool endOfBatch)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            if (data.SerializeHandlerId == _handlerId)
            {
              string jsonData = JsonSerializer.Serialize(data, options);
                data.UpdateCreditLimitPersistentEventString = jsonData;  
            }
        }
    }
}
