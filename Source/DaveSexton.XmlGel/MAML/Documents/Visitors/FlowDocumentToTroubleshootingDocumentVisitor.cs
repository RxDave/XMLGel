using System.Windows.Documents;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	internal sealed class FlowDocumentToTroubleshootingDocumentVisitor : FlowDocumentToMamlVisitor
	{
		public FlowDocumentToTroubleshootingDocumentVisitor(FlowDocument flowDocument, MamlDocument document)
			: base(flowDocument, document)
		{
		}
	}
}