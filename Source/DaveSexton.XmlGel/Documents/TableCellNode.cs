using System.Collections.Generic;
using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class TableCellNode : TextElementNode<TableCell>
	{
		public TableCellNode(TableCell tableCell, ITextElementNodeFactory factory)
			: base(tableCell, factory)
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