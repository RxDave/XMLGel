using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class BlockUIContainerNode : BlockNode<BlockUIContainer>
	{
		public BlockUIContainerNode(BlockUIContainer blockUI, ITextElementNodeFactory factory)
			: base(blockUI, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}