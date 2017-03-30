namespace ONEbook.UM.Services
{
    using System;

    public interface ILoggingService
    {
        void Error(string message);

        void Error(string message, Exception exception);

        void Warning(string message);
    }
}