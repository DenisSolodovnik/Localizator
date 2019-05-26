using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LocalizeSpace
{
#if UNITY_EDITOR
	[ExecuteInEditMode]
#endif
	public class UILocalize : MonoBehaviour
	{
		public enum Capitalization
		{
			None = 0,
			ToUpperCase,
			ToLowerCase,
			ToTitleCase,
			ToSentenceCase
		}

		private bool isEnabled = true;

		public Text TextCanvas;
		public TextMesh TextNoCanvas;
		public string Key = string.Empty;
		public Capitalization Capitalize = Capitalization.None;

		[SerializeField]
		public List<LangBool> checkFont = new List<LangBool>();
		[SerializeField]
		public List<LangBool> checkSize = new List<LangBool>();

		[SerializeField]
		public int DefaultSize = 0;
		[SerializeField]
		public Font DefaultFont;

		[SerializeField]
		private List<Font> langFont = new List<Font>();
		public List<Font> LangFont
		{
			get
			{
				if (langFont.Count == 0)
				{
					foreach (var item in Localizator.Instance.Languages)
					{
						langFont.Add(null);
					}
				}
				return langFont;
			}
			set
			{
				langFont = value;
			}
		}

		[SerializeField]
		private List<int> langSize = new List<int>();
		public List<int> LangSize
		{
			get
			{
				if (langSize.Count == 0)
				{
					foreach (var item in Localizator.Instance.Languages)
					{
						langSize.Add(0);
					}
				}
				return langSize;
			}
			set
			{
				langSize = value;
			}
		}

		public string LocalizedString;

		[SerializeField]
		public bool LocalizeToString = false;

		private void Awake()
		{
			Init();
			Localizator.OnLanguageChanged += HandleLanguageChanged;
		}

		private void OnDestroy()
		{
			Localizator.OnLanguageChanged -= HandleLanguageChanged;
		}

		public void Init()
		{
			GetTextComponent();

			if (isEnabled)
			{
				enabled = true;
			}
			else
			{
				enabled = false;
				Debug.LogError("No Text component on object: " + name);
				return;
			}

#if UNITY_EDITOR
			InitFontNameList();
			InitFontSizeList();
			InitDefaults();
#endif

		}

		private void Start()
		{
			LocalizeWord();
		}

		private void GetTextComponent()
		{
			TextCanvas = GetComponent<Text>();
			if (TextCanvas == null)
			{
				TextNoCanvas = GetComponent<TextMesh>();
				if (TextNoCanvas == null)
				{
					isEnabled = false;
				}
				else
				{
					isEnabled = true;
				}
			}
			else
			{
				isEnabled = true;
			}
		}

		public Font GetFont()
		{
			if (!isEnabled)
			{
				return null;
			}

			if (TextCanvas != null)
			{
				return TextCanvas.font;
			}

			if (TextNoCanvas != null)
			{
				return TextNoCanvas.font;
			}

			return null;
		}

		public int GetFontSize()
		{
			if (!isEnabled)
			{
				return -1;
			}

			if (TextCanvas != null)
			{
				return TextCanvas.fontSize;
			}

			if (TextNoCanvas != null)
			{
				return TextNoCanvas.fontSize;
			}

			return -1;
		}

		public void SetKey(string key)
		{
			if (!isEnabled)
			{
				return;
			}

			Key = key;
			LocalizeWord();
		}

		public void LocalizeWord()
		{
			if (!isEnabled)
			{
				return;
			}

			if (!Localizator.Instance.HasKey(Key))
			{
				if (Localizator.Instance.Keys == null)
				{
					Debug.Log("Keys == null");
				}

				Key = Localizator.Instance.Keys[0];
			}

			string lText = Localizator.Instance.GetWord(Key);
			switch (Capitalize)
			{
				case Capitalization.None:
					break;
				case Capitalization.ToUpperCase:
					lText = lText.ToUpper();
					break;
				case Capitalization.ToLowerCase:
					lText = lText.ToLower();
					break;
				case Capitalization.ToTitleCase:
					lText = System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(lText.ToLower());
					break;
				case Capitalization.ToSentenceCase:
					if (!string.IsNullOrEmpty(lText))
						lText = lText[0].ToString().ToUpper() + lText.ToLower().Substring(1);
					break;
				default:
					break;
			}

			int index;
			if (Localizator.Instance.IsInited)
			{
				index = Localizator.Instance.Languages.FindIndex((txt) => txt.Equals(Localizator.Instance.CurrentLanguage));
				if (index < 0)
				{
					index = 0;
				}
			}
			else
			{
				Debug.LogError("Localizator is NOT ready!");
				index = 0;
			}


			if (TextCanvas != null)
			{
				if (!LocalizeToString)
				{
					TextCanvas.text = lText;
				}

				LocalizedString = lText;

				TextCanvas.font = checkFont[index].Use ? LangFont[index] : DefaultFont;
				TextCanvas.fontSize = checkSize[index].Use ? LangSize[index] : DefaultSize;
			}
			if (TextNoCanvas != null)
			{
				if (!LocalizeToString)
				{
					TextNoCanvas.text = lText;
				}

				LocalizedString = lText;

				TextNoCanvas.font = checkFont[index].Use ? LangFont[index] : DefaultFont;
				TextNoCanvas.fontSize = checkSize[index].Use ? LangSize[index] : DefaultSize;
			}

#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
#endif
		}

		private void HandleLanguageChanged(object sender, LanguageChangedArgs e)
		{
			if (!isEnabled)
			{
				return;
			}

			LocalizeWord();
		}

		public void InitFontNameList()
		{
			if (checkFont.Count == Localizator.Instance.Languages.Count)
			{
				if (checkFont.Count > 0)
				{
					return;
				}
			}

			if (checkFont.Count < Localizator.Instance.Languages.Count)
			{
				for (int i = 0; i < Localizator.Instance.Languages.Count; i++)
				{
					int index = checkFont.FindIndex((LangBool str) => str.Language.Equals(Localizator.Instance.Languages[i], System.StringComparison.OrdinalIgnoreCase));
					if (index < 0)
					{
						checkFont.Add(new LangBool(Localizator.Instance.Languages[i], false));
					}
				}
			}
			else
			{
				for (int i = 0; i < checkFont.Count; i++)
				{
					int index = Localizator.Instance.Languages.FindIndex((string str) => str.Equals(checkFont[i].Language, System.StringComparison.OrdinalIgnoreCase));
					if (index < 0)
					{
						checkFont.RemoveAt(i);
					}
				}
			}
		}

		public void InitFontSizeList()
		{
			if (checkSize.Count == Localizator.Instance.Languages.Count)
			{
				if (checkSize.Count > 0)
				{
					return;
				}
			}

			if (checkSize.Count < Localizator.Instance.Languages.Count)
			{
				for (int i = 0; i < Localizator.Instance.Languages.Count; i++)
				{
					int index = checkSize.FindIndex((LangBool str) => str.Language.Equals(Localizator.Instance.Languages[i], System.StringComparison.OrdinalIgnoreCase));
					if (index < 0)
					{
						checkSize.Add(new LangBool(Localizator.Instance.Languages[i], false));
					}
				}
			}
			else
			{
				for (int i = 0; i < checkSize.Count; i++)
				{
					int index = Localizator.Instance.Languages.FindIndex((string str) => str.Equals(checkSize[i].Language, System.StringComparison.OrdinalIgnoreCase));
					if (index < 0)
					{
						checkSize.RemoveAt(i);
					}
				}
			}
		}

		public void InitDefaults()
		{
			DefaultFont = (TextCanvas != null) ? TextCanvas.font : TextNoCanvas.font;
			DefaultSize = (TextCanvas != null) ? TextCanvas.fontSize : TextNoCanvas.fontSize;
		}
	}
}