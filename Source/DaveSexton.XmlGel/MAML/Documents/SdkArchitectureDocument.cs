using System;
using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	public sealed class SdkArchitectureDocument : MamlDocument
	{
		public SdkArchitectureDocument()
			: base(MamlDocumentKind.SdkArchitecture)
		{
		}

		public SdkArchitectureDocument(Guid id, int revisionNumber, XDocument document, XElement documentRoot, IMamlMetadata metadata)
			: base(MamlDocumentKind.SdkArchitecture, id, revisionNumber, document, documentRoot, metadata)
		{
		}

		internal override MamlToFlowDocumentVisitor CreateVisitor(Action uiContainerChanged)
		{
			return new SdkArchitectureDocumentToFlowDocumentVisitor(this, uiContainerChanged);
		}

		internal override FlowDocumentToMamlVisitor CreateVisitor(FlowDocument document)
		{
			return new FlowDocumentToSdkArchitectureDocumentVisitor(document, this);
		}
	}
}