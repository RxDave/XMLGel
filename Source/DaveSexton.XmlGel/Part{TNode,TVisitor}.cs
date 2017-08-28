using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Documents;
using System.Xml.Linq;
using System.Xml.Schema;
using DaveSexton.XmlGel.Xml;

namespace DaveSexton.XmlGel
{
	internal abstract class Part<TNode, TVisitor> : Part
		where TNode : IXElementNode<TVisitor>
		where TVisitor : XContainerToFlowDocumentVisitor<TVisitor>
	{
		public TNode Node
		{
			get
			{
				if (node == null)
				{
					node = GetNode();

					Contract.Assume(node != null || Data == null || ElementOrDocument is FlowDocument);
				}

				return node;
			}
		}

		private TNode node;

		protected Part(FrameworkContentElement element, XElement data, XmlSchemaElement schema, Rect documentBox)
			: base(element, data, schema, documentBox)
		{
			Contract.Requires(element != null);
		}

		protected abstract TNode GetNode();

		protected abstract Part<TNode, TVisitor> TryCreatePartWithNode(FrameworkContentElement element);

		protected sealed override Part TryCreatePart(FrameworkContentElement element)
		{
			return TryCreatePartWithNode(element);
		}
	}
}