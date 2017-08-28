using System.Collections.Generic;
using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public class SpanNode : InlineNode<Span>
	{
		public SpanNode(Span span, ITextElementNodeFactory factory)
			: base(span, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}

		protected override IEnumerable<TextElement> GetChildren()
		{
			return Element.Inlines;
		}
	}
}