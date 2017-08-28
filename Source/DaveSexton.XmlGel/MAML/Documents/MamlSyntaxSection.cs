using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* syntaxSectionType (developerStructure.xsd)
	 * 
	 * Content:
	 *	- sections (MamlSections) element (0..1)
	 *	- legacySyntax (MamlLegacySyntax) (1..N)
	 */
	internal sealed class MamlSyntaxSection : MamlNode
	{
		public MamlSyntaxSection(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}