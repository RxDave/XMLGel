using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class Literal : Run
	{
		public Literal()
		{
		}

		public Literal(string text)
			: base(text)
		{
		}
	}
}