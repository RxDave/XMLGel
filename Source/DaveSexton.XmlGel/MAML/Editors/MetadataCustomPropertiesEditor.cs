using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows;

namespace DaveSexton.XmlGel.Maml.Editors
{
	internal sealed class MetadataCustomPropertiesEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			var window = new MetadataCustomPropertiesEditorWindow(((DocumentProperties) context.Instance).Metadata)
			{
				Owner = Application.Current.MainWindow
			};

			window.ShowDialog();

			return value;
		}
	}
}