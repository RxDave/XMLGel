using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Documents;

namespace DaveSexton.XmlGel.UI
{
	class FlowDocumentToXhtmlVisitor : FlowDocumentToXDocumentVisitor
	{
		public FlowDocumentToXhtmlVisitor(FlowDocument flowDocument)
			: base(flowDocument)
		{
		}

		protected override XDocument CreateDocument(out XElement root)
		{
			root = new XElement("body");

			return new XDocument(
				new XElement("html",
					new XElement("head"),
					root));
		}

		protected override XNode CreateReplacement(SectionNode section)
		{
			return new XElement("div");
		}

		protected override XNode CreateReplacement(ParagraphNode paragraph)
		{
			return new XElement("p");
		}

		protected override XNode CreateReplacement(ListNode list)
		{
			return new XElement("ul");
		}

		protected override XNode CreateReplacement(SpanNode span)
		{
			return new XElement("span");
		}

		protected override XNode CreateReplacement(ListItemNode listItem)
		{
			return new XElement("li");
		}

		protected override XNode CreateReplacement(TableNode table)
		{
			return new XElement("table");
		}

		protected override XNode CreateReplacement(TableRowNode tableRow)
		{
			return new XElement("tr");
		}

		protected override XNode CreateReplacement(TableCellNode tableCell)
		{
			return new XElement("td");
		}

		protected override XNode CreateReplacement(UnderlineNode underline)
		{
			// Unlike Bold and Italic nodes, an Underline node does not automatically change the font of the Runs that it contains.
			return new XElement("u");
		}

		protected override XNode CreateReplacement(InlineUIContainerNode inlineUI)
		{
			var comboBox = inlineUI.Element.Child as ComboBox;

			if (comboBox == null)
			{
				throw new NotSupportedException("Unsupported element in inline UI container: " + inlineUI.Element.Child.GetType().FullName);
			}

			var options = comboBox.Items.Cast<string>().Aggregate(
				new StringBuilder(),
				(builder, cur) => builder.Append(cur).Append(','),
				builder => builder.ToString(0, Math.Max(builder.Length - 1, 0)));

			return new XElement("span", new XAttribute("data-options", options), comboBox.SelectedItem);
		}

		protected override XNode CreateReplacement(BlockUIContainerNode blockUI)
		{
			var image = blockUI.Element.Child as Image;

			if (image == null)
			{
				throw new NotSupportedException("Unsupported element in block UI container: " + blockUI.Element.Child.GetType().FullName);
			}

			return new XElement("img", new XAttribute("src", image.Source.ToString()));
		}

		protected override XNode CreateReplacement(FigureNode figure)
		{
			return new XElement("div",
				new XAttribute("style",
					"width: " + (figure.Element.Width.IsAuto ? "auto" : figure.Element.Width.ToString() + "px") +
					";height: " + (figure.Element.Height.IsAuto ? "auto" : figure.Element.Height.ToString() + "px") +
					(figure.Element.HorizontalAnchor == FigureHorizontalAnchor.ContentRight
					? ";float: right"
					: ";float: left")));
		}

		protected override XNode CreateReplacement(FloaterNode floater)
		{
			return new XElement("div",
				new XAttribute("style",
					"width: " + (double.IsNaN(floater.Element.Width) ? "auto" : floater.Element.Width.ToString() + "px") +
					(floater.Element.HorizontalAlignment == HorizontalAlignment.Right
					? ";float: right"
					: ";float: left")));
		}

		protected override XNode CreateReplacement(HyperlinkNode hyperlink)
		{
			return new XElement("a", new XAttribute("href", hyperlink.Element.Tag));
		}

		protected override XNode CreateReplacement(LineBreakNode lineBreak)
		{
			return new XElement("br");
		}

		protected override XNode CreateReplacement(RunNode run)
		{
			var element = run.Element;

			var text = new XText(element.Text);

			XNode node = text;

			if (element.FontWeight > FontWeights.Normal)
			{
				node = new XElement("strong", node);
			}

			if (element.FontStyle == FontStyles.Italic)
			{
				node = new XElement("em", node);
			}

			var decorations = element.TextDecorations;

			if (decorations.Count > 0 && decorations.Any(d => d.Location == TextDecorationLocation.Underline))
			{
				node = new XElement("u", node);
			}

			return node;
		}

		protected override XNode CreateReplacement(LiteralNode literal)
		{
			return new XCData(literal.Element.Text);
		}

		protected override XNode CreateReplacement(CommentNode comment)
		{
			return new XComment(comment.Element.Text);
		}
	}
}