namespace LocalizeSpace
{
	public class KeyWord
	{
		public readonly string Key;
		public readonly string Word;

		public KeyWord(string key, string word)
		{
			Key = key;
			Word = word;
		}

		public override string ToString()
		{
			return "Key = " + Key + " / Word = " + Word;
		}
	}
}
