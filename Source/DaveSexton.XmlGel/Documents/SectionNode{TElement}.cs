using System.Windows.Documents;

namespace DaveSexton.XmlGel.Documents
{
	public abstract class SectionNode<TSection> : SectionNode
		where TSection : Section
	{
		public new TSection Element
		{
			get
			{
				return (TSection) base.Element;
			}
		}

		public SectionNode(TSection section, ITextElementNodeFactory factory)
			: base(section, factory)
		{
		}
	}
}