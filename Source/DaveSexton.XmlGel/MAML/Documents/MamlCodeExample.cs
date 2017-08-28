using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* codeExampleType (developerStructure.xsd)
	 * 
	 * Attributes: (contentIdentificationSharingAndConditionGroup - base.xsd)
	 *	- address (ID : NCName)
	 * 
	 * Sequence:
	 *	- description (MamlNamedSection) (0..1)
	 *	- (MamlCodeSection) - Required; Without spanning element;
	 *	- comments (MamlNamedSection) (0..1)
	 *	- input (MamlCodeSection) (0..1)
	 *	- output (MamlCodeSection) (0..1)
	 *	- sampleOutput (MamlCodeSection) (0..1)
	 */
	internal sealed class MamlCodeExample : MamlNode
	{
		public MamlCodeExample(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}