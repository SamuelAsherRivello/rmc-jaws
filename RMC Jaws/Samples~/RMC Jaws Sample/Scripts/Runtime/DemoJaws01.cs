using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;


namespace RMC.Backend.Baas.Aws.Samples
{
    //  Classes ------------------------------------
    
    // Demo-specific table
    public class InventoryTable : Table
    {
        public InventoryTable()
        {
            TableName = "InventoryTable";
        }
    }

    // Demo-specific item
    public class GoldInventoryItem : InventoryItem
    {
        public GoldInventoryItem()
        {
            Name = "Gold";
            Quantity = 0;
        }
    }
    
    
    // Demo-specific item
    public class DemoModel
    {
        public bool DatabaseHasTable = false;
        public int DatabaseItemQuantityPrevious = 1;
    }

    
    /// <summary>
    /// Main entry point for the Scene.
    /// </summary>
    public class DemoJaws01 : MonoBehaviour
    {
        
        //  Properties ------------------------------------
        private Button AccountsSignInButton { get { return _hudUI.Button01; }}
        private Button AccountsSignOutButton { get { return _hudUI.Button02; }}
        private Button DatabaseCreateTableButton { get { return _hudUI.Button03; }}
        private Button DatabaseUpdateTableButton { get { return _hudUI.Button04; }}
        private Button CloudCodeMethodCallButton { get { return _hudUI.Button05; }}
        private Button AIInvokeButton { get { return _hudUI.Button06; }}
        

        //  Fields ----------------------------------------
        [Header("UI")]
        [SerializeField]
        private HudUI _hudUI;

        // AWS

        //TODO: Optional, Add Unity UI to allow user to type in this info. 
        private const string _descriptionPrefix = "Jaws For Unity - ";
        private const string _userEmail = "test.email@test.email.com";
        private const string _userPassword = "test.password!AND@#%123";
        private const string _userNickname = "test.nickname";
        
        // State
        private DemoModel _demoModel = new DemoModel();
        private const int _flickerDurationMS = 100; //Make reload obvious with a UI flicker

        
        //  Unity Methods ---------------------------------
        protected async void Start()
        {
            Debug.Log($"{GetType().Name}.Start()");
            
            // Observe Jaws
            Jaws.Instance.OnInitialized.AddListener((jaws) => { Debug.LogWarning("@@ Jaws.OnInitialized()"); });
            
            // Observe Accounts
            Jaws.Instance.Accounts.OnUserCreated.AddListener((accounts) => { Debug.LogWarning("@@ Jaws.OnUserCreated()");  });
            Jaws.Instance.Accounts.OnUserDeleted.AddListener((accounts) => { Debug.LogWarning("@@ Jaws.OnUserDeleted()"); });
            Jaws.Instance.Accounts.OnUserSignedIn.AddListener(async (accounts) => { Debug.LogWarning("@@ Jaws.OnUserSignedIn()"); await RefreshUI();});
            Jaws.Instance.Accounts.OnUserSignedOut.AddListener(async (accounts) => { Debug.LogWarning("@@ Jaws.OnUserSignedOut()"); await RefreshUI();});
            
            // Observe CloudCode
            Jaws.Instance.CloudCode.OnMethodCalled.AddListener((cloudCode) => { Debug.LogWarning("@@ Jaws.OnMethodCalled()"); });
            
            // Observe Database
            Jaws.Instance.Database.OnTableRead.AddListener((database) => { Debug.LogWarning("@@ Jaws.OnTableRead()"); });
            Jaws.Instance.Database.OnItemCreated.AddListener((database) => { Debug.LogWarning("@@ Jaws.OnItemCreated()"); });
            Jaws.Instance.Database.OnItemRead.AddListener((database) => { Debug.LogWarning("@@ Jaws.OnItemRead()"); });
            Jaws.Instance.Database.OnItemUpdated.AddListener((database) => { Debug.LogWarning("@@ Jaws.OnItemUpdated()"); });
            
            // Set Interactivity
            _hudUI.IsEnabledUI = true;

            // UI
            _hudUI.ClearTextField(_hudUI.InputTextField01);
            _hudUI.ClearTextField(_hudUI.InputTextField02);
            _hudUI.ClearTextField(_hudUI.OutputTextField01);
            _hudUI.ClearTextField(_hudUI.OutputTextField02);
            
            AccountsSignInButton.text = "1. Create & Sign In\n(Accounts)";
            AccountsSignInButton.clicked += async () =>
            {
                await DoAccountsSignIn();
            };
            
            AccountsSignOutButton.text = "2. Sign Out\n(Accounts)";
            AccountsSignOutButton.clicked += async () =>
            {
                await DoAccountsSignOut();
            };
            
            
            DatabaseCreateTableButton.text = "3. Create Table\n(Database)";
            DatabaseCreateTableButton.clicked += async () =>
            {
                // Create Table
                await DoDatabaseCreateTable();
                
                // Update Table (Resetting the Item Quantity)
                GoldInventoryItem item = new GoldInventoryItem();
                item.Quantity = 1;
                await DoDatabaseUpdateTable(item);
                
            };
            
            DatabaseUpdateTableButton.text = "4. Update Table\n(Database)";
            DatabaseUpdateTableButton.clicked += async () =>
            {
                // Update Table (Incrementing Item Quantity)
                GoldInventoryItem item = new GoldInventoryItem();
                item.Quantity = _demoModel.DatabaseItemQuantityPrevious + 1;
                await DoDatabaseUpdateTable(item);
            };
            
            CloudCodeMethodCallButton.text = "5. Method Call\n(CloudCode)";
            CloudCodeMethodCallButton.clicked += async () =>
            {
                await DoCloudCodeMethodCall();
            };
            
            AIInvokeButton.text = "6. Invoke\n(AI)";
            AIInvokeButton.clicked += async () =>
            {
                await DoAIInvoke();
            };
            
            // Init
            await Jaws.Instance.InitializeAsync();
            
            // UI
            await RefreshUI();
            
            // Load first demo
            await DoAccountsSignIn();

        }



