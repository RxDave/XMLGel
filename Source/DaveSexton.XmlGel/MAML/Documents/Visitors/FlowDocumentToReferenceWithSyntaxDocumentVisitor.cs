using System.Windows.Documents;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	internal sealed class FlowDocumentToReferenceWithSyntaxDocumentVisitor : FlowDocumentToMamlVisitor
	{
		public FlowDocumentToReferenceWithSyntaxDocumentVisitor(FlowDocument flowDocument, MamlDocument document)
			: base(flowDocument, document)
		{
		}
	}
}