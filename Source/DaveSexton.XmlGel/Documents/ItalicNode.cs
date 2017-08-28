using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class ItalicNode : SpanNode<Italic>
	{
		public ItalicNode(Italic italic, ITextElementNodeFactory factory)
			: base(italic, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}