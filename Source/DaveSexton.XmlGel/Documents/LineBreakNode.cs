using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class LineBreakNode : InlineNode<LineBreak>
	{
		public LineBreakNode(LineBreak lineBreak, ITextElementNodeFactory factory)
			: base(lineBreak, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}