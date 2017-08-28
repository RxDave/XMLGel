using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* glossaryDiv (glossary.xsd)
	 * 
	 * Attributes: (contentIdentificationSharingAndConditionGroup - base.xsd)
	 *	- address (ID : NCName)
	 * 
	 * Attributes:
	 *	- class (string XSD)
	 * 
	 * Sequence:
	 *	- title (MamlTitle) (0..1)
	 *	- glossaryEntry (MamlGlossaryEntry) (1..N)
	 */
	internal sealed class MamlGlossaryDiv : MamlNode
	{
		public MamlGlossaryDiv(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}