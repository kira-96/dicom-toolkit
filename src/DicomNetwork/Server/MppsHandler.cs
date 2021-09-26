// https://github.com/fo-dicom/fo-dicom-samples/blob/master/Desktop/Worklist%20SCP/Model/MppsHandler.cs

// Copyright (c) 2012-2020 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

using System.Collections.Generic;
using System.Linq;
using SimpleDICOMToolkit.Infrastructure;

namespace SimpleDICOMToolkit.Server
{
    /// <summary>
    /// An implementation of IMppsSource, that does only logging but does not store the MPPS messages
    /// </summary>
    public class MppsHandler : IMppsSource
    {
        private readonly Dictionary<string, IWorklistItem> PendingProcedures = new Dictionary<string, IWorklistItem>();

        private readonly IEnumerable<IWorklistItem> _worklistItems;

        private readonly FellowOakDicom.Log.ILogger _logger;

        public MppsHandler(IEnumerable<IWorklistItem> worklistItems, FellowOakDicom.Log.ILogger logger)
        {
            _worklistItems = worklistItems;
            _logger = logger;
        }

        public bool SetInProgress(string sopInstanceUID, string procedureStepId)
        {
            var workItem = _worklistItems.FirstOrDefault(w => w.ProcedureStepID == procedureStepId);
            if (workItem == null)
            {
                // the procedureStepId provided cannot be found any more, so the data is invalid or the 
                // modality tries to start a procedure that has been deleted/changed on the ris side...
                return false;
            }

            // now here change the sate of the procedure in the database or do similar stuff...
            _logger.Info($"Procedure with id {workItem.ProcedureStepID} of Patient {workItem.PatientName} is started");

            // remember the sopInstanceUID and store the worklistitem to which the sopInstanceUID belongs. 
            // You should do this more permanent like in database or in file
            PendingProcedures.Add(sopInstanceUID, workItem);
            workItem.UpdateStatus(MppsStatus.InProgress);
            return true;
        }


        public bool SetDiscontinued(string sopInstanceUID, string reason)
        {
            if (!PendingProcedures.ContainsKey(sopInstanceUID))
            {
                // there is no pending procedure with this sopInstanceUID!
                return false;
            }
            var workItem = PendingProcedures[sopInstanceUID];

            // now here change the sate of the procedure in the database or do similar stuff...
            _logger.Info($"Procedure with id {workItem.ProcedureStepID} of Patient {workItem.PatientName} is discontinued for reason {reason}");

            // since the procedure was stopped, we remove it from the list of pending procedures
            PendingProcedures.Remove(sopInstanceUID);
            workItem.UpdateStatus(MppsStatus.Discontinued);
            return true;
        }


        public bool SetCompleted(string sopInstanceUID, string doseDescription, List<string> affectedInstanceUIDs)
        {
            if (!PendingProcedures.ContainsKey(sopInstanceUID))
            {
                // there is no pending procedure with this sopInstanceUID!
                return false;
            }
            var workItem = PendingProcedures[sopInstanceUID];

            // now here change the sate of the procedure in the database or do similar stuff...
            _logger.Info($"Procedure with id {workItem.ProcedureStepID} of Patient {workItem.PatientName} is completed");

            // the MPPS completed message contains some additional informations about the performed procedure.
            // this informations are very vendor depending, so read the DICOM Conformance Statement or read
            // the DICOM logfiles to see which informations the vendor sends

            // since the procedure was completed, we remove it from the list of pending procedures
            PendingProcedures.Remove(sopInstanceUID);
            workItem.UpdateStatus(MppsStatus.Completed);
            return true;
        }
    }
}
