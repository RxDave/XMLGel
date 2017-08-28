using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using DaveSexton.XmlGel.Maml;
using Microsoft.Win32;

namespace DaveSexton.XmlGel.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const string dialogDefaultFolder = @"C:\Users\Dave\Documents\SandcastleMAMLGuide\Content\";

		private MamlDocument document;
		private bool loadingDocument;
		private string file;

		public MainWindow()
		{
			ApplicationCommands.New.Subscribe(typeof(MainWindow), _ => New());
			ApplicationCommands.Open.Subscribe(typeof(MainWindow), _ => Open());
			ApplicationCommands.Save.Subscribe(typeof(MainWindow), _ => Save());
			ApplicationCommands.SaveAs.Subscribe(typeof(MainWindow), _ => SaveAs());

			InitializeComponent();
		}

		private void UpdateTools()
		{
			switch (document.Kind)
			{
				case MamlDocumentKind.Conceptual:
					break;
			}
		}

		private void UpdateOutput()
		{
			if (!loadingDocument)
			{
				document.Update();

				xhtmlTextBox.Text = document.Document.ToString();
			}
		}

		private void LoadDocument(MamlDocument newDocument)
		{
			loadingDocument = true;

			try
			{
				richTextBox.Document = newDocument.GenerateFlowDocument(UpdateOutput);

				document = newDocument;
			}
			catch (XmlException ex)
			{
				// TODO: Log error

				MessageBox.Show(this, ex.Message, "Invalid File", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			finally
			{
				loadingDocument = false;
			}

			UpdateTools();
			UpdateOutput();
		}

		private void New()
		{
			file = null;

			LoadDocument(MamlDocument.Create(MamlDocumentKind.Conceptual));
		}

		private void Open()
		{
			var dialog = new OpenFileDialog()
			{
				CheckFileExists = true,
				AddExtension = true,
				DefaultExt = ".aml",
				Filter = "MAML Files (*.aml;*.maml)|*.aml;*.maml|All Files (*.*)|*.*",
				Title = "Open File",
				InitialDirectory = dialogDefaultFolder
			};

			if (dialog.ShowDialog(this) ?? false)
			{
				file = dialog.FileName;

				LoadDocument(MamlDocument.FromFile(file));
			}
		}

		private void Save()
		{
			if (file == null)
			{
				SaveAs();
			}
			else
			{
				SaveCore();
			}
		}

		private void SaveAs()
		{
			var dialog = new SaveFileDialog()
			{
				AddExtension = true,
				DefaultExt = ".aml",
				Filter = "MAML Files (*.aml;*.maml)|*.aml;*.maml|All Files (*.*)|*.*",
				Title = "Save File",
				InitialDirectory = dialogDefaultFolder
			};

			if (dialog.ShowDialog(this) ?? false)
			{
				file = dialog.FileName;

				SaveCore();
			}
		}

		private void SaveCore()
		{
			document.Save(file, incrementRevisionNumber: true, setLastModifiedDateTime: true);
		}

		private void visitButton_Click(object sender, RoutedEventArgs e)
		{
			UpdateOutput();
		}

		private void richTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateOutput();
		}
	}
}