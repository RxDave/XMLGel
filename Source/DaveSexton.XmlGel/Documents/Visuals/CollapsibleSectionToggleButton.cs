using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DaveSexton.XmlGel.Documents.Visuals
{
	/* This class must be public otherwise an exception may be thrown by the RichTextBox when a section 
	 * that has an instance of the class in a BlockUIContainer is deleted.  (This is an old comment from when
	 * I was developing the editor in .NET 3.5, but it may still apply in .NET 4.0 so I'm leaving it.)
	 */
	public sealed class CollapsibleSectionToggleButton : Button
	{
		public static readonly DependencyProperty CollapsibleSectionProperty = DependencyProperty.Register("CollapsibleSection", typeof(CollapsibleSection), typeof(CollapsibleSectionToggleButton), new FrameworkPropertyMetadata(CollapsibleSectionChanged));

		public CollapsibleSection CollapsibleSection
		{
			get
			{
				return (CollapsibleSection) GetValue(CollapsibleSectionProperty);
			}
			set
			{
				SetValue(CollapsibleSectionProperty, value);
			}
		}

		private ImageSource Collapse
		{
			get
			{
				// Delay loading is required because FindResource returns null until the control is parented.
				if (collapse == null)
				{
					collapse = (ImageSource) FindResource(new ComponentResourceKey(typeof(GenericKey), "Collapse"));
				}

				return collapse;
			}
		}

		private ImageSource Expand
		{
			get
			{
				// Delay loading is required because FindResource returns null until the control is parented.
				if (expand == null)
				{
					expand = (ImageSource) FindResource(new ComponentResourceKey(typeof(GenericKey), "Expand"));
				}

				return expand;
			}
		}

		private readonly Image image = new Image();
		private ImageSource collapse, expand;

		public CollapsibleSectionToggleButton()
		{
			Content = image;
		}

		private void RegisterButton(CollapsibleSection section, CollapsibleSection oldSection)
		{
			if (oldSection != null)
			{
				oldSection.UnregisterToggleButton(this);
			}

			if (section != null)
			{
				section.RegisterToggleButton(this);

				UpdateImage(section);
			}
		}

		private void UpdateImage(CollapsibleSection section)
		{
			image.Source = section.IsExpanded ? Collapse : Expand;
		}

		internal void Toggle(bool toggleSection)
		{
			if (toggleSection)
			{
				CollapsibleSection.Toggle();
			}
			else
			{
				UpdateImage(CollapsibleSection);
			}
		}

		protected override void OnClick()
		{
			Toggle(toggleSection: true);

			base.OnClick();
		}

		private static void CollapsibleSectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			((CollapsibleSectionToggleButton) sender).RegisterButton((CollapsibleSection) e.NewValue, (CollapsibleSection) e.OldValue);
		}
	}
}