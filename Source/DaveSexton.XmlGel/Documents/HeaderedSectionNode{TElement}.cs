namespace DaveSexton.XmlGel.Documents
{
	public abstract class HeaderedSectionNode<TSection> : HeaderedSectionNode
		where TSection : HeaderedSection
	{
		public new TSection Element
		{
			get
			{
				return (TSection) base.Element;
			}
		}

		public HeaderedSectionNode(TSection section, ITextElementNodeFactory factory)
			: base(section, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}