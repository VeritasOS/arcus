/******************************************************************************
 * VERITAS:    Copyright (c) 2018 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using System.Collections.Generic;

namespace BEArcus.Agent
{
    /// <summary>
    /// Interface containing methods: 
    /// To save Alert,Job,Job History data.
    /// </summary>
    public interface IStorageService
    {
        void SaveAlertData(List<Alert> alertObject, string storageId);
        void SaveJobData(List<Job> jobObject, string storageId);
        void SaveJobHistoryData(List<JobHistory> jobHistoryObject, string storageId);
        void SaveMediaServerData(MediaServer mediaServerObject, string storageId);
    }
}
