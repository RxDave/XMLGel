using System.Windows.Documents;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	internal sealed class FlowDocumentToSdkOrientationDocumentVisitor : FlowDocumentToMamlVisitor
	{
		public FlowDocumentToSdkOrientationDocumentVisitor(FlowDocument flowDocument, MamlDocument document)
			: base(flowDocument, document)
		{
		}
	}
}