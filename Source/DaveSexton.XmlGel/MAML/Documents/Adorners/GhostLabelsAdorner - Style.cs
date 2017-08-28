using System.Windows;
using System.Windows.Media;

namespace DaveSexton.XmlGel.Maml.Documents.Adorners
{
	partial class GhostLabelsAdorner
	{
		public static readonly DependencyProperty TabRoundedEdgeSizeProperty = DependencyProperty.Register("TabRoundedEdgeSize", typeof(double), typeof(GhostLabelsAdorner));
		public static readonly DependencyProperty TabRadiusProperty = DependencyProperty.Register("TabRadius", typeof(double), typeof(GhostLabelsAdorner));
		public static readonly DependencyProperty TabBrushProperty = DependencyProperty.Register("TabBrush", typeof(Brush), typeof(GhostLabelsAdorner));
		public static readonly DependencyProperty TabPaddingProperty = DependencyProperty.Register("TabPadding", typeof(Thickness), typeof(GhostLabelsAdorner));
		public static readonly DependencyProperty TabOffsetProperty = DependencyProperty.Register("TabOffset", typeof(Vector), typeof(GhostLabelsAdorner));
		public static readonly DependencyProperty DocumentMarginProperty = DependencyProperty.Register("DocumentMargin", typeof(Thickness), typeof(GhostLabelsAdorner));
		public static readonly DependencyProperty CaptionBrushProperty = DependencyProperty.Register("CaptionBrush", typeof(Brush), typeof(GhostLabelsAdorner));
		public static readonly DependencyProperty CaptionFontSizeProperty = DependencyProperty.Register("CaptionFontSize", typeof(double), typeof(GhostLabelsAdorner));
		public static readonly DependencyProperty CaptionFontFamilyProperty = DependencyProperty.Register("CaptionFontFamily", typeof(FontFamily), typeof(GhostLabelsAdorner));
		public static readonly DependencyProperty CaptionFontStyleProperty = DependencyProperty.Register("CaptionFontStyle", typeof(FontStyle), typeof(GhostLabelsAdorner));
		public static readonly DependencyProperty CaptionFontWeightProperty = DependencyProperty.Register("CaptionFontWeight", typeof(FontWeight), typeof(GhostLabelsAdorner));
		public static readonly DependencyProperty CaptionFontStretchProperty = DependencyProperty.Register("CaptionFontStretch", typeof(FontStretch), typeof(GhostLabelsAdorner));

		public double TabRoundedEdgeSize
		{
			get
			{
				return (double) GetValue(TabRoundedEdgeSizeProperty);
			}
			set
			{
				SetValue(TabRoundedEdgeSizeProperty, value);
			}
		}

		public double TabRadius
		{
			get
			{
				return (double) GetValue(TabRadiusProperty);
			}
			set
			{
				SetValue(TabRadiusProperty, value);
			}
		}

		public Brush TabBrush
		{
			get
			{
				return (Brush) GetValue(TabBrushProperty);
			}
			set
			{
				SetValue(TabBrushProperty, value);
			}
		}

		public Thickness TabPadding
		{
			get
			{
				return (Thickness) GetValue(TabPaddingProperty);
			}
			set
			{
				SetValue(TabPaddingProperty, value);
			}
		}

		public Vector TabOffset
		{
			get
			{
				return (Vector) GetValue(TabOffsetProperty);
			}
			set
			{
				SetValue(TabOffsetProperty, value);
			}
		}

		public Thickness DocumentMargin
		{
			get
			{
				return (Thickness) GetValue(DocumentMarginProperty);
			}
			set
			{
				SetValue(DocumentMarginProperty, value);
			}
		}

		public Brush CaptionBrush
		{
			get
			{
				return (Brush) GetValue(CaptionBrushProperty);
			}
			set
			{
				SetValue(CaptionBrushProperty, value);
			}
		}

		public double CaptionFontSize
		{
			get
			{
				return (double) GetValue(CaptionFontSizeProperty);
			}
			set
			{
				SetValue(CaptionFontSizeProperty, value);
			}
		}

		public FontFamily CaptionFontFamily
		{
			get
			{
				return (FontFamily) GetValue(CaptionFontFamilyProperty);
			}
			set
			{
				SetValue(CaptionFontFamilyProperty, value);
			}
		}

		public FontStyle CaptionFontStyle
		{
			get
			{
				return (FontStyle) GetValue(CaptionFontStyleProperty);
			}
			set
			{
				SetValue(CaptionFontStyleProperty, value);
			}
		}

		public FontWeight CaptionFontWeight
		{
			get
			{
				return (FontWeight) GetValue(CaptionFontWeightProperty);
			}
			set
			{
				SetValue(CaptionFontWeightProperty, value);
			}
		}

		public FontStretch CaptionFontStretch
		{
			get
			{
				return (FontStretch) GetValue(CaptionFontStretchProperty);
			}
			set
			{
				SetValue(CaptionFontStretchProperty, value);
			}
		}

		public Typeface CaptionTypeface
		{
			get;
			private set;
		}

		protected override void FreezeStyle()
		{
			CaptionTypeface = new Typeface(CaptionFontFamily, CaptionFontStyle, CaptionFontWeight, CaptionFontStretch);

			TabBrush.Freeze();
			CaptionBrush.Freeze();
		}
	}
}