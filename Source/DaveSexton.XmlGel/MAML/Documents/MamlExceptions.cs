using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* exceptionsType (developerReference.xsd)
	 * 
	 *	- Allows (0..N) exception (MamlCodeReferenceDescription)
	 */
	internal sealed class MamlExceptions : MamlNode
	{
		public MamlExceptions(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}