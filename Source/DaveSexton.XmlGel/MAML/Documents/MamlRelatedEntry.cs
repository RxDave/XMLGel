using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* Anonymous type defined in glossaryEntry (glossary.xsd)
	 * 
	 * Attributes:
	 *	- termId (token XSD) - required
	 *	- relationType (string XSD)
	 */
	internal sealed class MamlRelatedEntry : MamlNode
	{
		public string TermId
		{
			get
			{
				return (string) Element.Attribute(Maml.TermId);
			}
		}

		public MamlRelatedEntry(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}