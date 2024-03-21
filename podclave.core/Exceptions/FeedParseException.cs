
namespace Podclave.Core.Exceptions;

public class FeedParseException : Exception
{
    public FeedParseException()
    {
    }

    public FeedParseException(string message)
        : base(message)
    {
    }

    public FeedParseException(string message, Exception inner)
        : base(message, inner)
    {
    }    
}