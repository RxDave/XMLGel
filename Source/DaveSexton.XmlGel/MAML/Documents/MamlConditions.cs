using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* conditionsType (conditionSet.xsd)
	 * 
	 * Content:
	 *	- Allows 1..N programmingLanguage (MamlEnum<DevLanguage>) elements as children
	 */
	internal sealed class MamlConditions : MamlNode
	{
		public MamlConditions(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}