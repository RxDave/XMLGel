using System.Xml.Linq;
namespace DaveSexton.XmlGel.Documents
{
	public interface ITextElementNode : INode<ITextElementNode, ITextElementVisitor>
	{
		XElement GetStructure(XNamespace defaultNamespace);
	}
}