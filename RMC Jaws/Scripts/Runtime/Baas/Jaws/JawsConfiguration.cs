using Amazon;
using UnityEngine;

namespace RMC.Backend.Baas.Aws
{
	//  Class Attributes ----------------------------------
	
	/// <summary>
	/// Backend configuration details. 
	/// </summary>
	[CreateAssetMenu (
		fileName = "JawsConfiguration",
		menuName = "RMC/RMC Jaws/JawsConfiguration",
		order = -1000)]

	public class JawsConfiguration : Configuration
	{
		//  Properties ------------------------------------
		public string ClientId { get { return _clientId; }}
		public string IdentityPoolId { get { return _identityPoolId; }}
		public RegionEndpoint RegionEndpoint { get { return _regionEndpoint; }}
		
		//  Fields ----------------------------------------
        
		// Update to match the EXACT value from Amazon
		[SerializeField]
		private string _clientId = "";
        
		// Update to match the EXACT value from Amazon
		[SerializeField]
		private string _identityPoolId = "";

		[SerializeField]
		// Update to match the EXACT value from Amazon
		private RegionEndpoint _regionEndpoint = RegionEndpoint.USEast1;

		//  Methods ---------------------------------------
		public override bool IsValid()
		{
			return !string.IsNullOrEmpty(ClientId);
		}
	}
}
