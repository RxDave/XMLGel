namespace DaveSexton.XmlGel.Xml
{
	public interface IXVisitor : IVisitor<IXVisitor, IXNode>
	{
		void Visit(XVisitableProcessingInstruction pragma);

		void Visit(XVisitableComment comment);

		void Visit(XVisitableCData cData);

		void Visit(XVisitableText text);

		void Visit(XVisitableElement element);
	}
}