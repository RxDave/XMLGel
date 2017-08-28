using System;
using System.Xml.Linq;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	/* developerReferenceWithSyntaxDocumentType (developer.xsd)
	 * 
	 * Sequence:
	 *	- summary (MamlSummary) (0..1)
	 *	- introduction (MamlIntroduction)
	 *	- syntaxSection (MamlSyntaxSection) (1..N)
	 *	- Choice: (1..N)
	 *		- parameters (MamlNamedSection)
	 *		- returnValue (MamlNamedSection)
	 *		- exceptions (MamlNamedSection)
	 *		- languageReferenceRemarks (MamlNamedSection)
	 *		- codeExample (MamlCodeExample)
	 *		- dotNetFrameworkEquivalent (MamlNamedSection)
	 *		- requirements (MamlNamedSection)
	 *		- section (MamlSection)
	 *	- relatedTopics (MamlRelatedTopics)
	 */
	internal sealed class ReferenceWithSyntaxDocumentToFlowDocumentVisitor : MamlToFlowDocumentVisitor
	{
		public ReferenceWithSyntaxDocumentToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document, uiContainerChanged)
		{
		}

		protected override MamlNode CreateParametersNode(XElement element)
		{
			if (CurrentNode == null)
			{
				return new MamlNamedSection(element);
			}
			else
			{
				return base.CreateParametersNode(element);
			}
		}

		protected override MamlNode CreateReturnValueNode(XElement element)
		{
			if (CurrentNode == null)
			{
				return new MamlNamedSection(element);
			}
			else
			{
				return base.CreateReturnValueNode(element);
			}
		}

		protected override MamlNode CreateExceptionsNode(XElement element)
		{
			if (CurrentNode == null)
			{
				return new MamlNamedSection(element);
			}
			else
			{
				return base.CreateExceptionsNode(element);
			}
		}
	}
}