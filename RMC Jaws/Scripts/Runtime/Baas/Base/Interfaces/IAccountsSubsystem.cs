using System.Threading.Tasks;

namespace RMC.Backend.Baas
{
    /// <summary>
    /// Defines the API: This subsystem
    /// </summary>
    public interface IAccountsSubsystem : IInitializableAsync, IConfigurable
    {
        //  Properties  ------------------------------------
        AccountsEvent OnInitialized { get; }
        AccountsEvent OnUserCreated { get; }
        AccountsEvent OnUserDeleted { get; }
        AccountsEvent OnUserSignedIn { get; }
        AccountsEvent OnUserSignedOut { get; }
        
        bool HasUser();
        User GetUser();
        
        //  General Methods  ------------------------------
        Task<UserCreateResponse> UserCreateAsync(string email, string password, string nickname);
        Task<UserSignInResponse> UserSignInAsync(string email, string password);
        Task<UserSignOutResponse> UserSignOutAsync(string tokenId);
    }
}