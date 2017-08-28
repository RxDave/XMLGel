using System;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Schema;
using DaveSexton.XmlGel.Maml.Documents.Visitors;
using DaveSexton.XmlGel.Xml;

namespace DaveSexton.XmlGel.Maml.Documents.Adorners
{
	internal sealed class StructureTab
	{
		public Rect Box
		{
			get
			{
				return box;
			}
		}

		public Rect RenderedBox
		{
			get
			{
				return renderedBox;
			}
		}

		public StructureTabRow Row
		{
			get
			{
				return row;
			}
		}

		public IXElementNode<MamlToFlowDocumentVisitor> Node
		{
			get
			{
				return node;
			}
		}

		public string ToolTip
		{
			get
			{
				return node == null ? null : node.ToolTip;
			}
		}

		public XmlSchemaElement Schema
		{
			get
			{
				return schema;
			}
		}

		private readonly StructureTabRow row;
		private readonly IXElementNode<MamlToFlowDocumentVisitor> node;
		private readonly XmlSchemaElement schema;
		private readonly Rect box;
		private readonly FormattedText caption;
		private readonly ImageSource image;
		private readonly Action<StructureTab> onClick;
		private readonly bool isBehavior;
		private readonly StructureTabLocation location;
		private Rect renderedBox = Rect.Empty;

		private StructureTab(
			StructureTabRow row,
			IXElementNode<MamlToFlowDocumentVisitor> node,
			XmlSchemaElement schema,
			string caption,
			ImageSource image,
			double horizontalOffset,
			double verticalOffset,
			Action<StructureTab> onClick,
			StructureTabLocation location,
			bool floatRight,
			bool isBehavior)
		{
			this.row = row;
			this.node = node;
			this.schema = schema;
			this.image = image;
			this.onClick = onClick;
			this.location = location;
			this.isBehavior = isBehavior;

			var palette = row.Palette;

			this.caption = image == null
				? new FormattedText(
						((FontCapitals?) Typography.GetCapitals(row.Palette) ?? FontCapitals.Normal) == FontCapitals.AllSmallCaps
						? caption.ToUpperInvariant()
						: caption,
						CultureInfo.CurrentCulture,
						FlowDirection.LeftToRight,
						palette.CaptionTypeface,
						palette.TabCaptionFontSize,
						palette.TabCaptionBrush)
				: null;

			var tabWidth = (image == null ? this.caption.Width : image.Width) + palette.TabPadding.Left + palette.TabPadding.Right;
			var tabHeight = (image == null ? this.caption.Height : image.Height) + palette.TabPadding.Top + palette.TabPadding.Bottom;

			if (isBehavior || location == StructureTabLocation.Centered)
			{
				tabHeight -= palette.TabRoundedEdgeHeight * 2;
				verticalOffset -= tabHeight / 2;
			}
			else
			{
				tabHeight -= palette.TabRoundedEdgeHeight;

				if (location == StructureTabLocation.Above)
				{
					verticalOffset -= tabHeight - row.Palette.TabRowPenSize;
				}
			}

			this.box = floatRight
				? new Rect(row.BaseLine.Right - (tabWidth + horizontalOffset), verticalOffset, tabWidth, tabHeight)
				: new Rect(horizontalOffset, verticalOffset, tabWidth, tabHeight);
		}

		public static StructureTab CreateForBehavior(
			StructureTabRow row,
			IXElementNode<MamlToFlowDocumentVisitor> node,
			ImageSource image,
			double horizontalOffset,
			double verticalOffset,
			Action<StructureTab> onClick,
			StructureTabLocation location,
			bool floatRight)
		{
			return new StructureTab(row, node, null, null, image, horizontalOffset, verticalOffset, onClick, location, floatRight, isBehavior: true);
		}

		public static StructureTab CreateForSchema(
			StructureTabRow row,
			IXElementNode<MamlToFlowDocumentVisitor> node,
			XmlSchemaElement schema,
			double horizontalOffset,
			double verticalOffset,
			Action<StructureTab> onClick,
			StructureTabLocation location,
			bool floatRight)
		{
			return new StructureTab(row, node, schema, node.DisplayName, null, horizontalOffset, verticalOffset, onClick, location, floatRight, false);
		}

		public void OnClick()
		{
			onClick(this);
		}

		public void Draw(DrawingContext drawingContext)
		{
			var palette = row.Palette;

			var localBox = renderedBox = box;

			if (isBehavior)
			{
				localBox.Offset(0, localBox.Height / 2);
			}

			var contentPosition = localBox.TopLeft;

			if (isBehavior || location != StructureTabLocation.Below)
			{
				var topBox = new Rect(localBox.X, localBox.Y - palette.TabRoundedEdgeHeight, localBox.Width, palette.TabRoundedEdgeHeight);

				drawingContext.DrawRoundedRectangle(palette.TabBrush, palette.TabPen, topBox, palette.TabRadius, palette.TabRadius);

				contentPosition.Y -= palette.TabRoundedEdgeHeight;

				renderedBox.Union(topBox);
			}

			drawingContext.DrawRectangle(palette.TabBrush, palette.TabPen, localBox);

			if (isBehavior || location != StructureTabLocation.Above)
			{
				var bottomBox = new Rect(localBox.BottomLeft, new Size(localBox.Width, palette.TabRoundedEdgeHeight));

				drawingContext.DrawRoundedRectangle(palette.TabBrush, palette.TabPen, bottomBox, palette.TabRadius, palette.TabRadius);

				renderedBox.Union(bottomBox);
			}

			contentPosition.Offset(palette.TabPadding.Left + palette.TabCaptionMargin.Left, palette.TabPadding.Top + palette.TabCaptionMargin.Top);

			if (image == null)
			{
				drawingContext.DrawText(caption, contentPosition);
			}
			else
			{
				drawingContext.DrawImage(image, new Rect(contentPosition, new Size(image.Width, image.Height)));
			}
		}
	}
}