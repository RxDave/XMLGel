using System.Diagnostics.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaveSexton.XmlGel.UnitTests
{
	[TestClass]
	public class Global
	{
		[AssemblyInitialize]
		public static void Initialize(TestContext context)
		{
			Contract.ContractFailed += Contract_ContractFailed;
		}

		private static void Contract_ContractFailed(object sender, ContractFailedEventArgs e)
		{
			e.SetHandled();
			e.SetUnwind();

			var message = e.OriginalException == null ? e.Message : e.OriginalException.ToString();

			Assert.Fail(message);
		}
	}
}