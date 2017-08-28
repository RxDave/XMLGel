using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* codeReferenceType (blockSoftware.xsd)
	 * 
	 *	- Extends: token (MamlRestrictedText)
	 */
	internal sealed class MamlCodeReference : MamlRestrictedText
	{
		public MamlCodeReference(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}