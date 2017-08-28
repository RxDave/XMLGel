using System.Xml.Linq;

namespace DaveSexton.XmlGel.Xml
{
	public sealed class XVisitableComment : XVisitableNode
	{
		public string Text
		{
			get
			{
				return comment.Value;
			}
		}

		private readonly XComment comment;

		public XVisitableComment(XComment comment)
			: base(comment)
		{
			this.comment = comment;
		}

		public override void Accept(IXVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}