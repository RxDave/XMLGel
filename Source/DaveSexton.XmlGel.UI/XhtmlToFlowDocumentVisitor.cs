using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using DaveSexton.XmlGel.Xml;

namespace DaveSexton.XmlGel.UI
{
	class XhtmlToFlowDocumentVisitor : XContainerToFlowDocumentVisitor<XhtmlToFlowDocumentVisitor>
	{
		public XhtmlToFlowDocumentVisitor(XDocument document, Action flowDocumentChanged)
			: base(document, flowDocumentChanged)
		{
		}

		private bool TryParseStyle(string style, out IDictionary<string, string> rules)
		{
			if (string.IsNullOrEmpty(style))
			{
				rules = null;
				return false;
			}

			rules = style.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(rule => rule.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries))
				.ToDictionary(
					rule => rule[0].Trim(),
					rule => rule.Length == 1 ? string.Empty : rule[1].Trim(),
					StringComparer.OrdinalIgnoreCase);

			return true;
		}

		private TextElement Visit(XhtmlGeneralElement element, out TextElement contentContainer)
		{
			return contentContainer = null;
		}

		private TextElement Visit(XhtmlDiv div, out TextElement contentContainer)
		{
			IDictionary<string, string> style;

			if (TryParseStyle((string) div.Element.Attribute("style"), out style)
				&& style.ContainsKey("float")
				&& !string.Equals(style["float"], "none", StringComparison.OrdinalIgnoreCase))
			{
				var floatRight = string.Equals(style["float"], "right", StringComparison.OrdinalIgnoreCase);
				var width = style.ContainsKey("width") ? style["width"] : "auto";
				var height = style.ContainsKey("height") ? style["height"] : null;

				if (height == null)
				{
					var floater = new Floater()
					{
						HorizontalAlignment = floatRight ? HorizontalAlignment.Right : HorizontalAlignment.Left
					};

					if (!string.Equals(width, "auto", StringComparison.OrdinalIgnoreCase))
					{
						floater.Width = System.Web.UI.WebControls.Unit.Parse(width).Value;
					}

					return contentContainer = floater;
				}
				else
				{
					var figure = new Figure()
					{
						HorizontalAnchor = floatRight ? FigureHorizontalAnchor.ContentRight : FigureHorizontalAnchor.ContentLeft
					};

					if (!string.Equals(height, "auto", StringComparison.OrdinalIgnoreCase))
					{
						figure.Height = new FigureLength(System.Web.UI.WebControls.Unit.Parse(height).Value);
					}

					if (!string.Equals(width, "auto", StringComparison.OrdinalIgnoreCase))
					{
						figure.Width = new FigureLength(System.Web.UI.WebControls.Unit.Parse(width).Value);
					}

					return contentContainer = figure;
				}
			}
			else
			{
				return contentContainer = new Section();
			}
		}

		private TextElement Visit(XhtmlParagraph paragraph, out TextElement contentContainer)
		{
			return contentContainer = new Paragraph();
		}

		private TextElement Visit(XhtmlList unorderedList, out TextElement contentContainer)
		{
			return contentContainer = new List();
		}

		private TextElement Visit(XhtmlListItem listItem, out TextElement contentContainer)
		{
			return contentContainer = new ListItem();
		}

		private TextElement Visit(XhtmlSpan span, out TextElement contentContainer)
		{
			var options = span.Element.Attribute("data-options");

			if (options == null)
			{
				return contentContainer = new Span();
			}
			else
			{
				var combo = new ComboBox();

				SelectionChangedEventHandler changed = (sender, e) => FlowDocumentChanged();

				combo.SelectionChanged += changed;

				foreach (var option in options.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					combo.Items.Add(option.Trim());
				}

				combo.SelectedItem = span.Element.Value;

				span.Element.RemoveNodes();

				var inlineContainer = new InlineUIContainer(combo);

				RoutedEventHandler unloaded = null;
				unloaded = (sender, e) =>
				{
					combo.SelectionChanged -= changed;
					inlineContainer.Unloaded -= unloaded;
				};

				inlineContainer.Unloaded += unloaded;

				return contentContainer = inlineContainer;
			}
		}

		private TextElement Visit(XhtmlTable table, out TextElement contentContainer)
		{
			return contentContainer = new Table();
		}

