using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class HyperlinkNode : SpanNode<Hyperlink>
	{
		public HyperlinkNode(Hyperlink hyperlink, ITextElementNodeFactory factory)
			: base(hyperlink, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}