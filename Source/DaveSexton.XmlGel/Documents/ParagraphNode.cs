using System.Collections.Generic;
using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public class ParagraphNode : BlockNode<Paragraph>
	{
		public ParagraphNode(Paragraph paragraph, ITextElementNodeFactory factory)
			: base(paragraph, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}

		protected override IEnumerable<TextElement> GetChildren()
		{
			return Element.Inlines;
		}
	}
}