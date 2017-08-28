using System.Collections.Generic;
using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class ListNode : BlockNode<List>
	{
		public ListNode(List list, ITextElementNodeFactory factory)
			: base(list, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}

		protected override IEnumerable<TextElement> GetChildren()
		{
			return Element.ListItems;
		}
	}
}