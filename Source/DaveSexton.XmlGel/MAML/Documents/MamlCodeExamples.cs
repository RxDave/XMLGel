using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* codeExamplesType (developerStructure.xsd)
	 * 
	 *	- Allows (0..N) codeExample (MamlCodeExample) elements as children
	 */
	internal sealed class MamlCodeExamples : MamlNode
	{
		public MamlCodeExamples(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}