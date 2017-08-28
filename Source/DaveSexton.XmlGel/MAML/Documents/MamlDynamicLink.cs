using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* dynamicLinkType (inlineCommon.xsd)
	 * 
	 * Attributes:
	 *	- type (DaveSexton.XmlGel.Maml.CopyrightKind)
	 * 
	 * Sequence:
	 *	- prefix (MamlText) (0..1)
	 *	- separator (MamlRestrictedText) (0..1)
	 *	- keyword (MamlRestrictedText) (1..N)
	 *	- suffix (MamlText) (0..1)
	 */
	internal sealed class MamlDynamicLink : MamlNode
	{
		public MamlDynamicLink(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}