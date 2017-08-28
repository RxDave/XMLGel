using System.Collections.Generic;
using System.Windows.Documents;
using System.Xml.Linq;

namespace DaveSexton.XmlGel.Documents
{
	public class RunNode : InlineNode<Run>
	{
		public RunNode(Run run, ITextElementNodeFactory factory)
			: base(run, factory)
		{
		}

		public override void Accept(ITextElementVisitor visitor)
		{
			visitor.Visit(this);
		}

		protected override IEnumerable<object> GetStructureContent(XNamespace defaultNamespace)
		{
			yield return Element.Text;
		}
	}
}