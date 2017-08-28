using System.Windows;
using System.Windows.Media;

namespace DaveSexton.XmlGel.Maml.Documents.Adorners
{
	partial class StructurePalette
	{
		internal readonly double BlockRegionMarkerRadius = 3;
		internal readonly double BlockRegionMarkerMargin = 3;

		public static readonly DependencyProperty BehaviorTabSpacingProperty = DependencyProperty.Register("BehaviorTabSpacing", typeof(double), typeof(StructurePalette));
		public static readonly DependencyProperty TabMarginProperty = DependencyProperty.Register("TabMargin", typeof(Thickness), typeof(StructurePalette));
		public static readonly DependencyProperty TabPaddingProperty = DependencyProperty.Register("TabPadding", typeof(Thickness), typeof(StructurePalette));
		public static readonly DependencyProperty TabRadiusProperty = DependencyProperty.Register("TabRadius", typeof(double), typeof(StructurePalette));
		public static readonly DependencyProperty TabRoundedEdgeHeightProperty = DependencyProperty.Register("TabRoundedEdgeHeight", typeof(double), typeof(StructurePalette));
		public static readonly DependencyProperty TabRowPenSizeProperty = DependencyProperty.Register("TabRowPenSize", typeof(double), typeof(StructurePalette));
		public static readonly DependencyProperty TabPenSizeProperty = DependencyProperty.Register("TabPenSize", typeof(double), typeof(StructurePalette));
		public static readonly DependencyProperty TabCaptionMarginProperty = DependencyProperty.Register("TabCaptionMargin", typeof(Thickness), typeof(StructurePalette));
		public static readonly DependencyProperty TabCaptionFontSizeProperty = DependencyProperty.Register("TabCaptionFontSize", typeof(double), typeof(StructurePalette));
		public static readonly DependencyProperty InvalidBackgroundProperty = DependencyProperty.Register("InvalidBackground", typeof(Brush), typeof(StructurePalette));
		public static readonly DependencyProperty TabRowBrushProperty = DependencyProperty.Register("TabRowBrush", typeof(Brush), typeof(StructurePalette));
		public static readonly DependencyProperty TabBrushProperty = DependencyProperty.Register("TabBrush", typeof(Brush), typeof(StructurePalette));
		public static readonly DependencyProperty TabCaptionBrushProperty = DependencyProperty.Register("TabCaptionBrush", typeof(Brush), typeof(StructurePalette));
		public static readonly DependencyProperty CaptionFontFamilyProperty = DependencyProperty.Register("CaptionFontFamily", typeof(FontFamily), typeof(StructurePalette));
		public static readonly DependencyProperty CaptionFontStyleProperty = DependencyProperty.Register("CaptionFontStyle", typeof(FontStyle), typeof(StructurePalette));
		public static readonly DependencyProperty CaptionFontWeightProperty = DependencyProperty.Register("CaptionFontWeight", typeof(FontWeight), typeof(StructurePalette));
		public static readonly DependencyProperty CaptionFontStretchProperty = DependencyProperty.Register("CaptionFontStretch", typeof(FontStretch), typeof(StructurePalette));
		public static readonly DependencyProperty OptionsTabImageProperty = DependencyProperty.Register("OptionsTabImage", typeof(ImageSource), typeof(StructurePalette));
		public static readonly DependencyProperty DeleteTabImageProperty = DependencyProperty.Register("DeleteTabImage", typeof(ImageSource), typeof(StructurePalette));
		public static readonly DependencyProperty TabRowsFollowMouseActivationRegionWidthProperty = DependencyProperty.Register("TabRowsFollowMouseActivationRegionWidth", typeof(double), typeof(StructurePalette));
		public static readonly DependencyProperty TabRowsFollowMouseActivationRegionMarginProperty = DependencyProperty.Register("TabRowsFollowMouseActivationRegionMargin", typeof(double), typeof(StructurePalette));
		public static readonly DependencyProperty TabRowsFollowMouseActivationRegionDockLeftProperty = DependencyProperty.Register("TabRowsFollowMouseActivationRegionDockLeft", typeof(bool), typeof(StructurePalette));

		public double BehaviorTabSpacing
		{
			get
			{
				return (double) GetValue(BehaviorTabSpacingProperty);
			}
			set
			{
				SetValue(BehaviorTabSpacingProperty, value);
			}
		}

