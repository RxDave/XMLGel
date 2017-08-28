using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* xsd:any
	 * 
	 *	- May be a child of any element.
	 *	- Allows any child elements, including elements that are outside of the MAML namespace.
	 *	- Sandcastle Styles defines "any" elements in various places throughout the MAML schema.  They serve as points
	 *		of extensibility for custom build components, but they only allow child elements that are outside of the MAML 
	 *		namespace to be added.  This type is more forgiving to ensure that the editor is still usable even when the 
	 *		schema is violated.
	 */
	internal sealed class MamlAnyNode : MamlNode
	{
		public MamlAnyNode(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}