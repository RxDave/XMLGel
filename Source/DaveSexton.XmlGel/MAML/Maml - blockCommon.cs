using System.Xml.Linq;

namespace DaveSexton.XmlGel.Maml
{
	partial class Maml
	{
		/* blockType (block.xsd)
		 * 
		 * Not used in schema.
		 */

		public static readonly XName ClassAttribute = "class";

		public static readonly XName Para = Namespace + "para";
		public static readonly XName Quote = Namespace + "quote";
		public static readonly XName Title = Namespace + "title";
		public static readonly XName SecondaryErrorTitle = Namespace + "secondaryErrorTitle";
		public static readonly XName NonLocErrorTitle = Namespace + "nonLocErrorTitle";
		public static readonly XName Alert = Namespace + "alert";
		public static readonly XName SharedContentBlock = Namespace + "sharedContentBlock";
		public static readonly XName Introduction = Namespace + "introduction";
		public static readonly XName Description = Namespace + "description";
		public static readonly XName Conclusion = Namespace + "conclusion";
	}
}