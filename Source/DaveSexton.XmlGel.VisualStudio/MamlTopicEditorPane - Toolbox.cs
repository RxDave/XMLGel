using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace DaveSexton.XmlGel.VisualStudio
{
	partial class MamlTopicEditorPane : IVsToolboxUser
	{
		int IVsToolboxUser.IsSupported(Microsoft.VisualStudio.OLE.Interop.IDataObject pDO)
		{
			// Create a OleDataObject from the input interface.
			var oleData = new OleDataObject(pDO);

			// && editor.TextBox.CanPaste(DataFormats.GetFormat(DataFormats.UnicodeText))
			// Check if the data object is of type UnicodeText.
			if (oleData.GetDataPresent(DataFormats.UnicodeText))
			{
				return VSConstants.S_OK;
			}

			// In all the other cases return S_FALSE
			return VSConstants.S_FALSE;
		}

		int IVsToolboxUser.ItemPicked(Microsoft.VisualStudio.OLE.Interop.IDataObject pDO)
		{
			// Create a OleDataObject from the input interface.
			var oleData = new OleDataObject(pDO);

			// Check if the picked item is the one we can paste.
			if (oleData.GetDataPresent(DataFormats.UnicodeText))
			{
				editor.TextBox.Paste();
			}

			return VSConstants.S_OK;
		}
	}
}