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
        public void ConnectWithNoCredentials()
        {
            // Tests connecting with Kerberos, credential manager creds, etc.
            // This failing is normal in a lot of situations
            try
            {
                new SCCMAgent(hostname);
            }
            catch (Exception e)
            {
                if (e.Message.Equals("Unable to connect") && e.TargetSite.Name.Equals("connect"))
                {
                    // Assume test failed because of permissions/lack of credentials
                    Assert.Inconclusive("Test may have failed to due lack of permissions");
                }
                else
                {
                    throw e;
                }
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
