using System.Threading.Tasks;

namespace RMC.Backend.Baas
{
    /// <summary>
    /// Defines the API: Initialization flow
    /// </summary>
    public interface IInitializableAsync
    {
        //  Properties  ------------------------------------
        public bool IsInitialized { get; }

        //  General Methods  ------------------------------
        public Task InitializeAsync();
        void RequireIsInitialized();
    }
}