using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* No base type
	 * 
	 * Attributes: (linkingGroup - base.xsd)
	 *	- xlink:href
	 *	
	 * Attributes: (defined on mediaLinkType only, but also being applied here to mediaLinkInline)
	 *	- placement (DaveSexton.XmlGel.Maml.ImagePlacement) (Sandcastle Styles extension)
	 */
	internal sealed class MamlImage : MamlNode
	{
		public MamlImage(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}