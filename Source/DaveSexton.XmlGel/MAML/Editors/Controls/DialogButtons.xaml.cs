using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DaveSexton.XmlGel.Maml.Editors.Controls
{
	/// <summary>
	/// Interaction logic for DialogButtons.xaml
	/// </summary>
	public sealed partial class DialogButtons : UserControl
	{
		public static readonly DependencyProperty OkButtonContentProperty = DependencyProperty.Register("OkButtonContent", typeof(object), typeof(DialogButtons), new FrameworkPropertyMetadata("OK"));
		public static readonly DependencyProperty CancelButtonContentProperty = DependencyProperty.Register("CancelButtonContent", typeof(object), typeof(DialogButtons), new FrameworkPropertyMetadata("Cancel"));
		public static readonly DependencyProperty OkButtonVisibilityProperty = DependencyProperty.Register("OkButtonVisibility", typeof(object), typeof(DialogButtons), new FrameworkPropertyMetadata(Visibility.Visible));
		public static readonly DependencyProperty CancelButtonVisibilityProperty = DependencyProperty.Register("CancelButtonVisibility", typeof(object), typeof(DialogButtons), new FrameworkPropertyMetadata(Visibility.Visible));

		public Visibility CancelButtonVisibility
		{
			get
			{
				return (Visibility) GetValue(CancelButtonVisibilityProperty);
			}
			set
			{
				SetValue(CancelButtonVisibilityProperty, value);
			}
		}

		public Visibility OkButtonVisibility
		{
			get
			{
				return (Visibility) GetValue(OkButtonVisibilityProperty);
			}
			set
			{
				SetValue(OkButtonVisibilityProperty, value);
			}
		}

		public object CancelButtonContent
		{
			get
			{
				return GetValue(CancelButtonContentProperty);
			}
			set
			{
				SetValue(CancelButtonContentProperty, value);
			}
		}

		public object OkButtonContent
		{
			get
			{
				return GetValue(OkButtonContentProperty);
			}
			set
			{
				SetValue(OkButtonContentProperty, value);
			}
		}

		public DialogButtons()
		{
			InitializeComponent();

			CommandBindings.Add(new CommandBinding(DialogCommands.Ok,
				(sender, re) =>
				{
					re.Handled = true;

					OnOkClick();
				}));

			CommandBindings.Add(new CommandBinding(DialogCommands.Cancel,
				(sender, re) =>
				{
					re.Handled = true;

					OnCancelClick();
				}));
		}

		public static readonly RoutedEvent OkClickEvent = EventManager.RegisterRoutedEvent("OkClick",
			RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DialogButtons));

		public static readonly RoutedEvent CancelClickEvent = EventManager.RegisterRoutedEvent("CancelClick",
			RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DialogButtons));

		[Category("Action")]
		[Description("Event raised when the Cancel button is clicked.")]
		public event RoutedEventHandler CancelClick
		{
			add
			{
				AddHandler(CancelClickEvent, value);
			}
			remove
			{
				RemoveHandler(CancelClickEvent, value);
			}
		}

		private void OnCancelClick()
		{
			RaiseEvent(new RoutedEventArgs(CancelClickEvent, this));
		}

		[Category("Action")]
		[Description("Event raised when the OK button is clicked.")]
		public event RoutedEventHandler OkClick
		{
			add
			{
				AddHandler(OkClickEvent, value);
			}
			remove
			{
				RemoveHandler(OkClickEvent, value);
			}
		}

		private void OnOkClick()
		{
			RaiseEvent(new RoutedEventArgs(OkClickEvent, this));
		}
	}
}