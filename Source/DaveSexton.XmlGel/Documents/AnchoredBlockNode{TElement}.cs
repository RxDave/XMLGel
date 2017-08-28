using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public abstract class AnchoredBlockNode<TAnchoredBlock> : AnchoredBlockNode
		where TAnchoredBlock : AnchoredBlock
	{
		public new TAnchoredBlock Element
		{
			get
			{
				return (TAnchoredBlock) base.Element;
			}
		}

		public AnchoredBlockNode(TAnchoredBlock anchoredBlock, ITextElementNodeFactory factory)
			: base(anchoredBlock, factory)
		{
		}
	}
}