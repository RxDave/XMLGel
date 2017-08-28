using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Documents;
using System.Xml.Linq;
using System.Xml.Schema;
using DaveSexton.XmlGel.Extensions;
using DaveSexton.XmlGel.Maml.Documents;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml
{
	internal sealed class MamlPart : Part<MamlNode, MamlToFlowDocumentVisitor>
	{
		public Rect LogicalBox
		{
			get
			{
				EnsureLogicalBoxCalculated();

				return logicalBox.Value;
			}
		}

		public Rect PreviousSiblingInsertionLine
		{
			get
			{
				return previousSiblingInsertionLine
						?? (previousSiblingInsertionLine = MamlPartLayout.GetPreviousSiblingInsertionLine(ElementOrDocument, LogicalBox, DocumentBox)).Value;
			}
		}

		public Rect ChildInsertionLine
		{
			get
			{
				return childInsertionLine
						?? (childInsertionLine = MamlPartLayout.GetChildInsertionLine(LogicalBox)).Value;
			}
		}

		public Rect FollowingSiblingInsertionLine
		{
			get
			{
				return followingSiblingInsertionLine
						?? (followingSiblingInsertionLine = MamlPartLayout.GetFollowingSiblingInsertionLine(ElementOrDocument, LogicalBox, DocumentBox)).Value;
			}
		}

		/// <summary>
		/// A bounding box of the part based on <see cref="Box"/> and adjusted for the element's margin, padding and borders.
		/// </summary>
		public Rect BoundingBox
		{
			get
			{
				EnsureBoxCalculated();

				return boundingBox.Value;
			}
		}

		/// <summary>
		/// A measurement of the visual region of the part based on the type of the part's element, adjusted to ensure a minimum size.
		/// </summary>
		public Rect Box
		{
			get
			{
				EnsureBoxCalculated();

				return box;
			}
		}

		/// <summary>
		/// The character start position calculated based on the type of the part's element, adjusted to enusre a minimum size.
		/// </summary>
		/// <remarks>
		/// <see cref="Start"/> may exist after and outside of <see cref="Box"/>.
		/// </remarks>
		public Rect Start
		{
			get
			{
				EnsureBoxCalculated();

				return start;
			}
		}

		/// <summary>
		/// The character end position calculated based on the type of the part's element, adjusted to ensure a minimum size.
		/// </summary>
		/// <remarks>
		/// <see cref="End"/> may exist before and outside of <see cref="Box"/>.
		/// </remarks>
		public Rect End
		{
			get
			{
				EnsureBoxCalculated();

				return end;
			}
		}

		public bool IsStructure
		{
			get
			{
				return !(ElementOrDocument is Paragraph) && !(ElementOrDocument is Inline);
			}
		}

		public bool HasChildren
		{
			get
			{
				return ElementOrDocument.HasChildren();
			}
		}

		public bool HasContainer
		{
			get
			{
				return ElementOrDocument is TextElement;
			}
		}

		public MamlPart Container
		{
			get
			{
				Contract.Requires(HasContainer);

				if (container == null)
				{
					container = GetContainer(Element, DocumentBox);
				}

				return container;
			}
		}

		private MamlPart container;
		private Rect box, start, end;
		private Rect? boundingBox, logicalBox, previousSiblingInsertionLine, childInsertionLine, followingSiblingInsertionLine;

		private MamlPart(FrameworkContentElement element, XElement data, XmlSchemaElement schema, Rect documentBox)
			: base(element, data, schema, documentBox)
		{
		}

		public static MamlPart TryGetWithNode(FrameworkContentElement element, Rect documentBox)
		{
			var part = TryGetWithData(element, documentBox);

			return part == null || part.Node == null ? null : part;
		}

		public static MamlPart TryGetWithData(FrameworkContentElement element, Rect documentBox)
		{
			return TryGet(element, documentBox, isDataRequired: true, isSchemaRequired: false);
		}

		public static MamlPart TryGetWithSchema(FrameworkContentElement element, Rect documentBox)
		{
			return TryGet(element, documentBox, isDataRequired: true, isSchemaRequired: true);
		}

		public static MamlPart TryGet(FrameworkContentElement element, Rect documentBox)
		{
			return TryGet(element, documentBox, isDataRequired: false, isSchemaRequired: false);
		}

		private static MamlPart TryGet(FrameworkContentElement element, Rect documentBox, bool isDataRequired, bool isSchemaRequired)
		{
			Contract.Requires(!isSchemaRequired || isDataRequired);

			if (element == null)
			{
				return null;
			}

			var data = element.Tag as XElement;
			var schema = data == null ? null : data.GetSchema();

			return (data == null && isDataRequired) || (schema == null && isSchemaRequired)
				? null
				: new MamlPart(element, data, schema, documentBox);
		}

		private static MamlPart GetContainer(TextElement element, Rect documentBox)
		{
			Contract.Requires(element != null);

			MamlPart container = null;
			FlowDocument document = null;

			var parent = element.Parent as FrameworkContentElement;

			while (parent != null)
			{
				var part = TryGetWithNode(parent, documentBox);

				if (part != null)
				{
					container = part;
					break;
				}

				document = parent as FlowDocument;
				parent = parent.Parent as FrameworkContentElement;
			}

			if (container == null)
			// During testing, occurred for an empty document (all required elements were deleted, thus the document was invalid)
			{
				Contract.Assume(document != null);

				container = TryGet(document, documentBox);

				Contract.Assume(container != null);
			}

			return container;
		}

		protected override MamlNode GetNode()
		{
			return Data == null ? null : (MamlNode) Data.GetNode();
		}

		protected override Part<MamlNode, MamlToFlowDocumentVisitor> TryCreatePartWithNode(FrameworkContentElement element)
		{
			return TryGetWithSchema(element, DocumentBox);
		}

		private void EnsureBoxCalculated()
		{
			if (!boundingBox.HasValue)
			{
				boundingBox = MamlPartLayout.MeasureBoundingBox(ElementOrDocument, DocumentBox, out box, out start, out end);
			}
		}

		private void EnsureLogicalBoxCalculated()
		{
			if (!logicalBox.HasValue)
			{
				logicalBox = MamlPartLayout.MeasureLogicalBox(ElementOrDocument, DocumentBox);
			}
		}
	}
}