using System.Xml.Linq;

namespace DaveSexton.XmlGel.Maml
{
	internal static partial class Maml
	{
		public const string MamlNamespaceUri = "http://ddue.schemas.microsoft.com/authoring/2003/5";
		public const string XLinkNamespaceUri = "http://www.w3.org/1999/xlink";
		public const string XLinkPrefixValue = "xlink";

		public static readonly XNamespace Namespace = MamlNamespaceUri;
		public static readonly XNamespace XLinkNamespace = XLinkNamespaceUri;

		public static readonly XName XLinkPrefix = XNamespace.Xmlns + XLinkPrefixValue;
		public static readonly XName XLinkHref = XLinkNamespace + "href";

		public static readonly XName TopicRoot = "topic";
		public static readonly XName TopicId = "id";
		public static readonly XName TopicRevisionNumber = "revisionNumber";
	}
}