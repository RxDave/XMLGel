using System.Xml.Linq;

namespace DaveSexton.XmlGel.Xml
{
	public class XVisitableText : XVisitableNode
	{
		public string Value
		{
			get
			{
				return text.Value;
			}
		}

		private readonly XText text;

		public XVisitableText(XText text)
			: base(text)
		{
			this.text = text;
		}

		public override void Accept(IXVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}