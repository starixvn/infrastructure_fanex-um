namespace ONEbook.UM.Models
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class UMClientException : Exception
    {
        public UMClientException()
        {
        }

        public UMClientException(string message) : base(message)
        {
        }

        protected UMClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public UMClientException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}