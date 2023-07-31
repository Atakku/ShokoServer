// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;

namespace Shoko.Server.Services.Connectivity;

public interface IConnectivityMonitor
{
    public Task ExecuteCheckAsync(CancellationToken token);
    public bool HasConnected { get; }
}
