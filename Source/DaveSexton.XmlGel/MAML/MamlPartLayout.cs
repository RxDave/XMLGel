using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Documents;
using DaveSexton.XmlGel.Extensions;

namespace DaveSexton.XmlGel.Maml
{
	internal static class MamlPartLayout
	{
		private const double insertionLineHeight = 1;
		private const double maximumChildInsertionLineOffset = 20d;
		private const double minBoundingBoxSize = 3d;

		public static Rect EnsureMinimumSize(Rect box)
		{
			if (!box.IsEmpty)
			{
				if (box.Width < minBoundingBoxSize)
				{
					box.Width = minBoundingBoxSize;
				}

				if (box.Height < minBoundingBoxSize)
				{
					box.Height = minBoundingBoxSize;
				}
			}

			return box;
		}

		public static Rect Measure(TextElement element, out Rect characterStart, out Rect characterEnd)
		{
			if (element == null)
			{
				return characterStart = characterEnd = Rect.Empty;
			}

			characterStart = element.ElementStart.GetCharacterRect(LogicalDirection.Forward);

			var cell = element as TableCell;

			Rect box;

			if (cell != null)
			{
				characterEnd = element.ElementEnd.GetCharacterRect(LogicalDirection.Forward);

				var rowEnd = ((TextElement) element.Parent).ElementEnd.GetCharacterRect(LogicalDirection.Backward);

				characterEnd.Height = rowEnd.Bottom - characterEnd.Top;

				box = characterStart;
				box.Union(characterEnd);
			}
			else
			{
				characterEnd = element.ElementEnd.GetCharacterRect(LogicalDirection.Backward);

				if (AreReversed(characterStart, characterEnd))
				/* In testing, occurred for TableCells and empty Sections.
				 * 
				 * For an empty Section, the start is the end of the previous element and the end is the start of the following element.
				 * 
				 * For TableCell, start is the top-left corner and end is the top-right corner, though start is 1 pixel lower than end.
				 * Furthermore, start's height is the height of a single line of text, though end spans the full height of the cell.
				 */
				{
					var reversedStart = characterEnd;
					var reversedEnd = characterStart;

					Contract.Assume(reversedEnd.Y >= reversedStart.Y);
					Contract.Assume(reversedEnd.Top >= reversedStart.Bottom);

					var width = reversedEnd.X >= reversedStart.X
						? reversedEnd.X - reversedStart.X
						: reversedStart.X - reversedEnd.X;

					box = new Rect(
						Math.Min(reversedStart.X, reversedEnd.X),
						reversedStart.Bottom,
						width,
						reversedEnd.Top - reversedStart.Bottom);
				}
				else
				{
					box = characterStart;
					box.Union(characterEnd);
				}
			}

			characterStart = EnsureMinimumSize(characterStart);
			characterEnd = EnsureMinimumSize(characterEnd);

			return EnsureMinimumSize(box);
		}

		private static Rect Measure(FlowDocument document, Rect documentBox, out Rect characterStart, out Rect characterEnd)
		{
			Contract.Requires(document != null);

			characterStart = document.ContentStart.GetCharacterRect(LogicalDirection.Backward);
			characterEnd = document.ContentEnd.GetCharacterRect(LogicalDirection.Forward);

			characterStart = EnsureMinimumSize(characterStart);
			characterEnd = EnsureMinimumSize(characterEnd);

			Contract.Assume(characterEnd.X >= characterStart.X);
			Contract.Assume(characterEnd.Y >= characterStart.Y);

			return EnsureMinimumSize(documentBox);
		}

		public static Rect MeasureBoundingBox(FrameworkContentElement element, Rect documentBox, out Rect actualBoundingBox, out Rect characterStart, out Rect characterEnd)
		{
			if (element == null)
			{
				return actualBoundingBox = characterStart = characterEnd = Rect.Empty;
			}

			var document = element as FlowDocument;

			return document != null
				? MeasureBoundingBox(document, documentBox, out actualBoundingBox, out characterStart, out characterEnd)
				: MeasureBoundingBox((TextElement) element, documentBox, out actualBoundingBox, out characterStart, out characterEnd);
		}

		private static Rect MeasureBoundingBox(FlowDocument document, Rect documentBox, out Rect actualBoundingBox, out Rect characterStart, out Rect characterEnd)
		{
			return actualBoundingBox = Measure(document, documentBox, out characterStart, out characterEnd);
		}

