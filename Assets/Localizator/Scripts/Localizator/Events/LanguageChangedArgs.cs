using System;

public class LanguageChangedArgs : EventArgs
{
	private readonly string language;
	public string Language
	{
		get
		{
			return language;
		}
	}

	public LanguageChangedArgs(string language)
	{
		this.language = language;
	}
}