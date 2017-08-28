using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaveSexton.XmlGel.UnitTests.Documents
{
	partial class MamlPartLayoutTests
	{
		[TestMethod]
		public async Task Layout_Document()
		{
			await AssertMeasurements(new FlowDocument(), documentBox);
		}

		#region Single Childless Section
		[TestMethod]
		public async Task Layout_Section()
		{
			await AssertMeasurements(new FlowDocument(new Section()),
				documentBox,
				documentBox);
		}

		[TestMethod]
		public async Task Layout_ParagraphThenSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section());

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, documentHeight - lineHeight));
		}

		[TestMethod]
		public async Task Layout_SectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());

			var displacement = lineHeight * 2;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, displacement),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_ParagraphThenSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion

		#region Two Childless Sections
		[TestMethod]
		public async Task Layout_SectionThenSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section());

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, documentHeight - lineHeight));
		}

		[TestMethod]
		public async Task Layout_ParagraphThenSectionThenSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section());

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2));
		}

		[TestMethod]
		public async Task Layout_SectionThenSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());

			var displacement = lineHeight * 3;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_ParagraphThenSectionThenSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion

		#region Single Nested Section
		[TestMethod]
		public async Task Layout_NestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));

			await AssertMeasurements(document,
				documentBox,
				documentBox,
				documentBox);
		}

		[TestMethod]
		public async Task Layout_ParagraphThenNestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Section()));

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight, 200, displacement));
		}

		[TestMethod]
		public async Task Layout_NestedSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Paragraph());

			var displacement = lineHeight * 2;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, displacement),
				new Rect(0, 0, 200, displacement),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_ParagraphThenNestedSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion

		#region Two Sections; One Childless and One Nested
		[TestMethod]
		public async Task Layout_NestedSectionThenSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section());

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, documentHeight - lineHeight));
		}

		[TestMethod]
		public async Task Layout_SectionThenNestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Section()));

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, documentHeight - lineHeight),
				new Rect(0, lineHeight, 200, documentHeight - lineHeight));
		}

		[TestMethod]
		public async Task Layout_ParagraphThenNestedSectionThenSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section());

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2));
		}

		[TestMethod]
		public async Task Layout_ParagraphThenSectionThenNestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Section()));

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2));
		}

		[TestMethod]
		public async Task Layout_NestedSectionThenSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());

			var displacement = lineHeight * 3;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_SectionThenNestedSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Paragraph());

			var displacement = lineHeight * 3;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_ParagraphThenNestedSectionThenSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_ParagraphThenSectionThenNestedSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion

		#region Two Nested Sections
		[TestMethod]
		public async Task Layout_NestedSectionThenNestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Section()));

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, documentHeight - lineHeight),
				new Rect(0, lineHeight, 200, documentHeight - lineHeight));
		}

		[TestMethod]
		public async Task Layout_ParagraphThenNestedSectionThenNestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Section()));

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2));
		}

		[TestMethod]
		public async Task Layout_NestedSectionThenNestedSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Paragraph());

			var displacement = lineHeight * 3;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_ParagraphThenNestedSectionThenNestedSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion
	}
}