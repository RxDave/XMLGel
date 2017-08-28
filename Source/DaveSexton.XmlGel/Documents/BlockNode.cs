using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public class BlockNode : TextElementNode<Block>
	{
		public BlockNode(Block block, ITextElementNodeFactory factory)
			: base(block, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}