using System;
using EnvDTE;

namespace DaveSexton.XmlGel.VisualStudio
{
	partial class MamlTopicEditorPane : IExtensibleObject
	{
		private IExtensibleObjectSite extensibleObjectSite;

		/// <summary>
		/// This function is used for Macro playback.  Whenever a macro gets played this function will be
		/// called and then the IEditor functions will be called on the object that ppDisp is set to.
		/// Since MamlTopicEditorPane implements IEditor we will just set it to "this".
		/// </summary>
		/// <param name="Name"> Passing in either null, empty string or "Document" will work.  Anything
		/// else will result in ppDisp being set to null.</param>
		/// <param name="pParent"> An object of type IExtensibleObjectSite.  We will keep a reference to this
		/// so that in the Dispose method we can call the NotifyDelete function.</param>
		/// <param name="ppDisp"> The object that this is set to will act as the automation object for macro
		/// playback.  In our case since IEditor is the automation interface and MamlTopicEditorPane
		/// implements it we will just be setting this parameter to "this".</param>
		void IExtensibleObject.GetAutomationObject(string Name, IExtensibleObjectSite pParent, out Object ppDisp)
		{
			// null or empty string just means the default object, but if a specific string
			// is specified, then make sure it's the correct one, but don't enforce case
			if (!string.IsNullOrEmpty(Name) && !Name.Equals("Document", StringComparison.CurrentCultureIgnoreCase))
			{
				ppDisp = null;
				return;
			}

			// Set the out value to this
			ppDisp = (IEditor) editor;

			// Store the IExtensibleObjectSite object, it will be used in the Dispose method
			extensibleObjectSite = pParent;
		}
	}
}