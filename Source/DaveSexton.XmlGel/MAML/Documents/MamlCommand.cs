using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* command (inlineSoftware.xsd)
	 * 
	 *	- Allows text (mixed content)
	 *	- Allows 0..N system (MamlText) elements (Sandcastle Styles extension)
	 *	- Allows 0..N replaceable (MamlText) elements (Sandcastle Styles extension)
	 */
	internal sealed class MamlCommand : MamlNode
	{
		public MamlCommand(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}