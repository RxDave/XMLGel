using System.Collections.Generic;
using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class FloaterNode : AnchoredBlockNode<Floater>
	{
		public FloaterNode(Floater floater, ITextElementNodeFactory factory)
			: base(floater, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}