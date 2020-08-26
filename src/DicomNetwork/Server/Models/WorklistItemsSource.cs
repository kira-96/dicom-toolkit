using Dicom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using SimpleDICOMToolkit.Models;

namespace SimpleDICOMToolkit.Server
{
    public class WorklistItemsSource : IWorklistItemsSource, IWorklistRequestHandler
    {
        private readonly object locker = new object();

        public ObservableCollection<WorklistItem> WorklistItems { get; } = new ObservableCollection<WorklistItem>();

        public void AddItem(WorklistItem item)
        {
            lock (locker)
            {
                WorklistItems.Add(item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (locker)
            {
                WorklistItems.RemoveAt(index);
            }
        }

        public void RemoveItem(WorklistItem item)
        {
            lock (locker)
            {
                WorklistItems.Remove(item);
            }
        }

        public IEnumerable<DicomDataset> FilterWorklistItems(DicomDataset request)
        {
            var exams = WorklistItems.AsQueryable();

            if (request.TryGetSingleValue(DicomTag.PatientID, out string patientId) && !string.IsNullOrEmpty(patientId))
            {
                exams = exams.Where(x => x.PatientID == patientId);
            }

            if (request.TryGetSingleValue(DicomTag.PatientName, out string patientName))
            {
                exams = FilterWorklistItemsByName(exams, patientName);
            }

            DicomDataset procedureStep = null;
            if (request.TryGetSequence(DicomTag.ScheduledProcedureStepSequence, out DicomSequence procedureStepSequence) 
                && procedureStepSequence.Items.Any())
            {
                procedureStep = procedureStepSequence.First();

                if (procedureStep.TryGetSingleValue(DicomTag.ScheduledStationAETitle, out string scheduledStationAet) && !string.IsNullOrEmpty(scheduledStationAet))
                {
                    exams = exams.Where(x => x.ScheduledAET == scheduledStationAet);
                }

                if (procedureStep.TryGetSingleValue(DicomTag.Modality, out string modality) && !string.IsNullOrEmpty(modality))
                {
                    exams = exams.Where(x => x.Modality == modality);
                }

                if (procedureStep.TryGetSingleValue(DicomTag.PerformingPhysicianName, out string performingPhysician) && !string.IsNullOrEmpty(performingPhysician))
                {
                    exams = exams.Where(x => x.PerformingPhysician == performingPhysician);
                }

                if (procedureStep.TryGetSingleValue(DicomTag.ScheduledProcedureStepLocation, out string procedureStepLocation) && !string.IsNullOrEmpty(procedureStepLocation))
                {
                    exams = exams.Where(x => x.ExamRoom == procedureStepLocation);
                }

                if (procedureStep.TryGetSingleValue(DicomTag.ScheduledProcedureStepDescription, out string procedureStepDescription) && !string.IsNullOrEmpty(procedureStepDescription))
                {
                    exams = exams.Where(x => x.ExamDescription == procedureStepDescription);
                }

                if (procedureStep.TryGetSingleValue(DicomTag.ScheduledProcedureStepStartDateTime, out string scheduledStartDateTime) && !string.IsNullOrEmpty(scheduledStartDateTime))
                {
                    if (scheduledStartDateTime != "*")
                    {
                        var dateRange = new DicomDateTime(DicomTag.ScheduledProcedureStepStartDate, scheduledStartDateTime).Get<DicomDateRange>();
                        exams = exams.Where(x => dateRange.Contains(x.ExamDateAndTime));
                    }
                }
            }

            var results = exams.AsEnumerable();

            foreach (var item in results)
            {
                var resultDataset = new DicomDataset();

                AddIfExistsInRequest(resultDataset, request, DicomTag.AccessionNumber, item.AccessionNumber);
                AddIfExistsInRequest(resultDataset, request, DicomTag.InstitutionName, item.HospitalName);
                AddIfExistsInRequest(resultDataset, request, DicomTag.ReferringPhysicianName, item.ReferringPhysician);
                AddIfExistsInRequest(resultDataset, request, DicomTag.PatientName, item.PatientName);
                AddIfExistsInRequest(resultDataset, request, DicomTag.PatientID, item.PatientID);
                AddIfExistsInRequest(resultDataset, request, DicomTag.PatientBirthDate, item.DateOfBirth);
                AddIfExistsInRequest(resultDataset, request, DicomTag.PatientAge, item.Age);
                AddIfExistsInRequest(resultDataset, request, DicomTag.PatientSex, item.Sex);
                AddIfExistsInRequest(resultDataset, request, DicomTag.StudyInstanceUID, item.StudyUID);
                AddIfExistsInRequest(resultDataset, request, DicomTag.RequestingPhysician, item.ReferringPhysician);
                AddIfExistsInRequest(resultDataset, request, DicomTag.RequestedProcedureDescription, item.ExamDescription);
                AddIfExistsInRequest(resultDataset, request, DicomTag.RequestedProcedureID, item.ProcedureID);

                if (procedureStep != null)
                {
                    var resultingSps = new DicomDataset();

                    AddIfExistsInRequest(resultingSps, procedureStep, DicomTag.ScheduledStationAETitle, item.ScheduledAET);
                    AddIfExistsInRequest(resultingSps, procedureStep, DicomTag.ScheduledProcedureStepStartDate, item.ExamDateAndTime);
                    AddIfExistsInRequest(resultingSps, procedureStep, DicomTag.ScheduledProcedureStepStartTime, item.ExamDateAndTime);
                    AddIfExistsInRequest(resultingSps, procedureStep, DicomTag.Modality, item.Modality);
                    AddIfExistsInRequest(resultingSps, procedureStep, DicomTag.ScheduledPerformingPhysicianName, item.PerformingPhysician);
                    AddIfExistsInRequest(resultingSps, procedureStep, DicomTag.ScheduledProcedureStepDescription, item.ExamDescription);
                    AddIfExistsInRequest(resultingSps, procedureStep, DicomTag.ScheduledProcedureStepID, item.ProcedureStepID);
                    AddIfExistsInRequest(resultingSps, procedureStep, DicomTag.ScheduledStationName, item.ExamRoom);
                    AddIfExistsInRequest(resultingSps, procedureStep, DicomTag.ScheduledProcedureStepLocation, item.ExamRoom);

                    resultDataset.Add(new DicomSequence(DicomTag.ScheduledProcedureStepSequence, resultingSps));
                }

                // Put blanks in for unsupported fields which are type 2 (i.e. must have a value even if NULL)
                // In a real server, you may wish to support some or all of these, but they are not commonly supported
                AddIfExistsInRequest(resultDataset, request, DicomTag.ReferencedStudySequence, new DicomDataset());
                AddIfExistsInRequest(resultDataset, request, DicomTag.Priority, string.Empty);
                AddIfExistsInRequest(resultDataset, request, DicomTag.PatientTransportArrangements, string.Empty);
                AddIfExistsInRequest(resultDataset, request, DicomTag.AdmissionID, string.Empty);
                AddIfExistsInRequest(resultDataset, request, DicomTag.CurrentPatientLocation, string.Empty);
                AddIfExistsInRequest(resultDataset, request, DicomTag.ReferencedPatientSequence, new DicomDataset());
                AddIfExistsInRequest(resultDataset, request, DicomTag.PatientWeight, string.Empty);
                AddIfExistsInRequest(resultDataset, request, DicomTag.ConfidentialityConstraintOnPatientDataDescription, string.Empty);

                // Send Reponse Back
                yield return resultDataset;
            }
        }

        internal IQueryable<WorklistItem> FilterWorklistItemsByName(IQueryable<WorklistItem> exams, string patientName)
        {
            if (string.IsNullOrEmpty(patientName) || patientName == "*")
            {
                return exams;
            }

            if (patientName.Contains('*'))
            {
                Regex regex = new Regex("^" + Regex.Escape(patientName).Replace("\\*", ".*") + "$");
                exams = exams.Where(x => regex.IsMatch(x.PatientName));
            }
            else
            {
                exams = exams.Where(x => x.PatientName == patientName);
            }

            return exams;
        }

        internal void AddIfExistsInRequest<T>(DicomDataset dataset, DicomDataset request, DicomTag tag, T value)
        {
            if (request.Contains(tag))
            {
                if (value == null)
                {
                    value = default;
                }

                dataset.AddOrUpdate(tag, value);
            }
        }
    }
}
