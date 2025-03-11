namespace Application.Infrastructure.Exceptions;

public sealed class EmptyLineException : InvalidOperationException
{
    public EmptyLineException(string message)
        : base(message)
    {
    }
}