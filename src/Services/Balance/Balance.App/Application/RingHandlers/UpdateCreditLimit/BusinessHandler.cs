﻿namespace ECom.Services.Balance.App.Application.RingHandlers.UpdateCreditLimit
{
    public class BusinessHandler : IRingHandler<UpdateCreditLimitEvent>
    {
        public void OnEvent(UpdateCreditLimitEvent data, long sequence, bool endOfBatch)
        {
            Console.WriteLine("Business Handler OKE");
        }
    }
}
