using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace LocalizeSpace
{
	[CustomEditor(typeof(UILocalize))]
	public class UILocalizeEditor : Editor
	{
		private UILocalize instance;

		private bool foldoutFonts, foldoutSize;

		private string keyStr = string.Empty;
		private string key = string.Empty;

		private bool CanFind = true;
		private GUIStyle TextStyleRed;

		private UILocalize.Capitalization Capitalize = UILocalize.Capitalization.None;

		private void OnEnable()
		{
			if(instance == null) instance = (UILocalize)target;

			TextStyleRed = new GUIStyle();
			TextStyleRed.normal.textColor = Color.red;

			instance.Init();

			key = instance.Key;

			instance.LocalizeWord();
		}

		private void OnInspectorUpdate()
		{
			instance.LocalizeWord();
		}

		public override void OnInspectorGUI()
		{
			if(!instance.enabled)
			{
				return;
			}

			EditorGUI.BeginChangeCheck();
			key = EditorGUILayout.TextField("Find by key", key);
			int idx;
			if(EditorGUI.EndChangeCheck())
			{
				idx = Localizator.Instance.Keys.FindIndex((string obj) => obj.ToLower().Contains(key.ToLower()));
				if(idx < 0)
				{
					idx = 0;
					CanFind = false;
				}
				else
				{
					CanFind = true;
				}
			}
			else
			{
				idx = Localizator.Instance.Keys.FindIndex((string obj) => obj.Equals(instance.Key, System.StringComparison.OrdinalIgnoreCase));
				if(idx < 0)
				{
					idx = 0;
					key = Localizator.Instance.Keys[idx];
					instance.Key = key;
				}
			}

			if(!CanFind)
			{
				EditorGUILayout.LabelField("Can't find Key!", TextStyleRed);
			}

			string[] k = Localizator.Instance.Keys.ToArray();
			idx = EditorGUILayout.Popup("Localization Key", idx, k);
			instance.Key = Localizator.Instance.Keys[idx];
			if(!keyStr.Equals(instance.Key, System.StringComparison.OrdinalIgnoreCase))
			{
				key = instance.Key;
				keyStr = key;
				instance.LocalizeWord();
				EditorUtility.SetDirty(instance);
			}

			instance.Capitalize = (UILocalize.Capitalization)EditorGUILayout.EnumPopup("Capitalization", instance.Capitalize);
			if(instance.Capitalize != Capitalize)
			{
				Capitalize = instance.Capitalize;
				instance.LocalizeWord();
				EditorUtility.SetDirty(instance);
			}

			EditorGUILayout.Space();

			instance.LocalizeToString = EditorGUILayout.Toggle("Localize to sting *only*", instance.LocalizeToString);
			if(instance.LocalizeToString)
			{
				GUI.enabled = false;
				EditorGUILayout.TextField(instance.LocalizedString);
				GUI.enabled = true;
			}

			if(instance.TextCanvas != null)
			{
				instance.TextCanvas.font = instance.DefaultFont;
				instance.TextCanvas.fontSize = instance.DefaultSize;
			}
			if(instance.TextNoCanvas != null)
			{
				instance.TextNoCanvas.font = instance.DefaultFont;
				instance.TextNoCanvas.fontSize = instance.DefaultSize;
			}

			instance.DefaultFont = (Font)EditorGUILayout.ObjectField("Default Font", instance.DefaultFont, typeof(Font), true);
			instance.DefaultSize = EditorGUILayout.IntField("Default Size", instance.DefaultSize);

			foldoutFonts = EditorGUILayout.Foldout(foldoutFonts, "Fonts");
			if(foldoutFonts)
			{
				for(int i = 0; i < Localizator.Instance.Languages.Count; i++)
				{
					EditorGUILayout.BeginHorizontal();
					instance.checkFont[i].Use = EditorGUILayout.Toggle("Font " + Localizator.Instance.Languages[i], instance.checkFont[i].Use);
					if(instance.checkFont[i].Use)
					{
						if(instance.LangFont[i] == null)
						{
							instance.LangFont[i] = (instance.TextCanvas != null) ? instance.TextCanvas.font : instance.TextNoCanvas.font;
						}
						instance.LangFont[i] = (Font)EditorGUILayout.ObjectField("", instance.LangFont[i], typeof(Font), true);
						if(Localizator.Instance.Languages[i] == Localizator.Instance.CurrentLanguage)
						{
							if(instance.TextCanvas != null)
							{
								instance.TextCanvas.font = instance.LangFont[i];
							}
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}

			foldoutSize = EditorGUILayout.Foldout(foldoutSize, "Size");
			if(foldoutSize)
			{
				for(int i = 0; i < Localizator.Instance.Languages.Count; i++)
				{
					EditorGUILayout.BeginHorizontal();
					instance.checkSize[i].Use = EditorGUILayout.Toggle("Size " + Localizator.Instance.Languages[i], instance.checkSize[i].Use);
					if(instance.checkSize[i].Use)
					{
						if(instance.LangSize[i] < 1)
						{
							instance.LangSize[i] = (instance.TextCanvas != null) ? instance.TextCanvas.fontSize : instance.TextNoCanvas.fontSize;
							if(Localizator.Instance.Languages[i] == Localizator.Instance.CurrentLanguage)
							{
								if(instance.TextCanvas != null)
								{
									instance.TextCanvas.fontSize = instance.LangSize[i];
								}
							}
						}
						instance.LangSize[i] = EditorGUILayout.IntField(instance.LangSize[i]);
					}
					EditorGUILayout.EndHorizontal();
				}
			}
			instance.LocalizeWord();
		}
	}
}
