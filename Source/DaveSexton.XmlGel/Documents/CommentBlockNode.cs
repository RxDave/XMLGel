namespace DaveSexton.XmlGel.Documents
{
	public sealed class CommentBlockNode : ParagraphNode<CommentBlock>
	{
		public CommentBlockNode(CommentBlock comment, ITextElementNodeFactory factory)
			: base(comment, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}