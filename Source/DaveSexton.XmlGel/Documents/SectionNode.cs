using System.Collections.Generic;
using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public class SectionNode : BlockNode<Section>
	{
		public SectionNode(Section section, ITextElementNodeFactory factory)
			: base(section, factory)
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