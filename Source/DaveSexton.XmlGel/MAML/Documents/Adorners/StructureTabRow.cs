using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Xml.Schema;
using DaveSexton.XmlGel.Xml;

namespace DaveSexton.XmlGel.Maml.Documents.Adorners
{
	internal sealed class StructureTabRow : ReadOnlyCollection<StructureTab>
	{
		public MamlPart ContainerPart
		{
			get
			{
				return containerPart;
			}
		}

		public XmlSchemaParticlesExpected Expected
		{
			get
			{
				return expected;
			}
		}

		public Rect BaseLine
		{
			get
			{
				return baseLine;
			}
		}

		public Rect SchemaTabsTray
		{
			get
			{
				return schemaTabsTray;
			}
		}

		public Rect BehaviorTabsTray
		{
			get
			{
				return behaviorTabsTray;
			}
		}

		internal StructurePalette Palette
		{
			get
			{
				return palette;
			}
		}

		private readonly StructurePalette palette;
		private readonly XmlSchemaParticlesExpected expected;
		private readonly MamlPart targetPart, containerPart;
		private readonly Rect baseLine;
		private readonly StructureTabLocation tabLocation;
		private Rect schemaTabsTray, behaviorTabsTray;

		public StructureTabRow(
			StructurePalette palette,
			XmlSchemaParticlesExpected expected,
			MamlPart containerPart,
			StructureTabLocation tabLocation,
			Rect baseLine,
			bool addBehaviorTabs)
			: base(new List<StructureTab>(expected.Count + 2))
		{
			this.palette = palette;
			this.expected = expected;
			this.targetPart = (MamlPart) expected.TargetPart;
			this.containerPart = containerPart;
			this.tabLocation = tabLocation;
			this.baseLine = baseLine;

			schemaTabsTray = new Rect(baseLine.TopLeft, baseLine.BottomLeft);
			behaviorTabsTray = new Rect(baseLine.TopRight, baseLine.BottomRight);

			if (addBehaviorTabs)
			{
				AddBehaviorTabs();
			}

			AddSchemaTabs();

			var tabMargin = palette.TabMargin;

			schemaTabsTray.X -= tabMargin.Left;
			schemaTabsTray.Width += tabMargin.Left + tabMargin.Right;
			schemaTabsTray.Y -= tabMargin.Top;
			schemaTabsTray.Height += tabMargin.Top + tabMargin.Bottom;

			behaviorTabsTray.Width += tabMargin.Right;
			behaviorTabsTray.Y -= tabMargin.Top;
			behaviorTabsTray.Height += tabMargin.Top + tabMargin.Bottom;
		}

		private void AddBehaviorTabs()
		{
			var x = baseLine.X;
			var y = baseLine.Y;

			if (targetPart == null)
			{
				return;
			}

			x += palette.BehaviorTabSpacing;

			var targetNode = targetPart == null ? null : targetPart.Node;

			if (targetNode != null)
			{
				var options = targetNode.GetOptions(targetPart.Schema, palette.Document.XVisitor.Schemas);

				if (options.Count > 0)
				{
					var optionsTab = StructureTab.CreateForBehavior(this, targetNode, palette.OptionsTabImage, x, y, _ => palette.ShowTargetElementOptions(targetPart, options), tabLocation, floatRight: true);

					Add(optionsTab);

					behaviorTabsTray.Union(optionsTab.Box);

					x += optionsTab.Box.Width + palette.BehaviorTabSpacing;
				}
			}

			var deleteTab = StructureTab.CreateForBehavior(this, targetNode, palette.DeleteTabImage, x, y, _ => palette.DeleteTargetElement(this), tabLocation, floatRight: true);

			Add(deleteTab);

			behaviorTabsTray.Union(deleteTab.Box);
		}

		private void AddSchemaTabs()
		{
			var x = baseLine.X;
			var y = baseLine.Y;

			x += palette.TabMargin.Left;

			foreach (var particle in expected)
			{
				var node = palette.Document.XVisitor.CreateElementNode(particle);

				var tab = StructureTab.CreateForSchema(this, node, particle as XmlSchemaElement, x, y, palette.AddSibling, tabLocation, floatRight: false);

				Add(tab);

				schemaTabsTray.Union(tab.Box);

				x += tab.Box.Width + palette.TabMargin.Left + palette.TabMargin.Right;
			}
		}

		private void Add(StructureTab tab)
		{
			Items.Add(tab);
		}

		public void Draw(DrawingContext drawingContext)
		{
			drawingContext.DrawLine(palette.TabRowPen, baseLine.TopLeft, baseLine.TopRight);

			for (int i = 0; i < Count; i++)
			{
				var tab = this[i];

				tab.Draw(drawingContext);
			}
		}
	}
}