using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace DaveSexton.XmlGel.Extensions
{
	internal static class TextPointerExtensions
	{
		public static TPart TryGetPart<TPart>(this TextPointer pointer, PartFactory<TPart> partFactory, Rect documentBox)
			where TPart : Part
		{
			return pointer == null ? null : partFactory(pointer.Parent as FrameworkContentElement, documentBox);
		}

		[DebuggerHidden]
		public static TextPointer GetPositionFromPoint(this RichTextBox editor, Point point)
		{
			try
			{
				return editor.GetPositionFromPoint(point, snapToText: true);
			}
			catch (InvalidOperationException)
			{
				/* This exception occurred during testing, sometimes, when loading a new document.
				 * 
				 * According to the reference doc for RichTextBox.GetPositionFromPoint, it indicates that the editor's layout
				 * is currently outdated, which may indicate that a measure or arrange pass could be taking place now or soon.
				 */
				return null;
			}
		}
	}
}