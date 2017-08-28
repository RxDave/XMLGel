using System.Windows;

namespace DaveSexton.XmlGel
{
	internal delegate Part PartFactory(FrameworkContentElement element, Rect documentBox);
	internal delegate TPart PartFactory<TPart>(FrameworkContentElement element, Rect documentBox) where TPart : Part;
}