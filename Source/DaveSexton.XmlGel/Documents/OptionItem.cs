namespace DaveSexton.XmlGel.Documents
{
	public sealed class OptionItem
	{
		public object Name
		{
			get;
			set;
		}

		public object Value
		{
			get
			{
				return hasValue ? value : Name;
			}
		}

		public bool HasValue
		{
			get
			{
				return hasValue;
			}
		}

		private readonly object value;
		private readonly bool hasValue;

		// This is required to allow ComboBox to be editable while binding.
		public OptionItem()
		{
		}

		public OptionItem(object name, object value)
		{
			this.Name = name;
			this.value = value;

			hasValue = true;
		}
	}
}