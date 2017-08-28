using System.Collections.Generic;
using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class TableRowGroupNode : TextElementNode<TableRowGroup>
	{
		public TableRowGroupNode(TableRowGroup tableRowGroup, ITextElementNodeFactory factory)
			: base(tableRowGroup, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}

		protected override IEnumerable<TextElement> GetChildren()
		{
			return Element.Rows;
		}
	}
}