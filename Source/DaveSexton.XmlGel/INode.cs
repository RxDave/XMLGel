using System.Collections.Generic;

namespace DaveSexton.XmlGel
{
	public interface INode<out TSelf, in TVisitor>
		where TSelf : INode<TSelf, TVisitor>
		where TVisitor : IVisitor<TVisitor, TSelf>
	{
		IEnumerable<TSelf> Children
		{
			get;
		}

		void Accept(TVisitor visitor);
	}
}