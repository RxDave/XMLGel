namespace DaveSexton.XmlGel.Documents
{
	public sealed class LiteralNode : RunNode<Literal>
	{
		public LiteralNode(Literal run, ITextElementNodeFactory factory)
			: base(run, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}