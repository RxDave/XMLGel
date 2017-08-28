using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using DaveSexton.XmlGel.Maml.Documents.Adorners;

namespace DaveSexton.XmlGel.Maml
{
	[TemplatePart(Name = "PART_Editor", Type = typeof(MamlTopicEditorTextBox))]
	public class MamlTopicEditor : Control
	{
		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(MamlTopicEditor), new FrameworkPropertyMetadata(TitleChanged));
		public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(MamlTopicEditor), new FrameworkPropertyMetadata(false));
		public static readonly DependencyProperty ToggleDebugAdornersKeyGestureProperty = DependencyProperty.Register("ToggleDebugAdornersKeyGesture", typeof(KeyGesture), typeof(MamlTopicEditor), new FrameworkPropertyMetadata(new KeySequenceGesture(Key.M, Key.B, ModifierKeys.Control)));
		public static readonly DependencyProperty ToggleTabRowsFollowMouseKeyGestureProperty = DependencyProperty.Register("ToggleTabRowsFollowMouseKeyGesture", typeof(KeyGesture), typeof(MamlTopicEditor), new FrameworkPropertyMetadata(new KeySequenceGesture(Key.M, Key.M, ModifierKeys.Control)));
		public static readonly DependencyProperty StructurePaletteUseFocusActivationBehaviorProperty = DependencyProperty.Register("StructurePaletteUseFocusActivationBehavior", typeof(bool), typeof(MamlTopicEditor), new FrameworkPropertyMetadata(false, StructurePaletteUseFocusActivationBehaviorChanged));

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

		public bool IsReadOnly
		{
			get
			{
				return (bool) GetValue(IsReadOnlyProperty);
			}
			set
			{
				SetValue(IsReadOnlyProperty, value);
			}
		}

		public MamlDocument Document
		{
			get
			{
				return document;
			}
			set
			{
				LoadDocument(value);
			}
		}

		public MamlTopicEditorTextBox TextBox
		{
			get
			{
				return editor;
			}
		}

		public XmlResolver XmlResolver
		{
			get;
			set;
		}

		public ICollection<XmlSchema> XmlSchemas
		{
			get;
			set;
		}

		// TODO: Register key command with Visual Studio so that it can be modified by the user
		public KeyGesture ToggleDebugAdornersKeyGesture
		{
			get
			{
				return (KeyGesture) GetValue(ToggleDebugAdornersKeyGestureProperty);
			}
			set
			{
				SetValue(ToggleDebugAdornersKeyGestureProperty, value);
			}
		}

		// TODO: Register key command with Visual Studio so that it can be modified by the user
		public KeyGesture ToggleTabRowsFollowMouseKeyGesture
		{
			get
			{
				return (KeyGesture) GetValue(ToggleTabRowsFollowMouseKeyGestureProperty);
			}
			set
			{
				SetValue(ToggleTabRowsFollowMouseKeyGestureProperty, value);
			}
		}

		public bool StructurePaletteUseFocusActivationBehavior
		{
			get
			{
				return (bool?) GetValue(StructurePaletteUseFocusActivationBehaviorProperty) ?? false;
			}
			set
			{
				SetValue(StructurePaletteUseFocusActivationBehaviorProperty, value);
			}
		}

		protected virtual bool ShouldIncrementRevisionNumber
		{
			get
			{
				return true;
			}
		}

		private MamlTopicEditorTextBox editor;
		private MamlDocument document;
		private bool loadingDocument;
		private bool adornersInitialized;
		private bool debugAdornersDefaultEnabledState;
		private GhostLabelsAdorner ghostLabelsAdorner;
		private MouseAdorner mouseAdorner;
		private StructurePalette structurePalette;

		static MamlTopicEditor()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(MamlTopicEditor), new FrameworkPropertyMetadata(typeof(MamlTopicEditor)));
		}

		public MamlTopicEditor()
		{
			Loaded += (_, __) =>
				{
					if (document != null)
					{
						InitializeAdorners();
					}
				};
		}

		private void Initialize()
		{
			editor.TextChanged += editor_TextOrUIElementOrPropertyChanged;
			editor.PreviewKeyDown += editor_PreviewKeyDown;

			/* In the simple test app, OnApplyTemplate is called before LoadDocument; however, in Visual Studio it happens in reverse, 
			 * so don't assign the document after the editor is loaded.
			 */
			if (document == null)
			{
				editor.IsReadOnly = true;
			}
			else
			{
				editor.IsReadOnly = false;
				editor.Document = document.GenerateFlowDocument(
					OnInvalidNode,
					OnInvalidAttribute,
					() => editor_TextOrUIElementOrPropertyChanged(null, EventArgs.Empty),
					XmlResolver,
					XmlSchemas);

				InitializeAdorners();
			}
		}

		public void ToggleDebugAdorners()
		{
			if (mouseAdorner != null)
			{
				var toggledState = !mouseAdorner.IsAdornerEnabled;

				mouseAdorner.IsAdornerEnabled = toggledState;
				structurePalette.IsDebugEnabled = toggledState;
			}
			else
			{
				debugAdornersDefaultEnabledState = !debugAdornersDefaultEnabledState;
			}
		}

		public void ToggleTabRowsFollowMouse()
		{
			if (structurePalette != null)
			{
				structurePalette.TabRowsFollowMouse = !structurePalette.TabRowsFollowMouse;
			}
		}

		private void SetStructurePaletteUseFocusActivationBehavior(bool value)
		{
			if (structurePalette != null)
			{
				structurePalette.UseFocusActivationBehavior = value;
			}
		}

		public override void OnApplyTemplate()
		{
			editor = GetTemplateChild("PART_Editor") as MamlTopicEditorTextBox;

			OnTextBoxChanged(EventArgs.Empty);

			if (editor != null)
			{
				Initialize();
			}

			base.OnApplyTemplate();
		}

		private void InitializeAdorners()
		{
			if (!adornersInitialized && IsLoaded)
			{
				var layer = AdornerLayer.GetAdornerLayer(editor);

				var adorners = layer.GetAdorners(editor);

				// During testing adorners was null (instead of being an empty array, as it should be)
				if (adorners != null)
				{
					foreach (var adorner in adorners)
					{
						layer.Remove(adorner);

						var disposable = adorner as IDisposable;

						if (disposable != null)
						{
							disposable.Dispose();
						}
					}
				}

				mouseAdorner = new MouseAdorner(document, editor)
				{
					IsAdornerEnabled = debugAdornersDefaultEnabledState
				};

				ghostLabelsAdorner = new GhostLabelsAdorner(document, editor);

				structurePalette = new StructurePalette(document, editor, mouseAdorner, ghostLabelsAdorner)
				{
					UseFocusActivationBehavior = StructurePaletteUseFocusActivationBehavior,
					IsAdornerEnabled = true,
					IsDebugEnabled = debugAdornersDefaultEnabledState
				};

				structurePalette.BlockAdded += (sender, e) => document.UpdateDynamicContent();
				structurePalette.BlockRemoved += (sender, e) => document.UpdateDynamicContent();

				layer.Add(mouseAdorner);
				layer.Add(ghostLabelsAdorner);
				layer.Add(structurePalette);

				adornersInitialized = true;
			}
		}

		public virtual void Load(string file)
		{
			LoadDocument(MamlDocument.FromFile(file));
		}

		public virtual void Save(string file)
		{
			document.Update();

			document.Save(file, incrementRevisionNumber: ShouldIncrementRevisionNumber, setLastModifiedDateTime: true);
		}

		private void LoadDocument(MamlDocument newDocument)
		{
			loadingDocument = true;

			try
			{
				if (document != null)
				{
					document.PropertyChanged -= editor_TextOrUIElementOrPropertyChanged;
					document.Metadata.PropertyChanged -= editor_MetadataChanged;
				}

				document = newDocument;

				if (document == null)
				{
					Title = null;

					if (editor != null)
					{
						editor.Document = null;
						editor.IsReadOnly = true;
					}
				}
				else
				{
					document.PropertyChanged += editor_TextOrUIElementOrPropertyChanged;
					document.Metadata.PropertyChanged += editor_MetadataChanged;

					Title = document.Metadata.Title;

					if (editor != null)
					{
						editor.IsReadOnly = false;
						editor.Document = document.GenerateFlowDocument(
							OnInvalidNode,
							OnInvalidAttribute,
							() => editor_TextOrUIElementOrPropertyChanged(null, EventArgs.Empty),
							XmlResolver,
							XmlSchemas);

						adornersInitialized = false;

						InitializeAdorners();
					}
				}
			}
			finally
			{
				loadingDocument = false;
			}

			OnDocumentChanged(EventArgs.Empty);
		}

		private void UpdateTitle(string title)
		{
			if (!loadingDocument && document != null)
			{
				document.Metadata.Title = title;
			}
		}

		private void editor_MetadataChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Title")
			{
				Title = document.Metadata.Title;
			}

			editor_TextOrUIElementOrPropertyChanged(sender, e);
		}

		private void editor_TextOrUIElementOrPropertyChanged(object sender, EventArgs e)
		{
			OnDocumentContentChanged(e);
		}

		private void editor_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			var gesture = ToggleDebugAdornersKeyGesture;

			if (gesture != null && gesture.Matches(e.Source, e))
			{
				ToggleDebugAdorners();

				e.Handled = true;
			}
			else
			{
				gesture = ToggleTabRowsFollowMouseKeyGesture;

				if (gesture != null && gesture.Matches(e.Source, e))
				{
					ToggleTabRowsFollowMouse();
				}
			}
		}

		private static void TitleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			((MamlTopicEditor) sender).UpdateTitle((string) e.NewValue);
		}

		private static void StructurePaletteUseFocusActivationBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var editor = (MamlTopicEditor) d;

			editor.SetStructurePaletteUseFocusActivationBehavior((bool?) e.NewValue ?? false);
		}

		public event EventHandler TextBoxChanged;
		public event EventHandler DocumentChanged;
		public event EventHandler DocumentContentChanged;
		public event Action<XAttribute> InvalidAttribute;
		public event Action<XNode> InvalidNode;

		protected virtual void OnTextBoxChanged(EventArgs e)
		{
			var handler = TextBoxChanged;

			if (handler != null)
			{
				handler(this, e);
			}
		}

		protected virtual void OnDocumentChanged(EventArgs e)
		{
			var handler = DocumentChanged;

			if (handler != null)
			{
				handler(this, e);
			}
		}

		protected virtual void OnDocumentContentChanged(EventArgs e)
		{
			var handler = DocumentContentChanged;

			if (handler != null)
			{
				handler(this, e);
			}
		}

		protected virtual void OnInvalidAttribute(XAttribute attribute)
		{
			var handler = InvalidAttribute;

			if (handler != null)
			{
				handler(attribute);
			}
		}

		protected virtual void OnInvalidNode(XNode node)
		{
			var handler = InvalidNode;

			if (handler != null)
			{
				handler(node);
			}
		}
	}
}