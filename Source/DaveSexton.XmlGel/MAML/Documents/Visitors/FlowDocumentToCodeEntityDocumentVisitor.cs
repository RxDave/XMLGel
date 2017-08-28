using System.Windows.Documents;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	internal sealed class FlowDocumentToCodeEntityDocumentVisitor : FlowDocumentToMamlVisitor
	{
		public FlowDocumentToCodeEntityDocumentVisitor(FlowDocument flowDocument, MamlDocument document)
			: base(flowDocument, document)
		{
		}
	}
}