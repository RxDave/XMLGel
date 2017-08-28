namespace DaveSexton.XmlGel.Xml
{
	public interface IXNode : INode<IXNode, IXVisitor>
	{
		bool? TrimParagraphs { get; }

		bool? NormalizeText { get; }
	}
}