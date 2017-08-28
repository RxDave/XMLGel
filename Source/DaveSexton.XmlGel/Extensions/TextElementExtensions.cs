using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Documents;

namespace DaveSexton.XmlGel.Extensions
{
	internal static class TextElementExtensions
	{
		public static string GetText(this TextElement element)
		{
			var text = new StringBuilder();

			var inline = element as Inline;

			if (inline != null)
			{
				AppendText(inline, text);
			}
			else
			{
				var block = element as Block;

				if (block != null)
				{
					AppendText(block, text);
				}
				else
				{
					var listItem = element as ListItem;

					if (listItem != null)
					{
						foreach (var child in listItem.Blocks)
						{
							AppendText(child, text);
						}
					}
					else
					{
						var tableRowGroup = element as TableRowGroup;

						if (tableRowGroup != null)
						{
							foreach (var child in from row in tableRowGroup.Rows
																		from cell in row.Cells
																		from child in cell.Blocks
																		select child)
							{
								AppendText(child, text);
							}
						}
						else
						{
							var tableRow = element as TableRow;

							if (tableRow != null)
							{
								foreach (var child in from cell in tableRow.Cells
																			from child in cell.Blocks
																			select child)
								{
									AppendText(child, text);
								}
							}
							else
							{
								var tableCell = element as TableCell;

								if (tableCell != null)
								{
									foreach (var child in from child in tableCell.Blocks
																				select child)
									{
										AppendText(child, text);
									}
								}
								else
								{
									throw new InvalidOperationException("An unknown TextElement was found, which does not have a textual representation.");
								}
							}
						}
					}
				}
			}

			return text.ToString();
		}

		private static void AppendText(Inline inline, StringBuilder text)
		{
			var run = inline as Run;

			if (run != null)
			{
				text.Append(run.Text);
			}
			else
			{
				var span = inline as Span;

				if (span != null)
				{
					foreach (var child in span.Inlines)
					{
						AppendText(child, text);
					}
				}
				else
				{
					if (inline is LineBreak)
					{
						text.AppendLine();
					}
					else
					{
						var anchor = inline as AnchoredBlock;

						if (anchor != null)
						{
							foreach (var block in anchor.Blocks)
							{
								AppendText(block, text);
							}
						}
						else
						{
							throw new InvalidOperationException("An InlineUIContainer element was found, which does not have a textual representation.");
						}
					}
				}
			}
		}

		private static void AppendText(Block block, StringBuilder text)
		{
			var paragraph = block as Paragraph;

			if (paragraph != null)
			{
				foreach (var inline in paragraph.Inlines)
				{
					AppendText(inline, text);
				}
			}
			else
			{
				var section = block as Section;

				if (section != null)
				{
					foreach (var child in section.Blocks)
					{
						AppendText(child, text);
					}
				}
				else
				{
					var list = block as List;

					if (list != null)
					{
						foreach (var child in from item in list.ListItems
																	from child in item.Blocks
																	select child)
						{
							AppendText(child, text);
						}
					}
					else
					{
						var table = block as Table;

						if (table != null)
						{
							foreach (var child in from grp in table.RowGroups
																		from row in grp.Rows
																		from cell in row.Cells
																		from child in cell.Blocks
																		select child)
							{
								AppendText(child, text);
							}
						}
						else
						{
							throw new InvalidOperationException("A BlockUIContainer element was found, which does not have a textual representation.");
						}
					}
				}
			}
		}

		public static void RemoveFromParent(this TextElement element)
		{
			var document = element.Parent as FlowDocument;

			if (document != null)
			{
				document.Blocks.Remove((Block) element);
			}
			else
			{
				var parent = element.Parent as TextElement;

				if (parent == null)
				{
					throw new InvalidOperationException();
				}

				parent.TryGetChildCollection().Remove(element);
			}
		}

		public static void AddBeforeSelf(this TextElement followingSibling, TextElement newElement, FrameworkContentElement parent)
		{
			TextElementContainerItemKind kind;
			var children = parent.TryGetChildCollection(out kind);

			if (children == null)
			{
				// This should never occur
				throw new InvalidOperationException("The parent element \"" + parent.GetType() + "\" cannot contain children.");
			}

			switch (kind)
			{
				case TextElementContainerItemKind.Block:
					((BlockCollection) children).InsertBefore((Block) followingSibling, (Block) newElement);
					break;
				case TextElementContainerItemKind.Inline:
					((InlineCollection) children).InsertBefore((Inline) followingSibling, (Inline) newElement);
					break;
				case TextElementContainerItemKind.ListItem:
					((ListItemCollection) children).InsertBefore((ListItem) followingSibling, (ListItem) newElement);
					break;
				case TextElementContainerItemKind.TableRowGroup:
					((TableRowGroupCollection) children).Insert(children.IndexOf(followingSibling), (TableRowGroup) newElement);
					break;
				case TextElementContainerItemKind.TableRow:
					((TableRowCollection) children).Insert(children.IndexOf(followingSibling), (TableRow) newElement);
					break;
				case TextElementContainerItemKind.TableCell:
					((TableCellCollection) children).Insert(children.IndexOf(followingSibling), (TableCell) newElement);
					break;
			}
		}

