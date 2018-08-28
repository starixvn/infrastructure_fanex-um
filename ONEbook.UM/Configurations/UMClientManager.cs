namespace ONEbook.UM.Configurations
{
    using System;
    using Models;

    public static class UMClientManager
    {
        internal static Action<string> ErrorHandler { get; private set; } = (message) => throw new UMClientException(message);

        public static void WhenError(Action<string> errorHandler)
        {
            ErrorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler), "Error Handler cannot be null");
        }
    }
}