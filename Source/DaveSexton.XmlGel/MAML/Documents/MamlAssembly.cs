using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* assemblyType (developerReference.xsd)
	 * 
	 * Sequence:
	 *	- assemblyName (MamlRestrictedText)
	 *	- assemblyVersion (MamlRestrictedText) (0..1)
	 *	- assemblyCulture (MamlRestrictedText) (0..1)
	 *	- assemblyPublicKey (MamlRestrictedText) (0..1)
	 *	- assemblyHash (MamlRestrictedText) (0..1)
	 */
	internal sealed class MamlAssembly : MamlNode
	{
		public MamlAssembly(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}