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
		public async Task Layout_NestedParagraphAndNestedParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Paragraph());
			outerSection.Blocks.Add(new Section(new Paragraph()));

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, consecutiveParagraphOffset, 200, documentHeight - consecutiveParagraphOffset),
				new Rect(0, consecutiveParagraphOffset, 200, documentHeight - consecutiveParagraphOffset));
		}

		[TestMethod]
		public async Task Layout_NestedNestedParagraphAndParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section(new Paragraph()));
			outerSection.Blocks.Add(new Paragraph());

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, consecutiveParagraphOffset, 200, documentHeight - consecutiveParagraphOffset));
		}

		[TestMethod]
		public async Task Layout_NestedSectionAndParagraphAndNestedParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section());
			outerSection.Blocks.Add(new Paragraph());
			outerSection.Blocks.Add(new Section(new Paragraph()));

			var displacement = lineHeight * 2;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, displacement),
				new Rect(0, displacement, 200, lineHeight),
				new Rect(0, displacement + consecutiveParagraphOffset, 200, documentHeight - displacement - consecutiveParagraphOffset),
				new Rect(0, displacement + consecutiveParagraphOffset, 200, documentHeight - displacement - consecutiveParagraphOffset));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndNestedParagraphAndSection()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Paragraph());
			outerSection.Blocks.Add(new Section(new Paragraph()));
			outerSection.Blocks.Add(new Section());

			var displacement = documentHeight - consecutiveParagraphOffset - lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, consecutiveParagraphOffset, 200, lineHeight),
				new Rect(0, consecutiveParagraphOffset, 200, lineHeight),
				new Rect(0, consecutiveParagraphOffset + lineHeight, 200, displacement));
		}

		[TestMethod]
		public async Task Layout_NestedSectionAndNestedParagraphBeforeNestedParagraph()
		{
			var outerSection = new Section();

			outerSection.Blocks.Add(new Section());

			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section(new Paragraph()));

			outerSection.Blocks.Add(section);

			var displacement = lineHeight * 2;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
				documentBox,
				new Rect(0, 0, 200, displacement),
				new Rect(0, displacement, 200, documentHeight - displacement),
				new Rect(0, displacement, 200, lineHeight),
				new Rect(0, displacement + consecutiveParagraphOffset, 200, documentHeight - displacement - consecutiveParagraphOffset),
				new Rect(0, displacement + consecutiveParagraphOffset, 200, documentHeight - displacement - consecutiveParagraphOffset));
		}

		[TestMethod]
		public async Task Layout_NestedNestedParagraphBeforeNestedParagraphAndSection()
		{
			var outerSection = new Section();
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section(new Paragraph()));

			outerSection.Blocks.Add(section);
			outerSection.Blocks.Add(new Section());

			var displacement = documentHeight - consecutiveParagraphOffset - lineHeight;

			await AssertMeasurements(new FlowDocument(outerSection),
				documentBox,
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