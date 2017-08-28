using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* Undefined type used by codeExampleType (developerStructure.xsd)
	 * 
	 * Choice: 
	 *	- code (MamlCode)
	 *	- codeReference (MamlCodeReference)
	 */
	internal sealed class MamlCodeSection : MamlNode
	{
		public MamlCodeSection(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}