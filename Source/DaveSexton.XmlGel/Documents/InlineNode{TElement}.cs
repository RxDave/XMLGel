using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public abstract class InlineNode<TInline> : InlineNode
		where TInline : Inline
	{
		public new TInline Element
		{
			get
			{
				return (TInline) base.Element;
			}
		}

		public InlineNode(TInline inline, ITextElementNodeFactory factory)
			: base(inline, factory)
		{
		}
	}
}