		public static void AddAfterSelf(this TextElement previousSibling, TextElement newElement, FrameworkContentElement parent)
		{
			TextElementContainerItemKind kind;
			var children = parent.TryGetChildCollection(out kind);

			if (children == null)
			{
				// This should never occur
				throw new InvalidOperationException("The parent element \"" + parent.GetType() + "\" cannot contain children.");
			}

			switch (kind)
			{
				case TextElementContainerItemKind.Block:
					((BlockCollection) children).InsertAfter((Block) previousSibling, (Block) newElement);
					break;
				case TextElementContainerItemKind.Inline:
					((InlineCollection) children).InsertAfter((Inline) previousSibling, (Inline) newElement);
					break;
				case TextElementContainerItemKind.ListItem:
					((ListItemCollection) children).InsertAfter((ListItem) previousSibling, (ListItem) newElement);
					break;
				case TextElementContainerItemKind.TableRowGroup:
					((TableRowGroupCollection) children).Insert(children.IndexOf(previousSibling) - 1, (TableRowGroup) newElement);
					break;
				case TextElementContainerItemKind.TableRow:
					((TableRowCollection) children).Insert(children.IndexOf(previousSibling) - 1, (TableRow) newElement);
					break;
				case TextElementContainerItemKind.TableCell:
					((TableCellCollection) children).Insert(children.IndexOf(previousSibling) - 1, (TableCell) newElement);
					break;
			}
		}

		public static XDocument GetStructure(this FlowDocument document, XNamespace defaultNamespace, ITextElementNodeFactory factory)
		{
			return new XDocument(
				new XElement(defaultNamespace + "FlowDocument", document.Blocks.Select(block => block.GetStructure(defaultNamespace, factory))));
		}

		public static XDocument GetStructure(this FlowDocument document, ITextElementNodeFactory factory)
		{
			return GetStructure(document, XNamespace.None, factory);
		}

		public static XElement GetStructure(this TextElement element, XNamespace defaultNamespace, ITextElementNodeFactory factory)
		{
			return factory.Create(element).GetStructure(defaultNamespace);
		}

		public static XElement GetStructure(this TextElement element, ITextElementNodeFactory factory)
		{
			return GetStructure(element, XNamespace.None, factory);
		}

		public static IEnumerable<ITextElementNode> GetChildren(this TextElement element, ITextElementNodeFactory factory)
		{
			return factory.Create(element).Children;
		}

		public static IEnumerable<TextElement> GetChildren(this TextElement element)
		{
			return LogicalTreeHelper.GetChildren(element).OfType<TextElement>();
		}

		public static IEnumerable<TextElement> GetDescendants(this TextElement element)
		{
			foreach (var child in GetChildren(element))
			{
				var childElement = child as TextElement;

				if (childElement != null)
				{
					yield return childElement;

					foreach (var descendant in childElement.GetDescendants())
					{
						yield return descendant;
					}
				}
			}
		}

		public static TableColumn GetColumn(this TableCell cell)
		{
			if (cell != null)
			{
				var row = (TableRow) cell.Parent;

				if (row != null)
				{
					var rowGroup = (TableRowGroup) row.Parent;

					if (rowGroup != null)
					{
						var table = (Table) rowGroup.Parent;

						if (table != null)
						{
							var cellIndex = row.Cells.IndexOf(cell);

							if (cellIndex >= 0 && cellIndex < table.Columns.Count)
							{
								return table.Columns[cellIndex];
							}
						}
					}
				}
			}

			return null;
		}

		public static Rect GetVisualBox(this RichTextBox editor)
		{
			var editorPadding = editor.Padding;
			var editorBorder = editor.BorderThickness;

			var leftOffset = editorPadding.Left + editorBorder.Left;
			var rightOffset = editorPadding.Right + editorBorder.Right;
			var topOffset = editorPadding.Top + editorBorder.Top;
			var bottomOffset = editorPadding.Bottom + editorBorder.Bottom;

			return new Rect(
				leftOffset,
				topOffset,
				Math.Max(0, editor.ViewportWidth - (leftOffset + rightOffset)),			// ExtentWidth is not used since a separate ScrollViewer wraps the RichTextBox in the MamlTopicEditor XAML.
				Math.Max(0, editor.ViewportHeight - (topOffset + bottomOffset)));		// ExtentHeight is not used - See previous comment.
		}

