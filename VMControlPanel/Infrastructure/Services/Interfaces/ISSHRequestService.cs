﻿using Core.Entities;

namespace Infrastructure.Services.Interfaces
{
    public interface ISSHRequestService
    {
        Task<string> ExecuteCommandAsync(VirtualMachine virtualMachine, string command, string userId);
        Task<string> GetMetricsAsync(VirtualMachine virtualMachine, string userId);
        void DisposeClientAndStream(VirtualMachine virtualMachine, string userId);
    }
}
