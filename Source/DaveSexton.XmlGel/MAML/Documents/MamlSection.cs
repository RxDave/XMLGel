using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* sectionType (hierarchy.xsd)
	 * 
	 * Attributes: (contentIdentificationSharingAndConditionGroup - base.xsd)
	 *	- address (ID : NCName)
	 * 
	 * Sequence:
	 *	- title (MamlText) (0..1)
	 *	- content (MamlContent)
	 *	- sections (MamlSections) (0..1)
	 */
	internal sealed class MamlSection : MamlNode
	{
		public MamlSection(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}