using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Documents;
using DaveSexton.XmlGel.Extensions;
using DaveSexton.XmlGel.Xml;

namespace DaveSexton.XmlGel.Maml.Documents.Visitors
{
	internal abstract class FlowDocumentToMamlVisitor : FlowDocumentToXDocumentVisitor
	{
		private MamlDocument document;

		protected FlowDocumentToMamlVisitor(FlowDocument flowDocument, MamlDocument document)
			: base(flowDocument)
		{
			this.document = document;
		}

		protected sealed override XDocument CreateDocument(out XElement root)
		{
			return document.CreateEmptyMamlDocument(out root);
		}

		private static XElement CreateReplacementElementFromTag(TextElementNode node)
		{
			var element = node.Element.Tag as XElement;

			return element == null ? null : element.CloneWithAnnotations();
		}

		protected override XNode CreateReplacement(TextElementNode textElement, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(textElement);
		}

		protected override XNode CreateReplacement(SectionNode section, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(section);
		}

		protected override XNode CreateReplacement(HeaderedSectionNode section, out XElement contentContainer)
		{
			var element = CreateReplacementElementFromTag(section);

			if (element.Name == Maml.Alert)
			{
				var paragraph = section.Element.Header as Paragraph;

				if (paragraph != null)
				{
					var options = paragraph.Inlines.FirstInline as Options;

					element.SetAttributeValue(Maml.ClassAttribute, options.SelectedValueAsString);
				}
			}

			return contentContainer = element;
		}

		protected override XNode CreateReplacement(CollapsibleSectionNode section, out XElement contentContainer)
		{
			var replacement = CreateReplacementElementFromTag(section);

			contentContainer = replacement;

			if (replacement != null
				&& replacement.Name != Maml.RelatedTopics)
			{
				var title = section.Element.Title;

				if (replacement.Name == Maml.Code)
				{
					if (!string.IsNullOrWhiteSpace(title))
					{
						replacement.SetAttributeValue(Maml.TitleAttribute, title);
					}

					if (!string.IsNullOrEmpty((string) replacement.Attribute(Maml.SourceAttribute)))
					{
						contentContainer = null;
					}
				}
				else if (!string.IsNullOrWhiteSpace(title))
				{
					replacement.Add(new XElement(Maml.Title, title));
				}
			}

			return replacement;
		}

		protected override XNode CreateReplacement(CollapsibleSectionOutlineNode outline, out XElement contentContainer)
		{
			contentContainer = ContentContainerSkipChildrenIndicator;

			var element = CreateReplacementElementFromTag(outline);

			var depth = outline.Element.Depth;

			if (depth > 0)
			{
				element.Add(depth);
			}

			return element;
		}

		protected override XNode CreateReplacement(BlockNode block, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(block);
		}

		protected override XNode CreateReplacement(ErrorBlockNode error, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(error);
		}

		protected override XNode CreateReplacement(ParagraphNode paragraph, out XElement contentContainer)
		{
			var current = CurrentElement;

			if (current.Name == Maml.Code)
			{
				contentContainer = current;

				return current.Value.Length == 0 ? null : new XText(Environment.NewLine);
			}

			return contentContainer = CreateReplacementElementFromTag(paragraph);
		}

		protected override XNode CreateReplacement(ListNode list, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(list);
		}

		protected override XNode CreateReplacement(SpanNode span, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(span);
		}

		protected override XNode CreateReplacement(ListItemNode listItem, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(listItem);
		}

		protected override XNode CreateReplacement(TableNode table, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(table);
		}

		protected override XNode CreateReplacement(TableRowGroupNode tableRowGroup, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(tableRowGroup);
		}

		protected override XNode CreateReplacement(TableRowNode tableRow, out XElement contentContainer)
		{
			var element = CreateReplacementElementFromTag(tableRow);

			if (element != null && element.Name == Maml.TableHeader)
			{
				element.Add(contentContainer = new XElement(Maml.Row));
			}
			else
			{
				contentContainer = element;
			}

			return element;
		}

		protected override XNode CreateReplacement(TableCellNode tableCell, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(tableCell);
		}

		protected override XNode CreateReplacement(InlineNode inline, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(inline);
		}

		protected override XNode CreateReplacement(ErrorInlineNode error, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(error);
		}

		protected override XNode CreateReplacement(OptionsNode options, out XElement contentContainer)
		{
			var element = CreateReplacementElementFromTag(options);

			element.Value = options.Element.SelectedValueAsString;

			return contentContainer = element;
		}

		protected override XNode CreateReplacement(UnderlineNode underline, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(underline);
		}

