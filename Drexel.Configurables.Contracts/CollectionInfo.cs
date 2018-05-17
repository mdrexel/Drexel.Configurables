﻿namespace Drexel.Configurables.Contracts
{
    public sealed class CollectionInfo
    {
        public CollectionInfo(int minimumCount, int? maximumCount = null)
        {
            this.MinimumCount = minimumCount;
            this.MaximumCount = maximumCount;
        }

        public int MinimumCount { get; }

        public int? MaximumCount { get; }
    }
}
