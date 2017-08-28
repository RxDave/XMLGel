using System.Xml.Linq;

namespace DaveSexton.XmlGel.Maml
{
	partial class Maml
	{
		public static readonly XName ParameterReference = Namespace + "parameterReference";
		public static readonly XName LinkText = Namespace + "linkText";
		public static readonly XName LinkAlternateText = Namespace + "linkAlternateText";
		public static readonly XName LinkUri = Namespace + "linkUri";
		public static readonly XName LinkTarget = Namespace + "linkTarget";
		public static readonly XName Link = Namespace + "link";
		public static readonly XName LegacyLink = Namespace + "legacyLink";
		public static readonly XName MediaLinkInline = Namespace + "mediaLinkInline";
		public static readonly XName Caption = Namespace + "caption";
		public static readonly XName Image = Namespace + "image";
		public static readonly XName CodeEntityReference = Namespace + "codeEntityReference";
		public static readonly XName SharedContentParameter = Namespace + "sharedContentParameter";
		public static readonly XName SharedContent = Namespace + "sharedContent";
		public static readonly XName ExternalLink = Namespace + "externalLink";

		// Attributes for codeEntityReference
		public static readonly XName QualifyHint = "qualifyHint";
		public static readonly XName AutoUpgrade = "autoUpgrade";
	}
}