namespace DaveSexton.XmlGel.Documents
{
	public sealed class ErrorInlineNode : SpanNode<ErrorInline>
	{
		public ErrorInlineNode(ErrorInline error, ITextElementNodeFactory factory)
			: base(error, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}