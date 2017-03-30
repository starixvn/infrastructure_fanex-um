namespace ONEbook.UM.Configurations
{
    using System;
    using Models;

    public static class UMClientManager
    {
        private static Action<string> _errorHandler = (message) => { throw new UMClientException(message); };

        internal static Action<string> ErrorHandler => _errorHandler;

        public static void WhenError(Action<string> errorHandler)
        {
            if (errorHandler == null)
            {
                throw new ArgumentNullException(nameof(errorHandler), "Error Handler cannot be null");
            }

            _errorHandler = errorHandler;
        }
    }
}