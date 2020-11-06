using LiteDB;
using Stylet;
using System;
using SimpleDICOMToolkit.Infrastructure;

namespace SimpleDICOMToolkit.Models
{

    /// <summary>
    /// This class contains the most important values that are transmitted per worklist
    /// </summary>
    public class WorklistItem : PropertyChangedBase, IWorklistItem
    {
        public ObjectId Id { get; }

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

        public MppsStatus MppsStatus { get; set; } = MppsStatus.Waiting;

        public WorklistItem()
        {
            Id = ObjectId.NewObjectId();
        }

        [BsonCtor]
        public WorklistItem(
            ObjectId _id,
            string accessionNumber,
            string patientId,
            string patientName,
            string sex,
            string age,
            DateTime dateOfBirth,
            string referringPhysician,
            string performingPhysician,
            string modality,
            DateTime examDateAndTime,
            string examRoom,
            string examDescription,
            string studyUID,
            string procedureID,
            string procedureStepID,
            string hospitalName,
            string scheduledAET)
        {
            Id = _id;
            AccessionNumber = accessionNumber;
            PatientID = patientId;
            PatientName = patientName;
            Sex = sex;
            Age = age;
            DateOfBirth = dateOfBirth;
            ReferringPhysician = referringPhysician;
            PerformingPhysician = performingPhysician;
            Modality = modality;
            ExamDateAndTime = examDateAndTime;
            ExamRoom = examRoom;
            ExamDescription = examDescription;
            StudyUID = studyUID;
            ProcedureID = procedureID;
            ProcedureStepID = procedureStepID;
            HospitalName = hospitalName;
            ScheduledAET = scheduledAET;
        }

        public void UpdateStatus(MppsStatus status)
        {
            MppsStatus = status;
            NotifyOfPropertyChange(() => MppsStatus);
        }

        public BsonDocument ToBsonDocument()
        {
            return new BsonDocument()
            {
                ["_id"] = Id,
                ["AccessionNumber"] = AccessionNumber,
                ["PatientID"] = PatientID,
                ["PatientName"] = PatientName,
                ["Sex"] = Sex,
                ["Age"] = Age,
                ["DateOfBirth"] = DateOfBirth,
                ["ReferringPhysician"] = ReferringPhysician,
                ["PerformingPhysician"] = PerformingPhysician,
                ["Modality"] = Modality,
                ["ExamDateAndTime"] = ExamDateAndTime,
                ["ExamRoom"] = ExamRoom,
                ["ExamDescription"] = ExamDescription,
                ["StudyUID"] = StudyUID,
                ["ProcedureID"] = ProcedureID,
                ["ProcedureStepID"] = ProcedureStepID,
                ["HospitalName"] = HospitalName,
                ["ScheduledAET"] = ScheduledAET
            };
        }
    }
}
