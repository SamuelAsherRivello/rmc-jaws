using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using RMC.Core.Exceptions;
using UnityEngine;


#pragma warning disable CS1998 // async/await
namespace RMC.Backend.Baas.Aws
{
	//  Class Attributes ----------------------------------


	/// <summary>
	/// Backend subsystem related to database tables.
	/// </summary>
	public class JawsDatabaseSubsystem : IDatabaseSubsystem
	{
		//  Events ----------------------------------------
		public DatabaseEvent OnInitialized { get; } = new DatabaseEvent();
		public DatabaseEvent OnTableRead { get; } = new DatabaseEvent();
		public DatabaseEvent OnItemCreated { get; } = new DatabaseEvent();
		public DatabaseEvent OnItemRead { get; } = new DatabaseEvent();
		public DatabaseEvent OnItemUpdated { get; } = new DatabaseEvent();
		
		
		//  Properties ------------------------------------
		public bool IsInitialized { get { return _isInitialized; } }
		public Configuration Configuration { get { return _configuration; } }

		
		/// <summary>
		/// Provide RAW access to power users
		/// </summary>
		public AmazonDynamoDBClient RawAmazonDynamoDBClient
		{
			get
			{
				RequireIsInitialized();
				RequireConfiguration();
				return _amazonDynamoDBClient;
			}
		}

		
		//  Fields ----------------------------------------
		private bool _isInitialized = false;
		private JawsConfiguration _configuration;
		private AmazonDynamoDBClient _amazonDynamoDBClient;
		private DynamoDBContext _databaseContext;


