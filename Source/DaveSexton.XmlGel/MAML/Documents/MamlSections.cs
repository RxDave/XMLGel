using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* sectionsType (hierarchy.xsd)
	 * 
	 * Attributes: (contentIdentificationSharingAndConditionGroup - base.xsd)
	 *	- address (ID : NCName)
	 * 
	 * Content:
	 *	- Allows 0..N section (MamlSection) elements
	 *	- Allows 0..N sectionSimple (MamlSectionSimple) elements
	 *	- Allows 0..N conditionalSection elements
	 *		- Sequence
	 *			- conditions (MamlConditions) element
	 *			- conditionalContent (MamlSections) element
	 */
	internal sealed class MamlSections : MamlNode
	{
		public MamlSections(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}