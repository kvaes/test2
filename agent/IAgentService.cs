using System;
using System.Threading.Tasks;

namespace Agent;

public interface IAgentService
{
    Task InitializeAsync();
    Task ProcessRequestAsync(string request);
    Task ShutdownAsync();
}