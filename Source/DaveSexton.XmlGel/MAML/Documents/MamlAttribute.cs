using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* attributeType (developerStructure.xsd)
	 * 
	 *	- Allows text (mixed content)
	 *	- Sequence:
	 *		- title (MamlTitle)
	 *		- table (MamlTable)
	 */
	internal sealed class MamlAttribute : MamlNode
	{
		public MamlAttribute(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}