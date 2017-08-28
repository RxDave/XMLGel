using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Linq;
using System.Xml.Schema;
using DaveSexton.XmlGel.Documents;
using DaveSexton.XmlGel.Extensions;
using DaveSexton.XmlGel.Properties;
using DaveSexton.XmlGel.Xml;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	internal abstract partial class MamlToFlowDocumentVisitor : XContainerToFlowDocumentVisitor<MamlToFlowDocumentVisitor>
	{
		private readonly MamlDocument document;
		private readonly List<Action> dynamicUpdates = new List<Action>();

		public MamlToFlowDocumentVisitor(MamlDocument document, Action uiContainerChanged)
			: base(document.DocumentRoot, uiContainerChanged, document.Resolver, document.Schemas)
		{
			this.document = document;
		}

		public void UpdateDynamicContent()
		{
			for (int i = 0; i < dynamicUpdates.Count; i++)
			{
				dynamicUpdates[i]();
			}
		}

		private void LiftNamespaces(XElement element)
		{
			foreach (var ns in element.Attributes()
				.Select(attr => attr.Name.Namespace)
				.Concat(new[] { element.Name.Namespace })
				.Distinct())
			{
				if (!document.Namespaces.ContainsKey(ns))
				{
					document.Namespaces.Add(ns, element.GetPrefixOfNamespace(ns));
				}
			}
		}

		private IXElementNode<MamlToFlowDocumentVisitor> CreateElementNode(XElement element, bool attachNode)
		{
			IXElementNode<MamlToFlowDocumentVisitor> node;
			Func<MamlToFlowDocumentVisitor, XElement, MamlNode> nodeFactory;

			if (nodeFactories.TryGetValue(element.Name, out nodeFactory))
			{
				node = nodeFactory(this, element);
			}
			else
			{
				node = new MamlAnyNode(element);
			}

			if (attachNode)
			{
				element.SetNode(node);
			}

			return node;
		}

		protected override IXElementNode<MamlToFlowDocumentVisitor> CreateElementNode(XElement element)
		{
			return CreateElementNode(element, attachNode: true);
		}

		protected override IXElementNode<MamlToFlowDocumentVisitor> CreateElementNode(XmlSchemaElement elementSchema)
		{
			return CreateElementNode(elementSchema.CreateElement(Schemas), attachNode: false);
		}

		protected override bool IsDocumentRootElement(XElement element)
		{
			return Maml.IsDocumentName(element.Name);
		}

		protected override bool IsInlineType(XElement element)
		{
			return Maml.IsInline(element);
		}

		protected override XmlSchemaElement GetParagraph(IEnumerable<XmlSchemaElement> elements)
		{
			return elements.FirstOrDefault(element => element.GetXName() == Maml.Para);
		}

		private Options CreateOptionsElement<TEnum>(TEnum? defaultValue)
			where TEnum : struct
		{
			var options = new Options(
				value => EnumStringConverter.FromDocumentValue<TEnum>(value),
				value => EnumStringConverter.ToDocumentValue<TEnum>((TEnum?) value),
				EnumStringConverter.GetOptions<TEnum>())
				{
					SelectedValue = defaultValue
				};

			options.ValueChanged += (_, __) => UIElementContainerChanged();

			return options;
		}

		public override void Visit(XVisitableText text)
		{
			var value = text.Value;

			if (!string.IsNullOrEmpty(value))
			{
				var outline = CurrentElement as CollapsibleSectionOutline;

				if (outline != null)
				{
					int depth;
					if (int.TryParse(value, out depth))
					{
						outline.Depth = depth;
					}

					return;
				}
			}

			base.Visit(text);
		}

		public override void Visit(XVisitableElement element)
		{
			LiftNamespaces(element.Element);

			base.Visit(element);

			UpdateDynamicContent();
		}

		public virtual TextElement Visit(MamlSummary summary, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = summary.Element.AsDataOnly(summary)
			};
		}

		public virtual TextElement Visit(MamlPara para, out TextElement contentContainer)
		{
			return contentContainer = new Paragraph()
			{
				Tag = para.Element.AsDataOnly(para)
			};
		}

		public virtual TextElement Visit(MamlAlert alert, out TextElement contentContainer)
		{
			var options = CreateOptionsElement(alert.Kind);

			return contentContainer = new HeaderedSection()
			{
				Header = new Paragraph(options),
				Tag = alert.Element.AsDataOnly(alert)
			};
		}

		public virtual TextElement Visit(MamlQuote quote, out TextElement contentContainer)
		{
			return contentContainer = new Paragraph()
			{
				Tag = quote.Element.AsDataOnly(quote)
			};
		}

		public virtual TextElement Visit(MamlIntroduction introduction, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = introduction.Element.AsDataOnly(introduction)
			};
		}

		public virtual TextElement Visit(MamlAutoOutline autoOutline, out TextElement contentContainer)
		{
			FrameworkContentElement outlineRoot = CurrentElement;

			XElement rootElement = null;

			while (outlineRoot != null && (rootElement = outlineRoot.Tag as XElement) == null)
			{
				outlineRoot = outlineRoot.Parent as FrameworkContentElement;
			}

			if (rootElement == null)
			{
				return contentContainer = null;
			}

			if (rootElement.Name == Maml.Introduction || rootElement.Name == Maml.Content)
			{
				outlineRoot = (FrameworkContentElement) outlineRoot.Parent;

				if (outlineRoot == null)
				{
					return contentContainer = null;
				}
			}
			else
			{
				return contentContainer = null;
			}

			var outline = new CollapsibleSectionOutline()
			{
				Tag = autoOutline.Element.AsDataOnly(autoOutline)
			};

			var isDocumentRoot = outlineRoot is FlowDocument;

			outline.CaptionVisible = autoOutline.ShowLead;

			if (outline.CaptionVisible)
			{
				var lead = autoOutline.Lead;

				if (string.IsNullOrEmpty(lead))
				{
					lead = isDocumentRoot ? Text.AutoOutlineDefaultLead : Text.AutoOutlineSubsectionDefaultLead;
				}

				outline.Caption = lead;
			}

			var excludeRelatedTopics = autoOutline.ExcludeRelatedTopics;

			dynamicUpdates.Add(() => outline.Update(
				outlineRoot,
				section =>
				{
					var element = section.Tag as XElement;

					return element != null
							&& (element.Name == Maml.Section
								|| (element.Name == Maml.RelatedTopics && !excludeRelatedTopics));
				}));

			return contentContainer = outline;
		}

		public virtual TextElement Visit(MamlSection section, out TextElement contentContainer)
		{
			return contentContainer = new CollapsibleSection()
			{
				Tag = section.Element.AsDataOnly(section)
			};
		}

		public virtual TextElement Visit(MamlSectionSimple section, out TextElement contentContainer)
		{
			return contentContainer = new CollapsibleSection()
			{
				Tag = section.Element.AsDataOnly(section)
			};
		}

		public virtual TextElement Visit(MamlNamedSection section, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = section.Element.AsDataOnly(section)
			};
		}

		public virtual TextElement Visit(MamlTitle title, out TextElement contentContainer)
		{
			var section = CurrentElement as CollapsibleSection;

			if (section != null)
			{
				section.Title = title.Text;

				return contentContainer = null;
			}
			else
			{
				var paragraph = new Paragraph();

				contentContainer = paragraph;

				var table = CurrentElement as Table;

				if (table == null)
				{
					var tableRowGroup = CurrentElement as TableRowGroup;

					if (tableRowGroup != null)
					{
						table = (Table) tableRowGroup.Parent;
					}
				}

				if (table != null)
				{
					table.RowGroups.Insert(
						0,
						new TableRowGroup()
						{
							Tag = title.Element.AsDataOnly(title),
							Rows =
							{
								new TableRow()
								{
									Cells = { new TableCell(paragraph) }
								}
							}
						});

					// Don't return the paragraph because it's already been added to the current table
					return null;
				}
				else
				{
					paragraph.Tag = title.Element.AsDataOnly(title);

					return paragraph;
				}
			}
		}

		public virtual TextElement Visit(MamlContent content, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = content.Element.AsDataOnly(content)
			};
		}

		public virtual TextElement Visit(MamlSectionContent content, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = content.Element.AsDataOnly(content)
			};
		}

		public virtual TextElement Visit(MamlSections sections, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = sections.Element.AsDataOnly(sections)
			};
		}

		public virtual TextElement Visit(MamlConditions conditions, out TextElement contentContainer)
		{
			return contentContainer = new Paragraph()
			{
				Tag = conditions.Element.AsDataOnly(conditions)
			};
		}

		public virtual TextElement Visit(MamlMediaLink link, out TextElement contentContainer)
		{
			if (CurrentNode is MamlInlineContainer)
			{
				return contentContainer = new Span()
				{
					Tag = link.Element.AsDataOnly(link)
				};
			}
			else
			{
				return contentContainer = new Paragraph()
				{
					Tag = link.Element.AsDataOnly(link)
				};
			}
		}

		public virtual TextElement Visit(MamlImage image, out TextElement contentContainer)
		{
			contentContainer = null;

			return new InlineUIContainer(new Image())
			{
				Tag = image.Element.AsDataOnly(image)
			};
		}

		public virtual TextElement Visit(MamlEnum enumNode, out TextElement contentContainer)
		{
			return contentContainer = new Options(enumNode.GetValueFromText, enumNode.GetTextFromValue, enumNode.Options)
			{
				Tag = enumNode.Element.AsDataOnly(enumNode)
			};
		}

		public virtual TextElement Visit(MamlRelatedTopics relatedTopics, out TextElement contentContainer)
		{
			var element = new CollapsibleSection()
			{
				Tag = relatedTopics.Element.AsDataOnly(relatedTopics)
			};

			element.Title = Properties.Text.RelatedTopicsTitle;
			element.TitleIsReadOnly = true;

			return contentContainer = element;
		}

		public virtual TextElement Visit(MamlLink link, out TextElement contentContainer)
		{
			var hyperlink = new Hyperlink(new Run(link.Href == null ? "{LINK}" : link.Href.ToString()))
			{
				Tag = link.Element.AsDataOnly(link),
				NavigateUri = link.Href
			};

			contentContainer = hyperlink;

			return CurrentNode is MamlInlineContainer ? (TextElement) hyperlink : new Paragraph(hyperlink);
		}

		public virtual TextElement Visit(MamlExternalLink link, out TextElement contentContainer)
		{
			var hyperlink = new Hyperlink()
			{
				Tag = link.Element.AsDataOnly(link),
				NavigateUri = link.Href
			};

			contentContainer = hyperlink;

			return CurrentNode is MamlInlineContainer ? (TextElement) hyperlink : new Paragraph(hyperlink);
		}

		public virtual TextElement Visit(MamlExternalLinkUri uri, out TextElement contentContainer)
		{
			var hyperlink = CurrentElement as Hyperlink;

			if (hyperlink != null)
			{
				hyperlink.NavigateUri = new Uri(uri.Text, UriKind.RelativeOrAbsolute);

				return contentContainer = null;
			}
			else
			{
				return contentContainer = new Span()
				{
					Tag = uri.Element.AsDataOnly(uri)
				};
			}
		}

		public virtual TextElement Visit(MamlExternalLinkText text, out TextElement contentContainer)
		{
			var hyperlink = CurrentElement as Hyperlink;

			if (hyperlink != null)
			{
				hyperlink.Inlines.Add(new Run(text.Text));

				return contentContainer = null;
			}
			else
			{
				return contentContainer = new Span()
				{
					Tag = text.Element.AsDataOnly(text)
				};
			}
		}

		public virtual TextElement Visit(MamlExternalLinkAlternateText text, out TextElement contentContainer)
		{
			var hyperlink = CurrentElement as Hyperlink;

			if (hyperlink != null)
			{
				hyperlink.ToolTip = text.Text;

				return contentContainer = null;
			}
			else
			{
				return contentContainer = new Span()
				{
					Tag = text.Element.AsDataOnly(text)
				};
			}
		}

		public virtual TextElement Visit(MamlExternalLinkTarget target, out TextElement contentContainer)
		{
			var hyperlink = CurrentElement as Hyperlink;

			if (hyperlink != null)
			{
				hyperlink.TargetName = target.Text;

				return contentContainer = null;
			}
			else
			{
				return contentContainer = new Span()
				{
					Tag = target.Element.AsDataOnly(target)
				};
			}
		}

		public virtual TextElement Visit(MamlSampleRef sampleRef, out TextElement contentContainer)
		{
			return contentContainer = new BlockUIContainer(new TextBlock(new Run(
				(string) sampleRef.Element.Attribute("srcID") + ' ' + (string) sampleRef.Element.Attribute("databaseID"))))
			{
				Tag = sampleRef.Element.AsDataOnly(sampleRef)
			};
		}

		public virtual TextElement Visit(MamlCodeEntityReference codeEntity, out TextElement contentContainer)
		{
			if (CurrentNode is MamlInlineContainer)
			{
				return contentContainer = new Span()
				{
					Tag = codeEntity.Element.AsDataOnly(codeEntity)
				};
			}
			else
			{
				return contentContainer = new Paragraph()
				{
					Tag = codeEntity.Element.AsDataOnly(codeEntity)
				};
			}
		}

		public virtual TextElement Visit(MamlParameterReference parameter, out TextElement contentContainer)
		{
			if (CurrentNode is MamlInlineContainer)
			{
				return contentContainer = new Span()
				{
					Tag = parameter.Element.AsDataOnly(parameter)
				};
			}
			else
			{
				return contentContainer = new Paragraph()
				{
					Tag = parameter.Element.AsDataOnly(parameter)
				};
			}
		}

		public virtual TextElement Visit(MamlCodeReference codeReference, out TextElement contentContainer)
		{
			return contentContainer = new Paragraph()
			{
				Tag = codeReference.Element.AsDataOnly(codeReference)
			};
		}

		public virtual TextElement Visit(MamlList list, out TextElement contentContainer)
		{
			return contentContainer = new List()
			{
				Tag = list.Element.AsDataOnly(list)
			};
		}

		public virtual TextElement Visit(MamlListItem listItem, out TextElement contentContainer)
		{
			return contentContainer = new ListItem()
			{
				Tag = listItem.Element.AsDataOnly(listItem)
			};
		}

		public virtual TextElement Visit(MamlTable table, out TextElement contentContainer)
		{
			var group = new TableRowGroup();

			contentContainer = group;

			return new Table()
			{
				RowGroups = { group },
				Tag = table.Element.AsDataOnly(table)
			};
		}

		public virtual TextElement Visit(MamlTableHeader header, out TextElement contentContainer)
		{
			return contentContainer = new TableRow()
			{
				Tag = header.Element.AsDataOnly(header)
			};
		}

		public virtual TextElement Visit(MamlTableRow row, out TextElement contentContainer)
		{
			var header = CurrentElement as TableRow;

			if (header != null)
			{
				contentContainer = header;
				return null;
			}

			return contentContainer = new TableRow()
			{
				Tag = row.Element.AsDataOnly(row)
			};
		}

		public virtual TextElement Visit(MamlTableEntry entry, out TextElement contentContainer)
		{
			var row = (TableRow) CurrentElement;

			var table = ((Table) ((TableRowGroup) row.Parent).Parent);

			var titleGroup = table.RowGroups[0];

			var title = titleGroup.Tag as XElement;

			if (title != null && title.Name == Maml.Title)
			{
				var cell = titleGroup.Rows[0].Cells[0];

				cell.ColumnSpan = Math.Max(cell.ColumnSpan, row.Cells.Count + 1);
			}

			return contentContainer = new TableCell()
			{
				Tag = entry.Element.AsDataOnly(entry)
			};
		}

		public virtual TextElement Visit(MamlProcedure procedure, out TextElement contentContainer)
		{
			return contentContainer = new CollapsibleSection()
			{
				Tag = procedure.Element.AsDataOnly(procedure)
			};
		}

		public virtual TextElement Visit(MamlSteps steps, out TextElement contentContainer)
		{
			return contentContainer = new List()
			{
				MarkerStyle = System.Windows.TextMarkerStyle.Decimal,
				Tag = steps.Element.AsDataOnly(steps)
			};
		}

		public virtual TextElement Visit(MamlStep step, out TextElement contentContainer)
		{
			return contentContainer = new ListItem()
			{
				Tag = step.Element.AsDataOnly(step)
			};
		}

		public virtual TextElement Visit(MamlCode code, out TextElement contentContainer)
		{
			var paragraph = new Paragraph();

			contentContainer = paragraph;

			return new CollapsibleSection(paragraph)
			{
				Tag = code.Element.AsDataOnly(code)
			};
		}

		public virtual TextElement Visit(MamlCodeSection section, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = section.Element.AsDataOnly(section)
			};
		}

		public virtual TextElement Visit(MamlSharedContent block, out TextElement contentContainer)
		{
			return contentContainer = new Paragraph()
			{
				Tag = block.Element.AsDataOnly(block)
			};
		}

		public virtual TextElement Visit(MamlDefinitionTable definitionTable, out TextElement contentContainer)
		{
			var group = new TableRowGroup()
			{
				Tag = definitionTable.Element.AsDataOnly(definitionTable)
			};

			contentContainer = group;

			return new Table()
			{
				RowGroups = { group },
				Columns =
				{
					new TableColumn(),
					new TableColumn()
				}
			};
		}

		public virtual TextElement Visit(MamlDefinedTerm definedTerm, out TextElement contentContainer)
		{
			var paragraph = new Paragraph();

			contentContainer = paragraph;

			return new TableRow()
			{
				Cells =
				{
					new TableCell(paragraph)
					{
						Tag = definedTerm.Element.AsDataOnly(definedTerm)
					}
				}
			};
		}

		public virtual TextElement Visit(MamlDefinition definition, out TextElement contentContainer)
		{
			var rowGroup = (TableRowGroup) CurrentElement;
			var row = rowGroup.Rows[rowGroup.Rows.Count - 1];
			var cell = new TableCell()
			{
				Tag = definition.Element.AsDataOnly(definition)
			};

			contentContainer = cell;

			row.Cells.Add(cell);

			// Don't return the cell because it's already been added to the current row
			return null;
		}

		public virtual TextElement Visit(MamlBold bold, out TextElement contentContainer)
		{
			return contentContainer = new Bold()
			{
				Tag = bold.Element.AsDataOnly(bold)
			};
		}

		public virtual TextElement Visit(MamlItalic italic, out TextElement contentContainer)
		{
			return contentContainer = new Italic()
			{
				Tag = italic.Element.AsDataOnly(italic)
			};
		}

		public virtual TextElement Visit(MamlUnderline underline, out TextElement contentContainer)
		{
			return contentContainer = new Underline()
			{
				Tag = underline.Element.AsDataOnly(underline)
			};
		}

		public virtual TextElement Visit(MamlInline inline, out TextElement contentContainer)
		{
			return contentContainer = new Span()
			{
				Tag = inline.Element.AsDataOnly(inline)
			};
		}

		public virtual TextElement Visit(MamlString stringNode, out TextElement contentContainer)
		{
			return contentContainer = new Span()
			{
				Tag = stringNode.Element.AsDataOnly(stringNode)
			};
		}

		public virtual TextElement Visit(MamlNode node, out TextElement contentContainer)
		{
			return contentContainer = new Span()
			{
				Tag = node.Element.AsDataOnly(node)
			};
		}

		// How-to, Sample, Error Message, Ref-With-Syntax, Ref-Without-Syntax and XML Reference documents
		public virtual TextElement Visit(MamlCodeExample example, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = example.Element.AsDataOnly(example)
			};
		}

		// How-to and Code Entity (Reference) documents
		public virtual TextElement Visit(MamlCodeExamples examples, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = examples.Element.AsDataOnly(examples)
			};
		}

		// Orientation and SDK Technology Overview Orientation documents
		public virtual TextElement Visit(MamlExternalResources resources, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = resources.Element.AsDataOnly(resources)
			};
		}

		// Whitepaper and XML Reference documents
		public virtual TextElement Visit(MamlSingleParagraphSimpleContent simple, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = simple.Element.AsDataOnly(simple)
			};
		}

		// XML Reference and Reference With Syntax documents
		public virtual TextElement Visit(MamlSyntaxSection section, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = section.Element.AsDataOnly(section)
			};
		}

		// XML Reference and Reference With Syntax documents
		public virtual TextElement Visit(MamlLegacySyntax syntax, out TextElement contentContainer)
		{
			return contentContainer = new Section()
			{
				Tag = syntax.Element.AsDataOnly(syntax)
			};
		}

		// Error Message document
		public virtual TextElement Visit(MamlNonLocErrorTitle title, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlSecondaryErrorTitle title, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		// XML Reference document
		public virtual TextElement Visit(MamlSchemaHierarchy schema, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlAttributesAndElements attributesAndElements, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlAttribute attribute, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlAttributes attributes, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlElementInformation information, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlRelatedEntry relatedEntry, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		// Glossary
		public virtual TextElement Visit(MamlGlossary glossary, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlGlossaryDiv glossaryDiv, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlGlossaryEntry glossaryEntry, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlTerms terms, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		// Code Entity (Reference) document
		public virtual TextElement Visit(MamlAssembly assembly, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlCodeEntities entities, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlCodeEntity entity, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlCodeReferenceDescription description, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlParameterReferenceDescription description, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlGenericParameters parameters, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlParameters parameters, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlExceptions exceptions, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlPlatformNotes notes, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlPlatformNote note, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlPlatforms platforms, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		public virtual TextElement Visit(MamlPermissions permissions, out TextElement contentContainer)
		{
			return contentContainer = null;
		}
	}
}