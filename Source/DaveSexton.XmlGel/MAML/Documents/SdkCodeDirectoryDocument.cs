using System;
using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	public sealed class SdkCodeDirectoryDocument : MamlDocument
	{
		public SdkCodeDirectoryDocument()
			: base(MamlDocumentKind.SdkCodeDirectory)
		{
		}

		public SdkCodeDirectoryDocument(Guid id, int revisionNumber, XDocument document, XElement documentRoot, IMamlMetadata metadata)
			: base(MamlDocumentKind.SdkCodeDirectory, id, revisionNumber, document, documentRoot, metadata)
		{
		}

		internal override MamlToFlowDocumentVisitor CreateVisitor(Action uiContainerChanged)
		{
			return new SdkCodeDirectoryDocumentToFlowDocumentVisitor(this, uiContainerChanged);
		}

		internal override FlowDocumentToMamlVisitor CreateVisitor(FlowDocument document)
		{
			return new FlowDocumentToSdkCodeDirectoryDocumentVisitor(document, this);
		}
	}
}