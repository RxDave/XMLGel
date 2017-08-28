using System.Windows.Documents;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	internal sealed class FlowDocumentToUIReferenceDocumentVisitor : FlowDocumentToMamlVisitor
	{
		public FlowDocumentToUIReferenceDocumentVisitor(FlowDocument flowDocument, MamlDocument document)
			: base(flowDocument, document)
		{
		}
	}
}