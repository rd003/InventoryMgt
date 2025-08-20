namespace InventoryMgt.Api.CustomExceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException() : base("User is not authorized")
    {

    }

    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
