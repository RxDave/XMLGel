using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class BoldNode : SpanNode<Bold>
	{
		public BoldNode(Bold bold, ITextElementNodeFactory factory)
			: base(bold, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}