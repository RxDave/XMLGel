﻿using System.Windows.Documents;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	internal sealed class FlowDocumentToHowToDocumentVisitor : FlowDocumentToMamlVisitor
	{
		public FlowDocumentToHowToDocumentVisitor(FlowDocument flowDocument, MamlDocument document)
			: base(flowDocument, document)
		{
		}
	}
}