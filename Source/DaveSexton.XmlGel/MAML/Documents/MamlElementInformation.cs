using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* tableType (developerStructure.xsd)
	 * 
	 * Requires:
	 *	- table (MamlTable)
	 */
	internal sealed class MamlElementInformation : MamlNode
	{
		public MamlElementInformation(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}