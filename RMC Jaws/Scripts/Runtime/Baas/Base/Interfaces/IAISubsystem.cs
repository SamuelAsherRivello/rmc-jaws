using System.Threading.Tasks;

namespace RMC.Backend.Baas
{
    /// <summary>
    /// Defines the API: This subsystem
    /// </summary>
    public interface IAISubsystem : IInitializableAsync, IConfigurable
    {
        //  Properties  ------------------------------------
        AIEvent OnInitialized { get; }
        
        //  General Methods  ------------------------------
        Task<ListAIModelsResponse> ListAIModels();
        Task<InvokeAIModelResponse> InvokeAIModel(InvokeAIModelRequest invokeAIModelRequest);
    }
}