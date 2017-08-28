using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public abstract class ParagraphNode<TParagraph> : ParagraphNode
		where TParagraph : Paragraph
	{
		public new TParagraph Element
		{
			get
			{
				return (TParagraph) base.Element;
			}
		}

		public ParagraphNode(TParagraph paragraph, ITextElementNodeFactory factory)
			: base(paragraph, factory)
		{
		}
	}
}