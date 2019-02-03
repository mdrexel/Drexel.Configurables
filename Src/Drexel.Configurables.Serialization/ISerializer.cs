﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Drexel.Configurables.Contracts.Configurations;

namespace Drexel.Configurables.Serialization
{
    public interface ISerializer : IDisposable
    {
        Task SerializeAsync(Configuration configuration, CancellationToken cancellationToken = default);
    }
}
