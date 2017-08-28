using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* integer (XSD type)
	 * 
	 *	- any integer
	 */
	internal sealed class MamlInteger : MamlNode
	{
		public int? Value
		{
			get
			{
				int value;
				return int.TryParse(Element.Value, out value)
					? value
					: (int?) null;
			}
		}

		public MamlInteger(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}