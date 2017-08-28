using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaveSexton.XmlGel.UnitTests.Documents
{
	partial class MamlPartLayoutTests
	{
		#region Two Paragraphs, One Nested
		[TestMethod]
		public async Task Layout_ParagraphThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Paragraph()));

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, consecutiveParagraphOffset, 200, documentHeight - consecutiveParagraphOffset),
				new Rect(0, consecutiveParagraphOffset, 200, documentHeight - consecutiveParagraphOffset));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Paragraph());

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, consecutiveParagraphOffset, 200, documentHeight - consecutiveParagraphOffset));
		}

		[TestMethod]
		public async Task Layout_SectionThenParagraphThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Paragraph()));

			var displacement = lineHeight * 2;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, displacement),
				new Rect(0, displacement, 200, lineHeight),
				new Rect(0, displacement + consecutiveParagraphOffset, 200, documentHeight - displacement - consecutiveParagraphOffset),
				new Rect(0, displacement + consecutiveParagraphOffset, 200, documentHeight - displacement - consecutiveParagraphOffset));
		}

		[TestMethod]
		public async Task Layout_ParagraphThenNestedParagraphThenSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section());

			var displacement = documentHeight - consecutiveParagraphOffset - lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, consecutiveParagraphOffset, 200, lineHeight),
				new Rect(0, consecutiveParagraphOffset, 200, lineHeight),
				new Rect(0, consecutiveParagraphOffset + lineHeight, 200, displacement));
		}

		[TestMethod]
		public async Task Layout_SectionThenNestedParagraphAndNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());

			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section(new Paragraph()));

			document.Blocks.Add(section);

			var displacement = lineHeight * 2;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, displacement),
				new Rect(0, displacement, 200, documentHeight - displacement),
				new Rect(0, displacement, 200, lineHeight),
				new Rect(0, displacement + consecutiveParagraphOffset, 200, documentHeight - displacement - consecutiveParagraphOffset),
				new Rect(0, displacement + consecutiveParagraphOffset, 200, documentHeight - displacement - consecutiveParagraphOffset));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndNestedParagraphThenSection()
		{
			var document = new FlowDocument();
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section(new Paragraph()));

			document.Blocks.Add(section);
			document.Blocks.Add(new Section());

			var displacement = documentHeight - consecutiveParagraphOffset - lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight + consecutiveParagraphOffset),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, consecutiveParagraphOffset, 200, lineHeight),
				new Rect(0, consecutiveParagraphOffset, 200, lineHeight),
				new Rect(0, consecutiveParagraphOffset + lineHeight, 200, displacement));
		}
		#endregion
	}
}