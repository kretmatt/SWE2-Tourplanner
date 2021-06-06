using DataAccessLayer.UnitOfWork;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using DataAccessLayer.DBConnection;
using DataAccessLayer.DBCommands;

namespace SWE2_Tourplanner_Tests.DALTests
{
    class UnitOfWorkTests
    {
        IUnitOfWork uow;
        Mock<IDBConnection> db;
        List<IDBCommand> commitCommands;
        List<IDBCommand> rollbackCommands;
        List<Mock<IDBCommand>> mockCommitCommands;
        List<Mock<IDBCommand>> mockRollbackCommands;

        [SetUp]
        public void Setup()
        {
            //Setup of mock database connection
            db = new Mock<IDBConnection>();
            db.Setup(d => d.ExecuteStatement(It.IsAny<IDbCommand>())).Returns(1);
            db.Setup(d => d.QueryDatabase(It.IsAny<IDbCommand>())).Returns(new List<object[]>());
            //Setup of mock commit commands to verify correct calls in Commit function
            commitCommands = new List<IDBCommand>();
            mockCommitCommands = new List<Mock<IDBCommand>>();
            Mock<IDBCommand> mockCommitCommand = new Mock<IDBCommand>();
            mockCommitCommand.Setup(mcc => mcc.Execute()).Returns(1);
            mockCommitCommand.Setup(mcc => mcc.Undo()).Returns(1);
            commitCommands.Add(mockCommitCommand.Object);
            mockCommitCommands.Add(mockCommitCommand);
            //Setup of mock rollback commands to verify correct calls in Rollback function
            rollbackCommands = new List<IDBCommand>();
            mockRollbackCommands = new List<Mock<IDBCommand>>();
            Mock<IDBCommand> mockRollbackCommand = new Mock<IDBCommand>();
            mockRollbackCommand.Setup(mrc => mrc.Execute()).Returns(1);
            mockRollbackCommand.Setup(mrc => mrc.Undo()).Returns(1);
            rollbackCommands.Add(mockRollbackCommand.Object);
            mockRollbackCommands.Add(mockRollbackCommand);
            //Pass mock objects to unit of work
            uow = new UnitOfWork(db.Object,commitCommands,rollbackCommands);
        }

        // The following calls are expected in the commit function: OpenConnection()*1, CloseConnection()*1, Execute()*n (n=Count of commit command list)
        [Test]
        public void CommitCorrectCallsMock()
        {
            //arrange is in Setup()
            //act
            uow.Commit();
            //assert
            db.Verify(d => d.OpenConnection(), Times.Once);
            db.Verify(d => d.CloseConnection(), Times.Once);
            foreach(Mock<IDBCommand> mcc in mockCommitCommands)
            {
                mcc.Verify(m => m.Execute(), Times.Once);
            }
        }
        // The following calls are expected in the rollback function: OpenConnection()*1, CloseConnection()*1, Undo()*n (n=Count of rollback command list)
        [Test]
        public void RollbackCorrectCallsMock()
        {
            //arrange is in Setup()
            //act
            uow.Rollback();
            //assert
            db.Verify(d => d.OpenConnection(), Times.Once);
            db.Verify(d => d.CloseConnection(), Times.Once);
            foreach(Mock<IDBCommand> mrc in mockRollbackCommands)
            {
                mrc.Verify(m => m.Undo(), Times.Once);
            }
        }

        // The following calls are expected in the commit and rollback functions: OpenConnection()*2, CloseConnection()*2, Execute()*n (n=mockCommitCommands.Count), Undo()*m (m=mockCommitCommands.Count+mockRollbackCommands.Count)
        [Test]
        public void CommitAndRollbackCorrectCallsMock()
        {
            //arrange is in Setup()
            //act
            uow.Commit();
            uow.Rollback();
            //assert
            db.Verify(d => d.OpenConnection(), Times.Exactly(2));
            db.Verify(d => d.CloseConnection(), Times.Exactly(2));
            //After Execute() is called in the UnitOfWork-class, the command is transferred to the rollback commands. That's the reason why the commit commands need to be checked for Execute() and Undo()
            foreach (Mock<IDBCommand> mcc in mockCommitCommands)
            {
                mcc.Verify(m => m.Execute(), Times.Once);
                mcc.Verify(m => m.Undo(), Times.Once);
            }
            foreach (Mock<IDBCommand> mrc in mockRollbackCommands)
            {
                mrc.Verify(m => m.Undo(), Times.Once);
            }
        }
    }
}
