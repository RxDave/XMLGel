using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public abstract class TextElementNode<TElement> : TextElementNode
		where TElement : TextElement
	{
		public new TElement Element
		{
			get
			{
				return (TElement) base.Element;
			}
		}

		public TextElementNode(TElement element, ITextElementNodeFactory factory)
			: base(element, factory)
		{
		}
	}
}