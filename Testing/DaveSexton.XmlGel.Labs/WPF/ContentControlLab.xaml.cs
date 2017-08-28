using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DaveSexton.XmlGel.Labs.WPF
{
	[DisplayName("")]
	[Description("")]
	public partial class ContentControlLab : BaseLab
	{
		public string Text
		{
			get
			{
				return "Bound text";
			}
		}

		public ContentControlLab()
		{
			InitializeComponent();
		}
	}
}