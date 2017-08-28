using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* attributesType (developerStructure.xsd)
	 * 
	 *	- Allows text (mixed content)
	 *	- Sequence:
	 *		- para (MamlPara)
	 *		- table (MamlTable)
	 */
	internal sealed class MamlAttributes : MamlNode
	{
		public MamlAttributes(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}