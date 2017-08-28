using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace DaveSexton.XmlGel.Xml
{
	public abstract class XVisitableNode : IXNode
	{
		bool? IXNode.TrimParagraphs
		{
			get
			{
				return null;
			}
		}

		bool? IXNode.NormalizeText
		{
			get
			{
				return null;
			}
		}

		public IEnumerable<IXNode> Children
		{
			get
			{
				var container = node as XContainer;

				return container == null
					? Enumerable.Empty<IXNode>()
					: container.Nodes().Select(XVisitableNode.Create);
			}
		}

		private readonly XNode node;

		protected XVisitableNode(XNode node)
		{
			this.node = node;
		}

		public static XVisitableNode Create(XNode node)
		{
			switch (node.NodeType)
			{
				case XmlNodeType.ProcessingInstruction:
					return new XVisitableProcessingInstruction((XProcessingInstruction) node);
				case XmlNodeType.Comment:
					return new XVisitableComment((XComment) node);
				case XmlNodeType.CDATA:
					return new XVisitableCData((XCData) node);
				case XmlNodeType.Text:
					return new XVisitableText((XText) node);
				case XmlNodeType.Element:
					return new XVisitableElement((XElement) node);
				default:
					throw new ArgumentException();
			}
		}

		public abstract void Accept(IXVisitor visitor);
	}
}