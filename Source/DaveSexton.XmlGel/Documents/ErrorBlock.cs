using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class ErrorBlock : Section
	{
		public ErrorBlock()
		{
		}

		public ErrorBlock(Block block)
			: base(block)
		{
		}
	}
}