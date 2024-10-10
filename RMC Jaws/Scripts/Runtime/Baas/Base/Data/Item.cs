
namespace RMC.Backend.Baas
{
	/// <summary>
	/// Represents the single item in database
	/// </summary>
	public class Item
	{
		public string Name;

		public Item()
		{
		}
		
		public Item(string name)
		{
			Name = name;
		}
		
	
	}
	
	public class InventoryItem : Item
	{
		public int Quantity = 0;
	}

}
