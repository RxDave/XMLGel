using System.Windows.Documents;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	internal sealed class FlowDocumentToSdkScenariosDocumentVisitor : FlowDocumentToConceptualDocumentVisitor
	{
		public FlowDocumentToSdkScenariosDocumentVisitor(FlowDocument flowDocument, MamlDocument document)
			: base(flowDocument, document)
		{
		}
	}
}