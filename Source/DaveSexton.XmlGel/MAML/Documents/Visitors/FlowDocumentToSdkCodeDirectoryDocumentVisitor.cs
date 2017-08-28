using System.Windows.Documents;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	internal sealed class FlowDocumentToSdkCodeDirectoryDocumentVisitor : FlowDocumentToMamlVisitor
	{
		public FlowDocumentToSdkCodeDirectoryDocumentVisitor(FlowDocument flowDocument, MamlDocument document)
			: base(flowDocument, document)
		{
		}
	}
}