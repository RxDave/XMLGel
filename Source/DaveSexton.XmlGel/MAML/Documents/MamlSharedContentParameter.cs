using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* sharedContentParameterType (inlineLinking.xsd)
	 * 
	 * Attributes:
	 *	- name (token XSD)
	 *	
	 * Content:
	 *	- Allows 0..N inline group (MamlText unless otherwise specified) elements as children
	 *		- application, database, fictitiousUri (MamlFictitiousUri), localUri (MamlLocalUri), environmentVariable, errorInline, hardware, 
	 *			literal, markup (any type), command (MamlCommand), token, codeInline (MamlCodeInline), languageKeyword, computerOutputInline, 
	 *			ui, math (MamlInline), system, newTerm, placeholder, unmanagedCodeEntityReference
	 *		- userInput (MamlRestrictedText), userInputLocalizable (MamlRestrictedText)
	 *		- quoteInline, date, foreignPhrase, phrase (MamlInline), copyright (MamlCopyright), corporation, country,
	 *			notLocalizable (MamlInline), subscript (MamlRestrictedText), superscript (MamlRestrictedText), dynamicLink (MamlDynamicLink), 
	 *			localizedText (MamlInline), embeddedLabel (MamlInline)
	 *		- codeEntityReference (MamlCodeEntityReference), parameterReference, link (MamlLink), mediaLinkInline (MamlMediaLinkInline), 
	 *			externalLink (MamlExternalLink)
	 */
	internal sealed class MamlSharedContentParameter : MamlInlineContainer
	{
		public MamlSharedContentParameter(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}