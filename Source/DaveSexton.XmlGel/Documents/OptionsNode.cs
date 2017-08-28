namespace DaveSexton.XmlGel.Documents
{
	public sealed class OptionsNode : InlineUIContainerNode<Options>
	{
		public OptionsNode(Options options, ITextElementNodeFactory factory)
			: base(options, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}