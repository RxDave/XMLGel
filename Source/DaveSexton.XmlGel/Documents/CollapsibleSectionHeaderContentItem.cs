namespace DaveSexton.XmlGel.Documents
{
	// Used for binding in XAML
	public sealed class CollapsibleSectionHeaderContentItem
	{
		public CollapsibleSection Parent
		{
			get
			{
				return section;
			}
		}

		private readonly CollapsibleSection section;

		internal CollapsibleSectionHeaderContentItem(CollapsibleSection section)
		{
			this.section = section;
		}
	}
}