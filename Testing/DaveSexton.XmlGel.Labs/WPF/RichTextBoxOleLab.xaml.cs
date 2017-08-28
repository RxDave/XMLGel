using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DaveSexton.XmlGel.Labs.WPF
{
	[DisplayName("")]
	[Description("")]
	public partial class RichTextBoxOleLab : BaseLab
	{
		public RichTextBoxOleLab()
		{
			InitializeComponent();
		}

		private void run_Click(object sender, RoutedEventArgs e)
		{
			/*
			// To get the IRichEditOle interface we need to call SendMessage, which
			// we imported from user32.dll
			object editOle = null;
			NativeMethods.SendMessage(textBox.Handle,                // The rich text box handle
																getOleInterfaceCommandId,   // The command ID for EM_GETOLEINTERFACE
																IntPtr.Zero,                // null
																out editOle                 // This will be set to the IRichEditOle interface
																);

			// Call GetIUnknownForObject with the IRichEditOle interface that we
			// just got so that we have an IntPtr to pass into QueryInterface
			IntPtr editOlePtr = IntPtr.Zero;
			editOlePtr = Marshal.GetIUnknownForObject(editOle);

			// Call QueryInterface to get the pointer to the ITextDocument
			IntPtr iTextDocument = IntPtr.Zero;
			Guid iTextDocumentGuid = typeof(ITextDocument).GUID;
			Marshal.QueryInterface(editOlePtr, ref iTextDocumentGuid, out iTextDocument);

			// We need to call Marshal.Release with the pointer that we got
			// from the GetIUnknownForObject call
			Marshal.Release(editOlePtr);

			// Call GetObjectForIUnknown passing in the pointer that was set
			// by QueryInterface and return it as an ITextDocument
			textDocument = Marshal.GetObjectForIUnknown(iTextDocument) as ITextDocument;
			return textDocument;*/
		}
	}
}