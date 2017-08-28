using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public abstract class RunNode<TRun> : RunNode
		where TRun : Run
	{
		public new TRun Element
		{
			get
			{
				return (TRun) base.Element;
			}
		}

		public RunNode(TRun run, ITextElementNodeFactory factory)
			: base(run, factory)
		{
		}
	}
}