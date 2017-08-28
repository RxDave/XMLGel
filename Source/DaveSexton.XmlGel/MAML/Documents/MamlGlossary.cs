using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* glossary (glossary.xsd)
	 * 
	 * Attributes: (contentIdentificationSharingAndConditionGroup - base.xsd)
	 *	- address (ID : NCName)
	 * 
	 * Attributes:
	 *	- contentType (token XSD)
	 * 
	 * Sequence:
	 *	- title (MamlTitle) (0..1)
	 *	- Choice: (1..N)
	 *		- glossaryDiv (MamlGlossaryDiv)
	 *		- glossaryEntry (MamlGlossaryEntry)
	 */
	internal sealed class MamlGlossary : MamlNode
	{
		public MamlGlossary(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}