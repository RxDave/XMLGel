using System.Collections.Generic;
using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public class AnchoredBlockNode : InlineNode<AnchoredBlock>
	{
		public AnchoredBlockNode(AnchoredBlock anchoredBlock, ITextElementNodeFactory factory)
			: base(anchoredBlock, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}

		protected override IEnumerable<TextElement> GetChildren()
		{
			return Element.Blocks;
		}
	}
}