using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace LocalizeSpace
{
	public partial class Localizator : MonoBehaviour, ILocalizator
	{
		private const string _menuLangPrefix = "Project/Tools/Language/Set Debug Language/";

#if UNITY_EDITOR
		[MenuItem(_menuLangPrefix + "EN", false)]
		static void SetDeveloperLanguageEN() { SetDebugLanguage("EN"); }
		[MenuItem(_menuLangPrefix + "EN", true)]
		static bool IsDeveloperLanguageEN() { return !DebugLanguage.Equals("EN"); }

		[MenuItem(_menuLangPrefix + "RU", false)]
		static void SetDeveloperLanguageRU() { SetDebugLanguage("RU"); }
		[MenuItem(_menuLangPrefix + "RU", true)]
		static bool IsDeveloperLanguageRU() { return !DebugLanguage.Equals("RU"); }

		[MenuItem(_menuLangPrefix + "ZH", false)]
		static void SetDeveloperLanguageZH() { SetDebugLanguage("ZH"); }
		[MenuItem(_menuLangPrefix + "ZH", true)]
		static bool IsDeveloperLanguageZH() { return !DebugLanguage.Equals("ZH"); }

		[MenuItem(_menuLangPrefix + "DE", false)]
		static void SetDeveloperLanguageDE() { SetDebugLanguage("DE"); }
		[MenuItem(_menuLangPrefix + "DE", true)]
		static bool IsDeveloperLanguageDE() { return !DebugLanguage.Equals("DE"); }

		[MenuItem(_menuLangPrefix + "FR", false)]
		static void SetDeveloperLanguageFR() { SetDebugLanguage("FR"); }
		[MenuItem(_menuLangPrefix + "FR", true)]
		static bool IsDeveloperLanguageFR() { return !DebugLanguage.Equals("FR"); }

		[MenuItem(_menuLangPrefix + "JA", false)]
		static void SetDeveloperLanguageJA() { SetDebugLanguage("JA"); }
		[MenuItem(_menuLangPrefix + "JA", true)]
		static bool IsDeveloperLanguageJA() { return !DebugLanguage.Equals("JA"); }

		[MenuItem(_menuLangPrefix + "KO", false)]
		static void SetDeveloperLanguageKO() { SetDebugLanguage("KO"); }
		[MenuItem(_menuLangPrefix + "KO", true)]
		static bool IsDeveloperLanguageKO() { return !DebugLanguage.Equals("KO"); }

		[MenuItem(_menuLangPrefix + "DA", false)]
		static void SetDeveloperLanguageDA() { SetDebugLanguage("DA"); }
		[MenuItem(_menuLangPrefix + "DA", true)]
		static bool IsDeveloperLanguageDA() { return !DebugLanguage.Equals("DA"); }

		[MenuItem(_menuLangPrefix + "IT", false)]
		static void SetDeveloperLanguageIT() { SetDebugLanguage("IT"); }
		[MenuItem(_menuLangPrefix + "IT", true)]
		static bool IsDeveloperLanguageIT() { return !DebugLanguage.Equals("IT"); }

		[MenuItem(_menuLangPrefix + "ES", false)]
		static void SetDeveloperLanguageES() { SetDebugLanguage("ES"); }
		[MenuItem(_menuLangPrefix + "ES", true)]
		static bool IsDeveloperLanguageES() { return !DebugLanguage.Equals("ES"); }

		[MenuItem(_menuLangPrefix + "PL", false)]
		static void SetDeveloperLanguagePL() { SetDebugLanguage("PL"); }
		[MenuItem(_menuLangPrefix + "PL", true)]
		static bool IsDeveloperLanguagePL() { return !DebugLanguage.Equals("PL"); }

		[MenuItem(_menuLangPrefix + "PT", false)]
		static void SetDeveloperLanguagePT() { SetDebugLanguage("PT"); }
		[MenuItem(_menuLangPrefix + "PT", true)]
		static bool IsDeveloperLanguagePT() { return !DebugLanguage.Equals("PT"); }

		[MenuItem(_menuLangPrefix + "NB", false)]
		static void SetDeveloperLanguageNB() { SetDebugLanguage("NB"); }
		[MenuItem(_menuLangPrefix + "NB", true)]
		static bool IsDeveloperLanguageNB() { return !DebugLanguage.Equals("NB"); }

		[MenuItem(_menuLangPrefix + "TR", false)]
		static void SetDeveloperLanguageTR() { SetDebugLanguage("TR"); }
		[MenuItem(_menuLangPrefix + "TR", true)]
		static bool IsDeveloperLanguageTR() { return !DebugLanguage.Equals("TR"); }
#endif
		static void SetDebugLanguage(string lang)
		{
			DebugLanguage = lang;
			PlayerPrefs.SetString("debugLanguageS", lang);
			PlayerPrefs.Save();
			Instance.SetLanguage(DebugLanguage);
			OnLanguageChanged(Instance, new LanguageChangedArgs(DebugLanguage));
		}
	}
}
