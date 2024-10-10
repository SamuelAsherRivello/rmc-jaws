
using System.Threading.Tasks;

namespace RMC.Backend.Baas
{
    /// <summary>
    /// Defines the API: This subsystem
    /// </summary>
    public interface IDatabaseSubsystem : IInitializableAsync, IConfigurable
    {
        //  Properties  ------------------------------------
        DatabaseEvent OnInitialized { get; }
        DatabaseEvent OnTableRead { get; }
        DatabaseEvent OnItemCreated { get; }
        DatabaseEvent OnItemRead { get; }
        DatabaseEvent OnItemUpdated { get; }
        
        
        //  General Methods  ------------------------------
        Task<TableReadResponse> TableReadAsync(string tableName);
        Task<ItemReadResponse> ItemReadAsync(Table table, User user, InventoryItem item);
        Task<ItemCreateResponse> ItemCreateAsync(Table table, User user, InventoryItem item);
        Task<ItemUpdateResponse> ItemUpdateAsync(Table table, User user, InventoryItem item);
    }
}