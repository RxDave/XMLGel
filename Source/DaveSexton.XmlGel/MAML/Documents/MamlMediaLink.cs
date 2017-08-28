using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* mediaLinkType (inlineLinking.xsd)
	 * 
	 * Sequence:
	 *	- caption (string) (0..1)
	 *		- Attributes: (Sandcastle Styles extension)
	 *			- placement (DaveSexton.XmlGel.Maml.CaptionPlacement)
	 *			- lead (string)
	 *	- image (MamlImage)
	 */
	internal sealed class MamlMediaLink : MamlNode
	{
		public MamlMediaLink(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}