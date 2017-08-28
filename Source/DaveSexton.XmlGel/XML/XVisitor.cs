using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DaveSexton.XmlGel.Xml
{
	public abstract class XVisitor : Visitor<IXVisitor, IXNode>, IXVisitor
	{
		protected XContainer Container
		{
			get
			{
				return container;
			}
		}

		private readonly XContainer container;

		public XVisitor(XContainer container)
			: this(container.Nodes())
		{
			this.container = container;
		}

		public XVisitor(IEnumerable<XNode> nodes)
			: base(nodes.Select(XVisitableNode.Create))
		{
		}

		public virtual void Visit(XVisitableProcessingInstruction pragma)
		{
			VisitChildren(pragma);
		}

		public virtual void Visit(XVisitableComment comment)
		{
			VisitChildren(comment);
		}

		public virtual void Visit(XVisitableCData cData)
		{
			VisitChildren(cData);
		}

		public virtual void Visit(XVisitableText text)
		{
			VisitChildren(text);
		}

		public virtual void Visit(XVisitableElement element)
		{
			VisitChildren(element);
		}

		public static XElement CreateAny()
		{
			return new XElement("any");
		}
	}
}