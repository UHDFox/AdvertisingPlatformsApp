namespace Application.Infrastructure.Exceptions;

public sealed class AdDataNotLoadedException : InvalidOperationException
{
    public AdDataNotLoadedException(string message)
        : base(message)
    {
    }
}