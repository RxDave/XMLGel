using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DaveSexton.XmlGel.Extensions;

namespace DaveSexton.XmlGel.Maml.Documents.Adorners
{
	internal abstract class EditorAdornerBase : Adorner, IDisposable
	{
		public bool UseFocusActivationBehavior
		{
			get
			{
				return useFocusActivationBehavior;
			}
			set
			{
				if (useFocusActivationBehavior != value)
				{
					useFocusActivationBehavior = value;

					UpdateActivationBehavior();
				}
			}
		}

		public bool IsAdornerEnabled
		{
			get
			{
				return isAdornerEnabled;
			}
			set
			{
				if (isAdornerEnabled != value)
				{
					isAdornerEnabled = value;

					if (isAdornerEnabled)
					{
						AdornerEnabled();
					}
					else
					{
						AdornerDisabled();
					}
				}
			}
		}

		public MamlTopicEditorTextBox Editor
		{
			get
			{
				return editor;
			}
		}

		protected abstract bool CanRender
		{
			get;
		}

		private readonly MamlDocument document;
		private readonly MamlTopicEditorTextBox editor;
		private bool useFocusActivationBehavior, isAdornerEnabled;
		private DispatcherOperation scheduledUpdate;
#if DEBUG
		/* When Code Contract assertion dialogs are enabled, a contract violation that occurs during an update will free 
		 * the UI thread, allowing the user to interact with the dialog; however, as a result, the adorner continues processing
		 * mouse movements, thus breaking invariants before the user can stop the debugger for the original error.
		 * 
		 * Furthermore, broken invariants sometimes cause additional contract dialgs, making it very difficult to track down the 
		 * original error.
		 * 
		 * This variable simply prevents re-entry into the UpdateCore method, allowing the user to ineract with the first and 
		 * only assertion dialog.
		 */
		private bool updating;
#endif

		protected EditorAdornerBase(MamlDocument document, MamlTopicEditorTextBox editor)
			: base(editor)
		{
			this.document = document;
			this.editor = editor;

			Loaded += EditorAdornerBase_Loaded;
		}

		protected virtual void AdornerEnabled()
		{
			editor.TextChanged += editor_TextChanged;

			UpdateActivationBehavior();

			if (!useFocusActivationBehavior && IsLoaded)
			{
				Update(Mouse.GetPosition(editor));
			}
		}

		protected virtual void AdornerDisabled()
		{
			UnregisterEventHandlers();

			ClearRenderState();

			InvalidateVisual();
		}

		private void UpdateActivationBehavior()
		{
			if (isAdornerEnabled)
			{
				UnregisterActivationEventHandlers();

				if (useFocusActivationBehavior)
				{
					editor.SelectionChanged += editor_SelectionChanged;
				}
				else
				{
					editor.PreviewMouseDown += editor_PreviewMouseDown;
					editor.MouseMove += editor_MouseMove;
					editor.MouseLeave += editor_MouseLeave;
					editor.SizeChanged += editor_SizeChanged;
				}
			}
		}

		private void UnregisterEventHandlers()
		{
			editor.TextChanged -= editor_TextChanged;

			UnregisterActivationEventHandlers();
		}

		private void UnregisterActivationEventHandlers()
		{
			editor.PreviewMouseDown -= editor_PreviewMouseDown;
			editor.MouseMove -= editor_MouseMove;
			editor.MouseLeave -= editor_MouseLeave;
			editor.SizeChanged -= editor_SizeChanged;
			editor.SelectionChanged -= editor_SelectionChanged;
		}

		protected abstract void ClearRenderState();

		protected void ScheduleUpdate(bool forceInvalidate = false)
		{
			if (scheduledUpdate == null)
			{
				scheduledUpdate = Dispatcher.BeginInvoke(
					new Action<bool>(invalidate =>
					{
						scheduledUpdate = null;

						Update(null, forceUpdate: true, forceInvalidate: invalidate);
					}),
					DispatcherPriority.Input,
					forceInvalidate);
			}
		}

		protected void Update(Point point, bool forceUpdate = false, bool forceInvalidate = false)
		{
			if (isAdornerEnabled)
			{
				Update(editor.GetPositionFromPoint(point), forceUpdate, forceInvalidate);
			}
		}

		protected void Update(TextPointer pointer, bool forceUpdate = false, bool forceInvalidate = false)
		{
			if (scheduledUpdate != null)
			{
				scheduledUpdate.Abort();
				scheduledUpdate = null;
			}

			if (isAdornerEnabled)
			{
				if (pointer == null && !forceUpdate)
				{
					ClearRenderState();
				}
				else
				{
#if DEBUG
					if (updating)
					{
						return;
					}

					updating = true;

					try
#endif
					{
						if (!UpdateCore(pointer) && !forceInvalidate)
						{
							return;
						}
					}
#if DEBUG
					finally
					{
						updating = false;
					}
#endif
				}

				InvalidateVisual();
			}
		}

		protected abstract bool UpdateCore(TextPointer pointer);

		protected sealed override void OnRender(DrawingContext drawingContext)
		{
			if (isAdornerEnabled && CanRender)
			{
				RenderCore(drawingContext);
			}
		}

		protected abstract void RenderCore(DrawingContext drawingContext);

		protected virtual void FreezeStyle()
		{
			// for derived classes
		}

		private void EditorAdornerBase_Loaded(object sender, RoutedEventArgs e)
		{
			FreezeStyle();
		}

		protected virtual void OnEditorTextChanged(TextChangedEventArgs e)
		{
			// for derived classes
		}

		protected virtual void OnEditorMouseDown(MouseButtonEventArgs e)
		{
			// for derived classes
		}

		private void editor_TextChanged(object sender, TextChangedEventArgs e)
		{
			OnEditorTextChanged(e);
		}

		private void editor_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			OnEditorMouseDown(e);
		}

		private void editor_MouseMove(object sender, MouseEventArgs e)
		{
			Update(e.GetPosition(editor));
		}

		private void editor_MouseLeave(object sender, MouseEventArgs e)
		{
			if (!this.IsMouseOver)
			{
#if DEBUG
				if (updating)
				{
					return;
				}
#endif

				ClearRenderState();

				InvalidateVisual();
			}
		}

		private void editor_SizeChanged(object sender, SizeChangedEventArgs e)
		{
#if DEBUG
			if (updating)
			{
				return;
			}
#endif
			ClearRenderState();
		}

		private void editor_SelectionChanged(object sender, RoutedEventArgs e)
		{
			Update(editor.CaretPosition);
		}

		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				UnregisterEventHandlers();
			}
		}
	}
}