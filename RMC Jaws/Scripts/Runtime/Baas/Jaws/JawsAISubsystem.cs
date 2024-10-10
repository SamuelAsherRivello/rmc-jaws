using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon.Bedrock;
using Amazon.Bedrock.Model;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.CognitoIdentity;
using Amazon.Runtime;
using Newtonsoft.Json;
using RMC.Core.Exceptions;
using UnityEngine;

#pragma warning disable CS1998 // async/await
namespace RMC.Backend.Baas.Aws
{
	//  Class Attributes ----------------------------------

	/// <summary>
	/// Backend subsystem related to calling AI backend.
	/// </summary>
	public class JawsAISubsystem : IAISubsystem
	{
		//  Events ----------------------------------------
		public AIEvent OnInitialized { get; } = new AIEvent();
		

		//  Properties ------------------------------------
		public bool IsInitialized { get { return _isInitialized; }}
		public Configuration Configuration { get { return _configuration; }}

		/// <summary>
		/// Provide RAW access to power users
		/// </summary>
		public AmazonBedrockRuntimeClient RawAmazonBedrockRuntimeRuntimeClient
		{
			get
			{
				RequireIsInitialized();
				RequireConfiguration();
				return _amazonBedrockRuntimeClient;
			}
		}

		//  Fields ----------------------------------------
		private bool _isInitialized = false;
		private JawsConfiguration _configuration;
		private AmazonBedrockRuntimeClient _amazonBedrockRuntimeClient;
		private AmazonBedrockClient _amazonBedrockClient;


		//  Initialization --------------------------------
		public JawsAISubsystem()
		{
			Debug.Log("00000000000: " + _isInitialized);
		}

		public async Task InitializeAsync()
		{
			if (_isInitialized)
			{
				return;
			}

			_configuration = Resources.Load<JawsConfiguration>("JawsConfiguration");
			RequireConfiguration();

			var credentials = new CognitoAWSCredentials
			(
				_configuration.IdentityPoolId,
				_configuration.RegionEndpoint
			);
			
			_amazonBedrockRuntimeClient = new AmazonBedrockRuntimeClient
			(
				credentials,
				_configuration.RegionEndpoint
			);

			_amazonBedrockClient = new AmazonBedrockClient
			(
				credentials,
				_configuration.RegionEndpoint
			);

			_isInitialized = true;
			OnInitialized.Invoke(this);
			return;
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
		public async Task<ListAIModelsResponse> ListAIModels()
		{
			Console.WriteLine("List foundation models with no filter");

			ListAIModelsResponse response = new ListAIModelsResponse();
			try
			{
				ListFoundationModelsResponse listFoundationModelsResponse = await _amazonBedrockClient.ListFoundationModelsAsync(new ListFoundationModelsRequest()
				{
				});

				if (listFoundationModelsResponse?.HttpStatusCode == System.Net.HttpStatusCode.OK)
				{
					foreach (var fm in listFoundationModelsResponse.ModelSummaries)
					{
						Debug.Log(fm.ModelName);
					}
				}
				else
				{
					Console.WriteLine("Something wrong happened");
				}
			}
			catch (AmazonBedrockException e)
			{
				Console.WriteLine(e.Message);
			}

			response.IsSuccess = true;
			return response;
		}
		
		public async Task<InvokeAIModelResponse> InvokeAIModel (InvokeAIModelRequest invokeAIModelRequest ) 
		{
			RequireIsInitialized();
			RequireConfiguration();

			InvokeAIModelResponse response = new InvokeAIModelResponse();
			
			var modelId = "amazon.titan-text-express-v1";
			
			var nativeRequest = JsonConvert.SerializeObject(new
			{
				inputText = "What is your favorite food?",
				textGenerationConfig = new
				{
					maxTokenCount = 512,
					temperature = 0.5
				}
			});

			var request = new InvokeModelRequest
			{
				Body = new MemoryStream(Encoding.UTF8.GetBytes(nativeRequest)),
				ContentType = "application/json",
				ModelId = modelId
			};
			
			try
			{
				var invokeModelResponse = await _amazonBedrockRuntimeClient.InvokeModelAsync(request);
        
				//TODO: Change to async?
				var responseBody = new StreamReader(invokeModelResponse.Body).ReadToEnd();
				Debug.Log("Response: " + responseBody);

				response.IsSuccess = true;
			}
			catch (AmazonServiceException ex)
			{
				Debug.LogError($"AmazonServiceException: {ex.Message}");
				Debug.LogError($"Status Code: {ex.StatusCode}");
				Debug.LogError($"Error Code: {ex.ErrorCode}");
				Debug.LogError($"Request ID: {ex.RequestId}");
			}
			catch (Exception ex)
			{
				Debug.LogError($"Exception: {ex.Message}");
			}
			return response;
		}
	}
}
