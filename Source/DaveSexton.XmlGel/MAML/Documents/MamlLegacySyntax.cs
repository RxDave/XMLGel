using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* legacySyntaxType (developerStructure.xsd)
	 * 
	 * Attributes:
	 *	- language (DaveSexton.XmlGel.Maml.DevLanguage)
	 *	- xml:space
	 *
	 * Choice: (0..N)
	 *	- parameterReference (MamlParameterReference)
	 *	- legacyBold (MamlBold)
	 *	- legacyLink (MamlLink)
	 *	- legacyUnderline (MamlUnderline)
	 */
	internal sealed class MamlLegacySyntax : MamlNode
	{
		public MamlLegacySyntax(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}