using System.Windows.Documents;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	internal sealed class FlowDocumentToSdkSummaryDocumentVisitor : FlowDocumentToMamlVisitor
	{
		public FlowDocumentToSdkSummaryDocumentVisitor(FlowDocument flowDocument, MamlDocument document)
			: base(flowDocument, document)
		{
		}
	}
}