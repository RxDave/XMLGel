using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaveSexton.XmlGel.UnitTests.Documents
{
	/* The editor automatically inserts a blank line between paragraphs.
	 * It's a visual spacer only, not an actual insertion point.
	 * That's why the consecutive paragraph in the following tests starts at lineHeight * 2 instead of lineHeight * 1.
	 */

	partial class MamlPartLayoutTests
	{
		[TestMethod]
		public async Task Layout_Paragraph()
		{
			await AssertMeasurements(new FlowDocument(new Paragraph()),
				documentBox,
				documentBox);
		}

		[TestMethod]
		public async Task Layout_NestedParagraph()
		{
			await AssertMeasurements(new FlowDocument(new Section(new Paragraph())),
				documentBox,
				documentBox,
				documentBox);
		}

		[TestMethod]
		public async Task Layout_ParagraphThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Paragraph());

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, consecutiveParagraphOffset, 200, documentHeight - consecutiveParagraphOffset));
		}

		[TestMethod]
		public async Task Layout_ParagraphRunThenParagraphRun()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph(new Run(paragraph1)));
			document.Blocks.Add(new Paragraph(new Run(paragraph2)));

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, consecutiveParagraphOffset, 200, documentHeight - consecutiveParagraphOffset));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndParagraph()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Paragraph());

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, consecutiveParagraphOffset, 200, documentHeight - consecutiveParagraphOffset));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphRunAndParagraphRun()
		{
			var section = new Section();

			section.Blocks.Add(new Paragraph(new Run(paragraph1)));
			section.Blocks.Add(new Paragraph(new Run(paragraph2)));

			await AssertMeasurements(new FlowDocument(section),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, consecutiveParagraphOffset, 200, documentHeight - consecutiveParagraphOffset));
		}
	}
}