namespace ECom.Services.Balance.Domain.Exceptions;
/// <summary>
/// Exception type for domain exceptions
/// </summary>
public class BalanceDomainException : Exception
{
    public BalanceDomainException()
    { }

    public BalanceDomainException(string message)
        : base(message)
    { }

    public BalanceDomainException(string message, Exception innerException)
        : base(message, innerException)
    { }
}
