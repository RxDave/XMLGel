using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public abstract class InlineUIContainerNode<TInlineUIContainer> : InlineUIContainerNode
		where TInlineUIContainer : InlineUIContainer
	{
		public new TInlineUIContainer Element
		{
			get
			{
				return (TInlineUIContainer) base.Element;
			}
		}

		public InlineUIContainerNode(TInlineUIContainer inlineUI, ITextElementNodeFactory factory)
			: base(inlineUI, factory)
		{
		}
	}
}