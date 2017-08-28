namespace DaveSexton.XmlGel.Documents
{
	public sealed class CommentInlineNode : RunNode<CommentInline>
	{
		public CommentInlineNode(CommentInline comment, ITextElementNodeFactory factory)
			: base(comment, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}