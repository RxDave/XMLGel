using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace DaveSexton.XmlGel.Documents
{
	public class HeaderedSection : Section, IUIElementContainer
	{
		public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(HeaderedSection), new FrameworkPropertyMetadata(HeaderTemplateChanged));
		public static readonly DependencyProperty HeaderContentProperty = DependencyProperty.Register("HeaderContent", typeof(object), typeof(HeaderedSection), new FrameworkPropertyMetadata(HeaderContentChanged));

		public DataTemplate HeaderTemplate
		{
			get
			{
				return (DataTemplate) GetValue(HeaderTemplateProperty);
			}
			set
			{
				SetValue(HeaderTemplateProperty, value);
			}
		}

		public object HeaderContent
		{
			get
			{
				return GetValue(HeaderContentProperty);
			}
			set
			{
				SetValue(HeaderContentProperty, value);
			}
		}

		public virtual Block Header
		{
			get
			{
				return header;
			}
			set
			{
				HeaderContent = null;
				HeaderTemplate = null;

				SetHeader(value);
			}
		}

		public virtual ICollection<Block> Content
		{
			get
			{
				return content;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public new BlockCollection Blocks
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected internal BlockCollection BaseBlocks
		{
			get
			{
				return base.Blocks;
			}
		}

		private readonly ICollection<Block> content;
		private Block header;
		private ContentControl headerContainer;

		public HeaderedSection()
			: base()
		{
			this.content = new ContentBlockCollection(this);
		}

		public HeaderedSection(Block content)
			: base(content)
		{
			this.content = new ContentBlockCollection(this);
		}

		public HeaderedSection(Block header, Block content)
			: base(content)
		{
			this.Header = header;
			this.content = new ContentBlockCollection(this);
		}

		private void SetHeader(Block value)
		{
			if (header != value)
			{
				if (header != null)
				{
					base.Blocks.Remove(header);
				}

				header = value;

				if (header != null)
				{
					if (base.Blocks.Count == 0)
					{
						base.Blocks.Add(header);
					}
					else
					{
						base.Blocks.InsertBefore(base.Blocks.FirstBlock, header);
					}
				}
			}
		}

		private void SetHeaderTemplate(DataTemplate template)
		{
			if (template != null || headerContainer != null)
			{
				if (template == null)
				{
					if (headerContainer.Content == null)
					{
						headerContainer = null;
						SetHeader(null);
					}
					else
					{
						headerContainer.ContentTemplate = null;
					}
				}
				else
				{
					EnsureHeaderContainer();

					if (headerContainer.Content == null)
					{
						headerContainer.Content = CreateDefaultHeaderContent();
					}

					headerContainer.ContentTemplate = template;
				}
			}
		}

		private void SetHeaderContent(object content)
		{
			if (content != null || headerContainer != null)
			{
				if (content == null)
				{
					if (headerContainer.ContentTemplate == null)
					{
						headerContainer = null;
						SetHeader(null);
					}
					else
					{
						headerContainer.Content = null;
					}
				}
				else
				{
					EnsureHeaderContainer();

					headerContainer.Content = content;
				}
			}
		}

		private void EnsureHeaderContainer()
		{
			if (headerContainer == null)
			{
				headerContainer = new ContentControl();

				headerContainer.CommandBindings.Add(new CommandBinding(FlowDocumentCommands.UIElementChanged, (sender, e) => OnValueChanged(e)));

				SetHeader(new BlockUIContainer(headerContainer));
			}
		}

		protected virtual object CreateDefaultHeaderContent()
		{
			return null;
		}

		private static void HeaderTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			((HeaderedSection) sender).SetHeaderTemplate((DataTemplate) e.NewValue);
		}

		private static void HeaderContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			((HeaderedSection) sender).SetHeaderContent(e.NewValue);
		}

		public event RoutedEventHandler ValueChanged;

		protected virtual void OnValueChanged(RoutedEventArgs e)
		{
			var handler = ValueChanged;

			if (handler != null)
			{
				handler(this, e);
			}
		}

		private sealed class ContentBlockCollection : ICollection<Block>
		{
			public int Count
			{
				get
				{
					return section.BaseBlocks.Count - (section.header == null ? 0 : 1);
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return section.BaseBlocks.IsReadOnly;
				}
			}

			private IEnumerable<Block> Content
			{
				get
				{
					return section.header == null
						? section.BaseBlocks
						: section.BaseBlocks.Skip(1);
				}
			}

			private readonly HeaderedSection section;

			public ContentBlockCollection(HeaderedSection section)
			{
				this.section = section;
			}

			public void Add(Block item)
			{
				section.BaseBlocks.Add(item);
			}

			public void Clear()
			{
				section.BaseBlocks.Clear();

				if (section.header != null)
				{
					section.BaseBlocks.Add(section.header);
				}
			}

			public bool Contains(Block item)
			{
				return Content.Contains(item);
			}

			public void CopyTo(Block[] array, int arrayIndex)
			{
				foreach (var block in Content)
				{
					array[arrayIndex++] = block;
				}
			}

			public bool Remove(Block item)
			{
				return section.header != item && section.BaseBlocks.Remove(item);
			}

			public IEnumerator<Block> GetEnumerator()
			{
				return Content.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return Content.GetEnumerator();
			}
		}
	}
}