using System;

namespace SimpleDICOMToolkit.Infrastructure
{
    public enum MppsStatus
    {
        Waiting,
        InProgress,
        Completed,
        Discontinued
    }

    public interface IWorklistItem
    {
        string AccessionNumber { get; set; }

        string PatientID { get; set; }

        string PatientName { get; set; }

        string Sex { get; set; }

        string Age { get; set; }

        DateTime DateOfBirth { get; set; }

        string ReferringPhysician { get; set; }

        string PerformingPhysician { get; set; }

        string Modality { get; set; }

        DateTime ExamDateAndTime { get; set; }

        string ExamRoom { get; set; }

        string ExamDescription { get; set; }

        string StudyUID { get; set; }

        string ProcedureID { get; set; }

        string ProcedureStepID { get; set; }

        string HospitalName { get; set; }

        string ScheduledAET { get; set; }

        MppsStatus MppsStatus { get; set; }

        void UpdateStatus(MppsStatus status);
    }
}
