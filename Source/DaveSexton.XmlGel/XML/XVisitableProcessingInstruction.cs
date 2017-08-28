using System.Xml.Linq;

namespace DaveSexton.XmlGel.Xml
{
	public sealed class XVisitableProcessingInstruction : XVisitableNode
	{
		public XVisitableProcessingInstruction(XProcessingInstruction pragma)
			: base(pragma)
		{
		}

		public override void Accept(IXVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}