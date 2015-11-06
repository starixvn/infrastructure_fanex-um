namespace ONEbook.UM.Helpers
{
    using System;
    using System.Linq;

    internal static class RandomHelper
    {
        private static Random _randomHandler = new Random();

        internal static Uri GetRandomUri(string sourceUris)
        {
            if (string.IsNullOrWhiteSpace(sourceUris))
            {
                throw new ArgumentOutOfRangeException("sourceUris", "Uris cannot be null or white space");
            }

            var uris = sourceUris.Split(',');
            var numOfUri = uris.Count();

            if (numOfUri == 1)
            {
                return new Uri(uris[0]);
            }

            var selectedUri = uris[_randomHandler.Next(0, numOfUri)];

            return new Uri(selectedUri);
        }
    }
}