		private TextElement Visit(XhtmlTableRow row, out TextElement contentContainer)
		{
			return contentContainer = new TableRow();
		}

		private TextElement Visit(XhtmlTableCell cell, out TextElement contentContainer)
		{
			return contentContainer = new TableCell();
		}

		private TextElement Visit(XhtmlUnderline underline, out TextElement contentContainer)
		{
			var run = CurrentRun;

			if (run != null)
			{
				run.TextDecorations = TextDecorations.Underline;

				return contentContainer = null;
			}
			else
			{
				return contentContainer = new Underline();
			}
		}

		private TextElement Visit(XhtmlImage image, out TextElement contentContainer)
		{
			var path = image.Element.Attribute("src");

			BitmapImage source;

			if (path != null)
			{
				source = new BitmapImage(new Uri((string) path));
			}
			else
			{
				source = null;
			}

			return contentContainer = new BlockUIContainer(
				new Image()
				{
					Source = source
				});
		}

		private TextElement Visit(XhtmlHyperlink hyperlink, out TextElement contentContainer)
		{
			return contentContainer = new Hyperlink()
			{
				Tag = (string) hyperlink.Element.Attribute("href")
			};
		}

		private TextElement Visit(XhtmlLineBreak lineBreak, out TextElement contentContainer)
		{
			return contentContainer = new LineBreak();
		}

		private TextElement Visit(XhtmlStrong strong, out TextElement contentContainer)
		{
			var run = CurrentRun;

			if (run != null)
			{
				run.FontWeight = FontWeights.Bold;

				return contentContainer = null;
			}
			else
			{
				return contentContainer = new Run()
				{
					FontWeight = FontWeights.Bold
				};
			}
		}

		private TextElement Visit(XhtmlEmphasis emphasis, out TextElement contentContainer)
		{
			var run = CurrentRun;

			if (run != null)
			{
				run.FontStyle = FontStyles.Italic;

				return contentContainer = null;
			}
			else
			{
				return contentContainer = new Run()
				{
					FontStyle = FontStyles.Italic
				};
			}
		}

		protected override IXElementNode<XhtmlToFlowDocumentVisitor> CreateElementNode(XElement element)
		{
			switch (element.Name.LocalName)
			{
				case "div":
					return new XhtmlDiv(element);
				case "p":
					return new XhtmlParagraph(element);
				case "ul":
				case "ol":
					return new XhtmlList(element);
				case "li":
					return new XhtmlListItem(element);
				case "span":
					return new XhtmlSpan(element);
				case "table":
					return new XhtmlTable(element);
				case "tr":
					return new XhtmlTableRow(element);
				case "td":
					return new XhtmlTableCell(element);
				case "u":
					return new XhtmlUnderline(element);
				case "img":
					return new XhtmlImage(element);
				case "a":
					return new XhtmlHyperlink(element);
				case "br":
					return new XhtmlLineBreak(element);
				case "strong":
					return new XhtmlStrong(element);
				case "em":
					return new XhtmlEmphasis(element);
				default:
					return new XhtmlGeneralElement(element);
			}
		}

		private sealed class XhtmlGeneralElement : XElementNode<XhtmlToFlowDocumentVisitor>
		{
			public XhtmlGeneralElement(XElement element)
				: base(element)
			{
			}

			public override TextElement Accept(XhtmlToFlowDocumentVisitor visitor, out TextElement contentContainer)
			{
				return visitor.Visit(this, out contentContainer);
			}
		}

		private sealed class XhtmlDiv : XElementNode<XhtmlToFlowDocumentVisitor>
		{
			public XhtmlDiv(XElement element)
				: base(element)
			{
			}

			public override TextElement Accept(XhtmlToFlowDocumentVisitor visitor, out TextElement contentContainer)
			{
				return visitor.Visit(this, out contentContainer);
			}
		}

		private sealed class XhtmlParagraph : XElementNode<XhtmlToFlowDocumentVisitor>
		{
			public XhtmlParagraph(XElement element)
				: base(element)
			{
			}

			public override TextElement Accept(XhtmlToFlowDocumentVisitor visitor, out TextElement contentContainer)
			{
				return visitor.Visit(this, out contentContainer);
			}
		}

		private sealed class XhtmlList : XElementNode<XhtmlToFlowDocumentVisitor>
		{
			public XhtmlList(XElement element)
				: base(element)
			{
			}

