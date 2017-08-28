using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public abstract class BlockNode<TBlock> : BlockNode
		where TBlock : Block
	{
		public new TBlock Element
		{
			get
			{
				return (TBlock) base.Element;
			}
		}

		public BlockNode(TBlock block, ITextElementNodeFactory factory)
			: base(block, factory)
		{
		}
	}
}