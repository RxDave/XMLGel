using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaveSexton.XmlGel.UnitTests.Maml
{
	[TestClass]
	public class UseCasesTests : BaseTests
	{
		[TestMethod]
		public void Maml_UseCases_mmp_player_framework_Architecture()
		{
			TestRoundTrip(
				topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\mmp-player-framework\Architecture.aml"), 
				expected: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\mmp-player-framework\Architecture-Expected.aml"));
		}

		[TestMethod]
		public void Maml_UseCases_mmp_player_framework_SilverlightMediaFramework()
		{
			TestRoundTrip(
				topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\mmp-player-framework\SilverlightMediaFramework.aml"), 
				expected: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\mmp-player-framework\SilverlightMediaFramework-Expected.aml"));
		}

		[TestMethod]
		public void Maml_UseCases_mmp_player_framework_Diagnostics_Configuration()
		{
			TestRoundTrip(
				topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\mmp-player-framework\Diagnostics\Configuration.aml"), 
				expected: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\mmp-player-framework\Diagnostics\Configuration-Expected.aml"));
		}

		[TestMethod]
		public void Maml_UseCases_mmp_player_framework_Diagnostics_GettingStarted()
		{
			TestRoundTrip(
				topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\mmp-player-framework\Diagnostics\GettingStarted.aml"), 
				expected: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\mmp-player-framework\Diagnostics\GettingStarted-Expected.aml"));
		}

		[TestMethod]
		public void Maml_UseCases_mmp_player_framework_Diagnostics_HealthMonitor()
		{
			TestRoundTrip(
				topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\mmp-player-framework\Diagnostics\HealthMonitor.aml"), 
				expected: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\mmp-player-framework\Diagnostics\HealthMonitor-Expected.aml"));
		}

		[TestMethod]
		public void Maml_UseCases_mmp_player_framework_Diagnostics_Overview()
		{
			TestRoundTrip(topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\mmp-player-framework\Diagnostics\Overview.aml"));
		}

		[TestMethod]
		public void Maml_UseCases_mmp_player_framework_Diagnostics_Pip()
		{
			TestRoundTrip(
				topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\mmp-player-framework\Diagnostics\Pip.aml"), 
				expected: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\mmp-player-framework\Diagnostics\Pip-Expected.aml"));
		}

		[TestMethod]
		public void Maml_UseCases_mmp_player_framework_Diagnostics_RemoteLogging()
		{
			TestRoundTrip(
				topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\mmp-player-framework\Diagnostics\RemoteLogging.aml"), 
				expected: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\mmp-player-framework\Diagnostics\RemoteLogging-Expected.aml"));
		}

		[TestMethod]
		public void Maml_UseCases_slimtune_Basic_Concepts()
		{
			TestRoundTrip(
				topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\slimtune\Basic Concepts.aml"), 
				expected: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\slimtune\Basic Concepts-Expected.aml"));
		}

		[TestMethod]
		public void Maml_UseCases_slimtune_Connect_Dialog()
		{
			TestRoundTrip(
				topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\slimtune\Connect Dialog.aml"), 
				expected: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\slimtune\Connect Dialog-Expected.aml"));
		}

		[TestMethod]
		public void Maml_UseCases_slimtune_Profiler_API()
		{
			TestRoundTrip(
				topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\slimtune\Profiler API.aml"), 
				expected: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\slimtune\Profiler API-Expected.aml"));
		}

		[TestMethod]
		public void Maml_UseCases_slimtune_Run_Dialog()
		{
			TestRoundTrip(
				topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\slimtune\Run Dialog.aml"), 
				expected: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\slimtune\Run Dialog-Expected.aml"),
				expectedInvalidNodeCount: 1);
		}

		[TestMethod]
		public void Maml_UseCases_slimtune_User_Guide()
		{
			TestRoundTrip(
				topic: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\slimtune\User Guide.aml"), 
				expected: File.ReadAllText(@"C:\Users\Dave\OneDrive\Projects\XmlGel\DaveSexton.XmlGel\Main\Testing\DaveSexton.XmlGel.UnitTests\Maml\UseCases\slimtune\User Guide-Expected.aml"));
		}

	}
}
