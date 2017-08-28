using System.Windows.Documents;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	internal sealed class FlowDocumentToWhitePaperDocumentVisitor : FlowDocumentToMamlVisitor
	{
		public FlowDocumentToWhitePaperDocumentVisitor(FlowDocument flowDocument, MamlDocument document)
			: base(flowDocument, document)
		{
		}
	}
}