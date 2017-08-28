using System.Collections.Generic;
using System.Xml.Linq;

namespace DaveSexton.XmlGel.Maml
{
	partial class Maml
	{
		public static readonly XName Phrase = Namespace + "phrase";
		public static readonly XName LegacyBold = Namespace + "legacyBold";
		public static readonly XName LegacyItalic = Namespace + "legacyItalic";
		public static readonly XName LegacyUnderline = Namespace + "legacyUnderline";
		public static readonly XName NotLocalizable = Namespace + "notLocalizable";
		public static readonly XName LocalizedText = Namespace + "localizedText";
		public static readonly XName EmbeddedLabel = Namespace + "embeddedLabel";
		public static readonly XName QuoteInline = Namespace + "quoteInline";
		public static readonly XName Date = Namespace + "date";
		public static readonly XName ForeignPhrase = Namespace + "foreignPhrase";
		public static readonly XName Corporation = Namespace + "corporation";
		public static readonly XName Country = Namespace + "country";
		public static readonly XName Suffix = Namespace + "suffix";
		public static readonly XName Prefix = Namespace + "prefix";
		public static readonly XName Subscript = Namespace + "subscript";
		public static readonly XName Superscript = Namespace + "superscript";
		public static readonly XName Trademark = Namespace + "trademark";
		public static readonly XName Holder = Namespace + "holder";
		public static readonly XName Separator = Namespace + "separator";
		public static readonly XName Keyword = Namespace + "keyword";
		public static readonly XName Year = Namespace + "year";
		public static readonly XName Copyright = Namespace + "copyright";
		public static readonly XName DynamicLink = Namespace + "dynamicLink";

		private static readonly HashSet<XName> inlines = new HashSet<XName>()
		{
			Application, 
			Database, 
			FictitiousUri, 
			LocalUri, 
			EnvironmentVariable, 
			ErrorInline, 
			Hardware, 
			Literal, 
			Markup, 
			Command, 
			Token, 
			CodeInline, 
			LanguageKeyword, 
			ComputerOutputInline, 
			UI, 
			Math, 
			System, 
			NewTerm, 
			Placeholder, 
			UnmanagedCodeEntityReference, 
			UserInput, 
			UserInputLocalizable, 
			QuoteInline, 
			Date, 
			ForeignPhrase, 
			Phrase, 
			Copyright, 
			Corporation, 
			Country,
			NotLocalizable,
			Subscript, 
			Superscript, 
			LegacyBold, 
			LegacyItalic,
			LegacyUnderline,
			DynamicLink, 
			LocalizedText, 
			EmbeddedLabel, 
			CodeEntityReference, 
			ParameterReference, 
			Link, 
			MediaLinkInline, 
			ExternalLink, 
			SharedContent, 
			Conditions
		};

		internal static bool IsInline(XElement element)
		{
			return inlines.Contains(element.Name);
		}
	}
}