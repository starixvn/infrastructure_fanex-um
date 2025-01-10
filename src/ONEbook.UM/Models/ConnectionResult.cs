namespace ONEbook.UM.Models;

public class ConnectionResult(bool isOk, string message)
{
    public ConnectionResult() : this(true, string.Empty)
    {
    }

    public bool IsOk { get; } = isOk;

    public bool IsNotOk => !IsOk;

    public string Message { get; } = message;
}