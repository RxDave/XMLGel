using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using DaveSexton.XmlGel.Documents.Visuals;

namespace DaveSexton.XmlGel.Documents
{
	public sealed class CollapsibleSection : HeaderedSection
	{
		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(CollapsibleSection), new FrameworkPropertyMetadata(TitleChanged));
		public static readonly DependencyProperty TitleIsReadOnlyProperty = DependencyProperty.Register("TitleIsReadOnly", typeof(bool), typeof(CollapsibleSection));

		public string Title
		{
			get
			{
				return (string) GetValue(TitleProperty);
			}
			set
			{
				SetValue(TitleProperty, value);
			}
		}

		public bool TitleIsReadOnly
		{
			get
			{
				return (bool?) GetValue(TitleIsReadOnlyProperty) ?? false;
			}
			set
			{
				SetValue(TitleIsReadOnlyProperty, value);
			}
		}

		public bool IsExpanded
		{
			get
			{
				return isExpanded;
			}
		}

		public override ICollection<Block> Content
		{
			get
			{
				return hiddenContent ?? base.Content;
			}
		}

		private CollapsibleSectionToggleButton registeredButton;
		private ICollection<Block> hiddenContent;
		private bool isExpanded = true;

		public CollapsibleSection()
			: base()
		{
		}

		public CollapsibleSection(Block content)
			: base(content)
		{
		}

		protected override object CreateDefaultHeaderContent()
		{
			return new CollapsibleSectionHeaderContentItem(this);
		}

		public void Toggle()
		{
			isExpanded = !isExpanded;

			if (isExpanded)
			{
				Contract.Assume(hiddenContent != null);

				BaseBlocks.AddRange(hiddenContent);
			}
			else
			{
				hiddenContent = base.Content.ToList().AsReadOnly();

				/* Removing blocks causes the TextChanged event to be raised, which causes the CollapsibleSection.Content property
				 * to be read.  That's why the Content property has already been set before removing the blocks from the section.
				 */
				foreach (var block in hiddenContent)
				{
					base.Content.Remove(block);
				}
			}

			if (registeredButton != null)
			{
				registeredButton.Toggle(toggleSection: false);
			}
		}

		internal void RegisterToggleButton(CollapsibleSectionToggleButton button)
		{
			registeredButton = button;
		}

		internal void UnregisterToggleButton(CollapsibleSectionToggleButton button)
		{
			if (registeredButton == button)
			{
				registeredButton = null;
			}
		}

		private static void TitleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			((CollapsibleSection) sender).OnValueChanged(new RoutedEventArgs());
		}
	}
}