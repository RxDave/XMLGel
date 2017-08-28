using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* localUri (inlineSoftware.xsd)
	 * 
	 *	- Extends textType
	 * 
	 * Attributes:
	 *	- type (DaveSexton.XmlGel.Maml.LocalUriKind)
	 */
	internal sealed class MamlLocalUri : MamlText
	{
		public MamlLocalUri(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}