			public override TextElement Accept(XhtmlToFlowDocumentVisitor visitor, out TextElement contentContainer)
			{
				return visitor.Visit(this, out contentContainer);
			}
		}

		private sealed class XhtmlListItem : XElementNode<XhtmlToFlowDocumentVisitor>
		{
			public XhtmlListItem(XElement element)
				: base(element)
			{
			}

			public override TextElement Accept(XhtmlToFlowDocumentVisitor visitor, out TextElement contentContainer)
			{
				return visitor.Visit(this, out contentContainer);
			}
		}

		private sealed class XhtmlSpan : XElementNode<XhtmlToFlowDocumentVisitor>
		{
			public XhtmlSpan(XElement element)
				: base(element)
			{
			}

			public override TextElement Accept(XhtmlToFlowDocumentVisitor visitor, out TextElement contentContainer)
			{
				return visitor.Visit(this, out contentContainer);
			}
		}

		private sealed class XhtmlTable : XElementNode<XhtmlToFlowDocumentVisitor>
		{
			public XhtmlTable(XElement element)
				: base(element)
			{
			}

			public override TextElement Accept(XhtmlToFlowDocumentVisitor visitor, out TextElement contentContainer)
			{
				return visitor.Visit(this, out contentContainer);
			}
		}

		private sealed class XhtmlTableRow : XElementNode<XhtmlToFlowDocumentVisitor>
		{
			public XhtmlTableRow(XElement element)
				: base(element)
			{
			}

			public override TextElement Accept(XhtmlToFlowDocumentVisitor visitor, out TextElement contentContainer)
			{
				return visitor.Visit(this, out contentContainer);
			}
		}

		private sealed class XhtmlTableCell : XElementNode<XhtmlToFlowDocumentVisitor>
		{
			public XhtmlTableCell(XElement element)
				: base(element)
			{
			}

			public override TextElement Accept(XhtmlToFlowDocumentVisitor visitor, out TextElement contentContainer)
			{
				return visitor.Visit(this, out contentContainer);
			}
		}

		private sealed class XhtmlUnderline : XElementNode<XhtmlToFlowDocumentVisitor>
		{
			public XhtmlUnderline(XElement element)
				: base(element)
			{
			}

			public override TextElement Accept(XhtmlToFlowDocumentVisitor visitor, out TextElement contentContainer)
			{
				return visitor.Visit(this, out contentContainer);
			}
		}

		private sealed class XhtmlImage : XElementNode<XhtmlToFlowDocumentVisitor>
		{
			public XhtmlImage(XElement element)
				: base(element)
			{
			}

			public override TextElement Accept(XhtmlToFlowDocumentVisitor visitor, out TextElement contentContainer)
			{
				return visitor.Visit(this, out contentContainer);
			}
		}

		private sealed class XhtmlHyperlink : XElementNode<XhtmlToFlowDocumentVisitor>
		{
			public XhtmlHyperlink(XElement element)
				: base(element)
			{
			}

			public override TextElement Accept(XhtmlToFlowDocumentVisitor visitor, out TextElement contentContainer)
			{
				return visitor.Visit(this, out contentContainer);
			}
		}

		private sealed class XhtmlLineBreak : XElementNode<XhtmlToFlowDocumentVisitor>
		{
			public XhtmlLineBreak(XElement element)
				: base(element)
			{
			}

			public override TextElement Accept(XhtmlToFlowDocumentVisitor visitor, out TextElement contentContainer)
			{
				return visitor.Visit(this, out contentContainer);
			}
		}

		private sealed class XhtmlStrong : XElementNode<XhtmlToFlowDocumentVisitor>
		{
			public XhtmlStrong(XElement element)
				: base(element)
			{
			}

			public override TextElement Accept(XhtmlToFlowDocumentVisitor visitor, out TextElement contentContainer)
			{
				return visitor.Visit(this, out contentContainer);
			}
		}

		private sealed class XhtmlEmphasis : XElementNode<XhtmlToFlowDocumentVisitor>
		{
			public XhtmlEmphasis(XElement element)
				: base(element)
			{
			}

			public override TextElement Accept(XhtmlToFlowDocumentVisitor visitor, out TextElement contentContainer)
			{
				return visitor.Visit(this, out contentContainer);
			}
		}
	}
}