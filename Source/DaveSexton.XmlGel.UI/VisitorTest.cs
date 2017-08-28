using System.Diagnostics;
using System.Windows.Documents;
using DaveSexton.XmlGel.Documents;

namespace DaveSexton.XmlGel.UI
{
	class VisitorTest : TextElementVisitor
	{
		public VisitorTest(FlowDocument document)
			: base(document)
		{
		}

		public override void Visit(AnchoredBlockNode block)
		{
			Debug.WriteLine(block.Element.GetType(), "Visiting AnchoredBlockNode: ");

			base.Visit(block);
		}

		public override void Visit(BlockNode block)
		{
			Debug.WriteLine(block.Element.GetType(), "Visiting BlockNode: ");

			base.Visit(block);
		}

		public override void Visit(BlockUIContainerNode blockUI)
		{
			Debug.WriteLine(blockUI.Element.GetType(), "Visiting BlockUIContainerNode: ");

			base.Visit(blockUI);
		}

		public override void Visit(BoldNode bold)
		{
			Debug.WriteLine(bold.Element.GetType(), "Visiting BoldNode: ");

			base.Visit(bold);
		}

		public override void Visit(FigureNode figure)
		{
			Debug.WriteLine(figure.Element.GetType(), "Visiting FigureNode: ");

			base.Visit(figure);
		}

		public override void Visit(FloaterNode floater)
		{
			Debug.WriteLine(floater.Element.GetType(), "Visiting FloaterNode: ");

			base.Visit(floater);
		}

		public override void Visit(HyperlinkNode hyperlink)
		{
			Debug.WriteLine(hyperlink.Element.GetType(), "Visiting HyperlinkNode: ");

			base.Visit(hyperlink);
		}

		public override void Visit(InlineNode inline)
		{
			Debug.WriteLine(inline.Element.GetType(), "Visiting InlineNode: ");

			base.Visit(inline);
		}

		public override void Visit(InlineUIContainerNode inlineUI)
		{
			Debug.WriteLine(inlineUI.Element.GetType(), "Visiting InlineUIContainerNode: ");

			base.Visit(inlineUI);
		}

		public override void Visit(ItalicNode italic)
		{
			Debug.WriteLine(italic.Element.GetType(), "Visiting ItalicNode: ");

			base.Visit(italic);
		}

		public override void Visit(LineBreakNode lineBreak)
		{
			Debug.WriteLine(lineBreak.Element.GetType(), "Visiting LineBreakNode: ");

			base.Visit(lineBreak);
		}

		public override void Visit(ListItemNode listItem)
		{
			Debug.WriteLine(listItem.Element.GetType(), "Visiting ListItemNode: ");

			base.Visit(listItem);
		}

		public override void Visit(ListNode list)
		{
			Debug.WriteLine(list.Element.GetType(), "Visiting ListNode: ");

			base.Visit(list);
		}

		public override void Visit(ParagraphNode paragraph)
		{
			Debug.WriteLine(paragraph.Element.GetType(), "Visiting ParagraphNode: ");

			base.Visit(paragraph);
		}

		public override void Visit(RunNode run)
		{
			Debug.WriteLine(run.Element.GetType(), "Visiting RunNode: ");

			base.Visit(run);
		}

		public override void Visit(SectionNode section)
		{
			Debug.WriteLine(section.Element.GetType(), "Visiting SectionNode: ");

			base.Visit(section);
		}

		public override void Visit(SpanNode span)
		{
			Debug.WriteLine(span.Element.GetType(), "Visiting SpanNode: ");

			base.Visit(span);
		}

		public override void Visit(TableCellNode tableCell)
		{
			Debug.WriteLine(tableCell.Element.GetType(), "Visiting TableCellNode: ");

			base.Visit(tableCell);
		}

		public override void Visit(TableNode table)
		{
			Debug.WriteLine(table.Element.GetType(), "Visiting TableNode: ");

			base.Visit(table);
		}

		public override void Visit(TableRowGroupNode tableRowGroup)
		{
			Debug.WriteLine(tableRowGroup.Element.GetType(), "Visiting TableRowGroupNode: ");

			base.Visit(tableRowGroup);
		}

		public override void Visit(TableRowNode tableRow)
		{
			Debug.WriteLine(tableRow.Element.GetType(), "Visiting TableRowNode: ");

			base.Visit(tableRow);
		}

		public override void Visit(TextElementNode textElement)
		{
			Debug.WriteLine(textElement.Element.GetType(), "Visiting TextElementNode: ");

			base.Visit(textElement);
		}

		public override void Visit(UnderlineNode underline)
		{
			Debug.WriteLine(underline.Element.GetType(), "Visiting UnderlineNode: ");

			base.Visit(underline);
		}
	}
}