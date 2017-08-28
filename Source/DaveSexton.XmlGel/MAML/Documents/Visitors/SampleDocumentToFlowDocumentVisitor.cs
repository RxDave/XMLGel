using System;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	/* developerSampleDocumentType (developer.xsd)
	 * 
	 * Sequence:
	 *	- summary (MamlSummary) (0..1)
	 *	- introduction (MamlIntroduction)
	 *	- procedure (MamlProcedure) (0..N)
	 *	- requirements (MamlNamedSection) (0..1)
	 *	- demonstrates (MamlNamedSection) (0..1)
	 *  - codeExample (MamlCodeExample) (0..N)
	 *  - section (MamlSection) (0..N)
	 *	- relatedTopics (MamlRelatedTopics)
	 */
	internal sealed class SampleDocumentToFlowDocumentVisitor : MamlToFlowDocumentVisitor
	{
		public SampleDocumentToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document, uiContainerChanged)
		{
		}
	}
}