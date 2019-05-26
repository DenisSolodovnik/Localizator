using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LocalizeSpace
{
	public class WebLanguageParser : ILanguageParser
	{
		public static readonly string ParserName = typeof(WebLanguageParser).ToString();

		private bool isLocalizing = false;
		public Localizator Localizator { get; set; }

		public WebLanguageParser(Localizator localizator)
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
			TextAsset text = Resources.Load<TextAsset>("Languages/" + lang);

			if (text == null)
			{
				Debug.Log("No localization file " + lang + ".txt");

				if (!lang.Equals(Localizator.DefaultLanguage))
				{
					Debug.Log("Load default language " + Localizator.DefaultLanguage + ".txt");
					text = Resources.Load<TextAsset>("Languages/" + Localizator.DefaultLanguage);
				}
				if (text == null)
				{
					Debug.LogError("No localization file " + lang + ".txt");
					return;
				}
			}

			List<string> txt = new List<string>();
			string[] t1 = text.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
			txt.AddRange(t1);
			if (isDefault)
			{
				Localizator.DefaultWords.Clear();
			}
			else
			{
				Localizator.LocalizedWords.Clear();
			}

			for (int i = 0; i < txt.Count; i++)
			{
				txt[i] = txt[i].TrimEnd(new char[] { '\r', '\n' });
				string[] t = txt[i].Split(new char[] { ':' }, System.StringSplitOptions.RemoveEmptyEntries);

				if (t.Length == 2)
				{
					for (int j = 0; j < t.Length; j++)
					{
						t[j] = t[j].TrimEnd(new char[] { '\r', '\n' });
						t[j] = t[j].Trim();

						if (t[j][0].Equals('\''))
						{
							t[j] = t[j].Remove(0, 1);
						}
						if (t[j][t[j].Length - 1].Equals('\''))
						{
							t[j] = t[j].Remove(t[j].Length - 1, 1);
						}
					}
				}
				else if (t.Length > 2)
				{
					t[0] = t[0].TrimEnd(new char[] { '\r', '\n' });
					t[0] = t[0].Trim();

					if (t[0][0].Equals('\''))
					{
						t[0] = t[0].Remove(0, 1);
					}
					if (t[0][t[0].Length - 1].Equals('\''))
					{
						t[0] = t[0].Remove(t[0].Length - 1, 1);
					}
					t[1] = t[1].TrimEnd(new char[] { '\r', '\n' });
					string ts = t[1];
					for (int j = 2; j < t.Length; j++)
					{
						t[j] = t[j].TrimEnd(new char[] { '\r', '\n' });
						ts = ts + ":" + t[j];
					}
					ts = ts.Trim();
					if (ts[0].Equals('\''))
					{
						ts = ts.Remove(0, 1);
					}
					if (ts[ts.Length - 1].Equals('\''))
					{
						ts = ts.Remove(ts.Length - 1, 1);
					}
					t[1] = ts;
				}
				else if (t.Length < 2)
				{
					Debug.LogError("Incorrect file format!\n" + txt[i] + " #" + i + " (file: " + lang + ".txt)");
					return;
				}

				//Fills result list
				if (isDefault)
				{
					Localizator.DefaultWords.Add(new KeyWord(t[0], t[1]));
				}
				else
				{
					Localizator.LocalizedWords.Add(new KeyWord(t[0], t[1]));
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

			if (!isDefault && (Localizator.DefaultWords.Count != Localizator.LocalizedWords.Count))
			{
				Debug.LogWarning("Mismatch of entries count:\nDefault language (" + Localizator.DefaultLanguage + ") = " +
								 Localizator.DefaultWords.Count + " / Current language (" + Localizator.CurrentLanguage + ") = " + Localizator.LocalizedWords.Count);
				FixLocalize();
			}

			#region DebugBlock
			////Debug block!
			//if(isDefault)
			//{
			//	Debug.Log(defaultLanguage);
			//	DefaultWords.ForEach((a) => Debug.Log(a.ToString()));
			//	Debug.Log("---");
			//}
			//else
			//{
			//	Debug.Log(CurrentLanguage);
			//	LocalizedWords.ForEach((a) => Debug.Log(a.ToString()));
			//	Debug.Log("---");
			//}
			#endregion

			isLocalizing = false;
		}

		#endregion

		#region Fix Localize

		private void FixLocalize()
		{
			bool hasKey = false;

			foreach (var d in Localizator.DefaultWords)
			{
				foreach (var l in Localizator.LocalizedWords)
				{
					if (d.Key.Equals(l.Key))
					{
						hasKey = true;
						break;
					}
				}
				if (!hasKey)
				{
					Localizator.LocalizedWords.Add(new KeyWord(d.Key, "ERROR!"));
					hasKey = true;
				}
				else
				{
					hasKey = false;
				}
			}
		}

		#endregion

		#region Load / Get Languages

		public void LoadLanguages()
		{
			Localizator.Languages = GetLanguages();

			TextAsset text = null;

			foreach (string lang in Localizator.Languages)
			{
				text = Resources.Load<TextAsset>("Languages/" + lang);
				if (text == null)
				{
					CreateLanguageFile(lang);
				}
			}
		}

		public List<string> GetLanguages()
		{
			TextAsset text = Resources.Load<TextAsset>("Languages");
			if (text == null)
			{
				Debug.Log("No Languages file");
				if (!CreateLanguagesListFile())
				{
					return null;
				}
				text = Resources.Load<TextAsset>("Languages");
			}

			List<string> retString = new List<string>();
			retString.AddRange(text.text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries));
			for (int j = 0; j < retString.Count; j++)
			{
				retString[j] = retString[j].TrimEnd(new char[] { '\n', '\r' });
			}

			return retString;
		}

		#endregion

		#region CreateFiles

		private bool CreateLanguageFile(string lang)
		{
#if UNITY_EDITOR
			Debug.Log("Creating " + lang + ".txt...");

			try
			{
				if (!Directory.Exists(Application.dataPath + "/Resources/Languages/"))
				{
					Directory.CreateDirectory(Application.dataPath + "/Resources/Languages/");
				}
			}
			catch
			{
				Debug.LogError("Can't create directory " + Application.dataPath + "/Resources/Languages/");
				return false;
			}

			try
			{
				using (StreamWriter sw = new StreamWriter(Application.dataPath + "/Resources/Languages/" + lang.ToUpper() + ".txt", false, Encoding.Default))
				{
					sw.WriteLine("'Yes' : 'yes'");
					sw.WriteLine("'No' : 'no'");
				}

				AssetDatabase.Refresh();

				return true;
			}
			catch
			{
				Debug.LogError("Can't create file Resources/Languages/" + lang + ".txt");
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

#if UNITY_EDITOR
		WordsAppender webWordsAppender = null;
		WordEraser webWordsEraser = null;
#endif

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
#if UNITY_EDITOR
			webWordsAppender = ScriptableObject.CreateInstance<WordsAppender>();
			webWordsAppender.Init(this);
#endif
		}

		public void SaveWord()
		{
#if UNITY_EDITOR
			string keyValue = string.Empty;
			for (int i = 0; i < NewWords.Length; i++)
			{
				keyValue = string.Format("\n'{0}' : '{1}'", NewKey, NewWords[i]);
				File.AppendAllText(string.Format(Application.dataPath + "/Resources/Languages/{0}.txt", Localizator.Languages[i]), keyValue);
			}

			webWordsAppender.ShowWordWritten();

			Localizator.Init();
#endif
		}

		public void SaveWord(string key, Dictionary<string, string> pairs)
		{
#if UNITY_EDITOR
			bool isKeyExist = false;
			Localizator.Keys.ForEach((string oldKey) =>
			{
				isKeyExist |= string.Equals(oldKey, key, System.StringComparison.OrdinalIgnoreCase);
			});

			if (isKeyExist)
			{
				Debug.LogError("Key " + key + " is already in use!");
				return;
			}

			string keyValue = string.Empty;

			foreach (KeyValuePair<string, string> kvp in pairs)
			{
				keyValue = string.Format("\n'{0}' : '{1}'", key, kvp.Value);
				File.AppendAllText(string.Format(Application.dataPath + "/Resources/Languages/{0}.txt", kvp.Key), keyValue);
			}


			Debug.Log("Word " + key + " has been writen!");
			AssetDatabase.Refresh();

			Localizator.Init();
#endif
		}

		public void SaveWord(string key, string[] data)
		{
#if UNITY_EDITOR
			bool isKeyExist = false;
			Localizator.Keys.ForEach((string oldKey) =>
			{
				isKeyExist |= string.Equals(oldKey, key, System.StringComparison.OrdinalIgnoreCase);
			});

			if (isKeyExist)
			{
				Debug.LogError("Key " + key + " is already in use!");
				return;
			}

			string keyValue = string.Empty;
			for (int i = 0; i < data.Length; i++)
			{
				keyValue = string.Format("\n'{0}' : '{1}'", key, data[i]);
				File.AppendAllText(string.Format(Application.dataPath + "/Resources/Languages/{0}.txt", Localizator.Languages[i]), keyValue);
			}

			Debug.Log("Word " + key + " has been writen!");
			AssetDatabase.Refresh();

			Localizator.Init();
#endif
		}

		public void DeleteWord()
		{
#if UNITY_EDITOR
			webWordsEraser = ScriptableObject.CreateInstance<WordEraser>();
			webWordsEraser.Init(this);
#endif
		}

		public void SaveDeleted()
		{
#if UNITY_EDITOR
			string newText = string.Empty;
			string key = "'" + OldKey + "'";

			for (int i = 0; i < Localizator.Languages.Count; i++)
			{
				TextAsset text = Resources.Load<TextAsset>("Languages/" + Localizator.Languages[i]);
				if (text != null)
				{
					int startIndex = text.text.IndexOf(key, System.StringComparison.OrdinalIgnoreCase);
					if (startIndex > -1)
					{
						int endIndex = text.text.IndexOf("\n", startIndex, System.StringComparison.OrdinalIgnoreCase);
						if (endIndex < 0)
						{
							endIndex = text.text.Length;
						}
						newText = text.text.Remove(startIndex - 1, endIndex - startIndex + 1);
						System.IO.File.WriteAllText(string.Format(Application.dataPath + "/Resources/Languages/{0}.txt", Localizator.Languages[i]), newText);

					}
					else
					{
						Debug.LogWarning("No key: " + key);
					}
				}
				else
				{
					Debug.LogWarning("No file: " + "Languages/" + Localizator.Languages[i]);
				}
			}

			webWordsEraser.ShowWordErased();

			Localizator.Invoke("Init", 0.1f);
#endif
		}

		#endregion
	}
}