using System.Collections.Generic;
using UnityEngine;

namespace LocalizeSpace
{
	public interface ILocalizator
	{
		/// <summary>
		/// Выставляет дефолтным язык <paramref name="language"/>
		/// </summary>
		void SetLanguage(SystemLanguage language);
		/// <summary>
		/// Выставляет дефолтным язык <paramref name="language"/>
		/// </summary>
		void SetLanguage(string language);
		/// <summary>
		/// Выставляет дефолтным язык системыб загружает переводы 
		/// </summary>
		void Init(SystemLanguage language = SystemLanguage.Unknown);

		/// <summary>
		/// Возвращает переведенное слово по ключу <param name="key"/>
		/// </summary>
		string GetWord(string key);

		/// <summary>
		/// Возвращает доступные для перевода языки
		/// </summary>
		/// <returns>The languages.</returns>
		List<string> GetLanguages();

		bool HasKey(string key);

		void AddNewWord();
		void EraseWord();

		ILanguageParser LanguageParser { get; }
		bool IsCustomInitialize { get; }
		List<string> Keys { get; }
		List<string> Languages { get; }
		string CurrentLanguage { get; }
		bool IsInited { get; }
	}
}
