using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* string (XSD type)
	 * 
	 *	- any character data
	 */
	internal class MamlString : MamlNode
	{
		public virtual string Text
		{
			get
			{
				return Element.Value;
			}
		}

		public MamlString(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}