namespace DaveSexton.XmlGel.Documents
{
	public sealed class CollapsibleSectionOutlineNode : SectionNode<CollapsibleSectionOutline>
	{
		public CollapsibleSectionOutlineNode(CollapsibleSectionOutline outline, ITextElementNodeFactory factory)
			: base(outline, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}