using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Documents;
using DaveSexton.XmlGel.Extensions;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	internal sealed class FlowDocumentToGlossaryDocumentVisitor : FlowDocumentToMamlVisitor
	{
		public FlowDocumentToGlossaryDocumentVisitor(FlowDocument flowDocument, MamlDocument document)
			: base(flowDocument, document)
		{
		}

		protected override XNode CreateReplacement(ListItemNode listItem, out XElement contentContainer)
		{
			var element = (XElement) base.CreateReplacement(listItem, out contentContainer);

			if (element.Name == Maml.RelatedEntry)
			{
				element.SetAttributeValue(Maml.TermId, listItem.Element.GetText());
			}

			return element;
		}

		protected override XNode CreateReplacement(RunNode run, out XElement contentContainer)
		{
			if (CurrentElement.Name == Maml.RelatedEntry)
			{
				return contentContainer = null;
			}

			return base.CreateReplacement(run, out contentContainer);
		}
	}
}