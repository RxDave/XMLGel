using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public class InlineUIContainerNode : InlineNode<InlineUIContainer>
	{
		public InlineUIContainerNode(InlineUIContainer inlineUI, ITextElementNodeFactory factory)
			: base(inlineUI, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}