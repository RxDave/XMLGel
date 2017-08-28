using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* terms (glossary.xsd)
	 * 
	 * Content:
	 *	- term (1..N)
	 *		- Allows text (mixed content)
	 *		- Attributes: 
	 *			- termId (token XSD) (Sandcastle Styles extension)
	 *			- termClass (string XSD)
	 *			- partOfSpeech (string XSD) - Default: noun
	 *			- geographicalUsage (string XSD)
	 *			- language (string XSD)
	 */
	internal sealed class MamlTerms : MamlNode
	{
		public MamlTerms(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}