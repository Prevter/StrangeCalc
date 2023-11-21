namespace StrangeCalc.Exceptions;

public class UnexpectedTokenError : Error
{
    public UnexpectedTokenError(CodePosition start, CodePosition end, string details)
            : base("UnexpectedToken", details, start, end)
    {
    }
}
