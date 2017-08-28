using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Windows.Documents;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using DaveSexton.XmlGel.Documents;
using DaveSexton.XmlGel.Extensions;
using DaveSexton.XmlGel.Xml;

namespace DaveSexton.XmlGel
{
	public abstract class XContainerToFlowDocumentVisitor<TSelf> : XVisitor
		where TSelf : XContainerToFlowDocumentVisitor<TSelf>
	{
		public FlowDocument Document
		{
			get
			{
				return document;
			}
		}

		protected IXNode CurrentNode
		{
			get
			{
				return elements.Count > 0 ? elements.Peek().Key : null;
			}
		}

		protected TextElement CurrentElement
		{
			get
			{
				return elements.Count > 0 ? elements.Peek().Value : null;
			}
		}

		protected Run CurrentRun
		{
			get
			{
				return CurrentElement as Run;
			}
		}

		protected TextElementContainerItemKind CurrentContainerItemKind
		{
			get
			{
				if (elements.Count == 0)
				{
					// FlowDocument is a block container
					return TextElementContainerItemKind.Block;
				}

				var current = elements.Peek().Value;

				TextElementContainerItemKind kind;

				current.TryGetChildCollection(out kind);

				return kind;
			}
		}

		protected bool TrimParagraphsDefault
		{
			get;
			set;
		}

		protected bool NormalizeTextDefault
		{
			get;
			set;
		}

		protected bool TrimParagraphs
		{
			get
			{
				return trimParagraphs ?? TrimParagraphsDefault;
			}
			private set
			{
				trimParagraphs = value;
			}
		}

		protected bool NormalizeText
		{
			get
			{
				return normalizeText ?? NormalizeTextDefault;
			}
			private set
			{
				normalizeText = value;
			}
		}

		internal XmlSchemaSet Schemas
		{
			get
			{
				return schemas;
			}
		}

		private static readonly IList<XmlSchemaParticlesExpected> noParticlesExpected = new List<XmlSchemaParticlesExpected>(0).AsReadOnly();
		private readonly XmlSchemaSet schemas;
		private readonly XmlResolver resolver;
		private readonly Stack<KeyValuePair<IXNode, TextElement>> elements = new Stack<KeyValuePair<IXNode, TextElement>>();
		private readonly Action uiContainerChanged;
		private bool visiting;
		private bool? trimParagraphs, normalizeText;
		private FlowDocument document;

		protected XContainerToFlowDocumentVisitor(XContainer container, Action uiContainerChanged, XmlResolver resolver, params XmlSchema[] schemas)
			: this(container, uiContainerChanged, resolver, (IEnumerable<XmlSchema>) schemas)
		{
		}

		protected XContainerToFlowDocumentVisitor(XContainer container, Action uiContainerChanged, XmlResolver resolver, IEnumerable<XmlSchema> schemas)
			: base(container)
		{
			this.uiContainerChanged = uiContainerChanged;
			this.resolver = resolver;

			TrimParagraphsDefault = true;
			NormalizeTextDefault = true;

			var table = new NameTable();

			this.schemas = new XmlSchemaSet(table)
			{
				XmlResolver = resolver
			};

			if (schemas != null)
			{
				foreach (var schema in schemas)
				{
					this.schemas.Add(schema);
				}
			}
		}

		internal bool ValidateChildren(XElement element, XmlSchemaElement schema)
		{
			return schema.ValidateChildren(schemas, element);
		}

		internal bool GetExpectedElements(
			XElement targetElement,
			XElement containerElement,
			XmlSchemaElement containerSchema,
			out XmlSchemaParticlesExpected expectedBeforeChild,
			out XmlSchemaParticlesExpected expectedAfterChild)
		{
			Contract.Requires(containerElement != null);
			Contract.Requires(containerSchema != null);

			var isValid = containerSchema.ValidateChildren(schemas, containerElement, out expectedBeforeChild, out expectedAfterChild, targetElement);

			Contract.Assume(expectedBeforeChild != null);
			Contract.Assume(expectedAfterChild != null);

			return isValid;
		}

		private void EnsureValid()
		{
			var validator = new XmlFragmentSchemaValidator(resolver, CreateDefaultElement, IsInlineType, GetParagraph, this.schemas);

			validator.InvalidAttribute += OnInvalidAttribute;
			validator.InvalidNode += OnInvalidNode;

			validator.EnsureValid(Container, GetDocumentRootSchema);
		}

		protected void UIElementContainerChanged()
		{
			uiContainerChanged();
		}

		private XmlSchemaElement GetDocumentRootSchema(XElement element)
		{
			return IsDocumentRootElement(element)
					 ? (XmlSchemaElement) schemas.GlobalElements[new XmlQualifiedName(element.Name.LocalName, element.Name.NamespaceName)]
					 : null;
		}

		protected abstract bool IsDocumentRootElement(XElement element);

		protected abstract bool IsInlineType(XElement element);

		protected abstract XmlSchemaElement GetParagraph(IEnumerable<XmlSchemaElement> elements);

		protected abstract IXElementNode<TSelf> CreateElementNode(XmlSchemaElement schema);

		protected abstract IXElementNode<TSelf> CreateElementNode(XElement element);

		public IXElementNode<TSelf> CreateElementNode(XmlSchemaParticle particle)
		{
			if (particle is XmlSchemaAny)
			{
				return CreateElementNode(CreateAny());
			}
			else
			{
				return CreateElementNode((XmlSchemaElement) particle);
			}
		}

		private XElement CreateDefaultElement(XmlSchemaElement schema)
		{
			return CreateElementNode(new XElement(schema.GetXName())).CreateDefault(schema);
		}

		public TextElement CreateElement(IXElementNode<TSelf> node, XmlSchemaElement schema, TextElement container, IXNode containerNode)
		{
			Contract.Assert(elements.Count == 0);
			Contract.Assert(!visiting);

			var element = node.CreateDefault(schema) ?? schema.CreateRequiredElementTree(schemas);

			node = CreateElementNode(element);

			if (container != null)
			{
				elements.Push(new KeyValuePair<IXNode, TextElement>(containerNode, container));
			}

			visiting = true;

			try
			{
				var newElement = Visit(node, element, append: false);

				Contract.Assume(newElement != null);

				return newElement;
			}
			finally
			{
				if (container != null)
				{
					elements.Pop();
				}

				Contract.Assert(elements.Count == 0);

				visiting = false;
			}
		}

		private TextElement Accept(IXElementNode<TSelf> node, XElement element, out TextElement contentContainer)
		{
			var newElement = node.Accept((TSelf) this, out contentContainer);

			AttachToIUIElementContainer(newElement);
			AttachToIUIElementContainer(contentContainer);

			if (element.IsSchemaError())
			{
				newElement = CreateError(newElement);
			}

			return newElement;
		}

		public override void Visit()
		{
			Contract.Assert(elements.Count == 0);
			Contract.Assert(!visiting);

			visiting = true;

			try
			{
				EnsureValid();

				document = new FlowDocument();
				document.Tag = Container;

				base.Visit();
			}
			finally
			{
				visiting = false;
			}

			Contract.Assert(elements.Count == 0);
		}

		public override void Visit(XVisitableElement element)
		{
			var node = CreateElementNode(element.Element);

			if (node != null)
			{
				Visit(node, element.Element);
			}
		}

		private TextElement Visit(IXElementNode<TSelf> node, XElement element, bool append = true)
		{
			TextElement contentContainer;
			var newElement = Accept(node, element, out contentContainer);

			try
			{
				if (append && newElement != null)
				{
					Append(newElement);
				}

				if (contentContainer != null)
				{
					VisitChildrenInContext(node, contentContainer);
				}
			}
			catch (XmlException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw element.CreateXmlException("Failed to parse element \"" + element.Name + "\".", ex);
			}

			return newElement;
		}

		public override void Visit(XVisitableText text)
		{
			var value = text.Value;

			if (!string.IsNullOrEmpty(value))
			{
				var options = CurrentElement as Options;

				if (options != null)
				{
					options.SelectValueFromText(value);
				}
				else if (NormalizeText)
				{
					foreach (var run in RunNormalization.Normalize(value))
					{
						Append(run);
					}
				}
				else
				{
					var currentRun = CurrentRun;

					if (currentRun != null)
					{
						currentRun.Text += value;
					}
					else
					{
						Append(CreateRun(value));
					}
				}
			}
		}

		public override void Visit(XVisitableComment comment)
		{
			Append(new CommentInline(comment.Text));
		}

		public override void Visit(XVisitableCData cData)
		{
			Append(new Literal(cData.Value));
		}

		private TextElement CreateError(TextElement newElement)
		{
			if (newElement == null)
			{
				switch (CurrentContainerItemKind)
				{
					case TextElementContainerItemKind.Block:
						return new ErrorBlock();
					case TextElementContainerItemKind.ListItem:
						return new ErrorInline().ToListItem();
					case TextElementContainerItemKind.TableRowGroup:
						return new ErrorBlock().ToTableRowGroup();
					case TextElementContainerItemKind.TableRow:
						return new ErrorBlock().ToTableRow();
					case TextElementContainerItemKind.TableCell:
						return new ErrorBlock().ToTableCell();
					default:
						return new ErrorInline();
				}
			}
			else
			{
				switch (CurrentContainerItemKind)
				{
					case TextElementContainerItemKind.Block:
						return new ErrorBlock(newElement.ToBlock());
					case TextElementContainerItemKind.ListItem:
						return new ErrorInline(newElement.ToInline()).ToListItem();
					case TextElementContainerItemKind.TableRowGroup:
						return new ErrorBlock(newElement.ToBlock()).ToTableRowGroup();
					case TextElementContainerItemKind.TableRow:
						return new ErrorBlock(newElement.ToBlock()).ToTableRow();
					case TextElementContainerItemKind.TableCell:
						return new ErrorBlock(newElement.ToBlock()).ToTableCell();
					default:
						return new ErrorInline(newElement.ToInline());
				}
			}
		}

		private void AttachToIUIElementContainer(TextElement element)
		{
			var uiElementContainer = element as IUIElementContainer;

			if (uiElementContainer != null)
			{
				uiElementContainer.ValueChanged += (_, __) => UIElementContainerChanged();
			}
		}

		private void VisitChildrenInContext(IXNode node, TextElement contentContainer)
		{
			elements.Push(new KeyValuePair<IXNode, TextElement>(node, contentContainer));

			var previousNormalizeText = NormalizeText;
			var previousTrimParagraphs = TrimParagraphs;

			NormalizeText = node.NormalizeText ?? previousNormalizeText;
			TrimParagraphs = node.TrimParagraphs ?? previousTrimParagraphs;

			VisitChildren(node);

			if (elements.Count == 1)
			{
				normalizeText = null;
				trimParagraphs = null;
			}
			else
			{
				NormalizeText = previousNormalizeText;
				TrimParagraphs = previousTrimParagraphs;
			}

			var popped = elements.Pop().Value;

			if (TrimParagraphs)
			{
				RunNormalization.EnsureLastRunIsTrimmed(popped as Paragraph);
			}

			Contract.Assert(popped == contentContainer);
		}

		private Run CreateRun(string value)
		{
			Contract.Requires(!string.IsNullOrEmpty(value));

			if (TrimParagraphs)
			{
				var paragraph = CurrentElement as Paragraph;

				if (paragraph != null && paragraph.Inlines.Count == 0)
				{
					return RunNormalization.TrimStart(value);
				}
			}

			return new Run(value);
		}

		private void Append(TextElement element)
		{
			var block = element as Block;

			if (block != null)
			{
				Append(block);
			}
			else
			{
				var inline = element as Inline;

				if (inline != null)
				{
					Append(inline);
				}
				else
				{
					var listItem = element as ListItem;

					if (listItem != null)
					{
						Append(listItem);
					}
					else
					{
						var rowGroup = element as TableRowGroup;

						if (rowGroup != null)
						{
							Append(rowGroup);
						}
						else
						{
							var row = element as TableRow;

							if (row != null)
							{
								Append(row);
							}
							else
							{
								var cell = element as TableCell;

								if (cell != null)
								{
									Append(cell);
								}
								else
								{
									throw new InvalidOperationException("Unsupported element type \"" + element + "\".");
								}
							}
						}
					}
				}
			}
		}

		private void Append(Block block)
		{
			if (elements.Count == 0)
			{
				document.Blocks.Add(block);
				return;
			}

			var current = elements.Peek().Value;

			var section = current as Section;

			if (section != null)
			{
				section.Blocks.Add(block);
			}
			else
			{
				var anchored = current as AnchoredBlock;

				if (anchored != null)
				{
					anchored.Blocks.Add(block);
				}
				else
				{
					var listItem = current as ListItem;

					if (listItem != null)
					{
						listItem.Blocks.Add(block);
					}
					else
					{
						var tableCell = current as TableCell;

						if (tableCell != null)
						{
							tableCell.Blocks.Add(block);
						}
						else
						{
							var comment = block as CommentBlock;

							if (comment != null)
							{
								AppendToSpecialContainer(comment);
							}
							else
							{
								ThrowInvalidContainer(current, block);
							}
						}
					}
				}
			}
		}

		private void Append(Inline inline)
		{
			if (elements.Count == 0)
			{
				if (inline is CommentInline)
				{
					Append(new CommentBlock(inline));
				}
				else
				{
					ThrowInvalidContainer(null, inline);
				}
			}
			else
			{
				var current = elements.Peek().Value;

				var paragraph = current as Paragraph;

				if (paragraph != null)
				{
					paragraph.Inlines.Add(inline);
				}
				else
				{
					var span = current as Span;

					if (span != null)
					{
						span.Inlines.Add(inline);
					}
					else
					{
						if (inline is CommentInline)
						{
							Append(new CommentBlock(inline));
						}
						else
						{
							ThrowInvalidContainer(current, inline);
						}
					}
				}
			}
		}

		private void Append(ListItem listItem)
		{
			Contract.Assert(elements.Count > 0);

			var current = elements.Peek().Value;

			var list = current as List;

			if (list != null)
			{
				list.ListItems.Add(listItem);
			}
			else
			{
				ThrowInvalidContainer(current, listItem);
			}
		}

		private void Append(TableRowGroup rowGroup)
		{
			Contract.Assert(elements.Count > 0);

			var current = elements.Peek().Value;

			var table = current as Table;

			if (table != null)
			{
				table.RowGroups.Add(rowGroup);
			}
			else
			{
				ThrowInvalidContainer(current, rowGroup);
			}
		}

		private void Append(TableRow row)
		{
			Contract.Assert(elements.Count > 0);

			var current = elements.Peek().Value;

			var rowGroup = current as TableRowGroup;

			if (rowGroup != null)
			{
				rowGroup.Rows.Add(row);
			}
			else
			{
				ThrowInvalidContainer(current, row);
			}
		}

		private void Append(TableCell cell)
		{
			Contract.Assert(elements.Count > 0);

			var current = elements.Peek().Value;

			var row = current as TableRow;

			if (row != null)
			{
				row.Cells.Add(cell);
			}
			else
			{
				ThrowInvalidContainer(current, cell);
			}
		}

		private void AppendToSpecialContainer(CommentBlock comment)
		{
			var current = elements.Peek().Value;

			var list = current as List;

			if (list != null)
			{
				list.ListItems.Add(new ListItem(comment));
			}
			else
			{
				var table = current as Table;

				if (table != null)
				{
					table.RowGroups.Add(new TableRowGroup()
						{
							Rows =
							{
								new TableRow()
								{
									Cells = 
									{
										new TableCell(comment)
										{
											ColumnSpan = table.Columns.Count
										}
									}
								}
							}
						});
				}
				else
				{
					var group = current as TableRowGroup;

					if (group != null)
					{
						group.Rows.Add(
							new TableRow()
							{
								Cells = 
								{
									new TableCell(comment)
									{
										ColumnSpan = ((Table) group.Parent).Columns.Count
									}
								}
							});
					}
					else
					{
						var row = current as TableRow;

						if (row != null)
						{
							row.Cells.Add(new TableCell(comment));
						}
						else
						{
							throw new InvalidOperationException("Current element type \"" + current + "\" is not supported.");
						}
					}
				}
			}
		}

		private void ThrowInvalidContainer(TextElement current, TextElement child)
		{
			var run = child as Run;
			var runValue = run == null ? child.Tag : run.Text.Substring(0, Math.Min(15, run.Text.Length)) + "...";

			if (current == null)
			{
				throw new InvalidOperationException(string.Format(
					CultureInfo.CurrentCulture,
					"The document root cannot contain {0} element \"{1}\".",
					child.GetType().Name,
					runValue));
			}
			else
			{
				var currentRun = current as Run;
				var currentRunValue = currentRun == null ? current.Tag : currentRun.Text.Substring(0, Math.Min(15, currentRun.Text.Length)) + "...";

				throw new InvalidOperationException(string.Format(
					CultureInfo.CurrentCulture,
					"The current {0} element \"{1}\" cannot contain {2} element \"{3}\".",
					current.GetType().Name,
					currentRunValue,
					child.GetType().Name,
					runValue));
			}
		}

		public event Action<XAttribute> InvalidAttribute;
		public event Action<XNode> InvalidNode;

		private void OnInvalidAttribute(XAttribute attribute)
		{
			var handler = InvalidAttribute;

			if (handler != null)
			{
				handler(attribute);
			}
		}

		private void OnInvalidNode(XNode node)
		{
			var handler = InvalidNode;

			if (handler != null)
			{
				handler(node);
			}
		}
	}
}