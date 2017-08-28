using System.Windows.Documents;
using DaveSexton.XmlGel.Extensions;

namespace DaveSexton.XmlGel.Documents
{
	public class CommentBlock : Paragraph
	{
		public string Text
		{
			get
			{
				return this.GetText();
			}
		}

		public CommentBlock()
		{
		}

		public CommentBlock(Inline inline)
			: base(inline)
		{
		}
	}
}