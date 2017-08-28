using System.Collections.Generic;
using DaveSexton.Labs;
using DaveSexton.XmlGel.Labs.WPF;
using DaveSexton.XmlGel.Labs.XML;

namespace DaveSexton.XmlGel.Labs
{
	internal sealed partial class Catalog : LabCatalog
	{
		/* Priority Labs (optional)
		 * 
		 * Specify the labs that you are currently working on and they will be 
		 * executed before the other labs in this project. The remaining labs 
		 * will be discovered automatically by MEF composition.  The anonymous
		 * lab, if enabled, is always executed before the labs specified here.
		 */
		private static IEnumerable<ILab> GetPriorityLabs()
		{
			yield return new FlowDocumentLayoutLab();
			yield return new SchemaValidationLab();
			yield return new MamlSchemaLab();
			yield return new XmlBuilderLab();
			yield break;
		}
	}
}