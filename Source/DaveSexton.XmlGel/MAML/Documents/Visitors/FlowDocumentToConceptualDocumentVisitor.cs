using System.Windows.Documents;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	internal class FlowDocumentToConceptualDocumentVisitor : FlowDocumentToMamlVisitor
	{
		public FlowDocumentToConceptualDocumentVisitor(FlowDocument flowDocument, MamlDocument document)
			: base(flowDocument, document)
		{
		}
	}
}