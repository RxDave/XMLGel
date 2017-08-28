using System.Collections.Generic;

namespace DaveSexton.XmlGel.Maml.Editors
{
	internal sealed class GlobalOptions
	{
		public static readonly ICollection<string> CommonAttributeNames = new List<string>()
		{
			"DocSet",
			"Product",
			"DevLang",
			"Technology",
			"TargetOS",
			"LinkGroup",
			"HelpPriority",
			"Status"
		}
		.AsReadOnly();
	}
}