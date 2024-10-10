
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RMC.Backend.Baas
{
    /// <summary>
    /// Defines the API: This subsystem
    /// </summary>
    public interface ICloudCodeSubsystem : IInitializableAsync, IConfigurable
    {
        //  Properties  ------------------------------------
        CloudCodeEvent OnInitialized { get; }
        CloudCodeEvent OnMethodCalled { get; }
        
        //  General Methods  ------------------------------
        Task<MethodCallResponse<T>>MethodCallAsync<T>(string functionName, Dictionary<string, string> args) where T : class;
    }
}