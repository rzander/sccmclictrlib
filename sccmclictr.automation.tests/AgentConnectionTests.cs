using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using sccmclictr.automation;
using System.Security;
using System.Management.Automation;

namespace sccmclictr.automation.tests
{
    [TestClass]
    public class AgentConnectionTests
    {
        string hostname;
        string username;
        string password;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void SetupTests()
        {
            hostname = TestContext.Properties["hostname"].ToString();
            username = TestContext.Properties["username"].ToString();
            password = TestContext.Properties["password"].ToString();

            if (string.IsNullOrWhiteSpace(hostname))
            {
                Assert.Inconclusive("No hostname provided");
            }
        }

        [TestMethod]
        public void ConnectWithStringPassword()
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Assert.Inconclusive("No credential provided");
            }

            new SCCMAgent(hostname, username, password);
        }

        [TestMethod]
        public void ConnectWithSecureStringPassword()
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Assert.Inconclusive("No credential provided");
            }

            SecureString secPassword = new SecureString();
            foreach (char c in password) secPassword.AppendChar(c);
            
            new SCCMAgent(hostname, username, secPassword);
        }

        [TestMethod]
        public void ConnectWithPSCredential()
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Assert.Inconclusive("No credential provided");
            }

            SecureString secPassword = new SecureString();
            foreach (char c in password) secPassword.AppendChar(c);
            PSCredential credential = new PSCredential(username, secPassword);

            new SCCMAgent(hostname, credential);
        }
    }
}
