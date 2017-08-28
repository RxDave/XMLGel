using System.Xml.Linq;

namespace DaveSexton.XmlGel.Maml
{
	partial class Maml
	{
		public static readonly XName TitleAttribute = "title";
		public static readonly XName SourceAttribute = "source";
		public static readonly XName LanguageAttribute = "language";
		public static readonly XName RegionAttribute = "region";

		public static readonly XName ComputerOutput = Namespace + "computerOutput";
		public static readonly XName Code = Namespace + "code";
		public static readonly XName Comment = Namespace + "comment";
		public static readonly XName CodeReference = Namespace + "codeReference";
	}
}