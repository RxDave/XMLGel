using System.Windows.Documents;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	internal sealed class FlowDocumentToErrorMessageDocumentVisitor : FlowDocumentToMamlVisitor
	{
		public FlowDocumentToErrorMessageDocumentVisitor(FlowDocument flowDocument, MamlDocument document)
			: base(flowDocument, document)
		{
		}
	}
}