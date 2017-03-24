namespace ONEbook.UM.Models
{
    public class ConnectionResult
    {
        /// <summary>
        /// Default constructor here for deserialize json.
        /// </summary>
        public ConnectionResult() : this(true, string.Empty)
        {
        }

        public ConnectionResult(bool isOk, string message)
        {
            IsOk = isOk;
            Message = message;
        }

        public bool IsOk { get; set; }

        public bool IsNotOk
        {
            get
            {
                return !IsOk;
            }
        }

        public string Message { get; set; }
    }
}