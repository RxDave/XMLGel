using System.Xml.Linq;

namespace DaveSexton.XmlGel.Maml
{
	internal static class MamlAttributeFactory
	{
		public static XAttribute Href { get { return new XAttribute(Maml.XLinkHref, string.Empty); } }

		public static XAttribute Address { get { return new XAttribute("address", string.Empty); } }

		public static XAttribute StepsOrderedClass { get { return new XAttribute("class", "ordered"); } }
		public static XAttribute StepsBulletClass { get { return new XAttribute("class", "bullet"); } }

		public static XAttribute ListOrderedClass { get { return new XAttribute("class", "ordered"); } }
		public static XAttribute ListBulletClass { get { return new XAttribute("class", "bullet"); } }
		public static XAttribute ListNoBulletClass { get { return new XAttribute("class", "nobullet"); } }

		public static XAttribute CopyrightCopyrightClass { get { return new XAttribute("class", "copyright"); } }
		public static XAttribute CopyrightTraceClass { get { return new XAttribute("class", "trade"); } }
		public static XAttribute CopyrightRegisteredClass { get { return new XAttribute("class", "registered"); } }
		public static XAttribute CopyrightServiceClass { get { return new XAttribute("class", "service"); } }

		public static XAttribute CodeCPlusPlusLanguage { get { return new XAttribute("language", "cpp"); } }
		public static XAttribute CodeVisualBasicLanguage { get { return new XAttribute("language", "vb"); } }
		public static XAttribute CodeVisualBasicScriptLanguage { get { return new XAttribute("language", "vbs"); } }
		public static XAttribute CodeJavaScriptLanguage { get { return new XAttribute("language", "js"); } }
		public static XAttribute CodeCSharpLanguage { get { return new XAttribute("language", "c#"); } }
		public static XAttribute CodeJSharpLanguage { get { return new XAttribute("language", "j#"); } }
		public static XAttribute CodeXmlLanguage { get { return new XAttribute("language", "xml"); } }
		public static XAttribute CodeHtmlLanguage { get { return new XAttribute("language", "html"); } }
		public static XAttribute CodeVisualBasicAndCSharpLanguage { get { return new XAttribute("language", "vb-c#"); } }

		public static XAttribute DynamicLinkInlineType { get { return new XAttribute("type", "inline"); } }
		public static XAttribute DynamicLinkBulletedType { get { return new XAttribute("type", "bulleted"); } }
		public static XAttribute DynamicLinkTableType { get { return new XAttribute("type", "table"); } }

		public static XAttribute FictitiousUriEmailType { get { return new XAttribute("type", "email"); } }
		public static XAttribute FictitiousUriWebType { get { return new XAttribute("type", "web"); } }
		public static XAttribute FictitiousUriFtpType { get { return new XAttribute("type", "ftp"); } }

		public static XAttribute LocalUriAbsolutePathType { get { return new XAttribute("type", "absolutePath"); } }
		public static XAttribute LocalUriRelativePathType { get { return new XAttribute("type", "relativePath"); } }
		public static XAttribute LocalUriRegistryKeyType { get { return new XAttribute("type", "registryKey"); } }
		public static XAttribute LocalUriAllUsersDocAndSettingsType { get { return new XAttribute("type", "allUsersDocAndSettings"); } }
		public static XAttribute LocalUriCurrentUserDocAndSettingsType { get { return new XAttribute("type", "currentUserDocAndSettings"); } }
		public static XAttribute LocalUriProgramFilesType { get { return new XAttribute("type", "programFiles"); } }
		public static XAttribute LocalUriWindowsType { get { return new XAttribute("type", "Windows"); } }
	}
}