		protected override XNode CreateReplacement(BoldNode bold, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(bold);
		}

		protected override XNode CreateReplacement(ItalicNode italic, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(italic);
		}

		protected override XNode CreateReplacement(InlineUIContainerNode inlineUI, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(inlineUI);
		}

		protected override XNode CreateReplacement(BlockUIContainerNode blockUI, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(blockUI);
		}

		protected override XNode CreateReplacement(AnchoredBlockNode block, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(block);
		}

		protected override XNode CreateReplacement(FigureNode figure, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(figure);
		}

		protected override XNode CreateReplacement(FloaterNode floater, out XElement contentContainer)
		{
			return contentContainer = CreateReplacementElementFromTag(floater);
		}

		protected override XNode CreateReplacement(HyperlinkNode hyperlink, out XElement contentContainer)
		{
			var element = contentContainer = CreateReplacementElementFromTag(hyperlink);

			if (element.Name == Maml.ExternalLink)
			{
				var linkText = new XElement(Maml.LinkText, hyperlink.Element.GetText());

				var alternateText = hyperlink.Element.ToolTip as string;
				var linkAlternateText = string.IsNullOrWhiteSpace(alternateText) ? null : new XElement(Maml.LinkAlternateText, alternateText);

				var linkUri = new XElement(Maml.LinkUri, hyperlink.Element.NavigateUri);

				var target = hyperlink.Element.TargetName;
				var linkTarget = string.IsNullOrWhiteSpace(target) ? null : new XElement(Maml.LinkTarget, target);

				element.Add(linkText, linkAlternateText, linkUri, linkTarget);
			}
			else
			{
				var uri = hyperlink.Element.NavigateUri;

				if (uri != null)
				{
					element.SetAttributeValue(Maml.XLinkHref, uri.ToString());
				}
			}

			return element;
		}

		protected override XNode CreateReplacement(LineBreakNode lineBreak, out XElement contentContainer)
		{
			var element = CreateReplacementElementFromTag(lineBreak);

			if (element != null)
			{
				return contentContainer = element;
			}
			else
			{
				contentContainer = null;

				return new XText(Environment.NewLine);
			}
		}

		protected override XNode CreateReplacement(RunNode run, out XElement contentContainer)
		{
			contentContainer = null;

			var parent = CurrentElement;
			var element = run.Element;

			if (parent.Name == Maml.Code)
			{
				return new XEntitizedText(element.Text);
			}
			else if (parent.Name == Maml.ExternalLink
				|| (parent.Name == Maml.Link && string.Equals(element.Text, (string) parent.Attribute(Maml.XLinkHref), StringComparison.Ordinal)))
			{
				return null;
			}
			else
			{
				var text = new XEntitizedText(element.Text);

				XNode node = text;

				if (element.FontWeight > FontWeights.Normal
					&& element.ReadLocalValue(TextElement.FontWeightProperty) != DependencyProperty.UnsetValue		// Ensure style isn't inherited
					&& parent.Name != Maml.LegacyBold)
				{
					node = new XElement(Maml.LegacyBold, node);
				}

				if (element.FontStyle == FontStyles.Italic
					&& element.ReadLocalValue(TextElement.FontStyleProperty) != DependencyProperty.UnsetValue		// Ensure style isn't inherited
					&& parent.Name != Maml.LegacyItalic)
				{
					node = new XElement(Maml.LegacyItalic, node);
				}

				var decorations = element.TextDecorations;

				if (decorations.Count > 0
					&& decorations.Any(d => d.Location == TextDecorationLocation.Underline)
					&& parent.Name != Maml.LegacyUnderline)
				{
					node = new XElement(Maml.LegacyUnderline, node);
				}

				return node;
			}
		}

		protected override XNode CreateReplacement(LiteralNode literal, out XElement contentContainer)
		{
			contentContainer = null;

			return new XEntitizedCData(literal.Element.Text);
		}

		protected override XNode CreateReplacement(CommentInlineNode comment, out XElement contentContainer)
		{
			contentContainer = null;

			return new XEntitizedComment(comment.Element.Text);
		}

		protected override XNode CreateReplacement(CommentBlockNode comment, out XElement contentContainer)
		{
			/* Since contentContainer must be an XElement, it's not possible to create a comment context while visiting
			 * child text nodes; however, CommentBlockNode nodes may only contain inline CommentNode nodes anyway, which 
			 * create comments when visited.  Therefore, it's best to simply ignore block comments.  Their only purpose 
			 * is to allow comments anywhere within the source document; e.g., within block-containing nodes such as sections.
			 */
			return contentContainer = null;
		}
	}
}