using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* codeEntitiesType (developerReference.xsd)
	 * 
	 *	- Allows (0..N) codeEntity (MamlCodeEntity)
	 */
	internal sealed class MamlCodeEntities : MamlNode
	{
		public MamlCodeEntities(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}