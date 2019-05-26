using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
namespace LocalizeSpace
{
	public class WordsAppender : EditorWindow
	{
		private WordsAppender window;
		private ILanguageParser parser;

		private bool? isKeyExist = null;
		private bool isNoTranslations = false;
		private bool isWordWritten = false;

		private bool isInited = false;

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
			window = (WordsAppender)GetWindow(typeof(WordsAppender));
			window.ShowUtility();
			window.titleContent = new GUIContent("Add new word");
		}

		public void ShowWordWritten()
		{
			AssetDatabase.Refresh();
			isWordWritten = true;
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
			parser.NewKey = EditorGUILayout.TextField("Enter KEY", parser.NewKey);
			if (EditorGUI.EndChangeCheck())
			{
				isKeyExist = null;
				isWordWritten = false;
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

			if (isKeyExist == true)
			{
				EditorGUILayout.LabelField("This KEY allready in your base!", TextStyleRed);
			}
			else if (isKeyExist == false)
			{
				isNoTranslations = false;
				for (int i = 0; i < parser.Localizator.Languages.Count; i++)
				{
					if (parser.NewWords[i] == null)
					{
						parser.NewWords[i] = string.Empty;
					}
					parser.NewWords[i] = EditorGUILayout.TextField(parser.Localizator.Languages[i], parser.NewWords[i]);
					isNoTranslations |= parser.NewWords[i].Equals(string.Empty);
				}

				GUILayout.BeginHorizontal();
				GUI.enabled = !isNoTranslations;
				if (GUILayout.Button("Save"))
				{
					parser.SaveWord();
				}
				GUI.enabled = true;
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}

			if (isWordWritten)
			{
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Word saved!", TextStyleBlue);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
		}

		private void FindKey()
		{
			isWordWritten = false;
			isKeyExist = false;
			parser.NewWords = new string[parser.Localizator.Languages.Count];
			parser.Localizator.Keys.ForEach((string key) =>
			{
				isKeyExist |= string.Equals(key, parser.NewKey, System.StringComparison.OrdinalIgnoreCase);
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