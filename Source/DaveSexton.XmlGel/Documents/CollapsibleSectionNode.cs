using System.Collections.Generic;
using System.Xml.Linq;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class CollapsibleSectionNode : HeaderedSectionNode<CollapsibleSection>
	{
		public CollapsibleSectionNode(CollapsibleSection section, ITextElementNodeFactory factory)
			: base(section, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}

		protected override IEnumerable<object> GetStructureContent(XNamespace defaultNamespace)
		{
			yield return new XAttribute("Title", Element.Title ?? string.Empty);

			foreach (var item in base.GetStructureContent(defaultNamespace))
			{
				yield return item;
			}
		}
	}
}