        private async Task RefreshUI()
        {
            // Set Interactivity
            _hudUI.IsEnabledUI = false;
            
            // Don't use ****** during demos.
            _hudUI.InputTextField01.isPasswordField = false;
            _hudUI.InputTextField02.isPasswordField = false;
            _hudUI.OutputTextField01.isPasswordField = false;
            _hudUI.OutputTextField02.isPasswordField = false;

            //Cosmetic delay
            await Task.Delay(_flickerDurationMS/2);
            _hudUI.TitleLabel.text = $"{SceneManager.GetActiveScene().name}";
            
            //
            bool hasUser = Jaws.Instance.Accounts.HasUser();
            AccountsSignInButton.SetEnabled(!hasUser);
            AccountsSignOutButton.SetEnabled(hasUser);
            DatabaseCreateTableButton.SetEnabled(hasUser); //Allow calling this regardless of hasTable
            DatabaseUpdateTableButton.SetEnabled(hasUser && _demoModel.DatabaseHasTable);
            CloudCodeMethodCallButton.SetEnabled(hasUser);
            AIInvokeButton.SetEnabled(hasUser);
            
            //Cosmetic delay
            await Task.Delay(_flickerDurationMS/2);
            
            // Set Interactivity
            _hudUI.IsEnabledUI = true;
        }


