using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using DaveSexton.XmlGel.Extensions;

namespace DaveSexton.XmlGel.Maml.Documents.Adorners
{
	internal sealed class MouseAdorner : EditorAdornerBase
	{
		public StructureTabRowBuilder CurrentTabRowBuilder
		{
			get
			{
				return currentTabRowBuilder;
			}
			set
			{
				if (currentTabRowBuilder == null || !currentTabRowBuilder.Equals(value))
				{
					currentTabRowBuilder = value;

					Update();
				}
			}
		}

		protected override bool CanRender
		{
			get
			{
				return part != null
						|| currentTabRowBuilder != null;
			}
		}

		private const double fontSize = 12d;
		private const double textOffset = 5d;
		private const double borderSize = 1;

		private static readonly FontFamily captionFont = SystemFonts.SmallCaptionFontFamily;
		private static readonly Typeface captionFontType = new Typeface(captionFont, FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);

		private readonly Color actualColor = Colors.Green;
		private readonly Color paragraphColor = Colors.Orange;
		private readonly Color containerColor = Colors.Navy;

		private readonly Brush actualBrush, actualStartBrush, actualEndBrush;
		private readonly Brush paragraphBrush, paragraphStartBrush, paragraphEndBrush;
		private readonly Brush containerBrush, containerStartBrush, containerEndBrush;

		private readonly Brush validDebugBrush = Brushes.PaleTurquoise;
		private readonly Brush invalidDebugBrush = Brushes.PaleVioletRed;

		private readonly Pen actualBorderPen, paragraphBorderPen, containerBorderPen;

		private Rect paragraphBox = Rect.Empty, paragraphBoxStart, paragraphBoxEnd;

		private MamlPart part;
		private Paragraph paragraphElement;
		private StructureTabRowBuilder currentTabRowBuilder;

		public MouseAdorner(MamlDocument document, MamlTopicEditorTextBox editor)
			: base(document, editor)
		{
			IsHitTestVisible = false;

			actualBrush = new SolidColorBrush(actualColor);
			paragraphBrush = new SolidColorBrush(paragraphColor);
			containerBrush = new SolidColorBrush(containerColor);

			actualBrush.Freeze();
			paragraphBrush.Freeze();
			containerBrush.Freeze();

			actualBorderPen = new Pen(actualBrush, borderSize);
			paragraphBorderPen = new Pen(paragraphBrush, borderSize);
			containerBorderPen = new Pen(containerBrush, borderSize);

			actualBorderPen.Freeze();
			paragraphBorderPen.Freeze();
			containerBorderPen.Freeze();

			actualStartBrush = actualEndBrush = new SolidColorBrush(actualColor)
			{
				Opacity = 0.3d
			};

			paragraphStartBrush = paragraphEndBrush = new SolidColorBrush(paragraphColor)
			{
				Opacity = 0.3d
			};

			containerStartBrush = containerEndBrush = new SolidColorBrush(containerColor)
			{
				Opacity = 0.3d
			};

			actualStartBrush.Freeze();
			paragraphStartBrush.Freeze();
			containerStartBrush.Freeze();
		}

		public void Update()
		{
			Update(Mouse.GetPosition(Editor));
		}

		private void Update(IEnumerable<int> offsets)
		{
			var documentStart = Editor.Document.ContentStart;
			var documentBox = Editor.GetVisualBox();

			foreach (var offset in offsets)
			{
				var pointer = documentStart.GetPositionAtOffset(offset, LogicalDirection.Forward);

				var currentPart = pointer.TryGetPart(MamlPart.TryGet, documentBox);

				if (currentPart != null && (part == null || part.Box.IntersectsWith(currentPart.Box)))
				{
					part = currentPart;

					paragraphBox = MamlPartLayout.Measure(paragraphElement = pointer.Paragraph, out paragraphBoxStart, out paragraphBoxEnd);

					InvalidateVisual();
					break;
				}
			}
		}

