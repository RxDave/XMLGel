using System.Collections.Generic;
using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class TableRowNode : TextElementNode<TableRow>
	{
		public TableRowNode(TableRow tableRow, ITextElementNodeFactory factory)
			: base(tableRow, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}

		protected override IEnumerable<TextElement> GetChildren()
		{
			return Element.Cells;
		}
	}
}