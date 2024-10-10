using System.Threading.Tasks;
using RMC.Core.Exceptions;

namespace RMC.Backend.Baas.Aws
{
	/// <summary>
	/// The main entry point for this <see cref="IBackendSystem"/>
	///
	/// B.U.F.F. For Unity
	///		* Backend
	///		* Unity
	///		* Framework (For)
	///		* Firebase
	/// 
	/// </summary>
	public class Buff : IBackendSystem
	{
		//  Events ----------------------------------------
		public BackendSystemEvent OnInitialized { get; } = new BackendSystemEvent();

		
		//  Properties ------------------------------------
		public static Buff Instance
		{
			get
			{
				if (_Instance == null)
				{
					_Instance = new Buff();
				}
				return _Instance;
			}
		}

		public bool IsInitialized { get { return _isInitialized; }}
		public IAccountsSubsystem AccountsSubsystem { get { return _accountsSubsystem; }}
		public ICloudCodeSubsystem CloudCodeSubsystem { get { return _cloudCodeSubsystem; }}
		public IDatabaseSubsystem DatabaseSubsystem { get { return _databaseSubsystem; }}


		//  Fields ----------------------------------------
		private static Buff _Instance = null;
		private bool _isInitialized = false;
		
		private readonly JawsAccountsSubsystem _accountsSubsystem;    //TODO: Replace with new buff-type here...
		private readonly JawsCloudCodeSubsystem _cloudCodeSubsystem;  //TODO: Replace with new buff-type here...
		private readonly JawsDatabaseSubsystem _databaseSubsystem;    //TODO: Replace with new buff-type here...
        
		
		//  Initialization --------------------------------
		private Buff()
		{
			// Create all subsystems in constructor
			// To allow for any early subscriptions
			_accountsSubsystem = new JawsAccountsSubsystem();
			_cloudCodeSubsystem = new JawsCloudCodeSubsystem();
			_databaseSubsystem = new JawsDatabaseSubsystem();
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
			await _accountsSubsystem.InitializeAsync();
			await _cloudCodeSubsystem.InitializeAsync();
			await _databaseSubsystem.InitializeAsync();
	        
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

