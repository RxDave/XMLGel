using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* glossaryEntry (glossary.xsd)
	 * 
	 * Attributes: (contentIdentificationSharingAndConditionGroup - base.xsd)
	 *	- address (ID : NCName)
	 * 
	 * Attributes:
	 *	- platform (string XSD)
	 *	- product (string XSD)
	 * 
	 * Sequence:
	 *	- terms (MamlTerms)
	 *	- definition (MamlDefinition)
	 *	- relatedEntry (MamlRelatedEntry) (0..N)
	 */
	internal sealed class MamlGlossaryEntry : MamlNode
	{
		public MamlGlossaryEntry(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}