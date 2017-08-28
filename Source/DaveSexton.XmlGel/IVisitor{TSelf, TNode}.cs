namespace DaveSexton.XmlGel
{
	public interface IVisitor<out TSelf, in TNode> : IVisitor
		where TSelf : IVisitor<TSelf, TNode>
		where TNode : INode<TNode, TSelf>
	{
		void VisitChildren(TNode node);
	}
}