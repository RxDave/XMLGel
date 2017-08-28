using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* parametersType (developerReference.xsd)
	 * 
	 *	- Allows (0..N) parameter (MamlParameterReferenceDescription)
	 */
	internal sealed class MamlParameters : MamlNode
	{
		public MamlParameters(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}