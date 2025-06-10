using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnoopAPITechnicalTask
{
    [SetUpFixture]
    public class Hooks
    {
        public static ExtentReports Extent;
        public static ExtentTest Test;


        [OneTimeSetUp]
        public void GlobalSetup()
        {
            var reportPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Reports", "TestReport.html");
            Directory.CreateDirectory(Path.GetDirectoryName(reportPath));  // Ensure folder exists

            var sparkReporter = new ExtentSparkReporter(reportPath);
            Extent = new ExtentReports();
            Extent.AttachReporter(sparkReporter);
        }

        [OneTimeTearDown]
        public void GlobalTeardown()
        {
            Extent.Flush();
        }
    }

}
