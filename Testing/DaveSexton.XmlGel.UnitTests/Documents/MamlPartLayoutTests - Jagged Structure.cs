using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaveSexton.XmlGel.UnitTests.Documents
{
	partial class MamlPartLayoutTests
	{
		#region Single Childless Section
		[TestMethod]
		public async Task Layout_NestedParagraphThenSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section());

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, documentHeight - lineHeight));
		}

		[TestMethod]
		public async Task Layout_SectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Paragraph()));

			var displacement = lineHeight * 2;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, displacement),
				new Rect(0, displacement, 200, documentHeight - displacement),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndSectionThenParagraph()
		{
			var document = new FlowDocument();
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());

			document.Blocks.Add(section);
			document.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight + displacement),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_ParagraphThenNestedSectionAndParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());

			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			document.Blocks.Add(section);

			var displacement = sectionDisplacementBetweenParagraphs;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, documentHeight - lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion

		#region Two Childless Sections
		[TestMethod]
		public async Task Layout_NestedParagraphThenSectionThenSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section());

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2));
		}

		[TestMethod]
		public async Task Layout_SectionThenSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Paragraph()));

			var displacement = lineHeight * 3;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement, 200, documentHeight - displacement),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndSectionThenSection()
		{
			var document = new FlowDocument();
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());

			document.Blocks.Add(section);
			document.Blocks.Add(new Section());

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight + displacement / 2),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2));
		}

		[TestMethod]
		public async Task Layout_SectionThenNestedSectionAndParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());

			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			document.Blocks.Add(section);

			var displacement = lineHeight * 3;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, documentHeight - displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphAndSectionThenSectionThenParagraph()
		{
			var document = new FlowDocument();
			var section = new Section();

			section.Blocks.Add(new Paragraph());
			section.Blocks.Add(new Section());

			document.Blocks.Add(section);
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight + displacement / 2),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_ParagraphThenSectionThenNestedSectionAndParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section());

			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			document.Blocks.Add(section);

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, documentHeight - lineHeight - displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphThenSectionThenSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_ParagraphThenSectionThenSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section());

			var section = new Section();

			section.Blocks.Add(new Paragraph());

			document.Blocks.Add(section);

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_ParagraphThenNestedSectionAndSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());

			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Section());

			document.Blocks.Add(section);
			document.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion

		#region Single Nested Section
		[TestMethod]
		public async Task Layout_NestedParagraphThenNestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section(new Section()));

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight, 200, displacement));
		}

		[TestMethod]
		public async Task Layout_NestedSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Paragraph()));

			var displacement = lineHeight * 2;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, displacement),
				new Rect(0, 0, 200, displacement),
				new Rect(0, displacement, 200, documentHeight - displacement),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedNestedParagraphThenNestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section(new Paragraph())));
			document.Blocks.Add(new Section(new Section()));

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight, 200, displacement));
		}

		[TestMethod]
		public async Task Layout_NestedSectionThenNestedNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Section(new Paragraph())));

			var displacement = lineHeight * 2;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, displacement),
				new Rect(0, 0, 200, displacement),
				new Rect(0, displacement, 200, documentHeight - displacement),
				new Rect(0, displacement, 200, documentHeight - displacement),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphThenNestedSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_ParagraphThenNestedSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Paragraph()));

			var displacement = sectionDisplacementBetweenParagraphs;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight, 200, displacement),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion

		#region Two Sections; One Childless and One Nested
		[TestMethod]
		public async Task Layout_NestedParagraphThenNestedSectionThenSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section());

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphThenSectionThenNestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Section()));

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2));
		}

		[TestMethod]
		public async Task Layout_NestedSectionThenSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Paragraph()));

			var displacement = lineHeight * 3;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement, 200, documentHeight - displacement),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_SectionThenNestedSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Paragraph()));

			var displacement = lineHeight * 3;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement, 200, documentHeight - displacement),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphThenNestedSectionThenSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphThenSectionThenNestedSectionThenParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Paragraph());

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_ParagraphThenNestedSectionThenSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Paragraph()));

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}

		[TestMethod]
		public async Task Layout_ParagraphThenSectionThenNestedSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section());
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Paragraph()));

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion

		#region Two Nested Sections
		[TestMethod]
		public async Task Layout_NestedParagraphThenNestedSectionThenNestedSection()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Section()));

			var displacement = documentHeight - lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2));
		}

		[TestMethod]
		public async Task Layout_NestedSectionThenNestedSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Paragraph()));

			var displacement = lineHeight * 3;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, 0, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement / 2, 200, displacement / 2),
				new Rect(0, displacement, 200, documentHeight - displacement),
				new Rect(0, displacement, 200, documentHeight - displacement));
		}

		[TestMethod]
		public async Task Layout_NestedParagraphThenNestedSectionThenNestedSectionThenNestedParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Section(new Paragraph()));
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Section()));
			document.Blocks.Add(new Section(new Paragraph()));

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion

		#region Complex Nesting
		[TestMethod]
		public async Task Layout_ParagraphThenNestedSectionThenNestedSectionAndParagraph()
		{
			var document = new FlowDocument();

			document.Blocks.Add(new Paragraph());
			document.Blocks.Add(new Section(new Section()));

			var section = new Section();

			section.Blocks.Add(new Section());
			section.Blocks.Add(new Paragraph());

			document.Blocks.Add(section);

			var displacement = sectionDisplacementBetweenParagraphs + lineHeight;

			await AssertMeasurements(document,
				documentBox,
				new Rect(0, 0, 200, lineHeight),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight, 200, displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, documentHeight - lineHeight - displacement / 2),
				new Rect(0, lineHeight + displacement / 2, 200, displacement / 2),
				new Rect(0, lineHeight + displacement, 200, documentHeight - displacement - lineHeight));
		}
		#endregion
	}
}