using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace DaveSexton.XmlGel.Xml
{
	public class XVisitableElement : XVisitableNode
	{
		public XName Name
		{
			get
			{
				return element.Name;
			}
		}

		public XElement Element
		{
			get
			{
				return element;
			}
		}

		private readonly XElement element;

		public XVisitableElement(XElement element)
			: base(element)
		{
			Contract.Requires(element != null);

			this.element = element;
		}

		public override void Accept(IXVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}