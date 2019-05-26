using UnityEngine;

namespace LocalizeSpace
{
	public partial class Localizator : MonoBehaviour, ILocalizator
	{
		private string GetLanguage(bool device = true, SystemLanguage slang = SystemLanguage.Unknown)
		{
			string lang;
			switch(device ? Application.systemLanguage : slang)
			{
				case SystemLanguage.English:
					lang = "EN";
					break;
				case SystemLanguage.German:
					lang = "DE";
					break;
				case SystemLanguage.Polish:
					lang = "PL";
					break;
				case SystemLanguage.Portuguese:
					lang = "PT";
					break;
				case SystemLanguage.Spanish:
					lang = "ES";
					break;
				case SystemLanguage.French:
					lang = "FR";
					break;
				case SystemLanguage.Chinese:
				case SystemLanguage.ChineseSimplified:
				case SystemLanguage.ChineseTraditional:
					lang = "ZH";
					break;
				case SystemLanguage.Japanese:
					lang = "JA";
					break;
				case SystemLanguage.Italian:
					lang = "IT";
					break;
				case SystemLanguage.Korean:
					lang = "KO";
					break;
				case SystemLanguage.Russian:
					lang = "RU";
					break;
				case SystemLanguage.Turkish:
					lang = "TR";
					break;
				case SystemLanguage.Danish:
					lang = "DA";
					break;
				case SystemLanguage.Norwegian:
					lang = "NB";
					break;
				case SystemLanguage.Faroese:
					lang = "FO";
					break;
				default:
					lang = "XX";
					break;
			}

			int index = Languages.FindIndex((string obj) => obj.Equals(lang));

			return index < 0 ? DefaultLanguage : Languages[index];
		}

		private string GetLanguage(string slang)
		{
			string lang = slang;

			int index = Languages.FindIndex((string obj) => obj.Equals(lang));

			return index < 0 ? DefaultLanguage : Languages[index];
		}

		private string GetSteamLanguage()
		{
#if STEAM_APP
			string lang;

			if(!SteamManager.Initialized)
			{
				return DefaultLanguage;
			}

			switch(Steamworks.SteamApps.GetCurrentGameLanguage())
			{
				case "english":
					lang = "EN";
					break;
				case "german":
					lang = "DE";
					break;
				case "polish":
					lang = "PL";
					break;
				case "portuguese":
					lang = "PT";
					break;
				case "spanish":
					lang = "ES";
					break;
				case "french":
					lang = "FR";
					break;
				case "schinese":
				case "tchinese":
					lang = "ZH";
					break;
				case "japanese":
					lang = "JA";
					break;
				case "italian":
					lang = "IT";
					break;
				case "koreana":
					lang = "KO";
					break;
				case "russian":
					lang = "RU";
					break;
				case "turkish":
					lang = "TR";
					break;
				case "danish":
					lang = "DA";
					break;
				case "norwegian":
					lang = "NB";
					break;
				default:
					lang = "XX";
					break;
			}

			int index = Languages.FindIndex((string obj) => obj.Equals(lang));

			return index < 0 ? DefaultLanguage : Languages[index];
#else
			return string.Empty;
#endif
		}
	}
}
