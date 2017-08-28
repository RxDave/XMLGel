namespace DaveSexton.XmlGel.Documents
{
	public sealed class ErrorBlockNode : SectionNode<ErrorBlock>
	{
		public ErrorBlockNode(ErrorBlock error, ITextElementNodeFactory factory)
			: base(error, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}