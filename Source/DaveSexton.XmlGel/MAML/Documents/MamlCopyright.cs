using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* copyright (inlineCommon.xsd)
	 * 
	 * Attributes:
	 *	- class (DaveSexton.XmlGel.Maml.CopyrightKind)
	 * 
	 * Sequence:
	 *	- trademark (MamlRestrictedText)
	 *	- year (int) (0..N)
	 *	- holder (MamlRestrictedText) (0..N)
	 */
	internal sealed class MamlCopyright : MamlNode
	{
		public MamlCopyright(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}