using System.Xml.Linq;

namespace DaveSexton.XmlGel.Xml
{
	public sealed class XVisitableCData : XVisitableText
	{
		public XVisitableCData(XCData cData)
			: base(cData)
		{
		}

		public override void Accept(IXVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}