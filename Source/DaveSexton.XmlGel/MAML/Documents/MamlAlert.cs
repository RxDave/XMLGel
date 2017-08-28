using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* alert (blockCommon.xsd)
	 * 
	 * Attributes:
	 *	- class (DaveSexton.XmlGel.Maml.AlertKind)
	 *	
	 * Attributes: (contentIdentificationSharingAndConditionGroup - base.xsd)
	 *	- address (ID : NCName)
	 *	
	 * Content:
	 *	- Allows 1..N structureSimpleGroup elements as children
	 *		- para (MamlPara)
	 *		- list (MamlList)
	 */
	internal sealed class MamlAlert : MamlNode
	{
		public AlertKind? Kind
		{
			get
			{
				return EnumStringConverter.FromDocumentValue<AlertKind>((string) Element.Attribute(Maml.ClassAttribute));
			}
		}

		public MamlAlert(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}