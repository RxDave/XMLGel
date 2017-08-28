using System;
using System.Windows.Documents;
using System.Xml.Linq;
using DaveSexton.XmlGel.Maml.Documents.Visitors;

namespace DaveSexton.XmlGel.Maml.Documents
{
	/* externalLink (inlineLinking.xsd)
	 * 
	 * Attributes: (linkingGroup - base.xsd)
	 *	- xlink:href (URI)
	 *	
	 * Attributes: (contentIdentificationSharingAndConditionGroup - base.xsd)
	 *	- address (ID : NCName)
	 *	
	 * Sequence:
	 *	- linkText (MamlText)
	 *	- linkAlternateText (MamlText) (0..1)
	 *	- linkUri (MamlUri)
	 *	- linkTarget (MamlText) (0..1)	(Sandcastle Styles extension)
	 */
	internal sealed class MamlExternalLink : MamlNode
	{
		public Uri Href
		{
			get
			{
				var url = (string) Element.Attribute(Maml.XLinkHref);

				Uri uri;
				if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
				{
					return uri;
				}
				else
				{
					return null;
				}
			}
		}

		public MamlExternalLink(XElement element)
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
		#region Constructors
		/// <summary>
		/// Constructs a new instance of the <see cref="MamlLink" /> class.
		/// </summary>
		public MamlLink(ParserFactory factory, System.Windows.ResourceDictionary resources, TextElementStyleManager styleManager, TextElementFactory textElementFactory)
			: base(factory, resources, styleManager, textElementFactory)
		{
		}
		#endregion

		#region Methods
		private static MamlLinkData GetLink(ParserContext context)
		{
			MamlHyperlink hyperLink = (MamlHyperlink) context.Element;
			return hyperLink.Link;
		}

		public override ParseResult Parse(XElement element, BlockCollection rootBlocks)
		{
			Action assignParagraphStyle;
			Paragraph paragraph = GetOrCreateParagraph(element, element.Name.LocalName, out assignParagraphStyle);

			if (paragraph == null)
				throw NewXmlException(element, "Link-type elements may only be children of block or block-inline elements.");

			Action assignStyle;
			var block = new TextBlock()
			{
				Text = ParseText(element, false, "link", out assignStyle).Text
			};

			MamlLinkData data = MamlLinkData.Parse(element);
			MamlHyperlink link = new MamlHyperlink(data, block);

			Action assignLinkStyle;
			TextElementFactory.InitializeTextElement(this, link, element.Name.LocalName, element, out assignLinkStyle);

			SetTextElement(element, link);

			paragraph.Inlines.Add(link);

			return new ParseResult(link, true, () =>
			{
				if (assignParagraphStyle != null)
					assignParagraphStyle();

				assignStyle();
				assignLinkStyle();
			});
		}

		public override UnparseResult Unparse(ParserContext context, XContainer parent)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			MamlLinkData link = GetLink(context);
			XElement element = null;

			if (context.HasDefaultValue)
			{
				switch (link.LinkType)
				{
					case MamlLinkType.XLink:
						element = Maml.XLink;
						break;
					case MamlLinkType.CodeEntityReference:
						element = Maml.XCodeEntityReference;
						break;
					case MamlLinkType.ExternalLink:
						element = Maml.XExternalLink;
						break;
				}
			}
			else
				element = link.ToElement();

			parent.Add(element);

			return new UnparseResult(element, true);
		}
		#endregion

		#region Nested
		private sealed class MamlHyperlink : Hyperlink
		{
			#region Public Properties
			public MamlLinkData Link
			{
				get
				{
					return link;
				}
			}
			#endregion

			#region Private / Protected
			private readonly MamlLinkData link;
			private readonly TextBlock block;
			#endregion

			#region Constructors
			/// <summary>
			/// Constructs a new instance of the <see cref="MamlHyperlink" /> class.
			/// </summary>
			public MamlHyperlink(MamlLinkData link, TextBlock block)
			{
				this.link = link;
				this.block = block;

				link.PropertyChanged += link_PropertyChanged;

				Command = LinkCommand.Instance;
				CommandParameter = link;

				Inlines.Add(block);
			}
			#endregion

			#region Methods
			private void UpdateText()
			{
				block.Text = link.Value;
			}

			private void UpdateToolTip()
			{
				StringBuilder value = new StringBuilder(64);

				if (link.LinkType == MamlLinkType.XLink)
				{
					value.Append('{');

					if (link.QualifyHint ?? false)
						value.Append("Qualified");

					if (link.AutoUpgrade ?? false)
					{
						if (value.Length > 1)
							value.Append("; ");

						value.Append("Upgraded");
					}

					if (value.Length == 1)
						value.Remove(0, 1);
					else
						value.Append("} ");
				}

				value.Append(link.Value);

				block.ToolTip = value.ToString();
			}
			#endregion

			#region Event Handlers
			private void link_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
			{
				switch (e.PropertyName)
				{
					case "Text":
						UpdateText();
						break;
					case "DocumentId":
					case "EntityId":
					case "Uri":
						UpdateText();
						UpdateToolTip();
						break;
					case "LinkType":
					case "QualifyHint":
					case "AutoUpgrade":
						UpdateToolTip();
						break;
				}
			}

			protected override void OnMouseDown(MouseButtonEventArgs e)
			{
				// NOTE: The other mouse events (e.g., MouseUp) are suppressed by the Hyperlink class,
				// although for some unknown reason a breakpoint in their handlers can be hit if a breakpoint in 
				// this handler is hit first.  But without the debugger attached these events are never raised.
				// Therefore, MouseDown is handled to execute the link command.

				if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
				// The command will not be invoked by a single click unless Ctrl is pressed, so invoke it now.
				{
					Command.Execute(CommandParameter);
				}

				base.OnMouseDown(e);
			}
			#endregion
		}

		private sealed class LinkCommand : ICommand
		{
			#region Public Properties
			public static readonly LinkCommand Instance = new LinkCommand();
			#endregion

			#region Private / Protected
			#endregion

			#region Constructors
			/// <summary>
			/// Constructs a new instance of the <see cref="LinkCommand" /> class.
			/// </summary>
			private LinkCommand()
			{
			}
			#endregion

			#region ICommand Members
			public bool CanExecute(object parameter)
			{
				return true;
			}

			public void Execute(object parameter)
			{
				Visuals.LinkEditorWindow editor = new Visuals.LinkEditorWindow((MamlLinkData) parameter);
				editor.Owner = System.Windows.Application.Current.MainWindow;
				editor.ShowDialog();
			}

			public event EventHandler CanExecuteChanged;

			private void OnCanExecuteChanged(EventArgs e)
			{
				if (CanExecuteChanged != null)
					CanExecuteChanged(this, e);
			}
			#endregion
		}
		#endregion
	}
}*/