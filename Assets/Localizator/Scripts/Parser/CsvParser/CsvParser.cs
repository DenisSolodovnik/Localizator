using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LocalizeSpace
{
	public class CsvParser : ILanguageParser
	{
		public static readonly string ParserName = typeof(CsvParser).ToString();
		private bool isLocalizing = false;
		public Localizator Localizator { get; set; }

		public CsvParser(Localizator localizator)
		{
			this.Localizator = localizator;
		}

		#region Load Localization

		public void LoadLocalization(string lang, bool isDefault = false)
		{
			if (isLocalizing)
			{
				return;
			}

			isLocalizing = true;

			TextAsset text = Resources.Load<TextAsset>("localization");

			if (text == null)
			{
				Debug.LogError("No localization file " + lang + ".txt");
				return;
			}

			List<string> txt = new List<string>();
			string[] t1 = text.text.Split('\n');
			txt.AddRange(t1);
			txt.RemoveAll((a) => a.Equals(string.Empty));

			t1 = txt[0].Split('\t');
			List<string> fileLanguages = new List<string>();
			fileLanguages.AddRange(t1);

			int index = fileLanguages.FindIndex((string obj) => string.Equals(obj, lang, System.StringComparison.OrdinalIgnoreCase));
			int defaultIndex = -1;
			if (isDefault)
			{
				defaultIndex = index;
			}
			else
			{
				defaultIndex = fileLanguages.FindIndex((string obj) => string.Equals(obj, Localizator.DebugLanguage, System.StringComparison.OrdinalIgnoreCase));
			}

			if (index < 0)
			{
				Debug.LogError("No language: " + lang + " in localization file!");
				return;
			}

			if (isDefault)
			{
				Localizator.DefaultWords.Clear();
			}
			else
			{
				Localizator.LocalizedWords.Clear();
			}

			for (int i = 1; i < txt.Count; i++)
			{
				bool needCorrection = false;

				string[] t = txt[i].Split('\t');
				if (t.Length <= index)
				{
					Debug.LogWarning("Incorrect localization file format!\n" + "string #" + i + " - can't find language: " + lang + "! Try load default!");
					if (isDefault)
					{
						Debug.LogError("Failure! No default language!");
						return;
					}
					else
					{
						if (t.Length <= defaultIndex)
						{
							Debug.LogError("Failure! No default language!");
							return;
						}
						else
						{
							needCorrection = true;
							Debug.Log("Load succsess!");
						}
					}
				}

				string key = t[0];
				string val = string.Empty;

				val = needCorrection ? t[defaultIndex] : t[index];

				//Fills result list
				if (isDefault)
				{
					Localizator.DefaultWords.Add(new KeyWord(key, val));
				}
				else
				{
					Localizator.LocalizedWords.Add(new KeyWord(key, val));
				}
			}


			// Removing dublicates in result list
			if (isDefault)
			{
				for (int i = 0; i < Localizator.DefaultWords.Count; i++)
				{
					for (int j = 0; j < Localizator.DefaultWords.Count; j++)
					{
						if (i != j)
						{
							if (string.Equals(Localizator.DefaultWords[j].Key, Localizator.DefaultWords[i].Key))
							{
								Debug.LogWarning("Key dublicate! '" + Localizator.DefaultWords[j].Key + "' / '" + Localizator.DefaultWords[i].Key + "'");
								Debug.Log("Key '" + Localizator.DefaultWords[j].Key + "' was removed!");
								Localizator.DefaultWords.Remove(Localizator.DefaultWords[j]);
							}
						}
					}
				}
			}
			else
			{
				for (int i = 0; i < Localizator.LocalizedWords.Count; i++)
				{
					for (int j = 0; j < Localizator.LocalizedWords.Count; j++)
					{
						if (i != j)
						{
							if (string.Equals(Localizator.LocalizedWords[j].Key, Localizator.LocalizedWords[i].Key))
							{
								Debug.LogWarning("Key dublicate! '" + Localizator.LocalizedWords[j].Key + "' / '" + Localizator.LocalizedWords[i].Key + "'");
								Debug.Log("Key '" + Localizator.LocalizedWords[j].Key + "' was removed!");
								Localizator.LocalizedWords.Remove(Localizator.LocalizedWords[j]);
							}
						}
					}
				}
			}

			isLocalizing = false;
		}

		#endregion

		#region Load / Get Languages

		public void LoadLanguages()
		{
			Localizator.Languages = GetLanguages();
			TextAsset text = Resources.Load<TextAsset>("localization");

			if (text == null)
			{
				CreateLanguageFile();
			}
		}

		public List<string> GetLanguages()
		{
			TextAsset text = Resources.Load<TextAsset>("Languages");
			if (text == null)
			{
				Debug.LogError("Can't find file Resources/Languages.txt");

				if (!CreateLanguagesListFile())
				{
					return null;
				}

				text = Resources.Load<TextAsset>("Languages");
			}

			List<string> retString = new List<string>();
			retString.AddRange(text.text.Split('\n'));
			retString.RemoveAll((txt) => txt.Equals(string.Empty));

			return retString;
		}

		#endregion

		#region CreateFiles

		private bool CreateLanguageFile()
		{
#if UNITY_EDITOR
			Debug.Log("Creating localization.txt...");

			try
			{
				using (StreamWriter sw = new StreamWriter(Application.dataPath + "/Resources/localization.txt", false, Encoding.Default))
				{
					sw.WriteLine("key\tRU\tEN\tIT\tES\tPT\tJA\tZH\tDE\tFR\tKO\tTR\tPL\tDA\tNB\tFO\t\t\t\t");
					sw.WriteLine("Yes\tда\tyes");
					sw.WriteLine("No\tнет\tno");
				}

				AssetDatabase.Refresh();

				return true;
			}
			catch
			{
				Debug.LogError("Can't create file Resources/localization.txt");
				return false;
			}
#else
			return false;
#endif
		}

		private bool CreateLanguagesListFile()
		{
#if UNITY_EDITOR
			Debug.Log("Creating file...");

			try
			{
				if (!Directory.Exists(Application.dataPath + "/Resources/"))
				{
					Directory.CreateDirectory(Application.dataPath + "/Resources/");
				}
			}
			catch
			{
				Debug.LogError("Can't create directory " + Application.dataPath + "/ Resources/");
			}

			try
			{
				if (!Directory.Exists(Application.dataPath + "/Resources/"))
				{
					Directory.CreateDirectory(Application.dataPath + "/Resources/");
				}
			}
			catch
			{
				Debug.LogError("Can't create directory " + Application.dataPath + "/ Resources/");
			}

			try
			{
				using (StreamWriter sw = new StreamWriter(Application.dataPath + "/Resources/Languages.txt", false, Encoding.Default))
				{
					sw.WriteLine("EN");
					sw.WriteLine("RU");
				}

				AssetDatabase.Refresh();

				return true;
			}
			catch
			{
				Debug.LogError("Can't create file Resources/Languages.txt");
				return false;
			}
#else
			return false;
#endif
		}

		#endregion

		#region Edit Word

		private string newKey = string.Empty;
		public string NewKey
		{
			get
			{
				return newKey;
			}
			set
			{
				newKey = value;
			}
		}

		private string oldKey = string.Empty;
		public string OldKey
		{
			get
			{
				return oldKey;
			}
			set
			{
				oldKey = value;
			}
		}

		public string[] NewWords { get; set; }

		public void AddWord()
		{
		}

		public void SaveWord()
		{
		}

		public void SaveWord(string key, string[] data)
		{
		}

		public void DeleteWord()
		{
		}

		public void SaveDeleted()
		{
		}

		#endregion
	}
}