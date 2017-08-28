using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* singleParagraphSimpleContentType (developerStructure.xsd)
	 * 
	 *	 - Allows (0..1) para (MamlPara) element
	 */
	internal class MamlSingleParagraphSimpleContent : MamlNode
	{
		public MamlSingleParagraphSimpleContent(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}