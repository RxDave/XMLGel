using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* sectionContentWithOutlineType (structure.xsd)
	 * 
	 * Attributes: (contentIdentificationSharingAndConditionGroup - base.xsd)
	 *	- address (ID : NCName)
	 * 
	 * Content:
	 *	- Allows 0..N elements allowed by base class
	 *	- Allows 0..N autoOutline elements
	 *		- Attributes: 
	 *			- lead (string)
	 *			- excludeRelatedTopics (bool)
	 */
	internal abstract class MamlSectionContentWithOutline : MamlSectionContent
	{
		protected MamlSectionContentWithOutline(XElement element)
			: base(element)
		{
		}
	}
}