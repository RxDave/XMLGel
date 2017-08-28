using System.Collections.Generic;
using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class TableNode : BlockNode<Table>
	{
		public TableNode(Table table, ITextElementNodeFactory factory)
			: base(table, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}

		protected override IEnumerable<TextElement> GetChildren()
		{
			return Element.RowGroups;
		}
	}
}