using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* textType (base.xsd)
	 * 
	 *	- normalizedString (XSD type)
	 *		- any character data
	 *		- whitespace is normalized to a single space, without collapsing
	 */
	internal class MamlText : MamlString
	{
		public MamlText(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}