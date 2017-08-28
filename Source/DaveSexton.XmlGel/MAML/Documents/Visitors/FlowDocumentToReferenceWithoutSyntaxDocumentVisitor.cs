using System.Windows.Documents;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	internal sealed class FlowDocumentToReferenceWithoutSyntaxDocumentVisitor : FlowDocumentToMamlVisitor
	{
		public FlowDocumentToReferenceWithoutSyntaxDocumentVisitor(FlowDocument flowDocument, MamlDocument document)
			: base(flowDocument, document)
		{
		}
	}
}