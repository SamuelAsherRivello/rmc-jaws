
namespace RMC.Backend.Baas
{
    /// <summary>
    /// Defines the API: Main entry point to the backend system. 
    /// </summary>
    public interface IBackendSystem : IInitializableAsync
    {
        //  Properties  ------------------------------------
        BackendSystemEvent OnInitialized { get; }
        
        //  General Methods  ------------------------------
    }
}