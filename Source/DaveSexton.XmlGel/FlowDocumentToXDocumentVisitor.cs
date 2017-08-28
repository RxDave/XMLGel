using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Documents;

namespace DaveSexton.XmlGel
{
	public abstract class FlowDocumentToXDocumentVisitor : TextElementVisitor
	{
		public XDocument XDocument
		{
			get
			{
				return document;
			}
		}

		public XElement CurrentElement
		{
			get
			{
				return containers.Count > 0 ? containers.Peek() : null;
			}
		}

		protected static readonly XElement ContentContainerSkipChildrenIndicator = new XElement("_");

		private delegate XNode ReplacementSelector<TElementNode>(TElementNode node, out XElement contentContainer)
			where TElementNode : ITextElementNode;

		private readonly Stack<XElement> containers = new Stack<XElement>();
		private XDocument document;

		public FlowDocumentToXDocumentVisitor(FlowDocument flowDocument)
			: base(flowDocument)
		{
		}

		private void Push<TElementNode>(TElementNode original, ReplacementSelector<TElementNode> replacementSelector)
			where TElementNode : ITextElementNode
		{
			XElement contentContainer;
			var replacement = replacementSelector(original, out contentContainer);

			if (replacement != null)
			{
				Contract.Assert(containers.Count > 0);

				var element = replacement as XElement;

				if (element != null)
				{
					foreach (var attribute in from attribute in element.Attributes().ToList()
																		join defaultAttribute in element.Annotations<XAttribute>()
																		on new { attribute.Name, attribute.Value } equals new { defaultAttribute.Name, defaultAttribute.Value }
																		select attribute)
					{
						attribute.Remove();
					}
				}

				containers.Peek().Add(replacement);
			}

			if (!object.ReferenceEquals(contentContainer, ContentContainerSkipChildrenIndicator))
			{
				if (contentContainer != null)
				{
					containers.Push(contentContainer);
				}

				VisitChildren(original);

				if (contentContainer != null)
				{
					containers.Pop();
				}
			}
		}

		protected abstract XDocument CreateDocument(out XElement root);

		public override void Visit()
		{
			Contract.Assert(containers.Count == 0);

			XElement root;
			document = CreateDocument(out root);

			containers.Push(root);

			base.Visit();

			Contract.Assert(containers.Count == 1);
			Contract.Assert(containers.Pop() == root);
		}

		public sealed override void Visit(TextElementNode textElement)
		{
			Push(textElement, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(TextElementNode textElement, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(BlockNode block)
		{
			Push(block, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(BlockNode block, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(ErrorBlockNode error)
		{
			Push(error, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(ErrorBlockNode error, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public override void Visit(CommentBlockNode comment)
		{
			Push(comment, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(CommentBlockNode comment, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(SectionNode section)
		{
			Push(section, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(SectionNode section, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(HeaderedSectionNode section)
		{
			Push(section, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(HeaderedSectionNode section, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(CollapsibleSectionNode section)
		{
			Push(section, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(CollapsibleSectionNode section, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(CollapsibleSectionOutlineNode outline)
		{
			Push(outline, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(CollapsibleSectionOutlineNode outline, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(ParagraphNode paragraph)
		{
			Push(paragraph, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(ParagraphNode paragraph, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(ListNode list)
		{
			Push(list, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(ListNode list, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(SpanNode span)
		{
			if (!(span is ItalicNode)
				&& !(span is BoldNode)
				&& !(span is UnderlineNode)
				&& !(span is HyperlinkNode))
			{
				Push(span, CreateReplacement);
			}
			else
			{
				base.Visit(span);
			}
		}

		protected virtual XNode CreateReplacement(SpanNode span, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(ListItemNode listItem)
		{
			Push(listItem, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(ListItemNode listItem, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(TableNode table)
		{
			Push(table, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(TableNode table, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(TableRowGroupNode tableRowGroup)
		{
			Push(tableRowGroup, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(TableRowGroupNode tableRowGroup, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(TableRowNode tableRow)
		{
			Push(tableRow, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(TableRowNode tableRow, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(TableCellNode tableCell)
		{
			Push(tableCell, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(TableCellNode tableCell, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(UnderlineNode underline)
		{
			Push(underline, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(UnderlineNode underline, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(BoldNode bold)
		{
			Push(bold, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(BoldNode bold, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(ItalicNode italic)
		{
			Push(italic, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(ItalicNode italic, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(InlineNode inline)
		{
			Push(inline, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(InlineNode inline, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(ErrorInlineNode error)
		{
			Push(error, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(ErrorInlineNode error, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(OptionsNode options)
		{
			Push(options, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(OptionsNode options, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(InlineUIContainerNode inlineUI)
		{
			Push(inlineUI, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(InlineUIContainerNode inlineUI, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(BlockUIContainerNode blockUI)
		{
			Push(blockUI, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(BlockUIContainerNode blockUI, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(AnchoredBlockNode block)
		{
			Push(block, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(AnchoredBlockNode block, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(FigureNode figure)
		{
			Push(figure, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(FigureNode figure, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(FloaterNode floater)
		{
			Push(floater, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(FloaterNode floater, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(HyperlinkNode hyperlink)
		{
			Push(hyperlink, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(HyperlinkNode hyperlink, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(LineBreakNode lineBreak)
		{
			Push(lineBreak, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(LineBreakNode lineBreak, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(RunNode run)
		{
			Push(run, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(RunNode run, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(LiteralNode literal)
		{
			Push(literal, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(LiteralNode literal, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}

		public sealed override void Visit(CommentInlineNode comment)
		{
			Push(comment, CreateReplacement);
		}

		protected virtual XNode CreateReplacement(CommentInlineNode comment, out XElement contentContainer)
		{
			contentContainer = null;
			return null;
		}
	}
}