using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public abstract class SpanNode<TSpan> : SpanNode
		where TSpan : Span
	{
		public new TSpan Element
		{
			get
			{
				return (TSpan) base.Element;
			}
		}

		public SpanNode(TSpan span, ITextElementNodeFactory factory)
			: base(span, factory)
		{
		}
	}
}