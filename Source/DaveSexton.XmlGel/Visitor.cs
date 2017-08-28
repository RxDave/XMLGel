using System.Collections.Generic;

namespace DaveSexton.XmlGel
{
	public abstract class Visitor<TSelf, TNode> : IVisitor<TSelf, TNode>
		where TSelf : IVisitor<TSelf, TNode>
		where TNode : INode<TNode, TSelf>
	{
		private readonly IEnumerable<TNode> nodes;

		public Visitor(IEnumerable<TNode> nodes)
		{
			this.nodes = nodes;
		}

		public virtual void Visit()
		{
			foreach (var node in nodes)
			{
				node.Accept((TSelf) (object) this);
			}
		}

		public virtual void VisitChildren(TNode node)
		{
			foreach (var child in node.Children)
			{
				child.Accept((TSelf) (object) this);
			}
		}
	}
}