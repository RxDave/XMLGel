using System.Collections.Generic;
using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class ListItemNode : TextElementNode<ListItem>
	{
		public ListItemNode(ListItem listItem, ITextElementNodeFactory factory)
			: base(listItem, factory)
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