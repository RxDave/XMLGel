using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* sectionContentType (structure.xsd)
	 * 
	 * Attributes: (contentIdentificationSharingAndConditionGroup - base.xsd)
	 *	- address (ID : NCName)
	 * 
	 *	Content: (For non-procedure documents, such as DeveloperConceptualDocument)
	 *		- Allows 0..N para (MamlPara) elements
	 *		- Allows 0..N alert (MamlAlert) elements
	 *		- Allows 0..N quote (MamlQuote) elements
	 *		- Allows 0..N sharedContentBlock (MamlSharedContent) elements
	 *		- Allows 0..N computerOutput (MamlInline) elements
	 *		- Allows 0..N code (MamlCode) elements
	 *		- Allows 0..N codeReference (MamlRestrictedText) elements
	 *		- Allows 0..N list (MamlList) elements
	 *		- Allows 0..N definitionTable (MamlDefinitionTable) elements
	 *		- Allows 0..N table (MamlTable) elements
	 *		- Allows 0..N procedure (MamlProcedure) elements
	 *		- Allows 0..N mediaLink (MamlMediaLink) elements
	 *		- Allows 0..N conditionalSection elements
	 *			- Sequence
	 *				- conditions (MamlConditions) element
	 *				- conditionalContent (MamlSectionContent) element
	 *		- Allows 0..N children of any non-MAML type (MamlNode) (Sandcastle Styles extension)
	 *		
	 *	Content: (For procedure documents, such as DeveloperHowToDocument)
	 *		- Allows 0..N para (MamlPara) elements
	 *		- Allows 0..N alert (MamlAlert) elements
	 *		- Allows 0..N quote (MamlQuote) elements
	 *		- Allows 0..N sharedContentBlock (MamlSharedContent) elements
	 *		- Allows 0..N computerOutput (MamlInline) elements
	 *		- Allows 0..N code (MamlCode) elements
	 *		- Allows 0..N codeReference (MamlRestrictedText) elements
	 *		- Allows 0..N list (MamlList) elements
	 *		- Allows 0..N definitionTable (MamlDefinitionTable) elements
	 *		- Allows 0..N table (MamlTable) elements
	 *		- Allows 0..N mediaLink (MamlMediaLink) elements
	 *		- Allows 0..N conditionalSection elements
	 *			- Sequence
	 *				- conditions (MamlConditions) element
	 *				- conditionalContent (MamlSectionContent) element
	 *		- Requires procedure (MamlProcedure) elements
	 *		- Allows 0..N elements, including procedure, as defined by the above Content group for non-procedure documents).
	 */
	internal class MamlSectionContent : MamlNode
	{
		public MamlSectionContent(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}