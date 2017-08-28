using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* schemaHierarchyType (developerStructure.xsd)
	 * 
	 *	- Allows (0..N) link (MamlLink) elements as children
	 */
	internal sealed class MamlSchemaHierarchy : MamlInlineContainer
	{
		public MamlSchemaHierarchy(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}