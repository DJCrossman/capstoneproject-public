using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapstoneProject.UnitTests.Services
{
    /// <summary>
    /// Summary description for UserServicesTests
    /// </summary>
    [TestClass]
    public class UserServicesTests
    {
        public UserServices _userServices;
        public UserServicesTests()
        {
            _userServices = new UserServices("C:/TEMP/") {
                UserDataServices = A.Fake<IUserDataServices>()
            };
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        [TestInitialize]
        public void MyTestInitialize()
        {
            
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void VerifyLoginInformationWithRightUser()
        {
            UserData user = new UserData()
            {
                UID = Guid.NewGuid(),
                UserName = "TestUser",
                Email = "test@test.ca",
                DateCreated = DateTime.Now,
                Password = "cpNZFFs2mvpzHVMPZIbcRqYkKwJNkxZh:n4bQgYhMfWWaL+qgxVrQFaO/TxsrC4Is0V1sFbDwCgg="
            };
            A.CallTo(() => _userServices.UserDataServices.GetUserByLogin(user.Email)).Returns(user);
            _userServices.VerifyLoginInformation(user.Email, user.Password, false);
            Assert.AreEqual(_userServices.GetUserData(), user);
        }

        [TestMethod]
        public void VerifyLoginInformationWithWrongUser()
        {
            UserData user = new UserData()
            {
                UID = Guid.NewGuid(),
                UserName = "TestUser",
                Email = "test@test.ca",
                DateCreated = DateTime.Now,
                Password = "cpNZFFs2mvpzHVMPZIbcRqYkKwJNkxZh:n4bQgYhMfWWaL+qgxVrQFaO/TxsrC4Is0V1sFbDwCgg="
            };
            var wrongPassword = "OuZoOt+0ZO0pHzzdmjDpE+95qE/xBcre:/WGgOvT3fYcPwh4F5+gGeAlcktgIz7O1wnnuBMdKyhM=";
            A.CallTo(() => _userServices.UserDataServices.GetUserByLogin(user.Email)).Returns(user);
            _userServices.VerifyLoginInformation(user.Email, wrongPassword, false);
            Assert.AreEqual(_userServices.GetUserData(), null);
        }

        [TestMethod]
        public void ValidPasswordWithSamePasswords()
        {
            var password1 = "cpNZFFs2mvpzHVMPZIbcRqYkKwJNkxZh:n4bQgYhMfWWaL+qgxVrQFaO/TxsrC4Is0V1sFbDwCgg=";
            Assert.AreEqual(_userServices.ValidPassword(password1, password1), true);
        }

        [TestMethod]
        public void ValidPasswordWithDifferentPasswords()
        {
            var password1 = "cpNZFFs2mvpzHVMPZIbcRqYkKwJNkxZh:n4bQgYhMfWWaL+qgxVrQFaO/TxsrC4Is0V1sFbDwCgg=";
            var password2 = "cpNZFFs2mvpzHVMPZIbcRqYkKwJNkxZh:n4bQgYhMfWWaL+qgxVrQFaO/TxsrC4Is0V1sFbDwCgG=";
            Assert.AreEqual(_userServices.ValidPassword(password1, password2), false);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Username, Email, and Password cannot be empty.")]
        public void CreateAccountEmptyInformation()
        {
            UserData user = new UserData()
            {
                UID = Guid.NewGuid(),
                UserName = "TestUser",
                Email = "fake@easy.com",
                DateCreated = DateTime.Now,
                Password = "cpNZFFs2mvpzHVMPZIbcRqYkKwJNkxZh:n4bQgYhMfWWaL+qgxVrQFaO/TxsrC4Is0V1sFbDwCgg="
            };
            A.CallTo(() => _userServices.UserDataServices.CreateLocalProfile(user)).MustNotHaveHappened();
            _userServices.CreateAccount("","","");
        }

        [TestMethod]
        public void CreateAccountWithInformation()
        {
            UserData user = new UserData()
            {
                UserName = "TestUser",
                Email = "fake@easy.com",
                Password = "cpNZFFs2mvpzHVMPZIbcRqYkKwJNkxZh:n4bQgYhMfWWaL+qgxVrQFaO/TxsrC4Is0V1sFbDwCgg="
            };
            _userServices.CreateAccount(user.UserName, user.Email, user.Password);
            Assert.AreEqual(user.UserName, _userServices.GetUserData().UserName);
            Assert.AreEqual(user.Email, _userServices.GetUserData().Email);
            Assert.AreEqual(user.Password, _userServices.GetUserData().Password);
        }
    }
}
