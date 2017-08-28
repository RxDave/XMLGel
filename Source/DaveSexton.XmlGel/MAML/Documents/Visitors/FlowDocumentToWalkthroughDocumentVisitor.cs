using System.Windows.Documents;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	internal sealed class FlowDocumentToWalkthroughDocumentVisitor : FlowDocumentToMamlVisitor
	{
		public FlowDocumentToWalkthroughDocumentVisitor(FlowDocument flowDocument, MamlDocument document)
			: base(flowDocument, document)
		{
		}
	}
}