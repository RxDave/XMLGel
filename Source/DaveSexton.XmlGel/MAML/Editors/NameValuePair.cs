namespace DaveSexton.XmlGel.Maml.Editors
{
	public sealed class NameValuePair
	{
		// Must be writable for binding to a DataGrid.
		public string Name
		{
			get;
			set;
		}

		// Must be writable for binding to a DataGrid.
		public string Value
		{
			get;
			set;
		}

		// A parameterless constructor is required to display an empty row at the bottom of a DataGrid with CanUserAddRows="True"
		public NameValuePair()
		{
		}

		public NameValuePair(string name, string value)
		{
			this.Name = name;
			this.Value = value;
		}
	}
}