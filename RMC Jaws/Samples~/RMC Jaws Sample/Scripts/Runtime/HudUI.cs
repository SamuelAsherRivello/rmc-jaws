using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace RMC.Backend.Baas.Aws.Samples
{
    /// <summary>
    /// The UI for the <see cref="DemoJaws01"/>
    /// </summary>
    public class HudUI : MonoBehaviour
    {
        //  Properties ------------------------------------
        public Label TitleLabel { get { return _uiDocument?.rootVisualElement.Q<Label>("TitleLabel"); }}
        public Label DescriptionLabel { get { return _uiDocument?.rootVisualElement.Q<Label>("DescriptionLabel"); }}
        //
        public TextField InputTextField01 { get { return _uiDocument?.rootVisualElement.Q<TextField>("InputTextField01"); }}
        public TextField InputTextField02 { get { return _uiDocument?.rootVisualElement.Q<TextField>("InputTextField02"); }}
        public TextField OutputTextField01 { get { return _uiDocument?.rootVisualElement.Q<TextField>("OutputTextField01"); }}
        public TextField OutputTextField02 { get { return _uiDocument?.rootVisualElement.Q<TextField>("OutputTextField02"); }}
        public TextField OutputTextField03 { get { return _uiDocument?.rootVisualElement.Q<TextField>("OutputTextField03"); }}
        //
        public Button Button01 { get { return _uiDocument?.rootVisualElement.Q<VisualElement>("Buttons").ElementAt(0) as Button; }}
        public Button Button02 { get { return _uiDocument?.rootVisualElement.Q<VisualElement>("Buttons").ElementAt(1) as Button; }}
        public Button Button03 { get { return _uiDocument?.rootVisualElement.Q<VisualElement>("Buttons").ElementAt(2) as Button; }}
        public Button Button04 { get { return _uiDocument?.rootVisualElement.Q<VisualElement>("Buttons").ElementAt(3) as Button; }}
        public Button Button05 { get { return _uiDocument?.rootVisualElement.Q<VisualElement>("Buttons").ElementAt(4) as Button; }}
        public Button Button06 { get { return _uiDocument?.rootVisualElement.Q<VisualElement>("Buttons").ElementAt(5) as Button; }}
        public VisualElement Buttons { get { return _uiDocument?.rootVisualElement.Q<VisualElement>("Buttons"); }}
        
        public bool IsEnabledUI
        {
            get
            {
                return _isEnabledUI;
            }

            set
            {
                // Set value
                _isEnabledUI = value;
            
                //
                Buttons.SetEnabled(_isEnabledUI);
            }
        
        }
        
        public void ClearTextField(TextField textField) 
        { 
            textField.label = " "; 
            textField.value = " "; 
        }
        
        //  Fields ----------------------------------------
        [SerializeField]
        private UIDocument _uiDocument;

        private bool _isEnabledUI = true; // "__" to discourage direct usage
        private Stopwatch _latencyStopwatch;

        //  Unity Methods ---------------------------------
        protected void Start()
        {
            Debug.Log($"{GetType().Name}.Start()");
            
            
            _latencyStopwatch = new Stopwatch();

            IsEnabledUI = true;
            InputTextField01.isReadOnly = true;
            InputTextField02.isReadOnly = true;
            OutputTextField01.isReadOnly = true;
            OutputTextField02.isReadOnly = true;
            OutputTextField03.isReadOnly = true;
        }

        
        //  Methods ---------------------------------------
        public void OutputTextField02SetError(string message)
        {
            OutputTextField02.label = "Error";
            OutputTextField02.value = message;
        }
        
        //  Event Handlers --------------------------------


        public void MeasureLatencyBegin()
        {
            _latencyStopwatch.Restart();
            OutputTextField03.label = " ";
            OutputTextField03.value = $" ";
        }

        public void MeasureLatencyEnd()
        {
            _latencyStopwatch.Stop();
            OutputTextField03.label = "Latency";
            OutputTextField03.value = $"{_latencyStopwatch.ElapsedMilliseconds} Ms";
       
        }
    }
}