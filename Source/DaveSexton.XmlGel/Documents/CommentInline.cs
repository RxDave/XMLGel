using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public class CommentInline : Run
	{
		public CommentInline()
		{
		}

		public CommentInline(string text)
			: base(text)
		{
		}
	}
}