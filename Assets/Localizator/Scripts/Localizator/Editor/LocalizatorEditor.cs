using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace LocalizeSpace
{
	[CustomEditor(typeof(Localizator))]
	public class LocalizatorEditor : Editor
	{
		private Localizator instance = null;
		SerializedProperty SScript;
		private string Defines = string.Empty;
		private int parserIndex = 0;

		private void OnEnable()
		{
			if(instance == null) instance = (Localizator)target;
			SScript = serializedObject.FindProperty("m_Script");
			Defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			instance.IsSteam = Defines.Contains("STEAM_APP");
			parserIndex = instance.ParserIndex;
		}

		public override void OnInspectorGUI()
		{
			if(!instance.enabled)
			{
				return;
			}
			GUI.enabled = false;
			EditorGUILayout.ObjectField(SScript);
			GUI.enabled = true;
			EditorGUI.BeginChangeCheck();
			instance.IsSteam = EditorGUILayout.Toggle("Steam application?", instance.IsSteam);
			if(EditorGUI.EndChangeCheck())
			{
				Defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
				if(!instance.IsSteam)
				{
					Defines = Defines.Replace("STEAM_APP", "");
				}
				else
				{
					if(Defines.Equals(string.Empty))
					{
						Defines += "STEAM_APP";
					}
					else
					{
						if(!Defines.Contains("STEAM_APP"))
						{
							Defines += ";STEAM_APP";
						}
					}
				}

				PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, Defines);
			}
			string[] l = instance.Languages.ToArray();
			instance.defaultLInt = EditorGUILayout.Popup("Dafault Language", instance.defaultLInt, l);
			instance.defaultLanguage = instance.Languages[instance.defaultLInt];

			instance.ParserIndex = EditorGUILayout.Popup("Used parser", instance.ParserIndex, instance.ParsersName);
			if (instance.ParserIndex != parserIndex)
			{
				instance.Init();
				parserIndex = instance.ParserIndex;
			}

			instance.isCustomInitialize = EditorGUILayout.Toggle("Custom Initialization", instance.isCustomInitialize);
		}
	}
}
