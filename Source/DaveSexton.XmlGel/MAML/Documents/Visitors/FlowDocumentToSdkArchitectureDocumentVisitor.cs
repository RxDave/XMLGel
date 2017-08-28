using System.Windows.Documents;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	internal sealed class FlowDocumentToSdkArchitectureDocumentVisitor : FlowDocumentToConceptualDocumentVisitor
	{
		public FlowDocumentToSdkArchitectureDocumentVisitor(FlowDocument flowDocument, MamlDocument document)
			: base(flowDocument, document)
		{
		}
	}
}