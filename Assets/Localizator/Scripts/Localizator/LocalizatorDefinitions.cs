using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LocalizeSpace
{
	public partial class Localizator : MonoBehaviour, ILocalizator
	{
		public static event EventHandler<LanguageChangedArgs> OnLanguageChanged = (sender, e) => { };
		public static UnityAction<string> LangChanged;

		public ILanguageParser languageParser = null;
		public ILanguageParser LanguageParser
		{
			get
			{
				return languageParser;
			}
		}

		public bool isCustomInitialize = false;
		public bool IsCustomInitialize
		{
			get
			{
				return isCustomInitialize;
			}
		}

		public int ParserIndex = 0;
		public readonly string[] ParsersName = { WebLanguageParser.ParserName, CsvParser.ParserName };

		private static string debugLanguage = string.Empty;
		public static string DebugLanguage
		{
			get
			{
				if(debugLanguage.Equals(string.Empty))
				{
					debugLanguage = PlayerPrefs.GetString("debugLanguageS", "EN");
				}
				return debugLanguage;
			}
			set
			{
				debugLanguage = value;
			}
		}

		public bool IsInited
		{
			get;
			private set;
		}

		public string current = string.Empty;
		public string CurrentLanguage
		{
			get
			{
				if(current.Equals(string.Empty))
				{
					Debug.LogWarning("current language is empty!");
					//Init();
				}
				return current;
			}
			set
			{
				current = value;
			}
		}

		public bool IsSteam;

		public int defaultLInt = 2;

		public string defaultLanguage = "EN";
		public string DefaultLanguage
		{
			get
			{
				return defaultLanguage;
			}
			set
			{
				defaultLanguage = value;
			}
		}

		[SerializeField]
		private List<string> languages = new List<string>();
		public List<string> Languages
		{
			get
			{
				return languages;
			}
			set
			{
				languages = value;
			}
		}

		private static ILocalizator instance;
		public static ILocalizator Instance
		{
			get
			{
				if(instance == null)
				{
					instance = FindObjectOfType<Localizator>();
				}
				return instance;
			}
		}

		[SerializeField]
		public List<string> keys;
		public List<string> Keys
		{
			get
			{
				if(keys == null || keys.Count == 0)
				{
					Debug.LogWarning("No Keys yet!");
				}

				return keys;
			}
		}

		[SerializeField]
		public List<KeyWord> DefaultWords = new List<KeyWord>();
		[SerializeField]
		public List<KeyWord> LocalizedWords = new List<KeyWord>();
	}
}
