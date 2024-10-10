using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime;
using RMC.Core.Exceptions;
using UnityEngine;

#pragma warning disable CS1998 // async/await
namespace RMC.Backend.Baas.Aws
{
	//  Class Attributes ----------------------------------

	/// <summary>
	/// Backend subsystem related to <see cref="User"/>
	/// </summary>
	
	public class JawsAccountsSubsystem : IAccountsSubsystem
	{
		//  Events ----------------------------------------
		public AccountsEvent OnInitialized { get; } = new AccountsEvent();
		public AccountsEvent OnUserCreated { get; } = new AccountsEvent();
		public AccountsEvent OnUserDeleted { get; } = new AccountsEvent();
		public AccountsEvent OnUserSignedIn { get; } = new AccountsEvent();
		public AccountsEvent OnUserSignedOut { get; } = new AccountsEvent();
		
		//  Properties ------------------------------------
		public bool IsInitialized { get { return _isInitialized; }}
		public Configuration Configuration { get { return _configuration; }}
		
		
		/// <summary>
		/// Provide RAW access to power users
		/// </summary>
		public AmazonCognitoIdentityProviderClient RawAmazonAmazonCognitoIdentityProviderClient
		{
			get
			{
				RequireIsInitialized();
				RequireConfiguration();
				return _amazonCognitoIdentityProviderClient;
			}
		}

		
		//  Fields ----------------------------------------
		private bool _isInitialized = false;
		private User _user = null;
		private JawsConfiguration _configuration;
		private AmazonCognitoIdentityProviderClient _amazonCognitoIdentityProviderClient;

        
		//  Initialization --------------------------------
		public JawsAccountsSubsystem()
		{
			
		}
        
		public Task InitializeAsync()
		{
			if (_isInitialized)
			{
				return Task.CompletedTask;
			}

			_configuration = Resources.Load<JawsConfiguration>("JawsConfiguration");
			RequireConfiguration();
	        
			_amazonCognitoIdentityProviderClient = new AmazonCognitoIdentityProviderClient
			(
				new AnonymousAWSCredentials(),
				_configuration.RegionEndpoint
			);

			_isInitialized = true;
			OnInitialized.Invoke(this);
			return Task.CompletedTask;
		}
        
		public void RequireIsInitialized()
		{
			if (!IsInitialized)
			{
				throw new NotInitializedException(this);
			}
		}
        
		public void RequireConfiguration()
		{
			if (_configuration == null)
			{
				throw new Exception("RequireConfiguration() failed. Configuration must exist.");
			}
	        
			if (!_configuration.IsValid())
			{
				throw new Exception("RequireConfiguration() failed. Configuration.IsValid() must be true.");
			}

		}

		//  Methods ---------------------------------------


		public bool HasUser()
		{
			RequireIsInitialized();
			return _user != null;
		}
        
		public User GetUser()
		{
			RequireIsInitialized();
			return _user;
		}

		public async Task<UserCreateResponse> UserCreateAsync(string email, string password, string nickname)
		{
			RequireIsInitialized();
			RequireConfiguration();
	        
			UserCreateResponse response = new UserCreateResponse();
	        
			if (HasUser())
			{
				response.IsSuccess = false;
				response.ErrorMessage = $"SignUpAsync() failed. HasUser must not be '{HasUser()}'.";
				Debug.LogError($"{response.ErrorMessage}");
				return response;
			}

			var nicknameAttrs = new AttributeType
			{
				//Generally Optional. Required here. I chose to add this in the Amazon AWS console.
				Name = "nickname", 
				Value = nickname,
			};
            
			var versionAttrs = new AttributeType
			{
				//Generally Optional. Required here. I chose to add this in the Amazon AWS console.
				Name = "custom:app_version", 
				Value = "0.0.1",
			};

			var userAttrsList = new List<AttributeType>();
			userAttrsList.Add(nicknameAttrs);
			userAttrsList.Add(versionAttrs);

			var signUpRequest = new SignUpRequest
			{
				UserAttributes = userAttrsList,
				Username = email,
				ClientId = _configuration.ClientId,
				Password = password
			};

			try
			{
				SignUpResponse signUpResponse = await _amazonCognitoIdentityProviderClient.SignUpAsync(signUpRequest);
				response.RawResponse = signUpResponse;
				
				bool isSuccess = signUpResponse.HttpStatusCode == HttpStatusCode.OK;
				response.IsSuccess = isSuccess;
			}
			catch (Exception e)
			{
				response.ErrorMessage = $"{e.Message}";
			}
            
			OnUserCreated.Invoke(this);
			return response;

		}

        
		public async Task<UserSignInResponse> UserSignInAsync(string email, string password)
		{
			RequireIsInitialized();
			RequireConfiguration();
	        
			UserSignInResponse response = new UserSignInResponse();
	        
			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
			{
				response.IsSuccess = false;
				response.ErrorMessage = $"Bad parameters. Details...";
				Debug.LogError($"{response.ErrorMessage}");
				return response;
			}
            
			var authParameters = new Dictionary<string, string>();
			authParameters.Add("USERNAME", email);
			authParameters.Add("PASSWORD", password);

			var authRequest = new InitiateAuthRequest
			{
				ClientId = _configuration.ClientId,
				AuthParameters = authParameters,
				AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
			};

			try
			{
				InitiateAuthResponse initiateAuthResponse = await _amazonCognitoIdentityProviderClient.InitiateAuthAsync(authRequest);
				response.RawResponse = initiateAuthResponse;
				
				response.AccessToken = initiateAuthResponse.AuthenticationResult.AccessToken;
				response.IdToken = initiateAuthResponse.AuthenticationResult.IdToken;
				response.IsSuccess = true;
				_user = new User(email, response.IdToken, response.AccessToken );
			}
			catch (Exception e)
			{
				response.IsSuccess = false;
				response.ErrorMessage = e.Message;
			}
        
			OnUserSignedIn.Invoke(this);
			return response;
		}


		public async Task<UserSignOutResponse> UserSignOutAsync(string accessToken)
		{
			RequireIsInitialized();
			RequireConfiguration();
	        
			UserSignOutResponse response = new UserSignOutResponse();
	        
			if (string.IsNullOrEmpty(accessToken))
			{
				response.IsSuccess = false;
				response.ErrorMessage = $"Bad parameters. Details...";
				Debug.LogError($"{response.ErrorMessage}");
				return response;
			}
            
			var signOutRequest = new GlobalSignOutRequest
			{
				AccessToken = accessToken
			};
			
			try
			{
				GlobalSignOutResponse globalSignOutResponse = await _amazonCognitoIdentityProviderClient.GlobalSignOutAsync(signOutRequest);
				response.RawResponse = globalSignOutResponse;
				response.IsSuccess = true;
				_user = null;
			}
			catch (Exception e)
			{
				response.IsSuccess = false;
				response.ErrorMessage = e.Message;
			}
        
			OnUserSignedOut.Invoke(this);
			return response;
		}

	}
}
