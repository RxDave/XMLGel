using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* platformNotesType (developerReference.xsd)
	 * 
	 *	- Allows (0..N) platformNote (MamlPlatformNote)
	 */
	internal sealed class MamlPlatformNotes : MamlNode
	{
		public MamlPlatformNotes(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}