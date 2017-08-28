using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* externalResourcesType (glossary.xsd)
	 * 
	 * Requires:
	 *	- link (MamlLink)
	 */
	internal sealed class MamlExternalResources : MamlNode
	{
		public MamlExternalResources(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}