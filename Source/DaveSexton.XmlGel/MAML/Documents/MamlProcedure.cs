using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* procedureType (structureProcedure.xsd)
	 * 
	 * Attributes: (contentIdentificationSharingAndConditionGroup - base.xsd)
	 *	- address (ID : NCName)
	 * 
	 * Sequence:
	 *	- title (MamlText) element
	 *	- steps (MamlSteps) element
	 *	- conclusion (MamlNamedSection) element (0..1)
	 */
	internal sealed class MamlProcedure : MamlNode
	{
		public MamlProcedure(XElement element)
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
	public class MamlProcedure : GeneralMamlBlockParser
	{
		#region Methods
		protected override InputBehavior CreateInputBehavior()
		{
			return new ProcedureBehavior();
		}

		public override ParseResult Parse(XElement element, BlockCollection rootBlocks)
		{
			ParseResult result = base.Parse(element, rootBlocks);

			if (result.ParsedChildren)
				return result;
			else
				result.ParsedChildren = true;

			List<ParseResult> childResults = new List<ParseResult>(32);

			Section section = GetTextElement<Section>(element);

			XElement title = element.Element(Maml.Title);

			if (title == null)
			{
				title = Maml.XTitleDefault;
				element.Add(title);
			}

			childResults.AddRange(
				ParseAnother(title, rootBlocks, true));

			Paragraph titleParagraph = GetTextElement<Paragraph>(title);

			ParserContext context = ParserContext.GetContext(titleParagraph, false);
			context.InputBehavior = behavior.TitleBehavior;
			context.LogicalContainer = section;

			Action assignStyle;
			context.AssignSpecialStyle("procedure-title", out assignStyle);

			result.AddCompleteCallback(assignStyle);

			XElement steps = element.Element(Maml.Steps);

			if (steps == null)
			{
				steps = Maml.XStepsDefault;
				element.Add(steps);
			}

			childResults.AddRange(
				ParseAnother(steps, rootBlocks, true));

			result.ChildResults = childResults;

			return result;
		}

		public override UnparseResult Unparse(ParserContext context, XContainer parent)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			if (context.HasDefaultValue)
			{
				XNode node = Maml.XProcedure;
				parent.Add(node);

				return new UnparseResult(node, true);
			}

			UnparseResult result = base.Unparse(context, parent);

			if (result.UnparsedChildren)
				return result;
			else
				result.UnparsedChildren = true;

			List<UnparseResult> childResults = new List<UnparseResult>(32);

			XElement element = (XElement) result.Node;
			Section section = (Section) context.Element;

			Paragraph titleParagraph = (Paragraph) section.Blocks.FirstBlock;

			Inline titleText = titleParagraph.Inlines.FirstInline;

			XElement title = Maml.XTitle;

			element.Add(title);

			childResults.Add(
				ParserContext.GetContext(titleText, false).Unparse(title, true));

			childResults.Add(
				// NextBlock is <steps>, which was parsed by the MamlList type
				ParserContext.GetContext(titleParagraph.NextBlock, false).Unparse(element, true));

			result.ChildResults = childResults;

			return result;
		}

		public override TextElement GetContainer(XElement forElement, TextElementType forType, TextElement fromElement)
		{
			Section section = (Section) fromElement;
			Section content = (Section) section.Blocks.LastBlock;

			return base.GetContainer(forElement, forType, content);
		}
		#endregion
	}
}
*/