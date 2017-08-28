using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using DaveSexton.XmlGel.Extensions;

namespace DaveSexton.XmlGel.Documents
{
	public class CollapsibleSectionOutline : Section
	{
		public string Caption
		{
			get
			{
				return caption == null ? null : caption.Text;
			}
			set
			{
				Contract.Requires(CaptionVisible);

				caption.Text = value;
			}
		}

		public bool CaptionVisible
		{
			get
			{
				return caption != null;
			}
			set
			{
				if (value != CaptionVisible)
				{
					if (value)
					{
						CreateCaption();
					}
					else
					{
						Blocks.Remove(captionParagraph);

						captionParagraph = null;
						caption = null;
					}
				}
			}
		}

		public int Depth
		{
			get;
			set;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public new BlockCollection Blocks
		{
			get
			{
				return base.Blocks;
			}
		}

		private Paragraph captionParagraph;
		private Run caption;
		private List outline;

		public CollapsibleSectionOutline()
		{
		}

		private void CreateCaption()
		{
			captionParagraph = new Paragraph(caption = new Run());

			var first = Blocks.FirstBlock;

			if (first != null)
			{
				Blocks.InsertBefore(first, captionParagraph);
			}
			else
			{
				Blocks.Add(captionParagraph);
			}
		}

		private void EnsureEmptyOutline()
		{
			if (outline != null)
			{
				outline.ListItems.Clear();
			}
			else
			{
				outline = new List();

				Blocks.Add(outline);
			}
		}

		public void Update(FrameworkContentElement outlineRoot, Func<Section, bool> addToOutline)
		{
			EnsureEmptyOutline();

			if (outlineRoot != null)
			{
				Update(0, outline.ListItems, outlineRoot, addToOutline);
			}
		}

		private void Update(int currentDepth, ListItemCollection listItems, FrameworkContentElement element, Func<CollapsibleSection, bool> addToOutline)
		{
			if (currentDepth <= Depth)
			{
				foreach (var section in FindChildSectionsAtCurrentDepth(element).Where(addToOutline))
				{
					var link = new Hyperlink(new Run(section.Title));

					link.RequestNavigate += (sender, e) =>
						{
							// TODO: Scroll section into view
						};

					var item = new ListItem(new Paragraph(link));

					var childOutline = new List();

					Update(currentDepth + 1, childOutline.ListItems, section, addToOutline);

					item.Blocks.Add(childOutline);

					listItems.Add(item);
				}
			}
		}

		private IEnumerable<CollapsibleSection> FindChildSectionsAtCurrentDepth(FrameworkContentElement element)
		{
			var children = element.TryGetChildCollection();

			if (children != null)
			{
				for (int i = 0; i < children.Count; i++)
				{
					var child = children[i];

					var section = child as CollapsibleSection;

					if (section != null)
					{
						yield return section;
					}
					else
					{
						foreach (var siblingSection in FindChildSectionsAtCurrentDepth((FrameworkContentElement) child))
						{
							yield return siblingSection;
						}
					}
				}
			}
		}
	}
}