		//  Initialization --------------------------------
		public JawsDatabaseSubsystem()
		{

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
			
			_amazonDynamoDBClient = new AmazonDynamoDBClient(credentials, _configuration.RegionEndpoint);

			_isInitialized = true;
			OnInitialized.Invoke(this);
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
		public async Task<TableReadResponse> TableReadAsync(string tableName)
		{
			TableReadResponse response = new TableReadResponse();
			
			var request = new DescribeTableRequest
			{
				TableName = tableName
			};

			try
			{
				var describeTableResponse = await _amazonDynamoDBClient.DescribeTableAsync(request);
				if (describeTableResponse.HttpStatusCode == HttpStatusCode.OK)
				{
					response.IsSuccess = true;
					response.Table = new Table();
					response.Table.TableName = describeTableResponse.Table.TableName;
					response.Table.ItemCount = describeTableResponse.Table.ItemCount;
				}
				else
				{
					response.IsSuccess = false;
					response.ErrorMessage = $"TableReadAsync() failed. Not Ok.";
				}
			}
			catch (Exception e)
			{
				response.IsSuccess = false;
				response.ErrorMessage = $"TableReadAsync() failed. error = {e.Message}.";
			}

			OnTableRead.Invoke(this);
			return response;
		}
		
		
		public async Task<ItemReadResponse> ItemReadAsync(Table table, User user, InventoryItem item)
		{
			ItemReadResponse response = new ItemReadResponse();
			
			if (table == null || string.IsNullOrEmpty(table.TableName))
			{
				response.IsSuccess = false;
				response.ErrorMessage = $"ItemReadAsync() failed. Table.TableName must not be '{table?.TableName}'.";
				return response;
			}
			
			if (user == null || string.IsNullOrEmpty(user.Email))
			{
				response.IsSuccess = false;
				response.ErrorMessage = $"ItemReadAsync() failed. User.Email must not be '{user?.Email}'.";
				return response;
			}
			
			if (item == null || string.IsNullOrEmpty(item.Name))
			{
				response.IsSuccess = false;
				response.ErrorMessage = $"ItemReadAsync() failed. Item.Name must not be '{item?.Name}'.";
				return response;
			}
			
			var key = new Dictionary<string, AttributeValue>
			{
				{ "email".ToLower(), new AttributeValue { S = user.Email } }
			};

			var request = new GetItemRequest()
			{
				TableName = table.TableName,
				Key = key,
			};

			try
			{
				
				var getItemResponse = await _amazonDynamoDBClient.GetItemAsync(request);
				
				if (getItemResponse.HttpStatusCode == HttpStatusCode.OK 
				    && getItemResponse.Item.ContainsKey(item.Name.ToLower()))
				{
					var itemOfInterest = getItemResponse.Item[item.Name.ToLower()];
					response.IsSuccess = true;
					response.Table = table; //Caution: This is not FRESH from the server
					response.Item = new InventoryItem();
					response.Item.Name = itemOfInterest.S;
					response.Item.Quantity = int.Parse(itemOfInterest.N);

				}
				else
				{
					response.IsSuccess = false;
					response.ErrorMessage = $"ItemReadAsync() failed. Not Ok.";
				}
			}
			catch (Exception e)
			{
				response.IsSuccess = false;
				response.ErrorMessage = $"ItemReadAsync() failed. error = {e.Message}.";
			}

			OnItemRead.Invoke(this);
			return response;
		}

		
		public async Task<ItemCreateResponse> ItemCreateAsync(Table table, User user, InventoryItem item)
		{
			ItemCreateResponse response = new ItemCreateResponse();
			
			if (table == null || string.IsNullOrEmpty(table.TableName))
			{
				response.IsSuccess = false;
				response.ErrorMessage = $"ItemCreateAsync() failed. Table.TableName must not be '{table?.TableName}'.";
				return response;
			}
			
			if (user == null || string.IsNullOrEmpty(user.Email))
			{
				response.IsSuccess = false;
				response.ErrorMessage = $"ItemCreateAsync() failed. User.Email must not be '{user?.Email}'.";
				return response;
			}
			
			if (item == null || string.IsNullOrEmpty(item.Name))
			{
				response.IsSuccess = false;
				response.ErrorMessage = $"ItemCreateAsync() failed. Item.Name must not be '{item?.Name}'.";
				return response;
			}
			
			var itemAttributes = new Dictionary<string, AttributeValue>
			{
				{ "email".ToLower(), new AttributeValue { S = user.Email } },
				{ item.Name.ToLower(), new AttributeValue { N = item.Quantity.ToString() } }
			};
			
			var request = new PutItemRequest
			{
				TableName = table.TableName,
				Item = itemAttributes
			};

			try
			{
				var putItemResponse = await _amazonDynamoDBClient.PutItemAsync(request);
				if (putItemResponse.HttpStatusCode == HttpStatusCode.OK)
				{
					response.IsSuccess = true;
					response.Table = table; //Caution: This is not FRESH from the server
					response.Item = item; //Caution: This is not FRESH from the server
					
					
					// Check the attributes that were changed
					if (putItemResponse.Attributes != null && putItemResponse.Attributes.Count > 0)
					{
						foreach (var attribute in putItemResponse.Attributes)
						{
							Debug.Log($"Attribute {attribute.Key} was changed to {attribute.Value}");
						}
					}
				}
				else
				{
					response.IsSuccess = false;
					response.ErrorMessage = $"ItemCreateAsync() failed. Not Ok.";
				}
			}
			catch (Exception e)
			{
				response.IsSuccess = false;
				response.ErrorMessage = $"ItemCreateAsync() failed. error = {e.Message}.";
			}

			OnItemCreated.Invoke(this);
			return response;
		}
		
		public async Task<ItemUpdateResponse> ItemUpdateAsync(Table table, User user, InventoryItem item)
		{
			ItemUpdateResponse response = new ItemUpdateResponse();
			
			if (table == null || string.IsNullOrEmpty(table.TableName))
			{
				response.IsSuccess = false;
				response.ErrorMessage = $"ItemUpdateAsync() failed. Table.TableName must not be '{table?.TableName}'.";
				return response;
			}
			
			if (user == null || string.IsNullOrEmpty(user.Email))
			{
				response.IsSuccess = false;
				response.ErrorMessage = $"ItemUpdateAsync() failed. User.Email must not be '{user?.Email}'.";
				return response;
			}
			
			if (item == null || string.IsNullOrEmpty(item.Name))
			{
				response.IsSuccess = false;
				response.ErrorMessage = $"ItemUpdateAsync() failed. Item.Name must not be '{item?.Name}'.";
				return response;
			}
			
			var key = new Dictionary<string, AttributeValue>
			{
				{ "email".ToLower(), new AttributeValue { S = user.Email } }
			};

			var attributeUpdates = new Dictionary<string, AttributeValueUpdate>
			{
				{
					item.Name.ToLower(), 
					new AttributeValueUpdate
					{
						Action = AttributeAction.PUT, 
						Value = new AttributeValue { N = item.Quantity.ToString() }
					}
				}
			};

			var request = new UpdateItemRequest
			{
				TableName = table.TableName,
				Key = key,
				AttributeUpdates = attributeUpdates
			};

			try
			{
				var updateItemResponse = await _amazonDynamoDBClient.UpdateItemAsync(request);
				if (updateItemResponse.HttpStatusCode == HttpStatusCode.OK)
				{
					response.IsSuccess = true;
					response.Table = table; //Caution: This is not FRESH from the server
					response.Item = item; //Caution: This is not FRESH from the server
					
				}
				else
				{
					response.IsSuccess = false;
					response.ErrorMessage = $"ItemUpdateAsync() failed. Not Ok.";
				}
			}
			catch (Exception e)
			{
				response.IsSuccess = false;
				response.ErrorMessage = $"ItemUpdateAsync() failed. error = {e.Message}.";
			}

			OnItemUpdated.Invoke(this);
			return response;
		}

		
	}
}
