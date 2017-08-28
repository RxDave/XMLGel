using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* code (blockSoftware.xsd)
	 * 
	 * Attributes:
	 *	- language (DaveSexton.XmlGel.Maml.DevLanguage)
	 *	- title (string) (Sandcastle Styles extension)
	 *	- source (string) (Sandcastle Styles extension)
	 *	- region (string) (Sandcastle Styles extension)
	 *	
	 * Attributes: (contentIdentificationSharingAndConditionGroup - base.xsd)
	 *	- address (ID : NCName)
	 *	
	 * Content:
	 *	- Allows text (mixed content)
	 *	- Allows 0..N codeFeaturedElement (MamlText) elements as children
	 *	- Allows 0..N placeholder (MamlText) elements as children
	 *	- Allows 0..N comment (string) elements as children
	 */
	internal sealed class MamlCode : MamlNode
	{
		public override bool? NormalizeText
		{
			get
			{
				return false;
			}
		}

		public DevLanguage? Language
		{
			get
			{
				return EnumStringConverter.FromDocumentValue<DevLanguage>((string) Element.Attribute(Maml.LanguageAttribute));
			}
		}

		public string Title
		{
			get
			{
				return (string) Element.Attribute(Maml.TitleAttribute);
			}
		}

		public string Source
		{
			get
			{
				return (string) Element.Attribute(Maml.SourceAttribute);
			}
		}

		public string Region
		{
			get
			{
				return (string) Element.Attribute(Maml.RegionAttribute);
			}
		}

		public MamlCode(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}