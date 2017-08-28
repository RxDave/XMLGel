using System.Collections.Generic;
using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class FigureNode : AnchoredBlockNode<Figure>
	{
		public FigureNode(Figure figure, ITextElementNodeFactory factory)
			: base(figure, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}