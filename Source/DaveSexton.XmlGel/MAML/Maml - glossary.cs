using System.Xml.Linq;

namespace DaveSexton.XmlGel.Maml
{
	partial class Maml
	{
		public static readonly XName Glossary = Namespace + "glossary";
		public static readonly XName GlossaryDiv = Namespace + "glossaryDiv";
		public static readonly XName GlossaryEntry = Namespace + "glossaryEntry";
		public static readonly XName Terms = Namespace + "terms";
		public static readonly XName Term = Namespace + "term";
		public static readonly XName RelatedEntry = Namespace + "relatedEntry";

		// relatedEntry attributes
		public static readonly XName TermId = "termId";
	}
}