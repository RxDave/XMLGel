using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* stepsType (structureProcedure.xsd)
	 * 
	 * Attributes: (contentIdentificationSharingAndConditionGroup - base.xsd)
	 *	- address (ID : NCName)
	 *	
	 * Attribute:
	 *	- class (DaveSexton.XmlGel.Maml.StepsKind)
	 * 
	 * Content:
	 *	- Allows 0..N step (MamlStep) elements
	 *	- Allows 0..N procedure (MamlProcedure) elements
	 *	- Allows 0..N conditionalSection elements
	 *		- Sequence
	 *			- conditions (MamlConditions) element
	 *			- conditionalContent (MamlSteps) element
	 */
	internal sealed class MamlSteps : MamlNode
	{
		public MamlSteps(XElement element)
			: base(element)
		{
		}

		public override TextElement Accept(MamlToFlowDocumentVisitor visitor, out TextElement contentContainer)
		{
			return visitor.Visit(this, out contentContainer);
		}
	}
}
/*
		#region Public Properties
		#endregion

		#region Private / Protected
		private readonly XName itemElementName;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructs a new instance of the <see cref="MamlList" /> class.
		/// </summary>
		public MamlList(ParserFactory factory, System.Windows.ResourceDictionary resources, TextElementStyleManager styleManager, TextElementFactory textElementFactory, 
			XName itemElementName)
			: base(factory, resources, styleManager, textElementFactory)
		{
			if (itemElementName == null)
				throw new ArgumentNullException("itemElementName");

			this.itemElementName = itemElementName;
		}
		#endregion

		#region Methods
		private static System.Windows.TextMarkerStyle GetMarkerStyle(string listClass)
		{
			switch (listClass)
			{
				case "ordered":
					return System.Windows.TextMarkerStyle.Decimal;
				case "nobullet":
					return System.Windows.TextMarkerStyle.None;
				default:	// bullet
					return System.Windows.TextMarkerStyle.Disc;
			}
		}

		private static XAttribute GetListClass(System.Windows.TextMarkerStyle markerStyle)
		{
			switch (markerStyle)
			{
				case System.Windows.TextMarkerStyle.Decimal:
					return Maml.ListOrderedClass;
				case System.Windows.TextMarkerStyle.None:
					return Maml.ListNoBulletClass;
				default:	// Disc
					return Maml.ListBulletClass;
			}
		}

		public override ParseResult Parse(XElement element, BlockCollection rootBlocks)
		{
			BlockCollection blocks = GetBlocks(element, rootBlocks);

			if (blocks == null)
				throw NewXmlException(element, "List elements cannot be children of in-line elements.");

			Action assignStyle;
			List list = TextElementFactory.CreateList(this, element.Name.LocalName, element, out assignStyle);
			list.MarkerStyle = GetMarkerStyle((string) element.Attribute("class"));

			SetTextElement(element, list);

			blocks.Add(list);

			List<ParseResult> childResults = new List<ParseResult>(32);

			foreach (XElement itemElement in element.Elements(itemElementName))
			{
				Action assignItemStyle;
				ListItem item = TextElementFactory.CreateListItem(this, itemElementName.LocalName, itemElement, out assignItemStyle);
				list.ListItems.Add(item);

				SetTextElement(itemElement, item);

				ParserContext context = ParserContext.GetContext(item, false);
				context.LogicalContainer = list;

				// NOTE: This call does not parse the item element - only its children.
				IEnumerable<ParseResult> results = ParseDeep(itemElement, rootBlocks, Parsers);

				childResults.Add(new ParseResult(item, true, results, assignItemStyle));
			}

			return new ParseResult(list, true, childResults, assignStyle);
		}

		public override UnparseResult Unparse(ParserContext context, XContainer parent)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			if (context.HasDefaultValue)
			{
				XNode node = Maml.XList;
				parent.Add(node);

				return new UnparseResult(node, true);
			}

			UnparseResult result = base.Unparse(context, parent);

			if (result.UnparsedChildren)
				return result;
			else
				result.UnparsedChildren = true;

			List<UnparseResult> childResults = new List<UnparseResult>(32);

			XElement element = (XElement) result.Node;
			List list = (List) context.Element;

			XAttribute listClass = GetListClass(list.MarkerStyle);

			element.SetAttributeValue(listClass.Name, listClass.Value);

			foreach (ListItem item in list.ListItems)
			{
				XElement xItem = new XElement(itemElementName);

				element.Add(xItem);

				childResults.AddRange(UnparseDeep(ParserContext.GetContext(item, false), xItem));
			}

			result.ChildResults = childResults;

			return result;
		}

		public override TextElement GetContainer(XElement forElement, TextElementType forType, TextElement fromElement)
		{
			List list = (List) fromElement;

			switch (forType)
			{
				case TextElementType.ListItem:
					return list;
				default:
					return base.GetContainer(forElement, forType, fromElement);
			}
		}
		#endregion
	}
}*/