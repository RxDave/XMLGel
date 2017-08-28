using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* inlineType (inline.xsd)
	 * 
	 * Attributes: (contentIdentificationSharingAndConditionGroup - base.xsd)
	 *	- address (ID : NCName)
	 *
	 * Content:
	 *	- Allows text (mixed content)
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
	 *	- Allows 0..N sharedContent (MamlSharedContent) elements as children
	 *	- Allows 0..N conditionalSection elements as children
	 *		- Sequence:
	 *			- conditions (MamlConditions) element
	 *			- conditionalContent (MamlInline) element
	 *	- Allows 0..N children of any non-MAML type (MamlNode) (Sandcastle Styles extension)
	 */
	internal class MamlInline : MamlInlineContainer
	{
		public MamlInline(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}
/*
		#region Public Properties
		public override TextElementType ElementType
		{
			get
			{
				return TextElementType.Inline;
			}
		}
		#endregion

		#region Private / Protected
		#endregion

		#region Constructors
		/// <summary>
		/// Constructs a new instance of the <see cref="GeneralMamlInlineParser" /> class.
		/// </summary>
		public GeneralMamlInlineParser(ParserFactory factory, System.Windows.ResourceDictionary resources, TextElementStyleManager styleManager, TextElementFactory textElementFactory)
			: base(factory, resources, styleManager, textElementFactory)
		{
		}
		#endregion

		#region Methods
		public override ParseResult Parse(XElement element, BlockCollection rootBlocks)
		{
			ICollection<ParseResult> results = ParseMixedContent(element, rootBlocks);

			// NOTE: Specify null element as the parse result since only content was parsed.

			return new ParseResult(null, true, results);
		}

		public override UnparseResult Unparse(ParserContext context, XContainer parent)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			Run run = context.Element as Run;

			if (run == null)
			{
				XContainer container = context.CloneNode(false, false, true) as XContainer
					?? parent;

				parent.Add(container);

				UnparseResult result = new UnparseResult(container, false);

				result.ChildResults = UnparseDeep(context, container);
				result.UnparsedChildren = true;

				return result;
			}
			else
				return new UnparseResult(UnparseText(context, parent), true);
		}

		public override System.Windows.Documents.TextElement InsertNewElement(DaveSexton.MamlEditor.Elements.Behaviors.BehaviorEventContext context, System.Windows.Documents.TextElement element, out System.Action assignStyle)
		{
			// TODO
			assignStyle = null;
			return null;
		}
		#endregion
	}
}
*/