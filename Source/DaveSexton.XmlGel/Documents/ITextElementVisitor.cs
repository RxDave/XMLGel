using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	/// <summary>
	/// Processes the nodes of a document that derive from <see cref="TextElement"/>.
	/// </summary>
	public interface ITextElementVisitor : IVisitor<ITextElementVisitor, ITextElementNode>
	{
		void Visit(TextElementNode textElement);

		// Derived from TextElement:
		void Visit(InlineNode inline);
		void Visit(BlockNode block);
		void Visit(ListItemNode listItem);
		void Visit(TableCellNode tableCell);
		void Visit(TableRowNode tableRow);
		void Visit(TableRowGroupNode tableRowGroup);

		// Derived from Block:
		void Visit(BlockUIContainerNode blockUI);
		void Visit(ListNode list);
		void Visit(ParagraphNode paragraph);
		void Visit(SectionNode section);
		void Visit(TableNode table);
		void Visit(CommentBlockNode comment);
		void Visit(ErrorBlockNode error);

		// Derived from Inline:
		void Visit(AnchoredBlockNode block);
		void Visit(InlineUIContainerNode inlineUI);
		void Visit(LineBreakNode lineBreak);
		void Visit(RunNode run);
		void Visit(SpanNode span);
		void Visit(ErrorInlineNode error);
		void Visit(OptionsNode options);

		// Derived from AnchoredBlock:
		void Visit(FigureNode figure);
		void Visit(FloaterNode floater);

		// Derived from Span:
		void Visit(BoldNode bold);
		void Visit(HyperlinkNode hyperlink);
		void Visit(ItalicNode italic);
		void Visit(UnderlineNode underline);

		// Derived from Section:
		void Visit(CollapsibleSectionOutlineNode outline);
		void Visit(HeaderedSectionNode section);

		// Derived from HeaderedSection:
		void Visit(CollapsibleSectionNode section);

		// Derived from Run:
		void Visit(LiteralNode literal);
		void Visit(CommentInlineNode comment);
	}
}