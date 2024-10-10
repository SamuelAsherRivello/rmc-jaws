
namespace RMC.Backend.Baas
{
	/// <summary>
	/// Response from async calls
	/// </summary>
	public class Response
	{
		public object RawResponse { get; set; }
		public bool IsSuccess { get; set; }
		public string ErrorMessage { get; set; }
		public const string ErrorMessageDefault = "ErrorMessageDefault";
		
		public Response()
		{
			IsSuccess = false;
			ErrorMessage = ErrorMessageDefault;
		}
	}
	
	//Accounts
	public class UserCreateResponse : Response
	{
		
	}
	
	//Accounts
	public class UserSignInResponse : Response
	{
		//TODO: Does it make sense to store BOTH of these here?
		
		
		//Purpose: Used to get user profile information.
		public string IdToken { get; set; }
		
		//Purpose: Used to authorize access to secured resources (e.g., API Gateway, AWS services).
		public string AccessToken { get; set; }
		
	}
	
	public class UserSignOutResponse : Response
	{
		
	}
	
	//Cloud Code
	public class MethodCallResponse<T> : Response
	{
		public T Data { get; set; }
	}
	
	//Database
	public class Table
	{
		public string TableName { get; set; }
		public long ItemCount { get; set; }

		public Table()
		{
		}
		
		public Table(string tableName)
		{
			TableName = tableName;
		}
	}
	
	//Database
	public class TableReadResponse : Response
	{
		public Table Table { get; set; }
	}
	
	public class ItemCreateResponse : Response
	{
		public Table Table { get; set; }
		public InventoryItem Item { get; set; }
	}
	
	public class ItemUpdateResponse: Response
	{
		public Table Table { get; set; }
		public InventoryItem Item { get; set; }
	}
	
	public class ItemReadResponse: Response
	{
		public Table Table { get; set; }
		public InventoryItem Item { get; set; }
	}
	
	//AI
	public class ListAIModelsResponse: Response
	{
	
	}
	
	public class InvokeAIModelResponse: Response
	{
	
	}
	
}
