﻿using System;

namespace SimpleDICOMToolkit.Server
{
    /// <summary>
    /// This class contains the most important values that are transmitted per worklist
    /// </summary>
    [Serializable]
    public class WorklistItem
    {
        public string AccessionNumber { get; set; }

        public string PatientID { get; set; }

        public string PatientName { get; set; }

        public string Sex { get; set; }

        public string Age { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string ReferringPhysician { get; set; }

        public string PerformingPhysician { get; set; }

        public string Modality { get; set; }

        public DateTime ExamDateAndTime { get; set; }

        public string ExamRoom { get; set; }

        public string ExamDescription { get; set; }

        public string StudyUID { get; set; }

        public string ProcedureID { get; set; }

        public string ProcedureStepID { get; set; }

        public string HospitalName { get; set; }

        public string ScheduledAET { get; set; }
    }
}