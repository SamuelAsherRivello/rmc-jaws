using System.Threading.Tasks;
using RMC.Core.Exceptions;
using UnityEngine;

namespace RMC.Backend.Baas.Aws
{
	/// <summary>
	/// The main entry point for this <see cref="IBackendSystem"/>
	///
	/// J.A.W.S. For Unity
	///		* Just (For Unity And)
	///		* Amazon
	///		* Web
	///		* Services
	/// 
	/// </summary>
	public class Jaws : IBackendSystem
	{
		//  Events ----------------------------------------
		public BackendSystemEvent OnInitialized { get; } = new BackendSystemEvent();

		
		//  Properties ------------------------------------
		public static Jaws Instance
		{
			get
			{
				if (_Instance == null)
				{
					_Instance = new Jaws();
				}
				return _Instance;
			}
		}

		public bool IsInitialized { get { return _isInitialized; }}
		public IAccountsSubsystem Accounts { get { return _accounts; }}
		public IDatabaseSubsystem Database { get { return _database; }}
		
		public ICloudCodeSubsystem CloudCode { get { return _cloudCode; }}
		public IAISubsystem AI { get { return _ai; }}


		//  Fields ----------------------------------------
		private bool _isInitialized = false;
		private readonly JawsAccountsSubsystem _accounts;
		private readonly JawsCloudCodeSubsystem _cloudCode;
		private readonly JawsDatabaseSubsystem _database;
		private readonly JawsAISubsystem _ai;
		
		
		//  Statics ----------------------------------------
		private static Jaws _Instance = null;
		
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeOnLoad()
		{
			//Manually clear since fast compile is supported
			//https://docs.unity3d.com/6000.0/Documentation/ScriptReference/InitializeOnEnterPlayModeAttribute.html
			_Instance = null;
		}
        
		
		//  Initialization --------------------------------
		private Jaws()
		{
			// Create all subsystems in constructor
			// To allow for any early subscriptions
			_accounts = new JawsAccountsSubsystem();
			_cloudCode = new JawsCloudCodeSubsystem();
			_database = new JawsDatabaseSubsystem();
			_ai = new JawsAISubsystem();
		}
        
		
		/// <summary>
		/// Initialize
		/// </summary>
		public async Task InitializeAsync()
		{
			if (_isInitialized)
			{
				return;
			}
			_isInitialized = true;
	        
			// Initialize Subsystems
			await _accounts.InitializeAsync();
			await _cloudCode.InitializeAsync();
			await _database.InitializeAsync();
			await _ai.InitializeAsync();
	        
			//
			OnInitialized.Invoke(this);
		}
        
		
		/// <summary>
		/// Require initialization. Usage is optional
		/// </summary>
		/// <exception cref="NotInitializedException"></exception>
		public void RequireIsInitialized()
		{
			if (!IsInitialized)
			{
				throw new NotInitializedException(this);
			}
		}

		//  Methods ---------------------------------------

	}
}