		public static TPart TryGetPart<TPart>(this FrameworkContentElement element, PartFactory<TPart> partFactory, Rect documentBox)
			where TPart : Part
		{
			return element == null ? null : partFactory(element, documentBox);
		}

		public static bool EqualsOrContainsDescendant(this FrameworkContentElement parent, FrameworkContentElement selfOrDescendant)
		{
			if (parent == selfOrDescendant)
			{
				return true;
			}

			var ancestor = selfOrDescendant;

			while ((ancestor = ancestor.Parent as FrameworkContentElement) != null)
			{
				if (ancestor == parent)
				{
					return true;
				}
			}

			return false;
		}

		public static Inline ToInline(this Block block)
		{
			return new Figure(block);
		}

		public static Inline ToInline(this TextElement element)
		{
			return element as Inline ?? ToInline(element as Block ?? element.ToSpecialBlock());
		}

		public static Block ToBlock(this Inline inline)
		{
			return new Paragraph(inline);
		}

		public static Block ToBlock(this TextElement element)
		{
			var block = element as Block;

			if (block != null)
			{
				return block;
			}

			var inline = element as Inline;

			if (inline != null)
			{
				return new Paragraph(inline);
			}

			return element.ToSpecialBlock();
		}

		private static Block ToSpecialBlock(this TextElement element)
		{
			var listItem = element as ListItem;

			if (listItem != null)
			{
				return new List(listItem);
			}

			return new Table()
			{
				RowGroups = 
				{
					element.ToTableRowGroup(final: true)
				}
			};
		}

		public static ListItem ToListItem(this Inline inline)
		{
			return new ListItem(new Paragraph(inline));
		}

		public static ListItem ToListItem(this Block block)
		{
			var paragraph = block as Paragraph;

			if (paragraph == null)
			{
				paragraph = new Paragraph(block.ToInline());
			}

			return new ListItem(paragraph);
		}

		public static ListItem ToListItem(this TextElement element)
		{
			var listItem = element as ListItem;

			if (listItem != null)
			{
				return listItem;
			}

			var paragraph = element as Paragraph;

			if (paragraph == null)
			{
				paragraph = new Paragraph(element.ToInline());
			}

			return new ListItem(paragraph);
		}

		public static TableRowGroup ToTableRowGroup(this Inline inline)
		{
			return new Paragraph(inline).ToTableRowGroup();
		}

		public static TableRowGroup ToTableRowGroup(this Block block)
		{
			return new TableRowGroup()
			{
				Rows =
				{
					block.ToTableRow()
				}
			};
		}

		public static TableRowGroup ToTableRowGroup(this TextElement element)
		{
			return element.ToTableRowGroup(final: false);
		}

		private static TableRowGroup ToTableRowGroup(this TextElement element, bool final)
		{
			return element as TableRowGroup
					?? new TableRowGroup()
						 {
							 Rows =
							 {
								 element.ToTableRow(final)
							 }
						 };
		}

		public static TableRow ToTableRow(this Inline inline)
		{
			return new Paragraph(inline).ToTableRow();
		}

		public static TableRow ToTableRow(this Block block)
		{
			return new TableRow()
			{
				Cells =
				{
					new TableCell(block)
				}
			};
		}

		public static TableRow ToTableRow(this TextElement element)
		{
			return element.ToTableRow(final: false);
		}

		private static TableRow ToTableRow(this TextElement element, bool final)
		{
			return element as TableRow
					?? new TableRow()
						 {
							 Cells =
							 {
								 element.ToTableCell(final)
							 }
						 };
		}

		public static TableCell ToTableCell(this Inline inline)
		{
			return new TableCell(new Paragraph(inline));
		}

		public static TableCell ToTableCell(this Block block)
		{
			return new TableCell(block);
		}

		public static TableCell ToTableCell(this TextElement element)
		{
			return element.ToTableCell(final: false);
		}

		private static TableCell ToTableCell(this TextElement element, bool final)
		{
			var tableCell = element as TableCell;

			if (tableCell != null)
			{
				return tableCell;
			}

			if (final)
			{
				throw new InvalidOperationException("Element type \"" + element + "\" is not supported.");
			}

			return new TableCell(element.ToBlock());
		}

		public static IEnumerable<TextElement> GetDescendantStructure(this FrameworkContentElement element)
		{
			var children = element.TryGetChildCollection();

			if (children != null)
			{
				return children.Cast<TextElement>().GetDescendantStructure();
			}

			return Enumerable.Empty<TextElement>();
		}