        //  Methods ---------------------------------------
        private async Task DoAccountsSignIn()
        {
            // UI
            await RefreshUI();
            _hudUI.IsEnabledUI = false;
            _hudUI.DescriptionLabel.text = $"{_descriptionPrefix}Create / Sign In a User";
            _hudUI.InputTextField01.label = $"Email";
            _hudUI.InputTextField01.value = $"{_userEmail}";
            
            _hudUI.InputTextField02.label = $"Password";
            _hudUI.InputTextField02.value = $"{_userPassword}";
            _hudUI.ClearTextField(_hudUI.OutputTextField01);
            _hudUI.ClearTextField(_hudUI.OutputTextField02);

            
            
            
            //////////////////////////////////////
            // Subsystem: Accounts
            //////////////////////////////////////
            
            // Check
            if (!Jaws.Instance.Accounts.HasUser())
            {
                // Create User
               
                Stopwatch stopwatch = new Stopwatch();
                
                _hudUI.MeasureLatencyBegin();
                var userCreateResponse = await Jaws.Instance.Accounts.UserCreateAsync(_userEmail, _userPassword, _userNickname);
                _hudUI.MeasureLatencyEnd();
                
                LogResponse($"Jaws #1: userCreateResponse", userCreateResponse);
                if (!userCreateResponse.IsSuccess)
                {
                    _hudUI.OutputTextField02SetError(userCreateResponse.ErrorMessage);
                }

                // Sign In User
                _hudUI.MeasureLatencyBegin();
                var userSignInResponse = await Jaws.Instance.Accounts.UserSignInAsync(_userEmail, _userPassword);
                _hudUI.MeasureLatencyEnd();
                
                LogResponse($"Jaws #2: userSignInResponse", userSignInResponse);
                if (!userSignInResponse.IsSuccess)
                {
                    _hudUI.OutputTextField02SetError(userCreateResponse.ErrorMessage);
                }
            }
            
            // Results
            var user = Jaws.Instance.Accounts.GetUser();
            
            // UI
            _hudUI.OutputTextField01.label = $"User";
            _hudUI.OutputTextField01.value = $"Email = {user.Email}, TokenId = {user.TokenId}, AccessToken = {user.AccessToken}";
            _hudUI.ClearTextField(_hudUI.OutputTextField01);
            _hudUI.ClearTextField(_hudUI.OutputTextField02);
   
            _hudUI.IsEnabledUI = true;
        }

        
        private async Task DoAccountsSignOut()
        {
            // UI
            await RefreshUI();
            _hudUI.IsEnabledUI = false;
            _hudUI.DescriptionLabel.text = $"{_descriptionPrefix}Sign Out a User";
            _hudUI.InputTextField01.label = $"Email";
            _hudUI.InputTextField01.value = $"{_userEmail}";
            
            _hudUI.InputTextField02.label = $"Password";
            _hudUI.InputTextField02.value = $"{_userPassword}";
            _hudUI.ClearTextField(_hudUI.OutputTextField01);
            _hudUI.ClearTextField(_hudUI.OutputTextField02);
            
            
            //////////////////////////////////////
            // Subsystem: Accounts
            //////////////////////////////////////
            
            // Check
            if (Jaws.Instance.Accounts.HasUser())
            {
                // Sign In User
                string accessToken = Jaws.Instance.Accounts.GetUser().AccessToken;
                
                _hudUI.MeasureLatencyBegin();
                UserSignOutResponse userSignOutResponse = await Jaws.Instance.Accounts.UserSignOutAsync(accessToken);
                _hudUI.MeasureLatencyEnd();
                
                LogResponse($"Jaws #1: userSignOutResponse", userSignOutResponse);
                if (!userSignOutResponse.IsSuccess)
                {
                    _hudUI.OutputTextField02SetError(userSignOutResponse.ErrorMessage);
                }
            }

            _hudUI.ClearTextField(_hudUI.InputTextField01);
            _hudUI.ClearTextField(_hudUI.InputTextField02);
            _hudUI.ClearTextField(_hudUI.OutputTextField02);
            _hudUI.OutputTextField01.label = "Result";
            _hudUI.OutputTextField01.value = "Success";
            _hudUI.IsEnabledUI = true;
        }
        
        
        private async Task DoDatabaseCreateTable()
        {
            if (!Jaws.Instance.Accounts.HasUser())
            {
                await DoAccountsSignIn();
            }
            
            if (!Jaws.Instance.Accounts.HasUser())
            {
                Debug.Log($"DoDatabaseCreateTable() failed. HasUser() must not be {Jaws.Instance.Accounts.HasUser()}");
            }
            
            //UI
            await RefreshUI();
            _hudUI.IsEnabledUI = false;

            //////////////////////////////////////
            // Subsystem: Database Table Read
            //////////////////////////////////////
            TableReadResponse response = null;
            Table table = new InventoryTable();
            if (Jaws.Instance.Accounts.HasUser())
            {
                
                // UI
                _hudUI.DescriptionLabel.text = $"{_descriptionPrefix}Table Read";
                _hudUI.InputTextField01.label = $"TableName";
                _hudUI.InputTextField01.value = $"{table.TableName}";
                _hudUI.ClearTextField(_hudUI.OutputTextField01);
                _hudUI.ClearTextField(_hudUI.OutputTextField02);
                
                // Call
                _hudUI.MeasureLatencyBegin();
                response = await Jaws.Instance.Database.TableReadAsync(table.TableName);
                _hudUI.MeasureLatencyEnd();
                
                // UI
                if (response.IsSuccess)
                {
                    _hudUI.OutputTextField01.label = $"TableName";
                    _hudUI.OutputTextField01.value = $"{response.Table.TableName}";
                    _hudUI.OutputTextField02.label = $"ItemCount";
                    _hudUI.OutputTextField02.value = $"{response.Table.ItemCount}";

                    _demoModel.DatabaseHasTable = true;
                }
                else
                {
                    _hudUI.OutputTextField02SetError(response.ErrorMessage);
                }
            }
            
            //////////////////////////////////////
            // Subsystem: Database - Item Read
            //////////////////////////////////////
            User user = Jaws.Instance.Accounts.GetUser();
            GoldInventoryItem item = new GoldInventoryItem();
            
            ItemReadResponse itemReadResponse = null;
            if (user != null && table != null && item != null)
            {
                // Setup
                
                // UI
                _hudUI.DescriptionLabel.text = $"{_descriptionPrefix}Item Read";
                _hudUI.InputTextField01.label = $"TableName";
                _hudUI.InputTextField01.value = $"{table.TableName}";
                _hudUI.ClearTextField(_hudUI.OutputTextField01);
                _hudUI.ClearTextField(_hudUI.OutputTextField02);
                
                // Call
                _hudUI.MeasureLatencyBegin();
                itemReadResponse = await Jaws.Instance.Database.ItemReadAsync(table, user, item);
                _hudUI.MeasureLatencyEnd();
                
                // UI
                if (response.IsSuccess)
                {
                    _hudUI.OutputTextField01.label = $"TableName";
                    _hudUI.OutputTextField01.value = $"{itemReadResponse.Table.TableName}";
                    _hudUI.OutputTextField02.label = $"Item";
                    _hudUI.OutputTextField02.value = $"{itemReadResponse.Item.Quantity} {itemReadResponse.Item.Name}";
                    _demoModel.DatabaseItemQuantityPrevious = itemReadResponse.Item.Quantity;
                }
                else
                {
                    _hudUI.OutputTextField02SetError(response.ErrorMessage);
                }
            }

            
            //NOTE: Only create if it doesn't already exist
            if (!itemReadResponse.IsSuccess)
            {

                //////////////////////////////////////
                // Subsystem: Database - Item Create
                //////////////////////////////////////
                item.Quantity = 10;
                if (user != null && table != null && item != null)
                {
                    // Setup

                    // UI
                    _hudUI.DescriptionLabel.text = $"{_descriptionPrefix}Item Create";
                    _hudUI.InputTextField01.label = $"TableName";
                    _hudUI.InputTextField01.value = $"{table.TableName}";

                    // Call
                    _hudUI.MeasureLatencyBegin();
                    ItemCreateResponse itemCreateResponse = await Jaws.Instance.Database.ItemCreateAsync(table, user, item);
                    _hudUI.MeasureLatencyEnd();
                    
                    // UI
                    if (response.IsSuccess)
                    {
                        _hudUI.OutputTextField01.label = $"TableName";
                        _hudUI.OutputTextField01.value = $"{itemCreateResponse.Table.TableName}";
                        _hudUI.OutputTextField02.label = $"Item";
                        _hudUI.OutputTextField02.value =
                            $"{itemCreateResponse.Item.Quantity} {itemCreateResponse.Item.Name}";
                    }
                    else
                    {
                        _hudUI.OutputTextField02SetError(response.ErrorMessage);
                    }
                }
            }

            //UI
            _hudUI.IsEnabledUI = true;

        }
        
