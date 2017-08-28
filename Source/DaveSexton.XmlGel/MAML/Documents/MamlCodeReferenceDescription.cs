using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* codeReferenceDescriptionType (developerReference.xsd)
	 * 
	 * Sequence:
	 *	- codeEntityReference (MamlCodeEntityReference)
	 *	- content (MamlContent)
	 */
	internal sealed class MamlCodeReferenceDescription : MamlNode
	{
		public MamlCodeReferenceDescription(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}