using FellowOakDicom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SimpleDICOMToolkit.Infrastructure;

namespace SimpleDICOMToolkit.Server
{
    public class CFindRequestHandler
    {
        /// <summary>
        /// Filter worklist items from request
        /// </summary>
        /// <param name="worklistItems">all worklist items from local database</param>
        /// <param name="request">request dataset</param>
        /// <param name="fallbackEncoding">Encoding to use if encoding cannot be obtained from dataset</param>
        /// <returns>response datasets</returns>
        public static IEnumerable<DicomDataset> FilterWorklistItems(IEnumerable<IWorklistItem> worklistItems, DicomDataset request, Encoding fallbackEncoding = null)
        {
            Encoding encoding = fallbackEncoding ?? Encoding.UTF8;

            if (request.TryGetValue(DicomTag.SpecificCharacterSet, 0, out string charset))
            {
                encoding = DicomEncoding.GetEncoding(charset);
            }

            var exams = worklistItems.AsQueryable();

            if (request.TryGetSingleValue(DicomTag.PatientID, out string patientId) && !string.IsNullOrEmpty(patientId))
            {
                exams = exams.Where(x => x.PatientID == patientId);
            }

            string patientName = GetStringOrDefault(request, DicomTag.PatientName, encoding, string.Empty);

            if (!string.IsNullOrEmpty(patientName))
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

                string performingPhysician = GetStringOrDefault(procedureStep, DicomTag.PerformingPhysicianName, encoding, string.Empty);

                if (!string.IsNullOrEmpty(performingPhysician))
                {
                    exams = exams.Where(x => x.PerformingPhysician == performingPhysician);
                }

                string procedureStepLocation = GetStringOrDefault(procedureStep, DicomTag.ScheduledProcedureStepLocation, encoding, string.Empty);

                if (!string.IsNullOrEmpty(procedureStepLocation))
                {
                    exams = exams.Where(x => x.ExamRoom == procedureStepLocation);
                }

                string procedureStepDescription = GetStringOrDefault(procedureStep, DicomTag.ScheduledProcedureStepDescription, encoding, string.Empty);

                if (!string.IsNullOrEmpty(procedureStepDescription))
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
                var resultDataset = new DicomDataset()
                {
                    { DicomTag.SpecificCharacterSet, DicomEncoding.GetCharset(encoding) },
                };

                AddIfExistsInRequest(resultDataset, request, DicomTag.AccessionNumber, encoding, item.AccessionNumber);
                AddIfExistsInRequest(resultDataset, request, DicomTag.InstitutionName, encoding, item.HospitalName);
                AddIfExistsInRequest(resultDataset, request, DicomTag.ReferringPhysicianName, encoding, item.ReferringPhysician);
                AddIfExistsInRequest(resultDataset, request, DicomTag.PatientName, encoding, item.PatientName);
                AddIfExistsInRequest(resultDataset, request, DicomTag.PatientID, encoding, item.PatientID);
                AddIfExistsInRequest(resultDataset, request, DicomTag.PatientBirthDate, encoding, item.DateOfBirth);
                AddIfExistsInRequest(resultDataset, request, DicomTag.PatientAge, encoding, item.Age);
                AddIfExistsInRequest(resultDataset, request, DicomTag.PatientSex, encoding, item.Sex);
                AddIfExistsInRequest(resultDataset, request, DicomTag.StudyInstanceUID, encoding, item.StudyUID);
                AddIfExistsInRequest(resultDataset, request, DicomTag.RequestingPhysician, encoding, item.ReferringPhysician);
                AddIfExistsInRequest(resultDataset, request, DicomTag.RequestedProcedureDescription, encoding, item.ExamDescription);
                AddIfExistsInRequest(resultDataset, request, DicomTag.RequestedProcedureID, encoding, item.ProcedureID);

                if (procedureStep != null)
                {
                    var resultingSps = new DicomDataset();

                    AddIfExistsInRequest(resultingSps, procedureStep, DicomTag.ScheduledStationAETitle, encoding, item.ScheduledAET);
                    AddIfExistsInRequest(resultingSps, procedureStep, DicomTag.ScheduledProcedureStepStartDate, encoding, item.ExamDateAndTime);
                    AddIfExistsInRequest(resultingSps, procedureStep, DicomTag.ScheduledProcedureStepStartTime, encoding, item.ExamDateAndTime);
                    AddIfExistsInRequest(resultingSps, procedureStep, DicomTag.Modality, encoding, item.Modality);
                    AddIfExistsInRequest(resultingSps, procedureStep, DicomTag.ScheduledPerformingPhysicianName, encoding, item.PerformingPhysician);
                    AddIfExistsInRequest(resultingSps, procedureStep, DicomTag.ScheduledProcedureStepDescription, encoding, item.ExamDescription);
                    AddIfExistsInRequest(resultingSps, procedureStep, DicomTag.ScheduledProcedureStepID, encoding, item.ProcedureStepID);
                    AddIfExistsInRequest(resultingSps, procedureStep, DicomTag.ScheduledStationName, encoding, item.ExamRoom);
                    AddIfExistsInRequest(resultingSps, procedureStep, DicomTag.ScheduledProcedureStepLocation, encoding, item.ExamRoom);

                    resultDataset.Add(new DicomSequence(DicomTag.ScheduledProcedureStepSequence, resultingSps));
                }

                // Put blanks in for unsupported fields which are type 2 (i.e. must have a value even if NULL)
                // In a real server, you may wish to support some or all of these, but they are not commonly supported
                AddIfExistsInRequest(resultDataset, request, DicomTag.ReferencedStudySequence, encoding, new DicomDataset());
                AddIfExistsInRequest(resultDataset, request, DicomTag.Priority, encoding, string.Empty);
                AddIfExistsInRequest(resultDataset, request, DicomTag.PatientTransportArrangements, encoding, string.Empty);
                AddIfExistsInRequest(resultDataset, request, DicomTag.AdmissionID, encoding, string.Empty);
                AddIfExistsInRequest(resultDataset, request, DicomTag.CurrentPatientLocation, encoding, string.Empty);
                AddIfExistsInRequest(resultDataset, request, DicomTag.ReferencedPatientSequence, encoding, new DicomDataset());
                AddIfExistsInRequest(resultDataset, request, DicomTag.PatientWeight, encoding, string.Empty);
                AddIfExistsInRequest(resultDataset, request, DicomTag.ConfidentialityConstraintOnPatientDataDescription, encoding, string.Empty);

                // Send Reponse Back
                yield return resultDataset;
            }
        }

        internal static IQueryable<IWorklistItem> FilterWorklistItemsByName(IQueryable<IWorklistItem> exams, string patientName)
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

        internal static string GetStringOrDefault(DicomDataset dataset, DicomTag tag, Encoding encoding, string defaultValue)
        {
            return dataset.Contains(tag) ? encoding.GetString(dataset.GetValues<byte>(tag)) : defaultValue;
        }

        internal static void AddIfExistsInRequest<T>(DicomDataset dataset, DicomDataset request, DicomTag tag, Encoding encoding, T value)
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
