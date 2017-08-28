using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* fictitiousUri (inlineSoftware.xsd)
	 * 
	 *	- Extends textType
	 * 
	 * Attributes:
	 *	- type (DaveSexton.XmlGel.Maml.FictitiousUriKind)
	 */
	internal sealed class MamlFictitiousUri : MamlText
	{
		public MamlFictitiousUri(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}