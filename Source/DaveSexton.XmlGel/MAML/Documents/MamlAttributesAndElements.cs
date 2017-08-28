using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* attributesandElementsType (developerStructure.xsd)
	 * 
	 *	- Allows text (mixed content)
	 *	- Sequence:
	 *		- para (MamlPara)
	 *		- attributes (MamlAttributes)
	 *			- Allows text (mixed content)
	 *			- Choice:
	 *				- para (MamlPara)
	 *				- table (MamlTable)
	 *		- attribute (MamlAttribute) (0..N)
	 *		- childElement (MamlAttributes)
	 *			- Allows text (mixed content)
	 *			- Choice:
	 *				- para (MamlPara)
	 *				- table (MamlTable)
	 *		- parentElement (MamlAttributes)
	 *			- Allows text (mixed content)
	 *			- Choice:
	 *				- para (MamlPara)
	 *				- table (MamlTable)
	 */
	internal sealed class MamlAttributesAndElements : MamlNode
	{
		public MamlAttributesAndElements(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}