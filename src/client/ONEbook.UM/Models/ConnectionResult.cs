namespace ONEbook.UM.Models
{
    public class ConnectionResult
    {
        public ConnectionResult() : this(true, string.Empty)
        {
        }

        public ConnectionResult(bool isOk, string message)
        {
            IsOk = isOk;
            Message = message;
        }

        public bool IsOk { get; }

        public bool IsNotOk => !IsOk;

        public string Message { get; }
    }
}