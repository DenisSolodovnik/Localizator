using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
namespace LocalizeSpace
{
	public class WordEraser : EditorWindow
	{
		private WordEraser window;
		private ILanguageParser parser;

		private bool? isKeyExist = null;
		private bool isInited = false;
		private bool isWordErased = false;

		private GUIStyle TextStyleRed = new GUIStyle();
		private GUIStyle TextStyleBlue = new GUIStyle();

		public void Init(ILanguageParser parser)
		{
			this.parser = parser;
			TextStyleRed.normal.textColor = Color.red;
			TextStyleBlue.normal.textColor = Color.blue;
			ShowEditorWindow();
			isInited = true;
		}

		public void ShowEditorWindow()
		{
			window = (WordEraser)GetWindow(typeof(WordEraser));
			window.ShowUtility();
			window.titleContent = new GUIContent("Delete word");
		}

		public void ShowWordErased()
		{
			isKeyExist = null;
			AssetDatabase.Refresh();
			isWordErased = true;
		}

		private void OnEnable()
		{

		}

		private void OnGUI()
		{
			if (!isInited)
			{
				return;
			}

			GUILayout.Space(10);

			EditorGUI.BeginChangeCheck();
			GUI.SetNextControlName("Key");
			parser.OldKey = EditorGUILayout.TextField("Enter KEY", parser.OldKey);
			if (EditorGUI.EndChangeCheck())
			{
				isKeyExist = null;
				isWordErased = false;
			}

			SetActiveKeyText();

			if (Event.current.keyCode == KeyCode.Return)
			{
				if (GUI.GetNameOfFocusedControl().Equals("Key"))
				{
					FindKey();
				}
			}

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Next >"))
			{
				FindKey();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			if (isKeyExist == false)
			{
				EditorGUILayout.LabelField("There's no such KEY in your base!", TextStyleRed);
			}
			else if (isKeyExist == true)
			{
				GUI.enabled = false;
				EditorGUILayout.TextField(parser.Localizator.CurrentLanguage, parser.Localizator.GetWord(parser.OldKey));
				GUI.enabled = true;

				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Delete"))
				{
					parser.SaveDeleted();
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}

			if (isWordErased)
			{
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Word deleted!", TextStyleBlue);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
		}

		private void FindKey()
		{
			isWordErased = false;
			isKeyExist = false;
			parser.Localizator.Keys.ForEach((string key) =>
			{
				isKeyExist |= string.Equals(key, parser.OldKey, System.StringComparison.OrdinalIgnoreCase);
			});
		}

		private bool keyOnce = false;
		private void SetActiveKeyText()
		{
			if (keyOnce)
			{
				return;
			}

			keyOnce = true;
			EditorGUI.FocusTextInControl("Key");
		}
	}
}
#endif