using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* codeEntityType (developerReference.xsd)
	 * 
	 * Sequence:
	 *	- codeEntityReference (MamlCodeEntityReference)
	 *	- Choice: (0..N)
	 *		- internalOnly (MamlEmpty)
	 *		- useBase (MamlEmpty)
	 *		- clsCompliantAlternative (MamlCodeReferenceDescription)
	 *		- equivalentCodeEntity (MamlCodeReferenceDescription)
	 *		- obsoleteCodeEntity (MamlCodeReferenceDescription)
	 *		- unmanagedCodeEntity (MamlCodeReferenceDescription)
	 *	- summary (MamlSummary) (0..1)
	 *	- genericParameters (MamlGenericParameters) (0..1)
	 *	- parameters (MamlParameters) (0..1)
	 *	- returnValue (MamlSectionContent) (0..1)
	 *	- exceptions (MamlExceptions) (0..1)
	 *	- threadSafety (MamlSectionContent) (0..1)
	 *	- remarks (MamlNamedSection) (0..1)
	 *	- notesForImplementers (MamlSectionContent) (0..1)
	 *	- notesForCallers (MamlSectionContent) (0..1)
	 *	- notesForInheritors (MamlSectionContent) (0..1)
	 *	- platformNotes (MamlPlatformNotes) (0..1)
	 *	- codeExamples (MamlCodeExamples) (0..1)
	 *	- permissions (MamlPermissions) (0..1)
	 *	- relatedTopics (MamlRelatedTopics) (0..1)
	 */
	internal sealed class MamlCodeEntity : MamlNode
	{
		public MamlCodeEntity(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}