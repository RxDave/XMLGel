using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public class InlineNode : TextElementNode<Inline>
	{
		public InlineNode(Inline inline, ITextElementNodeFactory factory)
			: base(inline, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}