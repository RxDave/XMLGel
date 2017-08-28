using System;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	/* developerHowToDocumentType (developer.xsd)
	 * 
	 * Sequence:
	 *	- summary (MamlSummary) (0..1)
	 *	- introduction (MamlIntroduction)
	 *	- Choice:
	 *		- Sequence: 
	 *			- Choice: (1..N)
	 *				- procedure (MamlProcedure)
	 *				- section (MamlSection - procedure content flavor)
	 *			- codeExample (MamlCodeExample) (0..N)
	 *		- codeExample (MamlCodeExample) (1..N)
	 *	- buildInstructions (MamlNamedSection) (0..1)
	 *	- robustProgramming (MamlNamedSection) (0..1)
	 *	- security (MamlNamedSection) (0..1)
	 *	- relatedTopics (MamlRelatedTopics)
	 */
	internal sealed class HowToDocumentToFlowDocumentVisitor : MamlToFlowDocumentVisitor
	{
		public HowToDocumentToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document, uiContainerChanged)
		{
		}
	}
}