         private async Task DoDatabaseUpdateTable(InventoryItem item )
        {
            if (!Jaws.Instance.Accounts.HasUser())
            {
                await DoAccountsSignIn();
            }
            
            if (!_demoModel.DatabaseHasTable)
            {
                await DoDatabaseCreateTable();
            }
            
            //UI
            await RefreshUI();
            _hudUI.IsEnabledUI = false;


            //////////////////////////////////////
            // Subsystem: Database - Item Update
            //////////////////////////////////////
            User user = Jaws.Instance.Accounts.GetUser();
            InventoryTable table = new InventoryTable();
            if (user != null)
            {
                
                // UI
                _hudUI.InputTextField01.label = $"TableName";
                _hudUI.InputTextField01.value = $"{table.TableName}";
                _hudUI.DescriptionLabel.text = $"{_descriptionPrefix}Item Update";
                _hudUI.ClearTextField(_hudUI.OutputTextField01);
                _hudUI.ClearTextField(_hudUI.OutputTextField02);
                
                // Call
                _hudUI.MeasureLatencyBegin();
                ItemUpdateResponse itemUpdateResponse = await Jaws.Instance.Database.ItemUpdateAsync(table, user, item);
                _hudUI.MeasureLatencyEnd();
                
                // UI
                if (itemUpdateResponse.IsSuccess)
                {
                    _hudUI.OutputTextField01.label = $"TableName";
                    _hudUI.OutputTextField01.value = $"{itemUpdateResponse.Table.TableName}";
                    _hudUI.OutputTextField02.label = $"Item";
                    _hudUI.OutputTextField02.value = $"{itemUpdateResponse.Item.Quantity} {itemUpdateResponse.Item.Name}";
                    _demoModel.DatabaseItemQuantityPrevious = itemUpdateResponse.Item.Quantity;
                }
                else
                {
                    _hudUI.OutputTextField02SetError(itemUpdateResponse.ErrorMessage);
                }
            }
            
            //UI
            _hudUI.IsEnabledUI = true;

        }
        
