using BusinessLogicLayer.Exceptions;
using Common.Entities;
using Common.Logging;
using DataAccessLayer.Exceptions;
using DataAccessLayer.UnitOfWork;
using log4net;
using System;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Factories
{
    /// <summary>
    /// TourLogFactory is a concrete implementation of ITourLogFactory.
    /// </summary>
    public class TourLogFactory:ITourLogFactory
    {
        /// <summary>
        /// ILog instance used for logging errors, warnings etc.
        /// </summary>
        private ILog logger;
        /// <summary>
        /// IUnitOfWork instance used for accessing the datastore
        /// </summary>
        private IUnitOfWork uow;
        /// <summary>
        /// Default constructor of TourLogFactory
        /// </summary>
        public TourLogFactory()
        {
            logger = LogHelper.GetLogHelper().GetLogger();
            uow = new UnitOfWork();
        }

        public TourLogFactory(IUnitOfWork uow)
        {
            logger = LogHelper.GetLogHelper().GetLogger();
            this.uow = uow;
        }
        /// <summary>
        /// CreateTourLog is a method for crating new TourLog entities in the datastore
        /// </summary>
        /// <param name="tourLog">TourLog to be created</param>
        /// <returns>Task, which creates a new TourLog entity in the datastore</returns>
        /// <exception cref="BLFactoryException">Thrown, when there are DAL exceptions or other unhandled errors</exception>
        public Task CreateTourLog(TourLog tourLog) => Task.Run(() =>
        {
            try
            {
                using (uow)
                {
                    uow.TourLogRepository.Insert(tourLog);

                    if (uow.Commit() != 1)
                    {
                        uow.Rollback();
                        logger.Error("The amount of affected rows was not 1. Rollback to ensure data consistency.");
                    }
                }
            }
            catch (Exception e)
            {
                if (e is DALDBConnectionException || e is DALParameterException || e is DALRepositoryCommandException || e is DALUnitOfWorkException)
                {
                    logger.Error($"Tourlog could not be created. Details: {e.Message}");
                    throw new BLFactoryException($"Tourlog data could not be created due to the following reason: {e.Message}");
                }
                logger.Error($"An unhandled exception ocurred whilst creating the tourlog! {e.GetType()}: {e.Message}");
                throw new BLFactoryException("The tourlog could not be created! Look up the logs for further information!");
            }

        });
        /// <summary>
        /// UpdateTourLog is a method for updating TourLog entities in the datastore
        /// </summary>
        /// <param name="tourLog">Updated TourLog entity</param>
        /// <returns>Task, which updates a TourLog entity in the database</returns>
        /// <exception cref="BLFactoryException">Thrown, when there are DAL exceptions or if there are other unhandled errors</exception>
        public Task UpdateTourLog(TourLog tourLog) => Task.Run(() =>
        {
            try
            {
                using (uow)
                {
                    uow.TourLogRepository.Update(tourLog);

                    if (uow.Commit() != 1)
                    {
                        uow.Rollback();
                        logger.Error("The amount of affected rows was not 1. Rollback to ensure data consistency.");
                    }
                }
            }
            catch (Exception e)
            {
                if (e is DALDBConnectionException || e is DALParameterException || e is DALRepositoryCommandException || e is DALUnitOfWorkException)
                {
                    logger.Error($"Tourlog could not be updated. Details: {e.Message}");
                    throw new BLFactoryException($"Tourlog data could not be updated due to the following reason: {e.Message}");
                }
                logger.Error($"An unhandled exception ocurred whilst updating the tourlog! {e.GetType()}: {e.Message}");
                throw new BLFactoryException("The tourlog could not be updated! Look up the logs for further information!");
            }
        });
        /// <summary>
        /// DeleteTourLog is a method for deleting TourLog entities in the datastore
        /// </summary>
        /// <param name="tourLog">TourLog to be deleted</param>
        /// <returns>Task, which deletes a TourLog entity in the datastore</returns>
        /// <exception cref="BLFactoryException">Thrown, when there are DAL exceptions or other unhandled errors</exception>
        public Task DeleteTourLog(TourLog tourLog) => Task.Run(() =>
        {
            try
            {
                using (uow)
                {
                    uow.TourLogRepository.Delete(tourLog.Id);
                    if (uow.Commit() != 1)
                    {
                        uow.Rollback();
                        logger.Error("Delete statement affected more than 1 row. Reroll to ensure data consistency.");
                    }
                }
            }
            catch (Exception e)
            {
                if (e is DALDBConnectionException || e is DALParameterException || e is DALRepositoryCommandException || e is DALUnitOfWorkException)
                {
                    logger.Error($"Tourlog could not be deleted. Details: {e.Message}");
                    throw new BLFactoryException($"Tourlog data could not be deleted due to the following reason: {e.Message}");
                }
                logger.Error($"An unhandled exception ocurred whilst deleting the tourlog! {e.GetType()}: {e.Message}");
                throw new BLFactoryException("The tourlog could not be deleted! Look up the logs for further information!");
            }

        });
    }
}
