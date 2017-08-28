using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* relatedTopicsType (hierarchy.xsd)
	 * 
	 * Attributes: (contentIdentificationSharingAndConditionGroup - base.xsd)
	 *	- address (ID : NCName)
	 * 
	 * Attributes:
	 *	- topicType_id (DaveSexton.XmlGel.Maml.RelatedTopicCategory)
	 * 
	 * Content:
	 *	- Allows 0..N link (MamlLink) elements
	 *	- Allows 0..N codeEntityReference (MamlCodeEntityReference) elements
	 *	- Allows 0..N externalLink (MamlExternalLink) elements
	 *	- Allows 0..N sampleRef (MamlSampleRef) elements
	 *	- Allows 0..N token (MamlText) elements (Sandcastle Styles extension)
	 *	- Allows 0..N children of any non-MAML type (MamlNode) (Sandcastle Styles extension)
	 */
	internal sealed class MamlRelatedTopics : MamlNode
	{
		public MamlRelatedTopics(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}