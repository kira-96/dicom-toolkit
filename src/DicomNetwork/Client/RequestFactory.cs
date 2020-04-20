using Dicom;
using Dicom.Network;

namespace SimpleDICOMToolkit.Client
{
    public static class RequestFactory
    {
        /// <summary>
        /// Convenience method for creating a C-FIND modality worklist query.
        /// from https://github.com/fo-dicom/fo-dicom/blob/development/DICOM/Network/DicomCFindRequest.cs
        /// </summary>
        /// <param name="patientId">Patient ID.</param>
        /// <param name="patientName">Patient name.</param>
        /// <param name="stationAE">Scheduled station Application Entity Title.</param>
        /// <param name="stationName">Scheduled station name.</param>
        /// <param name="modality">Modality.</param>
        /// <param name="scheduledDateTime">Scheduled procedure step start time.</param>
        /// <returns>C-FIND modality worklist query object.</returns>
        public static DicomCFindRequest CreateWorklistQuery(
            string patientId = null,
            string patientName = null,
            string stationAE = null,
            string stationName = null,
            string modality = null,
            DicomDateRange scheduledDateTime = null)
        {
            DicomCFindRequest dimse = DicomCFindRequest.CreateWorklistQuery(patientId, patientName, stationAE, stationName, modality, scheduledDateTime);
            dimse.Dataset.Add(DicomTag.PatientAge, string.Empty);
            
            return dimse;
        }

        /// <summary>
        /// Convenience method for creating a C-FIND patient query.
        /// </summary>
        /// <param name="patientId">Patient ID</param>
        /// <param name="patientName">Patient name.</param>
        /// <returns>C-FIND patient query object.</returns>
        public static DicomCFindRequest CreatePatientQuery(string patientId = null, string patientName = null)
        {
            DicomCFindRequest dimse = DicomCFindRequest.CreatePatientQuery(patientId, patientName);

            return dimse;
        }

        /// <summary>
        /// Convenience method for creating a C-FIND study query.
        /// </summary>
        /// <param name="patientId">Patient ID.</param>
        /// <param name="patientName">Patient name.</param>
        /// <param name="studyDateTime">Time range of studies.</param>
        /// <param name="accession">Accession number.</param>
        /// <param name="studyId">Study ID.</param>
        /// <param name="modalitiesInStudy">Modalities in study.</param>
        /// <param name="studyInstanceUid">Study instance UID.</param>
        /// <returns>C-FIND study query object.</returns>
        public static DicomCFindRequest CreateStudyQuery(
            string patientId = null,
            string patientName = null,
            DicomDateRange studyDateTime = null,
            string accession = null,
            string studyId = null,
            string modalitiesInStudy = null,
            string studyInstanceUid = null)
        {
            DicomCFindRequest dimse = DicomCFindRequest.CreateStudyQuery(patientId, patientName, studyDateTime, accession, studyId, modalitiesInStudy, studyInstanceUid);

            return dimse;
        }

        /// <summary>
        /// Convenience method for creating a C-FIND series query.
        /// </summary>
        /// <param name="studyInstanceUid">Study instance UID.</param>
        /// <param name="modality">Modality.</param>
        /// <returns>C-FIND series query object.</returns>
        public static DicomCFindRequest CreateSeriesQuery(string studyInstanceUid, string modality = null)
        {
            DicomCFindRequest dimse = DicomCFindRequest.CreateSeriesQuery(studyInstanceUid, modality);

            return dimse;
        }

        /// <summary>
        /// Convenience method for creating a C-FIND image query.
        /// </summary>
        /// <param name="studyInstanceUid">Study instance UID.</param>
        /// <param name="seriesInstanceUid">Series instance UID.</param>
        /// <param name="modality">Modality.</param>
        /// <returns>C-FIND image query object.</returns>
        public static DicomCFindRequest CreateImageQuery(
            string studyInstanceUid,
            string seriesInstanceUid,
            string modality = null)
        {
            DicomCFindRequest dimse = DicomCFindRequest.CreateImageQuery(studyInstanceUid, seriesInstanceUid, modality);

            return dimse;
        }

        public static DicomCGetRequest CreateCGetBySeriesUID(string studyInstanceUID, string seriesInstanceUID, DicomPriority priority = DicomPriority.Medium)
        {
            var request = new DicomCGetRequest(studyInstanceUID, seriesInstanceUID, priority);
            // no more dicomtags have to be set
            return request;
        }

        public static DicomCGetRequest CreateCGetBySOPInstanceUID(string studyInstanceUID, string seriesInstanceUID, string sopInstanceUID, DicomPriority priority = DicomPriority.Medium)
        {
            var request = new DicomCGetRequest(studyInstanceUID, seriesInstanceUID, sopInstanceUID, priority);
            // no more dicomtags have to be set
            return request;
        }

        public static DicomCMoveRequest CreateCMoveByStudyUID(string destAET, string studyInstanceUID, DicomPriority priority = DicomPriority.Medium)
        {
            var request = new DicomCMoveRequest(destAET, studyInstanceUID, priority);
            // no more dicomtags have to be set
            return request;
        }

        public static DicomCMoveRequest CreateCMoveBySeriesUID(string destAET, string studyInstanceUID, string seriesInstanceUID, DicomPriority priority = DicomPriority.Medium)
        {
            var request = new DicomCMoveRequest(destAET, studyInstanceUID, seriesInstanceUID, priority);
            // no more dicomtags have to be set
            return request;
        }
    }
}
