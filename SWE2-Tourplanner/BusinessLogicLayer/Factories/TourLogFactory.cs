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
    public class TourLogFactory:ITourLogFactory
    {
        private ILog logger;
        private IUnitOfWork uow;
        public TourLogFactory()
        {
            logger = LogHelper.GetLogHelper().GetLogger();
            uow = new UnitOfWork();
        }
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