        private async Task DoCloudCodeMethodCall()
        {

            if (!Jaws.Instance.Accounts.HasUser())
            {
                await DoAccountsSignIn();
            }
            
            if (!Jaws.Instance.Accounts.HasUser())
            {
                Debug.Log($"DoCloudCodeMethodCall() failed. HasUser() must not be {Jaws.Instance.Accounts.HasUser()}");
            }
            
            //UI
            await RefreshUI();
            _hudUI.IsEnabledUI = false;
            _hudUI.DescriptionLabel.text = $"{_descriptionPrefix}Call Cloud Code";

            //////////////////////////////////////
            // Subsystem: Cloud Code
            //////////////////////////////////////
            if (Jaws.Instance.Accounts.HasUser())
            {
                // Prepare 
                string functionName = "HelloWorld";
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("message", "this is from the client");
                
                // UI
                _hudUI.InputTextField01.label = $"functionName";
                _hudUI.InputTextField01.value = $"{functionName}";
                _hudUI.InputTextField02.label = $"args";
                _hudUI.InputTextField02.value = $"{JsonConvert.SerializeObject(args)}";
                _hudUI.ClearTextField(_hudUI.OutputTextField01);
                _hudUI.ClearTextField(_hudUI.OutputTextField02);
                
                // Call
                _hudUI.MeasureLatencyBegin();
                MethodCallResponse<string> response = await Jaws.Instance.CloudCode.MethodCallAsync<string>(functionName, args);
                _hudUI.MeasureLatencyEnd();
                
                // UI
                if (response.IsSuccess)
                {
                    _hudUI.OutputTextField01.label = $"Result";
                    _hudUI.OutputTextField01.value = $"{response.Data}";
                    _hudUI.ClearTextField(_hudUI.OutputTextField01);
                    _hudUI.ClearTextField(_hudUI.OutputTextField02);
                    
                    //We may or may not have a table at this moment,
                    //but pretend we DO NOT for a clean UX
                    _demoModel.DatabaseHasTable = false;
                }
                else
                {
                    _hudUI.OutputTextField02SetError(response.ErrorMessage);
                }
       
            }
            //UI
            _hudUI.IsEnabledUI = true;
        }
        
        
         private async Task DoAIInvoke()
        {

            if (!Jaws.Instance.Accounts.HasUser())
            {
                await DoAccountsSignIn();
            }
            
            if (!Jaws.Instance.Accounts.HasUser())
            {
                Debug.Log($"DoAIInvoke() failed. HasUser() must not be {Jaws.Instance.Accounts.HasUser()}");
            }
            
            //UI
            await RefreshUI();
            _hudUI.IsEnabledUI = false;
            _hudUI.DescriptionLabel.text = $"{_descriptionPrefix}Invoke AI Prompt";

            //////////////////////////////////////
            // Subsystem: Cloud Code
            //////////////////////////////////////
            if (Jaws.Instance.Accounts.HasUser())
            {
                // Prepare 
                string functionName = "HelloWorld";
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("message", "this is from the client");
                
                // UI
                _hudUI.InputTextField01.label = $"functionName";
                _hudUI.InputTextField01.value = $"{functionName}";
                _hudUI.InputTextField02.label = $"args";
                _hudUI.InputTextField02.value = $"{JsonConvert.SerializeObject(args)}";
                _hudUI.ClearTextField(_hudUI.OutputTextField01);
                _hudUI.ClearTextField(_hudUI.OutputTextField02);
                
                // Call
                _hudUI.MeasureLatencyBegin();
                InvokeAIModelResponse response = await Jaws.Instance.AI.InvokeAIModel(new InvokeAIModelRequest());
                _hudUI.MeasureLatencyEnd();
                
                // UI
                if (response.IsSuccess)
                {
                    _hudUI.OutputTextField01.label = $"Result";
                    _hudUI.OutputTextField01.value = $"{response}";
                    _hudUI.ClearTextField(_hudUI.OutputTextField01);
                    _hudUI.ClearTextField(_hudUI.OutputTextField02);
                    
                    //We may or may not have a table at this moment,
                    //but pretend we DO NOT for a clean UX
                    _demoModel.DatabaseHasTable = false;
                }
                else
                {
                    _hudUI.OutputTextField02SetError(response.ErrorMessage);
                }
       
            }
            //UI
            _hudUI.IsEnabledUI = true;
        }
        
        
        
        
        private void LogResponse(string message, Response response)
        {
            Debug.Log($"{message}. IsSuccess = {response.IsSuccess}");
            
            if (!response.IsSuccess)
            {
                Debug.LogError($"\t{response.ErrorMessage}");
            }
        }

        //  Event Handlers --------------------------------

    }
}