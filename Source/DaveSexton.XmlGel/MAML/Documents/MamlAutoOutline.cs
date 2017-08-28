using System;
using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* Anonymous type used by sectionContentWithOutlineType (structure.xsd)
	 * 
	 * Choice: (0..N)
	 *		structureGroup (blocks)
	 *		autoOutline (MamlAutoOutline)
	 *		##other (MamlAnyNode)
	 */
	internal sealed class MamlAutoOutline : MamlNode
	{
		public string Lead
		{
			get
			{
				return (string) Element.Attribute(Maml.Lead);
			}
		}

		public bool ShowLead
		{
			get
			{
				return !string.Equals(Lead, "none", StringComparison.Ordinal);
			}
		}

		public bool ExcludeRelatedTopics
		{
			get
			{
				return (bool?) Element.Attribute(Maml.ExcludeRelatedTopics) ?? false;
			}
		}

		public MamlAutoOutline(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}