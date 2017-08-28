// Guids.cs
// MUST match guids.h
using System;

namespace DaveSexton.XmlGel.VisualStudio
{
	internal static class GuidList
	{
		public const string guidDaveSexton_XmlGel_VisualStudioPkgString = "fd6a47a9-0b6d-4a3d-b71d-b7f7b856ba7d";
		public const string guidDaveSexton_XmlGel_VisualStudioCmdSetString = "29582399-1ca9-4366-9985-0188716b4ed6";
		public const string guidToolWindowPersistanceString = "b611db7d-2b54-41ca-990e-ee7f879454ec";
		public const string guidDaveSexton_XmlGel_VisualStudioEditorFactoryString = "fca19289-4d91-42aa-9e3d-8a727ee4a789";

		public static readonly Guid guidDaveSexton_XmlGel_VisualStudioCmdSet = new Guid(guidDaveSexton_XmlGel_VisualStudioCmdSetString);
		public static readonly Guid guidDaveSexton_XmlGel_VisualStudioEditorFactory = new Guid(guidDaveSexton_XmlGel_VisualStudioEditorFactoryString);
	};
}