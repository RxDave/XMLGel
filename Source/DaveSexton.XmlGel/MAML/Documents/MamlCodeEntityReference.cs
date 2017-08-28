using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* codeEntityReferenceType (inlineLinking.xsd)
	 * 
	 *	- Extends: token (XSD)
	 * 
	 * Attributes:
	 *	- qualifyHint (bool)
	 *	- autoUpgrade (bool)
	 */
	internal sealed class MamlCodeEntityReference : MamlRestrictedText
	{
		public MamlCodeEntityReference(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}