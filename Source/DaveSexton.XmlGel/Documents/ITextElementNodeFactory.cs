using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public interface ITextElementNodeFactory
	{
		ITextElementNode Create(TextElement element);
	}
}