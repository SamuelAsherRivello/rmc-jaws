using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.CognitoIdentity;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Newtonsoft.Json;
using RMC.Core.Exceptions;
using UnityEngine;
using UnityEngine.Events;

#pragma warning disable CS1998 // async/await
namespace RMC.Backend.Baas.Aws
{
	//  Class Attributes ----------------------------------

	/// <summary>
	/// Backend subsystem related to calling backend cloud code.
	/// </summary>
	public class JawsCloudCodeSubsystem : ICloudCodeSubsystem
	{
		//  Events ----------------------------------------
		public CloudCodeEvent OnInitialized { get; } = new CloudCodeEvent();
		public CloudCodeEvent OnMethodCalled { get; } = new CloudCodeEvent();
		

		//  Properties ------------------------------------
		public bool IsInitialized { get { return _isInitialized; }}
		public Configuration Configuration { get { return _configuration; }}

		/// <summary>
		/// Provide RAW access to power users
		/// </summary>
		public AmazonLambdaClient RawAmazonLambdaClient
		{
			get
			{
				RequireIsInitialized();
				RequireConfiguration();
				return _amazonLambdaClient;
			}
		}

		//  Fields ----------------------------------------
		private bool _isInitialized = false;
		private JawsConfiguration _configuration;
		private AmazonLambdaClient _amazonLambdaClient;


		//  Initialization --------------------------------
		public JawsCloudCodeSubsystem()
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

			var credentials = new CognitoAWSCredentials
			(
				_configuration.IdentityPoolId,
				_configuration.RegionEndpoint
			);

			_amazonLambdaClient = new AmazonLambdaClient
			(
				credentials,
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
		public async Task<MethodCallResponse<T>> MethodCallAsync<T>(string functionName, Dictionary<string, string> args) where T : class
		{
			RequireIsInitialized();
			RequireConfiguration();

			MethodCallResponse<T> response = new MethodCallResponse<T>();
			
			try
			{
				var request = new InvokeRequest()
				{
					FunctionName = functionName,
					Payload = JsonConvert.SerializeObject(args),
					InvocationType = InvocationType.RequestResponse
				};
				
				// Invoke the Lambda function
				InvokeResponse invokeResponse = await _amazonLambdaClient.InvokeAsync(request);
					
				// Convert the response payload to a string
				//Debug.LogError("1: " + invokeResponse.Payload);
				var resultString = System.Text.Encoding.UTF8.GetString(invokeResponse.Payload.ToArray());
				//Debug.LogError("2: " + resultString);
				T resultInstance = JsonConvert.DeserializeObject<T>(resultString);
				//Debug.LogError("3: " + resultInstance);

				response.Data = resultInstance;
				response.IsSuccess = true;
			}
			catch (Exception e)
			{
				response.ErrorMessage = e.Message;
				response.IsSuccess = false;
			}

			OnMethodCalled.Invoke(this);
			return response;
		}
	}
}
