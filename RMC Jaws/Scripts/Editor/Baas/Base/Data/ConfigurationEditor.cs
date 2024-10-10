using RMC.Backend.Baas.Aws;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace RMC.Backend.Baas
{
	/// <summary>
	/// The editor for the configuration for the backend system.
	/// </summary>
	[CustomEditor(typeof(JawsConfiguration))]
	public class ConfigurationEditor : Editor
	{
		//  Properties ------------------------------------

		//  Fields ----------------------------------------
		[SerializeField]
		private Texture2D _bannerTexture2D;
		private static Color _bannerColor = new Color32(32, 38, 45, 255);
		
		
		//  Unity Methods ---------------------------------
		private void OnEnable()
		{
			Assert.IsNotNull(_bannerTexture2D, "Texture2D must NOT be null");
			
		}

		public override void OnInspectorGUI()
		{
			// Draw banner texture with colored background
			if (_bannerTexture2D != null)
			{
				var originalBackgroundColor = GUI.backgroundColor;

				// Draw a full-width blue box
				var fullWidthRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(100), GUILayout.ExpandWidth(true));
				EditorGUI.DrawRect(fullWidthRect, _bannerColor);

				// Draw the centered image inside the blue box
				var imageRect = new Rect(
					fullWidthRect.x + (fullWidthRect.width - _bannerTexture2D.width) / 2,
					fullWidthRect.y, // Padding from top of the blue box
					_bannerTexture2D.width,
					100
				);
				GUI.DrawTexture(imageRect, _bannerTexture2D, ScaleMode.ScaleToFit);

				GUI.backgroundColor = originalBackgroundColor;
			}



			// Draw default fields (existing fields)
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Required Fields", EditorStyles.boldLabel);
			EditorGUILayout.Space();
			DrawDefaultInspector();
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("#Hack", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("You must also set region in JawsConfiguration.cs", EditorStyles.wordWrappedLabel);
			
			
			// Add descriptive text
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Instructions", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("Populate the required fields.", EditorStyles.wordWrappedLabel);
			EditorGUILayout.LabelField("Visit aws.amazon.com to create and copy the values.", EditorStyles.wordWrappedLabel);

			// Create a clickable link
			EditorGUILayout.Space();
			if (GUILayout.Button("Visit aws.amazon.com", GUILayout.Height(25)))
			{
				Application.OpenURL("https://aws.amazon.com/");
			}
		}
	}
}