		public static Rect MeasureBoundingBox(TextElement element, Rect documentBox, out Rect actualBoundingBox, out Rect characterStart, out Rect characterEnd)
		{
			Rect box;
			actualBoundingBox = Measure(element, out characterStart, out characterEnd);

			var cell = element.Parent as TableCell;

			if (cell != null)
			{
				element = cell;

				Rect ignoreStart, ignoreEnd;
				box = Measure(element, out ignoreStart, out ignoreEnd);
			}
			else
			{
				box = actualBoundingBox;
			}

			if (!box.IsEmpty)
			{
				var block = element as Block;

				Thickness margin, padding, border;

				if (block != null)
				{
					margin = block.Margin;
					padding = block.Padding;
					border = block.BorderThickness;

					box.X = documentBox.X;
					box.Width = documentBox.Width;
				}
				else
				{
					var listItem = element as ListItem;

					if (listItem != null)
					{
						margin = listItem.Margin;
						padding = listItem.Padding;
						border = listItem.BorderThickness;

						box.X = documentBox.X;
						box.Width = documentBox.Width;
					}
					else
					{
						if (cell == null)
						{
							cell = element as TableCell;
						}

						if (cell != null)
						{
							margin = new Thickness(0);
							padding = cell.Padding;
							border = cell.BorderThickness;

							var leftOffset = padding.Left.ZeroIfNaN() + border.Left.ZeroIfNaN();
							var rightOffset = padding.Right.ZeroIfNaN() + border.Right.ZeroIfNaN();

							box.X -= leftOffset;

							var column = cell.GetColumn();

							if (column != null && column.Width.IsAbsolute)
							{
								box.Width = column.Width.Value;
							}
							else
							{
								box.Width += leftOffset + rightOffset;
							}
						}
						else
						{
							margin = padding = border = new Thickness(0);
						}
					}
				}

				var topOffset = margin.Top.ZeroIfNaN() + padding.Top.ZeroIfNaN() + border.Top.ZeroIfNaN();
				var bottomOffset = margin.Bottom.ZeroIfNaN() + padding.Bottom.ZeroIfNaN() + border.Bottom.ZeroIfNaN();

				box.Y -= topOffset;
				box.Height += topOffset + bottomOffset;
			}

			return EnsureMinimumSize(box);
		}

		public static IEnumerable<Tuple<MamlPart, Rect>> MeasureLogicalBoxesOfDescendants(FrameworkContentElement element, Rect documentBox)
		{
			foreach (var descendant in element.GetDescendantStructure())
			{
				var part = MamlPart.TryGetWithNode(descendant, documentBox);

				if (part != null)
				{
					var box = MeasureLogicalBox(part.ElementOrDocument, documentBox);

					yield return Tuple.Create(part, box);
				}
			}
		}

		public static Rect MeasureLogicalBox(FrameworkContentElement element, Rect documentBox)
		{
			return element is FlowDocument ? documentBox : MeasureLogicalBox((TextElement) element, documentBox);
		}

		public static Rect MeasureLogicalBox(TextElement element, Rect documentBox)
		{
			return MamlPartLayoutMeasurementContext.MeasureLogicalBox(element, documentBox);
		}

		private static bool AreReversed(Rect characterStart, Rect characterEnd)
		{
			// In testing, the Y values are sometimes off by 1.
			const double error = 1d;

			var roundedEndY = Math.Round(characterEnd.Y, 0, MidpointRounding.AwayFromZero);
			var roundedStartY = Math.Round(characterStart.Y, 0, MidpointRounding.AwayFromZero);

			return roundedEndY < roundedStartY && roundedEndY + error < roundedStartY && roundedEndY - error < roundedStartY;
		}

		public static Rect GetPreviousSiblingInsertionLine(FrameworkContentElement element, Rect logicalBox, Rect documentBox)
		{
			var previous = element.GetPreviousSiblingOrAncestor();
			var previousBottomLeft = previous == null ? documentBox.TopLeft : MeasureLogicalBox(previous, documentBox).BottomLeft;
			var aboveBetween = new Rect(previousBottomLeft, logicalBox.TopLeft).GetCenter();

			return new Rect(logicalBox.X, Math.Min(logicalBox.Y, aboveBetween.Y), logicalBox.Width, insertionLineHeight);
		}

		public static Rect GetChildInsertionLine(Rect logicalBox)
		{
			var y = logicalBox.GetCenter().Y;
			var offset = y - logicalBox.Y;

			if (offset > maximumChildInsertionLineOffset)
			{
				y -= offset - maximumChildInsertionLineOffset;
			}

			return new Rect(logicalBox.X, y, logicalBox.Width, insertionLineHeight);
		}

		public static Rect GetFollowingSiblingInsertionLine(FrameworkContentElement element, Rect logicalBox, Rect documentBox)
		{
			var following = element.GetFollowingSiblingOrAncestor();
			var followingTopLeft = following == null ? documentBox.BottomLeft : MeasureLogicalBox(following, documentBox).TopLeft;
			var belowBetween = new Rect(logicalBox.BottomLeft, followingTopLeft).GetCenter();
			var y = Math.Max(logicalBox.Y, belowBetween.Y);

			if (y >= documentBox.Bottom - 2)
			{
				var document = element as FlowDocument;
				var contentEndPointer = document == null ? ((TextElement) element).ContentEnd : document.ContentEnd;
				var contentEnd = contentEndPointer.GetCharacterRect(LogicalDirection.Forward);

				y = Math.Min(contentEnd.Bottom + maximumChildInsertionLineOffset, logicalBox.Top + maximumChildInsertionLineOffset);
			}

			return new Rect(logicalBox.X, y, logicalBox.Width, insertionLineHeight);
		}
	}
}