		protected override bool UpdateCore(TextPointer pointer)
		{
			part = pointer.TryGetPart(MamlPart.TryGet, Editor.GetVisualBox());

			paragraphBox = MamlPartLayout.Measure(paragraphElement = pointer.Paragraph, out paragraphBoxStart, out paragraphBoxEnd);

			return true;
		}

		protected override void RenderCore(DrawingContext drawingContext)
		{
			if (currentTabRowBuilder != null)
			{
				RenderCurrentTabRowBuilders(drawingContext);
			}

			if (part != null)
			{
				RenderCurrentPartAndAncestors(drawingContext);
				RenderMouseCaption(drawingContext);
			}
		}

		private void RenderCurrentTabRowBuilders(DrawingContext drawingContext)
		{
			var validDebugBrushStart = validDebugBrush;
			var invalidDebugBrushStart = invalidDebugBrush;

			foreach (var builder in currentTabRowBuilder.GetSelfAndAncestors())
			{
				Brush brush;

				if (currentTabRowBuilder.ShouldCreateTabRow)
				{
					brush = validDebugBrushStart = validDebugBrushStart.Clone();
				}
				else
				{
					brush = invalidDebugBrushStart = invalidDebugBrushStart.Clone();
				}

				brush.Opacity -= .25;

				drawingContext.DrawRectangle(brush, null, builder.LogicalBox);
			}
		}

		private void RenderCurrentPartAndAncestors(DrawingContext drawingContext)
		{
			var center = part.Box.GetCenter();

			drawingContext.DrawText(part.ElementOrDocument.GetType().Name, captionFontType, fontSize, actualBrush, part.Box.Rotate(-90).TopLeft, offsetY: textOffset, transform: new RotateTransform(90, center.X, center.Y));

			drawingContext.DrawRectangle(Brushes.Transparent, actualBorderPen, part.Box);
			drawingContext.DrawRectangle(actualStartBrush, null, part.Start);
			drawingContext.DrawRectangle(actualEndBrush, null, part.End);

			if (!paragraphBox.IsEmpty)
			{
				center = paragraphBox.GetCenter();

				drawingContext.DrawText(paragraphElement.GetType().Name, captionFontType, fontSize, paragraphBrush, paragraphBox.Rotate(90).TopLeft, offsetY: textOffset, transform: new RotateTransform(-90, center.X, center.Y));

				drawingContext.DrawRectangle(Brushes.Transparent, paragraphBorderPen, paragraphBox);
				drawingContext.DrawRectangle(paragraphStartBrush, null, paragraphBoxStart);
				drawingContext.DrawRectangle(paragraphEndBrush, null, paragraphBoxEnd);
			}

			if (part.HasContainer)
			{
				var container = part.Container;

				var containerBox = container.BoundingBox;

				drawingContext.DrawText(container.ElementOrDocument.GetType().Name, captionFontType, fontSize, containerBrush, containerBox.TopLeft, offsetY: -fontSize - textOffset);

				drawingContext.DrawRectangle(Brushes.Transparent, containerBorderPen, containerBox);
				drawingContext.DrawRectangle(containerStartBrush, null, container.Start);
				drawingContext.DrawRectangle(containerEndBrush, null, container.End);
			}
		}

		private void RenderMouseCaption(DrawingContext drawingContext)
		{
			var typeName = part.ElementOrDocument.GetType().Name;

			var position = Mouse.GetPosition(Editor);

			var mouseCaption = Math.Round(position.X, 2) + "," + Math.Round(position.Y, 2) + " - "
											 + (part.Data == null ? typeName : typeName + " - " + part.Data.Name);

			drawingContext.DrawText(mouseCaption, captionFontType, fontSize, actualBrush, position, 0, -fontSize - 2);
		}

		protected override void ClearRenderState()
		{
			part = null;
		}

		protected override void OnEditorTextChanged(TextChangedEventArgs e)
		{
			if (CanRender)
			{
				Update(e.Changes.Select(change => change.Offset));
			}
		}
	}
}