using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class UnderlineNode : SpanNode<Underline>
	{
		public UnderlineNode(Underline underline, ITextElementNodeFactory factory)
			: base(underline, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}