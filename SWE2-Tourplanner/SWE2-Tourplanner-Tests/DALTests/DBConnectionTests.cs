using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using DataAccessLayer.DBConnection;
using Common.Config;
using DataAccessLayer.Exceptions;

namespace SWE2_Tourplanner_Tests.DALTests
{
    class DBConnectionTests
    {
        private IDBConnection db;
        private Mock<IDbCommand> mockNpgsqlCommand;
        private Mock<IDataReader> mockNpgsqlDataReader;
        private Mock<ITourPlannerConfig> mockConfig;

        [SetUp]
        public void Setup()
        {
            mockConfig = new Mock<ITourPlannerConfig>();
            mockConfig.Setup(mc => mc.DatabaseConnectionString).Returns("Host=localhost;Port=5432;Username=test;Password=test;Database=test;");
            mockConfig.Setup(mc => mc.ExportsDirectory).Returns("Mock exports directory");
            mockConfig.Setup(mc => mc.MapQuestKey).Returns("Mock map quest key");
            mockConfig.Setup(mc => mc.PictureDirectory).Returns("Mock picture directory");
            db = DatabaseConnection.GetDBConnection(mockConfig.Object);
            mockNpgsqlDataReader = new Mock<IDataReader>();
            mockNpgsqlDataReader.SetupSequence(dr => dr.Read()).Returns(true).Returns(true).Returns(false);
            mockNpgsqlDataReader.SetupSequence(dr => dr.GetValue(It.IsAny<int>())).Returns("Mock");
            mockNpgsqlDataReader.Setup(dr => dr.FieldCount).Returns(2);
            mockNpgsqlCommand = new Mock<IDbCommand>();
            mockNpgsqlCommand.Setup(nc => nc.ExecuteReader()).Returns(mockNpgsqlDataReader.Object);
            mockNpgsqlCommand.Setup(nc => nc.ExecuteNonQuery()).Returns(1);
        }

        [Test]
        public void ExecuteStatementCorrectCallsMock()
        {
            //arrange
            //act
            int affectedRows = db.ExecuteStatement(mockNpgsqlCommand.Object);
            //assert
            Assert.AreEqual(1, affectedRows);
            mockNpgsqlCommand.VerifySet(nc=>nc.Connection = It.IsAny<Npgsql.NpgsqlConnection>(), Times.Once);
            mockNpgsqlCommand.Verify(nc => nc.ExecuteNonQuery(), Times.Once);
        }

        [Test]
        public void QueryDatabaseCorrectCallsMock()
        {
            //arrange
            //act
            List<object[]> results = db.QueryDatabase(mockNpgsqlCommand.Object);
            //assert
            mockNpgsqlCommand.VerifySet(nc => nc.Connection = It.IsAny<Npgsql.NpgsqlConnection>(), Times.Once);
            mockNpgsqlCommand.Verify(nc => nc.ExecuteReader(), Times.Once);
            mockNpgsqlDataReader.Verify(dr => dr.Read(), Times.Exactly(3));
            mockNpgsqlDataReader.Verify(dr => dr.FieldCount, Times.Exactly(2));
            mockNpgsqlDataReader.Verify(dr => dr.GetValue(It.IsAny<int>()), Times.Exactly(4));
            Assert.AreEqual(2, results.Count);
            foreach(object[] row in results)
            {
                Assert.AreEqual(2, row.Length);
            }
        }
        //Because DeclareParameter is called in DefineParameter, the methods are tested together.
        [Test]
        public void DefineParameterArgumentExceptionThrownTest()
        {
            //arrange
            IDbCommand command = new NpgsqlCommand("DELETE FROM test WHERE id=@id;");
            //act
            db.DefineParameter(command, "id", System.Data.DbType.String, "1");
            //assert
            Assert.Throws<DALParameterException>(() =>db.DefineParameter(command,"id",System.Data.DbType.String,"2"));
        }

        [Test]
        public void OpenConnectionPostgresExceptionThrownTest()
        {
            //arrange
            //act & assert - Throws exception due to incorrect credentials.
            Assert.Throws<DALDBConnectionException>(()=>db.OpenConnection());
        }
    }
}
