using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class ErrorInline : Span
	{
		public ErrorInline()
		{
		}

		public ErrorInline(Inline inline)
			: base(inline)
		{
		}
	}
}