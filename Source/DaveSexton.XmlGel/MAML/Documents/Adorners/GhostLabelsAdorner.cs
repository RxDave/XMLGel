using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using DaveSexton.XmlGel.Documents;
using DaveSexton.XmlGel.Extensions;

namespace DaveSexton.XmlGel.Maml.Documents.Adorners
{
	internal sealed partial class GhostLabelsAdorner : EditorAdornerBase
	{
		protected override bool CanRender
		{
			get
			{
				return true;
			}
		}

		static GhostLabelsAdorner()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GhostLabelsAdorner), new FrameworkPropertyMetadata(typeof(GhostLabelsAdorner)));
		}

		private bool isDirty = true;

		public GhostLabelsAdorner(MamlDocument document, MamlTopicEditorTextBox editor)
			: base(document, editor)
		{
			IsHitTestVisible = false;
		}

		protected override void ClearRenderState()
		{
			isDirty = true;
		}

		protected override bool UpdateCore(TextPointer pointer)
		{
			return isDirty;
		}

		protected override void RenderCore(DrawingContext drawingContext)
		{
			if (!Editor.Document.ContentStart.HasValidLayout)
			{
				return;
			}

			isDirty = false;

			var documentBox = Editor.GetVisualBox();
			var clipRegion = documentBox;

			clipRegion.X += DocumentMargin.Left;
			clipRegion.Width -= DocumentMargin.Left + DocumentMargin.Right;
			clipRegion.Y += DocumentMargin.Top;
			clipRegion.Height -= DocumentMargin.Top + DocumentMargin.Bottom;

			var clip = new RectangleGeometry(clipRegion);

			foreach (var pair in MamlPartLayout.MeasureLogicalBoxesOfDescendants(Editor.Document, documentBox))
			{
				var descendant = pair.Item1;

				if (descendant.Element is Paragraph || descendant.Element is HeaderedSection)
				{
					continue;
				}

				var text = new FormattedText(descendant.Node.DisplayName, CultureInfo.CurrentCulture, FlowDirection, CaptionTypeface, CaptionFontSize, CaptionBrush);

				var tabBox = pair.Item2;

				if (tabBox.X <= clipRegion.X)
				{
					tabBox.X = clipRegion.X;
				}

				if (tabBox.Y <= clipRegion.Y)
				{
					tabBox.Y = clipRegion.Y;
				}

				tabBox.Offset(TabOffset);

				tabBox.Width = text.Width + TabPadding.Left + TabPadding.Right;
				tabBox.Height = text.Height + TabPadding.Top + TabPadding.Bottom;

				drawingContext.DrawRoundedRectangle(TabBrush, null, tabBox, TabRadius, TabRadius);

				tabBox.Width = TabRoundedEdgeSize;

				drawingContext.DrawRectangle(TabBrush, null, tabBox);

				drawingContext.PushClip(clip);

				try
				{
					var textBox = tabBox.TopLeft;

					textBox.Offset(TabPadding.Left, TabPadding.Top);

					drawingContext.DrawText(text, textBox);
				}
				finally
				{
					drawingContext.Pop();
				}
			}
		}

		protected override void OnEditorTextChanged(TextChangedEventArgs e)
		{
			ClearRenderState();
			InvalidateVisual();
		}
	}
}