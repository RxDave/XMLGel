using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public class TextElementNodeFactory : ITextElementNodeFactory
	{
		protected virtual ITextElementNode TryCreate(TextElement element)
		{
			return null;
		}

		public ITextElementNode Create(TextElement element)
		{
			var node = TryCreate(element);

			if (node == null
				&& !TryCreateInline(element, out node)
				&& !TryCreateBlock(element, out node)
				&& !TryCreateListItem(element, out node)
				&& !TryCreateTable(element, out node))
			{
				node = new TextElementNode(element, this);
			}

			return node;
		}

		private bool TryCreateInline(TextElement element, out ITextElementNode node)
		{
			if (TryCreateRun(element, out node)
				|| TryCreateSpan(element, out node)
				|| TryCreateAnchoredBlock(element, out node)
				|| TryCreateInlineUIContainer(element, out node))
			{
				return true;
			}

			LineBreak lineBreak;
			if ((lineBreak = element as LineBreak) != null)
			{
				node = new LineBreakNode(lineBreak, this);
				return true;
			}

			Inline inline;
			if ((inline = element as Inline) != null)
			{
				node = new InlineNode(inline, this);
				return true;
			}

			node = null;
			return false;
		}

		private bool TryCreateRun(TextElement element, out ITextElementNode node)
		{
			Literal literal;
			if ((literal = element as Literal) != null)
			{
				node = new LiteralNode(literal, this);
				return true;
			}

			CommentInline comment;
			if ((comment = element as CommentInline) != null)
			{
				node = new CommentInlineNode(comment, this);
				return true;
			}

			Run run;
			if ((run = element as Run) != null)
			{
				node = new RunNode(RunNormalization.Denormalize(run), this);
				return true;
			}

			node = null;
			return false;
		}

		private bool TryCreateSpan(TextElement element, out ITextElementNode node)
		{
			Hyperlink hyperlink;
			if ((hyperlink = element as Hyperlink) != null)
			{
				node = new HyperlinkNode(hyperlink, this);
				return true;
			}

			Bold bold;
			if ((bold = element as Bold) != null)
			{
				node = new BoldNode(bold, this);
				return true;
			}

			Italic italic;
			if ((italic = element as Italic) != null)
			{
				node = new ItalicNode(italic, this);
				return true;
			}

			Underline underline;
			if ((underline = element as Underline) != null)
			{
				node = new UnderlineNode(underline, this);
				return true;
			}

			ErrorInline error;
			if ((error = element as ErrorInline) != null)
			{
				node = new ErrorInlineNode(error, this);
				return true;
			}

			Span span;
			if ((span = element as Span) != null)
			{
				node = new SpanNode(span, this);
				return true;
			}

			node = null;
			return false;
		}

		private bool TryCreateAnchoredBlock(TextElement element, out ITextElementNode node)
		{
			Floater floater;
			if ((floater = element as Floater) != null)
			{
				node = new FloaterNode(floater, this);
				return true;
			}

			Figure figure;
			if ((figure = element as Figure) != null)
			{
				node = new FigureNode(figure, this);
				return true;
			}

			AnchoredBlock anchoredBlock;
			if ((anchoredBlock = element as AnchoredBlock) != null)
			{
				node = new AnchoredBlockNode(anchoredBlock, this);
				return true;
			}

			node = null;
			return false;
		}

		private bool TryCreateInlineUIContainer(TextElement element, out ITextElementNode node)
		{
			Options options;
			if ((options = element as Options) != null)
			{
				node = new OptionsNode(options, this);
				return true;
			}

			InlineUIContainer inlineUIContainer;
			if ((inlineUIContainer = element as InlineUIContainer) != null)
			{
				node = new InlineUIContainerNode(inlineUIContainer, this);
				return true;
			}

			node = null;
			return false;
		}

		private bool TryCreateBlock(TextElement element, out ITextElementNode node)
		{
			if (TryCreateSection(element, out node)
				|| TryCreateParagraph(element, out node))
			{
				return true;
			}

			List list;
			if ((list = element as List) != null)
			{
				node = new ListNode(list, this);
				return true;
			}

			Table table;
			if ((table = element as Table) != null)
			{
				node = new TableNode(table, this);
				return true;
			}

			BlockUIContainer blockUIContainer;
			if ((blockUIContainer = element as BlockUIContainer) != null)
			{
				node = new BlockUIContainerNode(blockUIContainer, this);
				return true;
			}

			Block block;
			if ((block = element as Block) != null)
			{
				node = new BlockNode(block, this);
				return true;
			}

			node = null;
			return false;
		}

		private bool TryCreateSection(TextElement element, out ITextElementNode node)
		{
			CollapsibleSection collapsibleSection;
			if ((collapsibleSection = element as CollapsibleSection) != null)
			{
				node = new CollapsibleSectionNode(collapsibleSection, this);
				return true;
			}

			HeaderedSection headeredSection;
			if ((headeredSection = element as HeaderedSection) != null)
			{
				node = new HeaderedSectionNode(headeredSection, this);
				return true;
			}

			ErrorBlock error;
			if ((error = element as ErrorBlock) != null)
			{
				node = new ErrorBlockNode(error, this);
				return true;
			}

			CollapsibleSectionOutline collapsibleSectionOutline;
			if ((collapsibleSectionOutline = element as CollapsibleSectionOutline) != null)
			{
				node = new CollapsibleSectionOutlineNode(collapsibleSectionOutline, this);
				return true;
			}

			Section section;
			if ((section = element as Section) != null)
			{
				node = new SectionNode(section, this);
				return true;
			}

			node = null;
			return false;
		}

		private bool TryCreateParagraph(TextElement element, out ITextElementNode node)
		{
			CommentBlock comment;
			if ((comment = element as CommentBlock) != null)
			{
				node = new CommentBlockNode(comment, this);
				return true;
			}

			Paragraph paragraph;
			if ((paragraph = element as Paragraph) != null)
			{
				node = new ParagraphNode(paragraph, this);
				return true;
			}

			node = null;
			return false;
		}

		private bool TryCreateListItem(TextElement element, out ITextElementNode node)
		{
			ListItem listItem;
			if ((listItem = element as ListItem) != null)
			{
				node = new ListItemNode(listItem, this);
				return true;
			}

			node = null;
			return false;
		}

		private bool TryCreateTable(TextElement element, out ITextElementNode node)
		{
			TableRowGroup tableRowGroup;
			if ((tableRowGroup = element as TableRowGroup) != null)
			{
				node = new TableRowGroupNode(tableRowGroup, this);
				return true;
			}

			TableRow tableRow;
			if ((tableRow = element as TableRow) != null)
			{
				node = new TableRowNode(tableRow, this);
				return true;
			}

			TableCell tableCell;
			if ((tableCell = element as TableCell) != null)
			{
				node = new TableCellNode(tableCell, this);
				return true;
			}

			node = null;
			return false;
		}
	}
}