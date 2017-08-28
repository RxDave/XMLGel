using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using DaveSexton.XmlGel.Documents;

namespace DaveSexton.XmlGel.Maml
{
	public sealed class MamlTopicEditorTextBox : RichTextBox
	{
		public bool IgnoreMouseClicks
		{
			get;
			set;
		}

		static MamlTopicEditorTextBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MamlTopicEditorTextBox), new FrameworkPropertyMetadata(typeof(MamlTopicEditorTextBox)));
		}

		public MamlTopicEditorTextBox()
		{
			// This is required to enable controls within InlineUIContainer and BlockUIContainer elements
			IsDocumentEnabled = true;
		}

		private void SetCursor(bool cursorNo)
		{
			Cursor = cursorNo ? Cursors.No : null;
		}

		private void FilterMouseClick(RoutedEventArgs e)
		{
			if (IgnoreMouseClicks)
			{
				e.Handled = true;

				Focus();
			}
		}

		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			RunNormalization.UserModified(CaretPosition.Parent as Run);

			base.OnTextChanged(e);
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			SetCursor(IgnoreMouseClicks);

			base.OnMouseEnter(e);
		}

		protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
		{
			SetCursor(IgnoreMouseClicks);

			base.OnPreviewMouseDown(e);
		}

		protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
		{
			SetCursor(IgnoreMouseClicks);

			base.OnPreviewMouseUp(e);
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			if (IgnoreMouseClicks)
			{
				SetCursor(false);
			}

			base.OnMouseLeave(e);
		}

		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			FilterMouseClick(e);

			base.OnPreviewMouseLeftButtonDown(e);
		}

		protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
		{
			FilterMouseClick(e);

			base.OnPreviewMouseRightButtonDown(e);
		}

		protected override void OnPreviewMouseDoubleClick(MouseButtonEventArgs e)
		{
			FilterMouseClick(e);

			base.OnPreviewMouseDoubleClick(e);
		}
	}
}