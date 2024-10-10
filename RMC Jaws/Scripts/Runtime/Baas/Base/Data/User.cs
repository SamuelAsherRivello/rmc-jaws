
namespace RMC.Backend.Baas
{
	/// <summary>
	/// Represents the signed-in local user
	/// </summary>
	public class User
	{
		public readonly string Email;
		public readonly string TokenId;
		public readonly string AccessToken;
		
		public User(string email)
		{
			Email = email;
		}
		public User(string email, string tokenId, string accessToken )
		{
			Email = email;
			TokenId = tokenId;
			AccessToken = accessToken;
		}
	}

}
