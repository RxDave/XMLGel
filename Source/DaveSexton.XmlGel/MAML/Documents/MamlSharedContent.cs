using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* sharedContentType (inlineLinking.xsd)
	 * 
	 * Attributes: (linkingGroup - base.xsd)
	 *	- xlink:href (URI)
	 *	
	 * Attributes: (contentIdentificationSharingAndConditionGroup - base.xsd)
	 *	- address (ID : NCName)
	 *	
	 * Content:
	 *	- Allows 0..N sharedContentParameter (MamlSharedContentParameter) elements as children
	 */
	internal sealed class MamlSharedContent : MamlNode
	{
		public MamlSharedContent(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}