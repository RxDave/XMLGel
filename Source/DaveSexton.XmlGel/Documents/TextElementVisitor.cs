using System.Linq;
using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public abstract class TextElementVisitor : Visitor<ITextElementVisitor, ITextElementNode>, ITextElementVisitor
	{
		public ITextElementNodeFactory Factory
		{
			get
			{
				return factory;
			}
		}

		private readonly ITextElementNodeFactory factory;

		public TextElementVisitor(FlowDocument document)
			: this(document.Blocks)
		{
		}

		public TextElementVisitor(FlowDocument document, ITextElementNodeFactory factory)
			: this(document.Blocks, factory)
		{
		}

		public TextElementVisitor(BlockCollection blocks)
			: this(blocks, new TextElementNodeFactory())
		{
		}

		public TextElementVisitor(BlockCollection blocks, ITextElementNodeFactory factory)
			: base(blocks.Select(block => factory.Create(block)))
		{
			this.factory = factory;
		}

		public virtual void Visit(TextElementNode textElement)
		{
			VisitChildren(textElement);
		}

		public virtual void Visit(InlineNode inline)
		{
			VisitChildren(inline);
		}

		public virtual void Visit(BlockNode block)
		{
			VisitChildren(block);
		}

		public virtual void Visit(ErrorBlockNode error)
		{
			VisitChildren(error);
		}

		public virtual void Visit(CommentBlockNode comment)
		{
			VisitChildren(comment);
		}

		public virtual void Visit(ListItemNode listItem)
		{
			VisitChildren(listItem);
		}

		public virtual void Visit(TableCellNode tableCell)
		{
			VisitChildren(tableCell);
		}

		public virtual void Visit(TableRowNode tableRow)
		{
			VisitChildren(tableRow);
		}

		public virtual void Visit(TableRowGroupNode tableRowGroup)
		{
			VisitChildren(tableRowGroup);
		}

		public virtual void Visit(BlockUIContainerNode blockUI)
		{
			VisitChildren(blockUI);
		}

		public virtual void Visit(ListNode list)
		{
			VisitChildren(list);
		}

		public virtual void Visit(ParagraphNode paragraph)
		{
			VisitChildren(paragraph);
		}

		public virtual void Visit(SectionNode section)
		{
			VisitChildren(section);
		}

		public virtual void Visit(HeaderedSectionNode section)
		{
			VisitChildren(section);
		}

		public virtual void Visit(CollapsibleSectionNode section)
		{
			VisitChildren(section);
		}

		public virtual void Visit(CollapsibleSectionOutlineNode outline)
		{
			VisitChildren(outline);
		}

		public virtual void Visit(TableNode table)
		{
			VisitChildren(table);
		}

		public virtual void Visit(AnchoredBlockNode block)
		{
			VisitChildren(block);
		}

		public virtual void Visit(InlineUIContainerNode inlineUI)
		{
			VisitChildren(inlineUI);
		}

		public virtual void Visit(LineBreakNode lineBreak)
		{
			VisitChildren(lineBreak);
		}

		public virtual void Visit(ErrorInlineNode error)
		{
			VisitChildren(error);
		}

		public virtual void Visit(OptionsNode options)
		{
			VisitChildren(options);
		}

		public virtual void Visit(RunNode run)
		{
			VisitChildren(run);
		}

		public virtual void Visit(LiteralNode literal)
		{
			VisitChildren(literal);
		}

		public virtual void Visit(CommentInlineNode comment)
		{
			VisitChildren(comment);
		}

		public virtual void Visit(SpanNode span)
		{
			VisitChildren(span);
		}

		public virtual void Visit(FigureNode figure)
		{
			VisitChildren(figure);
		}

		public virtual void Visit(FloaterNode floater)
		{
			VisitChildren(floater);
		}

		public virtual void Visit(BoldNode bold)
		{
			VisitChildren(bold);
		}

		public virtual void Visit(HyperlinkNode hyperlink)
		{
			VisitChildren(hyperlink);
		}

		public virtual void Visit(ItalicNode italic)
		{
			VisitChildren(italic);
		}

		public virtual void Visit(UnderlineNode underline)
		{
			VisitChildren(underline);
		}
	}
}