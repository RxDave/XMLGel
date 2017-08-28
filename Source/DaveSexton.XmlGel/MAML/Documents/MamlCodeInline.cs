using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* codeInline (inlineSoftware.xsd)
	 * 
	 *	- Allows text (mixed content)
	 *	- Allows 0..N codeFeaturedElement (MamlText) elements
	 */
	internal sealed class MamlCodeInline : MamlNode
	{
		public MamlCodeInline(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}