using System.Collections.Generic;

namespace LocalizeSpace
{
	public interface ILanguageParser
	{
		void LoadLanguages();
		void LoadLocalization(string lang, bool isDefault = false);
		List<string> GetLanguages();
		void AddWord();
		void SaveWord();
		void SaveWord(string key, string[] data);
		void DeleteWord();
		void SaveDeleted();

		string NewKey { get; set; }
		string OldKey { get; set; }
		string[] NewWords { get; set; }
		Localizator Localizator { get; set; }
	}
}
