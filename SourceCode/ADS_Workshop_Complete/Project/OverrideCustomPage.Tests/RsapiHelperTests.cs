using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using kCura.Relativity.Client.Repositories;
using Moq;
using NUnit.Framework;
using OverrideCustomPage.Exceptions;
using OverrideCustomPage.Helpers;
using OverrideCustomPage.Models;
using Relativity.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OverrideCustomPage.Tests
{
    [TestFixture]
    public class RsapiHelperTests
    {
        #region Properties

        public RsapiHelper Sut { get; set; }
        public Mock<IAPILog> MockApiLog { get; set; }
        public Mock<IGenericRepository<RDO>> MockRdoRepository { get; set; }

        #endregion

        #region Setup and TearDown

        [SetUp]
        public void SetUp()
        {
            MockApiLog = new Mock<IAPILog>();
            MockRdoRepository = new Mock<IGenericRepository<RDO>>();
            APIOptions rsapiApiOptions = new APIOptions
            {
                WorkspaceID = -1
            };
            Sut = new RsapiHelper(MockApiLog.Object, MockRdoRepository.Object, rsapiApiOptions);
        }

        [TearDown]
        public void TearDown()
        {
            MockRdoRepository = null;
            Sut = null;
        }

        #endregion 

        #region TestConstants

        private const int TestWorkspaceArtifactId = 123;
        private const int TestRdoArtifactId = 456;
        private const string TestName = "name";
        private const string TestPhone = "1234567890";
        private const string TestEmail = "email@email.com";

        #endregion

        #region Tests

        [Test]
        public void SaveNewValuesTest_GoldenFlow()
        {
            //Arrange
            Mock_MockApiLog_LogError_Works();
            Mock_RdoRepository_CreateSingle_Works();
            NewRdoModel newRdoModel = new NewRdoModel
            {
                WorkspaceArtifactId = TestWorkspaceArtifactId,
                Name = TestName,
                Phone = TestPhone,
                Email = TestEmail
            };

            //Act
            //Assert
            Assert.DoesNotThrowAsync(async () => await Sut.SaveNewValuesAsync(newRdoModel));
            Verify_Mock_RdoRepository_CreateSingle_Works_Was_Called(1);
            Verify_Mock_MockApiLog_LogError_Works_Was_Called(0);
        }

        [Test]
        public void SaveNewValuesTest_Rsapi_Fails()
        {
            //Arrange
            Mock_MockApiLog_LogError_Works();
            Mock_RdoRepository_CreateSingle_Throws_Exception();
            NewRdoModel newRdoModel = new NewRdoModel
            {
                WorkspaceArtifactId = TestWorkspaceArtifactId,
                Name = TestName,
                Phone = TestPhone,
                Email = TestEmail
            };

            //Act
            OverrideCustomPageException overrideCustomPageException = Assert.ThrowsAsync<OverrideCustomPageException>(async () => await Sut.SaveNewValuesAsync(newRdoModel));

            //Assert
            StringAssert.Contains(Helpers.Constants.ErrorMessages.NewRdoCreateError, overrideCustomPageException.ToString());
            Verify_Mock_RdoRepository_CreateSingle_Works_Was_Called(1);
            Verify_Mock_MockApiLog_LogError_Works_Was_Called(2);
        }

        [Test]
        public void SaveNewValuesTest_Argument_Exception_NewRdoModel_Is_Null()
        {
            //Arrange
            Mock_MockApiLog_LogError_Works();
            Mock_RdoRepository_CreateSingle_Works();

            //Act
            ArgumentNullException argumentNullException = Assert.ThrowsAsync<ArgumentNullException>(async () => await Sut.SaveNewValuesAsync(newRdoModel: null));

            //Assert
            StringAssert.Contains(nameof(NewRdoModel), argumentNullException.ToString());
            Verify_Mock_RdoRepository_CreateSingle_Works_Was_Called(0);
            Verify_Mock_MockApiLog_LogError_Works_Was_Called(0);
        }

        [Test]
        public void SaveNewValuesTest_Argument_Exception_WorkspaceArtifactId_Is_Zero()
        {
            //Arrange
            Mock_MockApiLog_LogError_Works();
            Mock_RdoRepository_CreateSingle_Works();
            NewRdoModel newRdoModel = new NewRdoModel
            {
                WorkspaceArtifactId = 0,
                Name = TestName,
                Phone = TestPhone,
                Email = TestEmail
            };

            //Act
            ArgumentException argumentException = Assert.ThrowsAsync<ArgumentException>(async () => await Sut.SaveNewValuesAsync(newRdoModel));

            //Assert
            StringAssert.Contains("not valid", argumentException.ToString());
            Verify_Mock_RdoRepository_CreateSingle_Works_Was_Called(0);
            Verify_Mock_MockApiLog_LogError_Works_Was_Called(0);
        }

        [Test]
        public async Task RetrieveCurrentValuesAsyncTest_GoldenFlow()
        {
            //Arrange
            Mock_MockApiLog_LogError_Works();
            RDO mockRdo = new RDO
            {
                Fields = new List<FieldValue>
                {
                    new FieldValue(Helpers.Constants.Guids.Fields.OverrideRdo.Name) { Value = TestName},
                    new FieldValue(Helpers.Constants.Guids.Fields.OverrideRdo.Phone) { Value = TestPhone},
                    new FieldValue(Helpers.Constants.Guids.Fields.OverrideRdo.Email) { Value = TestEmail}
                }
            };
            Mock_RdoRepository_ReadSingle_Works(mockRdo);

            //Act
            EditRdoModel editRdoModel = await Sut.RetrieveCurrentValuesAsync(TestWorkspaceArtifactId, TestRdoArtifactId);

            //Assert
            Verify_Mock_RdoRepository_ReadSingle_Works_Was_Called(1);
            Verify_Mock_MockApiLog_LogError_Works_Was_Called(0);
            Assert.That(editRdoModel, Is.Not.Null);
            Assert.That(editRdoModel.WorkspaceArtifactId, Is.EqualTo(TestWorkspaceArtifactId));
            Assert.That(editRdoModel.RdoArtifactId, Is.EqualTo(TestRdoArtifactId));
            Assert.That(editRdoModel.Name, Is.EqualTo(TestName));
            Assert.That(editRdoModel.Phone, Is.EqualTo(TestPhone));
            Assert.That(editRdoModel.Email, Is.EqualTo(TestEmail));
        }

        [Test]
        public void UpdateNewValuesAsyncTest_GoldenFlow()
        {
            //Arrange
            EditRdoModel editRdoModel = new EditRdoModel()
            {
                WorkspaceArtifactId = TestWorkspaceArtifactId,
                RdoArtifactId = TestRdoArtifactId,
                Name = TestName,
                Phone = TestPhone,
                Email = TestEmail
            };
            Mock_MockApiLog_LogError_Works();
            Mock_RdoRepository_UpdateSingle_Works();

            //Act
            //Assert
            Assert.DoesNotThrowAsync(async () => await Sut.UpdateNewValuesAsync(editRdoModel));
            Verify_Mock_RdoRepository_UpdateSingle_Works_Was_Called(1);
            Verify_Mock_MockApiLog_LogError_Works_Was_Called(0);
        }

        #endregion

        #region Mocks

        private void Mock_MockApiLog_LogError_Works()
        {
            MockApiLog
                .Setup(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>()));
        }

        private void Mock_RdoRepository_CreateSingle_Works()
        {
            MockRdoRepository
                .Setup(x => x.CreateSingle(It.IsAny<RDO>()));
        }

        private void Mock_RdoRepository_CreateSingle_Throws_Exception()
        {
            MockRdoRepository
                .Setup(x => x.CreateSingle(It.IsAny<RDO>()))
                .Throws<Exception>();
        }

        private void Mock_RdoRepository_ReadSingle_Works(RDO mockRdo)
        {
            MockRdoRepository
                .Setup(x => x.ReadSingle(It.IsAny<int>()))
                .Returns(mockRdo);
        }

        private void Mock_RdoRepository_UpdateSingle_Works()
        {
            MockRdoRepository
                .Setup(x => x.UpdateSingle(It.IsAny<RDO>()));
        }

        #endregion

        #region Verify

        private void Verify_Mock_MockApiLog_LogError_Works_Was_Called(int timesCalled)
        {
            MockApiLog
                .Verify(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>())
                    , Times.Exactly(timesCalled));
        }

        private void Verify_Mock_RdoRepository_CreateSingle_Works_Was_Called(int timesCalled)
        {
            MockRdoRepository
                .Verify(x => x.CreateSingle(It.IsAny<RDO>())
                    , Times.Exactly(timesCalled));
        }

        private void Verify_Mock_RdoRepository_ReadSingle_Works_Was_Called(int timesCalled)
        {
            MockRdoRepository
                .Verify(x => x.ReadSingle(It.IsAny<int>())
                    , Times.Exactly(timesCalled));
        }

        private void Verify_Mock_RdoRepository_UpdateSingle_Works_Was_Called(int timesCalled)
        {
            MockRdoRepository
                .Verify(x => x.UpdateSingle(It.IsAny<RDO>())
                    , Times.Exactly(timesCalled));
        }

        #endregion
    }
}
