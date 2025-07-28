namespace ONEbook.UM.Models;

using System;

[Serializable]
public class UMClientException : Exception
{
    public UMClientException()
    {
    }

    public UMClientException(string message) : base(message)
    {
    }
}