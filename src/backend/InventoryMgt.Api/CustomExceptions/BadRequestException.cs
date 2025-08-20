namespace InventoryMgt.Api.CustomExceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {

    }
}