using System.Collections.Generic;
using System.Windows.Documents;
using System.Xml.Linq;

namespace DaveSexton.XmlGel.Documents
{
	public class HeaderedSectionNode : SectionNode<HeaderedSection>
	{
		public HeaderedSectionNode(HeaderedSection section, ITextElementNodeFactory factory)
			: base(section, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}

		protected override IEnumerable<TextElement> GetChildren()
		{
			return Element.Content;
		}
	}
}