		private static IEnumerable<TextElement> GetDescendantStructure(this IEnumerable<TextElement> elements)
		{
			foreach (var element in elements)
			{
				if (element is Block || element is ListItem || element is TableRowGroup || element is TableRow || element is TableCell)
				{
					yield return element;

					foreach (var descendant in element.GetDescendantStructure())
					{
						yield return descendant;
					}
				}
			}
		}

		public static int IndexOf(this BlockCollection blocks, TextElement element)
		{
			return ((IList) blocks).IndexOf(element);
		}

		public static Block ElementAt(this BlockCollection blocks, int index)
		{
			return (Block) ((IList) blocks)[index];
		}

		public static IList TryGetChildCollection(this FrameworkContentElement contentElement)
		{
			TextElementContainerItemKind kind;

			return contentElement.TryGetChildCollection(out kind);
		}

		public static IList TryGetChildCollection(this FrameworkContentElement contentElement, out TextElementContainerItemKind kind)
		{
			var document = contentElement as FlowDocument;

			if (document != null)
			{
				kind = TextElementContainerItemKind.Block;
				return document.Blocks;
			}

			var element = contentElement as TextElement;

			if (element == null)
			{
				throw new ArgumentException();
			}

			var blocks = element.TryGetBlockCollection();

			if (blocks != null)
			{
				kind = TextElementContainerItemKind.Block;
				return blocks;
			}

			var inlines = element.TryGetInlineCollection();

			if (inlines != null)
			{
				kind = TextElementContainerItemKind.Inline;
				return inlines;
			}

			var list = element as List;

			if (list != null)
			{
				kind = TextElementContainerItemKind.ListItem;
				return list.ListItems;
			}

			var table = element as Table;

			if (table != null)
			{
				kind = TextElementContainerItemKind.TableRowGroup;
				return table.RowGroups;
			}

			var tableRowGroup = element as TableRowGroup;

			if (tableRowGroup != null)
			{
				kind = TextElementContainerItemKind.TableRow;
				return tableRowGroup.Rows;
			}

			var tableRow = element as TableRow;

			if (tableRow != null)
			{
				kind = TextElementContainerItemKind.TableCell;
				return tableRow.Cells;
			}

			kind = TextElementContainerItemKind.None;
			return null;
		}

		public static BlockCollection TryGetBlockCollection(this FrameworkContentElement element)
		{
			var document = element as FlowDocument;

			if (document != null)
			{
				return document.Blocks;
			}

			var headeredSection = element as HeaderedSection;

			if (headeredSection != null)
			{
				return headeredSection.BaseBlocks;
			}

			var section = element as Section;

			if (section != null)
			{
				return section.Blocks;
			}

			var anchored = element as AnchoredBlock;

			if (anchored != null)
			{
				return anchored.Blocks;
			}

			var listItem = element as ListItem;

			if (listItem != null)
			{
				return listItem.Blocks;
			}

			var tableCell = element as TableCell;

			if (tableCell != null)
			{
				return tableCell.Blocks;
			}

			return null;
		}

		public static InlineCollection TryGetInlineCollection(this TextElement element)
		{
			var paragraph = element as Paragraph;

			if (paragraph != null)
			{
				return paragraph.Inlines;
			}

			var span = element as Span;

			if (span != null)
			{
				return span.Inlines;
			}

			return null;
		}

		public static TextElement GetPreviousSiblingOrAncestor(this FrameworkContentElement element)
		{
			var parent = element.Parent as FrameworkContentElement;

			while (parent != null)
			{
				var children = parent.TryGetChildCollection();

				var index = children.IndexOf(element);

				if (index > 0)
				{
					return (TextElement) children[index - 1];
				}

				element = parent;
				parent = element.Parent as FrameworkContentElement;
			}

			return null;
		}

		public static TextElement GetFollowingSiblingOrAncestor(this FrameworkContentElement element)
		{
			var parent = element.Parent as FrameworkContentElement;

			while (parent != null)
			{
				var children = parent.TryGetChildCollection();

				var index = children.IndexOf(element);

				if (index < children.Count - 1)
				{
					return (TextElement) children[index + 1];
				}

				element = parent;
				parent = element.Parent as FrameworkContentElement;
			}

			return null;
		}

		public static bool HasChildren(this FrameworkContentElement element)
		{
			var collapsibleSection = element as CollapsibleSection;

			if (collapsibleSection != null)
			{
				return collapsibleSection.Content.Count > 0;
			}
			else
			{
				var children = element.TryGetChildCollection();

				return children != null && children.Count > 0;
			}
		}
	}
}