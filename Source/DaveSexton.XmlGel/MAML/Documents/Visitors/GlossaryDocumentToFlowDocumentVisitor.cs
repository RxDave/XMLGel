using System;
using System.Windows.Documents;
using DaveSexton.XmlGel.Documents;
using DaveSexton.XmlGel.Extensions;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	/* developerGlossaryDocumentType (developer.xsd)
	 * 
	 * Requires:
	 *	- glossary (MamlGlossary)
	 */
	internal sealed class GlossaryDocumentToFlowDocumentVisitor : MamlToFlowDocumentVisitor
	{
		public GlossaryDocumentToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document, uiContainerChanged)
		{
		}

		public override TextElement Visit(MamlGlossary glossary, out TextElement contentContainer)
		{
			var table = new Table()
			{
				Columns =
				{
					new TableColumn(),
					new TableColumn(), 
					new TableColumn()
				},
				Tag = glossary.Element.AsDataOnly(glossary)
			};

			contentContainer = table;

			return new CollapsibleSection(table);
		}

		public override TextElement Visit(MamlGlossaryDiv glossaryDiv, out TextElement contentContainer)
		{
			var table = new Table()
			{
				Columns =
				{
					new TableColumn(),
					new TableColumn(), 
					new TableColumn()
				},
				Tag = glossaryDiv.Element.AsDataOnly(glossaryDiv)
			};

			contentContainer = table;

			return new TableRowGroup()
			{
				Rows =
				{
					new TableRow()
					{
						Cells =
						{
							new TableCell(new CollapsibleSection(table))
							{
								ColumnSpan = 3
							}
						}
					}
				}
			};
		}

		public override TextElement Visit(MamlGlossaryEntry glossaryEntry, out TextElement contentContainer)
		{
			var row = new TableRow();

			contentContainer = row;

			return new TableRowGroup()
			{
				Rows = { row },
				Tag = glossaryEntry.Element.AsDataOnly(glossaryEntry)
			};
		}

		public override TextElement Visit(MamlTerms terms, out TextElement contentContainer)
		{
			var paragraph = new Paragraph();

			contentContainer = paragraph;

			return new TableCell(paragraph)
			{
				Tag = terms.Element.AsDataOnly(terms)
			};
		}

		public override TextElement Visit(MamlDefinition definition, out TextElement contentContainer)
		{
			return contentContainer = new TableCell()
			{
				Tag = definition.Element.AsDataOnly(definition)
			};
		}

		public override TextElement Visit(MamlRelatedEntry relatedEntry, out TextElement contentContainer)
		{
			var row = (TableRow) CurrentElement;
			var group = (TableRowGroup) row.Parent;

			List list;

			if (group.Rows.Count == 1)
			{
				group.Rows.Add(new TableRow()
				{
					Cells =
					{
						new TableCell(list = new List())
						{
							ColumnSpan = 2
						}
					}
				});
			}
			else
			{
				list = (List) group.Rows[1].Cells[0].Blocks.FirstBlock;
			}

			var item = new ListItem(new Paragraph(new Run(relatedEntry.TermId)))
			{
				Tag = relatedEntry.Element.AsDataOnly(relatedEntry)
			};

			list.ListItems.Add(item);

			// Don't return the item because it's already been added to the current list
			return contentContainer = null;
		}
	}
}