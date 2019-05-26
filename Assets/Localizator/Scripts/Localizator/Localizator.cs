using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LocalizeSpace
{
#if UNITY_EDITOR
	[ExecuteInEditMode]
#endif
	public partial class Localizator : MonoBehaviour, ILocalizator
	{
		public static Hashtable Undestoyable;

		private void Awake()
		{
#if UNITY_EDITOR
			if (Application.isPlaying)
			{
				if (!Undestroy())
				{
					return;
				}
			}
#else
			if(!Undestroy())
			{
				return;
			}
#endif

			instance = this;
			if (!IsInited)
			{
				Init();
			}

			print("Localizator v.0.7.0 - Inited OK \nDefault language is " + DefaultLanguage + " / Current language is " + CurrentLanguage);
		}

		private bool Undestroy()
		{
			if (Undestoyable == null)
			{
				Undestoyable = new Hashtable();
			}
			if (!Undestoyable.ContainsKey(name))
			{
				Undestoyable.Add(name, this);
				DontDestroyOnLoad(this);

			}
			else
			{
				Destroy(gameObject);
				return false;
			}

			return true;
		}

		private List<string> GetKeys()
		{
			List<string> k = new List<string>();

			if(LocalizedWords.Count == 0)
			{
				Debug.LogWarning(LocalizedWords.Count);
			}
			LocalizedWords.ForEach((KeyWord obj) => k.Add(obj.Key));
			return k;
		}

		/// <summary>
		/// Выставляет дефолтным язык <paramref name="language"/>
		/// </summary>
		public void SetLanguage(SystemLanguage language)
		{
			CurrentLanguage = GetLanguage(false, language);
			SetLanguage();
		}

		/// <summary>
		/// Выставляет дефолтным язык <paramref name="language"/>
		/// </summary>
		public void SetLanguage(string language)
		{
			CurrentLanguage = GetLanguage(language);
			SetLanguage();
		}

		private void SetLanguage()
		{
			languageParser.LoadLocalization(DefaultLanguage, true);
			languageParser.LoadLocalization(CurrentLanguage);
			keys = GetKeys();
			OnLanguageChanged(this, new LanguageChangedArgs(CurrentLanguage));
		}

		public List<string> GetLanguages()
		{
			return languageParser.GetLanguages();
		}

		public bool HasKey(string key)
		{
			return Keys.Exists((string obj) => string.Equals(obj, key, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Выставляет дефолтным язык системы загружает переводы 
		/// </summary>
		public void Init(SystemLanguage language = SystemLanguage.Unknown)
		{
			switch (ParserIndex)
			{
				case 0:
					languageParser = new WebLanguageParser(this);
					break;
				case 1:
					languageParser = new CsvParser(this);
					break;
			}

			languageParser.LoadLanguages();
#if UNITY_EDITOR
			if (language == SystemLanguage.Unknown)
			{
				CurrentLanguage = GetLanguage(DebugLanguage);
			}
			else
			{
				CurrentLanguage = GetLanguage(false, language);
			}

#else
			if(IsSteam)
			{
				CurrentLanguage = GetSteamLanguage();
			}
			else
			{
				if (language == SystemLanguage.Unknown)
				{
					CurrentLanguage = GetLanguage(true, SystemLanguage.Unknown);
				}
				else
				{
					CurrentLanguage = GetLanguage(false, language);
				}
			}

			if (isCustomInitialize)
			{
				return;
			}
#endif

			languageParser.LoadLocalization(DefaultLanguage, true);
			languageParser.LoadLocalization(CurrentLanguage);
			keys = GetKeys();
			IsInited = true;
			OnLanguageChanged(this, new LanguageChangedArgs(CurrentLanguage));
		}

		public string GetWord(string key)
		{
			if(LocalizedWords.Count == 0)
			{
				Init();
			}

			int index = LocalizedWords.FindIndex((KeyWord obj) => string.Equals(obj.Key, key, StringComparison.OrdinalIgnoreCase));
			if(index < 0)
			{
				Debug.LogWarning("Wrong Key!");
				return string.Empty;
			}
			else
			{
				return LocalizedWords[index].Word;
			}
		}
#if UNITY_EDITOR
		[MenuItem("Project/Tools/Language/ReInit Dictionary")]
#endif
		public static void ReInit()
		{
			Debug.Log("ReInit Dictionary");
			Instance.Init();
		}

#if UNITY_EDITOR
		[MenuItem("Project/Tools/Language/Add Word")]
#endif
		public static void AddWord()
		{
			Instance.AddNewWord();
		}

#if UNITY_EDITOR
		[MenuItem("Project/Tools/Language/Delete Word")]
#endif
		public static void DeleteWord()
		{
			Instance.EraseWord();
		}

		public void AddNewWord()
		{
			if(languageParser == null)
			{
				Init();
			}
			languageParser.AddWord();
		}

		public void EraseWord()
		{
			if (languageParser == null)
			{
				Init();
			}
			languageParser.DeleteWord();
		}
	}
}
