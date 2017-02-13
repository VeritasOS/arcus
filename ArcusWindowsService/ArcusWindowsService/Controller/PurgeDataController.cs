/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

namespace BEArcus.Agent
{
    class PurgeDataController
    {

        public static void PurgeAlerts(string documentDBName)
        {
            DocumentDBDataController db = new DocumentDBDataController();
            db.CreateAlertStoredProcedure(documentDBName).Wait();
            db.Execute_spBulkDeleteAlerts(documentDBName).Wait();
            LogUtility.LogInfoFunctionFinished();
        }

        public static void PurgeJobHistories(string documentDBName)
        {
            DocumentDBDataController db = new DocumentDBDataController();
            db.CreateJobHistoryStoredProcedure(documentDBName).Wait();
            db.Execute_spBulkDeleteJobHistories(documentDBName).Wait();
            LogUtility.LogInfoFunctionFinished();
        }
    }
}