		public Thickness TabMargin
		{
			get
			{
				return (Thickness) GetValue(TabMarginProperty);
			}
			set
			{
				SetValue(TabMarginProperty, value);
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

		public double TabRoundedEdgeHeight
		{
			get
			{
				return (double) GetValue(TabRoundedEdgeHeightProperty);
			}
			set
			{
				SetValue(TabRoundedEdgeHeightProperty, value);
			}
		}

		public double TabRowPenSize
		{
			get
			{
				return (double) GetValue(TabRowPenSizeProperty);
			}
			set
			{
				SetValue(TabRowPenSizeProperty, value);
			}
		}

		public double TabPenSize
		{
			get
			{
				return (double) GetValue(TabPenSizeProperty);
			}
			set
			{
				SetValue(TabPenSizeProperty, value);
			}
		}

		public Thickness TabCaptionMargin
		{
			get
			{
				return (Thickness) GetValue(TabCaptionMarginProperty);
			}
			set
			{
				SetValue(TabCaptionMarginProperty, value);
			}
		}

		public double TabCaptionFontSize
		{
			get
			{
				return (double) GetValue(TabCaptionFontSizeProperty);
			}
			set
			{
				SetValue(TabCaptionFontSizeProperty, value);
			}
		}

		public Brush InvalidBackground
		{
			get
			{
				return (Brush) GetValue(InvalidBackgroundProperty);
			}
			set
			{
				SetValue(InvalidBackgroundProperty, value);
			}
		}

		public Brush TabRowBrush
		{
			get
			{
				return (Brush) GetValue(TabRowBrushProperty);
			}
			set
			{
				SetValue(TabRowBrushProperty, value);
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

		public Brush TabCaptionBrush
		{
			get
			{
				return (Brush) GetValue(TabCaptionBrushProperty);
			}
			set
			{
				SetValue(TabCaptionBrushProperty, value);
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

		public ImageSource OptionsTabImage
		{
			get
			{
				return (ImageSource) GetValue(OptionsTabImageProperty);
			}
			set
			{
				SetValue(OptionsTabImageProperty, value);
			}
		}

		public ImageSource DeleteTabImage
		{
			get
			{
				return (ImageSource) GetValue(DeleteTabImageProperty);
			}
			set
			{
				SetValue(DeleteTabImageProperty, value);
			}
		}

		public double TabRowsFollowMouseActivationRegionWidth
		{
			get
			{
				return (double) GetValue(TabRowsFollowMouseActivationRegionWidthProperty);
			}
			set
			{
				SetValue(TabRowsFollowMouseActivationRegionWidthProperty, value);
			}
		}

		public double TabRowsFollowMouseActivationRegionMargin
		{
			get
			{
				return (double) GetValue(TabRowsFollowMouseActivationRegionMarginProperty);
			}
			set
			{
				SetValue(TabRowsFollowMouseActivationRegionMarginProperty, value);
			}
		}

		public bool TabRowsFollowMouseActivationRegionDockLeft
		{
			get
			{
				return (bool) GetValue(TabRowsFollowMouseActivationRegionDockLeftProperty);
			}
			set
			{
				SetValue(TabRowsFollowMouseActivationRegionDockLeftProperty, value);
			}
		}

		public Pen TabPen
		{
			get
			{
				return tabPen;
			}
		}

		public Pen TabRowPen
		{
			get
			{
				return tabRowPen;
			}
		}

		public Typeface CaptionTypeface
		{
			get
			{
				return captionTypeface;
			}
		}

		private Pen tabPen, tabRowPen;
		private Typeface captionTypeface;

		protected override void FreezeStyle()
		{
			tabPen = new Pen(TabBrush, TabPenSize <= 0 ? TabRowPenSize : TabPenSize);
			tabRowPen = new Pen(TabRowBrush, TabRowPenSize <= 0 ? TabPenSize : TabRowPenSize);
			captionTypeface = new Typeface(CaptionFontFamily, CaptionFontStyle, CaptionFontWeight, CaptionFontStretch);

			TabRowBrush.Freeze();
			TabBrush.Freeze();
			TabCaptionBrush.Freeze();
			tabPen.Freeze();
			tabRowPen.Freeze();
		}
	}
}