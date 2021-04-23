using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Windows;

namespace UIDGenerator
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // Auto generated
        //var uids = DicomUID.Enumerate()
        //    .Where(x => (x.Type == DicomUidType.SOPClass || x.Type == DicomUidType.TransferSyntax)
        //        && !x.Name.Contains("Retired") && !x.Name.Contains("Private"))
        //    .OrderBy(x => x.Name)
        //    .ToList();

        //using (FileStream fs = new FileStream("DicomUids.cs", FileMode.Create, FileAccess.Write))
        //{
        //    using (StreamWriter writer = new StreamWriter(fs))
        //    {
        //        writer.WriteLine("Dictionary<string, string> myDict = new Dictionary<string, string>()");
        //        writer.WriteLine("{");

        //        foreach (var uid in uids)
        //        {
        //            writer.WriteLine("    {{ \"{0}\", \"{1}\" }},", uid.Name, uid.UID);
        //        }

        //        writer.WriteLine("}");
        //    }
        //}
        private readonly Dictionary<string, string> myDict = new Dictionary<string, string>()
        {
            { "12-lead ECG Waveform Storage", "1.2.840.10008.5.1.4.1.1.9.1.1" },
            { "Acquisition Context SR Storage", "1.2.840.10008.5.1.4.1.1.88.71" },
            { "Advanced Blending Presentation State Storage", "1.2.840.10008.5.1.4.1.1.11.8" },
            { "Ambulatory ECG Waveform Storage", "1.2.840.10008.5.1.4.1.1.9.1.3" },
            { "Arterial Pulse Waveform Storage", "1.2.840.10008.5.1.4.1.1.9.5.1" },
            { "Audio Waveform Real-Time Communication", "1.2.840.10008.10.3" },
            { "Autorefraction Measurements Storage", "1.2.840.10008.5.1.4.1.1.78.2" },
            { "Basic Annotation Box SOP Class", "1.2.840.10008.5.1.1.15" },
            { "Basic Color Image Box SOP Class", "1.2.840.10008.5.1.1.4.1" },
            { "Basic Film Box SOP Class", "1.2.840.10008.5.1.1.2" },
            { "Basic Film Session SOP Class", "1.2.840.10008.5.1.1.1" },
            { "Basic Grayscale Image Box SOP Class", "1.2.840.10008.5.1.1.4" },
            { "Basic Structured Display Storage", "1.2.840.10008.5.1.4.1.1.131" },
            { "Basic Text SR Storage", "1.2.840.10008.5.1.4.1.1.88.11" },
            { "Basic Voice Audio Waveform Storage", "1.2.840.10008.5.1.4.1.1.9.4.1" },
            { "Blending Softcopy Presentation State Storage", "1.2.840.10008.5.1.4.1.1.11.4" },
            { "Body Position Waveform Storage", "1.2.840.10008.5.1.4.1.1.9.8.1" },
            { "Breast Imaging Relevant Patient Information Query", "1.2.840.10008.5.1.4.37.2" },
            { "Breast Projection X-Ray Image Storage - For Presentation", "1.2.840.10008.5.1.4.1.1.13.1.4" },
            { "Breast Projection X-Ray Image Storage - For Processing", "1.2.840.10008.5.1.4.1.1.13.1.5" },
            { "Breast Tomosynthesis Image Storage", "1.2.840.10008.5.1.4.1.1.13.1.3" },
            { "Cardiac Electrophysiology Waveform Storage", "1.2.840.10008.5.1.4.1.1.9.3.1" },
            { "Cardiac Relevant Patient Information Query", "1.2.840.10008.5.1.4.37.3" },
            { "C-Arm Photon-Electron Radiation Record Storage", "1.2.840.10008.5.1.4.1.1.481.19" },
            { "C-Arm Photon-Electron Radiation Storage", "1.2.840.10008.5.1.4.1.1.481.13" },
            { "Chest CAD SR Storage", "1.2.840.10008.5.1.4.1.1.88.65" },
            { "Colon CAD SR Storage", "1.2.840.10008.5.1.4.1.1.88.69" },
            { "Color Palette Query/Retrieve Information Model - FIND", "1.2.840.10008.5.1.4.39.2" },
            { "Color Palette Query/Retrieve Information Model - GET", "1.2.840.10008.5.1.4.39.4" },
            { "Color Palette Query/Retrieve Information Model - MOVE", "1.2.840.10008.5.1.4.39.3" },
            { "Color Palette Storage", "1.2.840.10008.5.1.4.39.1" },
            { "Color Softcopy Presentation State Storage", "1.2.840.10008.5.1.4.1.1.11.2" },
            { "Composite Instance Retrieve Without Bulk Data - GET", "1.2.840.10008.5.1.4.1.2.5.3" },
            { "Composite Instance Root Retrieve - GET", "1.2.840.10008.5.1.4.1.2.4.3" },
            { "Composite Instance Root Retrieve - MOVE", "1.2.840.10008.5.1.4.1.2.4.2" },
            { "Compositing Planar MPR Volumetric Presentation State Storage", "1.2.840.10008.5.1.4.1.1.11.7" },
            { "Comprehensive 3D SR Storage", "1.2.840.10008.5.1.4.1.1.88.34" },
            { "Comprehensive SR Storage", "1.2.840.10008.5.1.4.1.1.88.33" },
            { "Computed Radiography Image Storage", "1.2.840.10008.5.1.4.1.1.1" },
            { "Content Assessment Results Storage", "1.2.840.10008.5.1.4.1.1.90.1" },
            { "Corneal Topography Map Storage", "1.2.840.10008.5.1.4.1.1.82.1" },
            { "CT Defined Procedure Protocol Storage", "1.2.840.10008.5.1.4.1.1.200.1" },
            { "CT Image Storage", "1.2.840.10008.5.1.4.1.1.2" },
            { "CT Performed Procedure Protocol Storage", "1.2.840.10008.5.1.4.1.1.200.2" },
            { "Defined Procedure Protocol Information Model - FIND", "1.2.840.10008.5.1.4.20.1" },
            { "Defined Procedure Protocol Information Model - GET", "1.2.840.10008.5.1.4.20.3" },
            { "Defined Procedure Protocol Information Model - MOVE", "1.2.840.10008.5.1.4.20.2" },
            { "Deflated Explicit VR Little Endian", "1.2.840.10008.1.2.1.99" },
            { "Deformable Spatial Registration Storage", "1.2.840.10008.5.1.4.1.1.66.3" },
            { "Dermoscopic Photography Image Storage", "1.2.840.10008.5.1.4.1.1.77.1.7" },
            { "DICOS 2D AIT Storage", "1.2.840.10008.5.1.4.1.1.501.4" },
            { "DICOS 3D AIT Storage", "1.2.840.10008.5.1.4.1.1.501.5" },
            { "DICOS CT Image Storage", "1.2.840.10008.5.1.4.1.1.501.1" },
            { "DICOS Digital X-Ray Image Storage - For Presentation", "1.2.840.10008.5.1.4.1.1.501.2.1" },
            { "DICOS Digital X-Ray Image Storage - For Processing", "1.2.840.10008.5.1.4.1.1.501.2.2" },
            { "DICOS Quadrupole Resonance (QR) Storage", "1.2.840.10008.5.1.4.1.1.501.6" },
            { "DICOS Threat Detection Report Storage", "1.2.840.10008.5.1.4.1.1.501.3" },
            { "Digital Intra-Oral X-Ray Image Storage - For Presentation", "1.2.840.10008.5.1.4.1.1.1.3" },
            { "Digital Intra-Oral X-Ray Image Storage - For Processing", "1.2.840.10008.5.1.4.1.1.1.3.1" },
            { "Digital Mammography X-Ray Image Storage - For Presentation", "1.2.840.10008.5.1.4.1.1.1.2" },
            { "Digital Mammography X-Ray Image Storage - For Processing", "1.2.840.10008.5.1.4.1.1.1.2.1" },
            { "Digital X-Ray Image Storage - For Presentation", "1.2.840.10008.5.1.4.1.1.1.1" },
            { "Digital X-Ray Image Storage - For Processing", "1.2.840.10008.5.1.4.1.1.1.1.1" },
            { "Display System SOP Class", "1.2.840.10008.5.1.1.40" },
            { "Eddy Current Image Storage", "1.2.840.10008.5.1.4.1.1.601.1" },
            { "Eddy Current Multi-frame Image Storage", "1.2.840.10008.5.1.4.1.1.601.2" },
            { "Electromyogram Waveform Storage", "1.2.840.10008.5.1.4.1.1.9.7.2" },
            { "Electrooculogram Waveform Storage", "1.2.840.10008.5.1.4.1.1.9.7.3" },
            { "Encapsulated CDA Storage", "1.2.840.10008.5.1.4.1.1.104.2" },
            { "Encapsulated MTL Storage", "1.2.840.10008.5.1.4.1.1.104.5" },
            { "Encapsulated OBJ Storage", "1.2.840.10008.5.1.4.1.1.104.4" },
            { "Encapsulated PDF Storage", "1.2.840.10008.5.1.4.1.1.104.1" },
            { "Encapsulated STL Storage", "1.2.840.10008.5.1.4.1.1.104.3" },
            { "Enhanced CT Image Storage", "1.2.840.10008.5.1.4.1.1.2.1" },
            { "Enhanced MR Color Image Storage", "1.2.840.10008.5.1.4.1.1.4.3" },
            { "Enhanced MR Image Storage", "1.2.840.10008.5.1.4.1.1.4.1" },
            { "Enhanced PET Image Storage", "1.2.840.10008.5.1.4.1.1.130" },
            { "Enhanced SR Storage", "1.2.840.10008.5.1.4.1.1.88.22" },
            { "Enhanced US Volume Storage", "1.2.840.10008.5.1.4.1.1.6.2" },
            { "Enhanced XA Image Storage", "1.2.840.10008.5.1.4.1.1.12.1.1" },
            { "Enhanced XRF Image Storage", "1.2.840.10008.5.1.4.1.1.12.2.1" },
            { "Explicit VR Little Endian", "1.2.840.10008.1.2.1" },
            { "Extensible SR Storage", "1.2.840.10008.5.1.4.1.1.88.35" },
            { "General Audio Waveform Storage", "1.2.840.10008.5.1.4.1.1.9.4.2" },
            { "General ECG Waveform Storage", "1.2.840.10008.5.1.4.1.1.9.1.2" },
            { "General Relevant Patient Information Query", "1.2.840.10008.5.1.4.37.1" },
            { "Generic Implant Template Information Model - FIND", "1.2.840.10008.5.1.4.43.2" },
            { "Generic Implant Template Information Model - GET", "1.2.840.10008.5.1.4.43.4" },
            { "Generic Implant Template Information Model - MOVE", "1.2.840.10008.5.1.4.43.3" },
            { "Generic Implant Template Storage", "1.2.840.10008.5.1.4.43.1" },
            { "Grayscale Planar MPR Volumetric Presentation State Storage", "1.2.840.10008.5.1.4.1.1.11.6" },
            { "Grayscale Softcopy Presentation State Storage", "1.2.840.10008.5.1.4.1.1.11.1" },
            { "Hanging Protocol Information Model - FIND", "1.2.840.10008.5.1.4.38.2" },
            { "Hanging Protocol Information Model - GET", "1.2.840.10008.5.1.4.38.4" },
            { "Hanging Protocol Information Model - MOVE", "1.2.840.10008.5.1.4.38.3" },
            { "Hanging Protocol Storage", "1.2.840.10008.5.1.4.38.1" },
            { "Hemodynamic Waveform Storage", "1.2.840.10008.5.1.4.1.1.9.2.1" },
            { "HEVC/H.265 Main 10 Profile / Level 5.1", "1.2.840.10008.1.2.4.108" },
            { "HEVC/H.265 Main Profile / Level 5.1", "1.2.840.10008.1.2.4.107" },
            { "Implant Assembly Template Information Model - FIND", "1.2.840.10008.5.1.4.44.2" },
            { "Implant Assembly Template Information Model - GET", "1.2.840.10008.5.1.4.44.4" },
            { "Implant Assembly Template Information Model - MOVE", "1.2.840.10008.5.1.4.44.3" },
            { "Implant Assembly Template Storage", "1.2.840.10008.5.1.4.44.1" },
            { "Implant Template Group Information Model - FIND", "1.2.840.10008.5.1.4.45.2" },
            { "Implant Template Group Information Model - GET", "1.2.840.10008.5.1.4.45.4" },
            { "Implant Template Group Information Model - MOVE", "1.2.840.10008.5.1.4.45.3" },
            { "Implant Template Group Storage", "1.2.840.10008.5.1.4.45.1" },
            { "Implantation Plan SR Storage", "1.2.840.10008.5.1.4.1.1.88.70" },
            { "Implicit VR Little Endian: Default Transfer Syntax for DICOM", "1.2.840.10008.1.2" },
            { "Instance Availability Notification SOP Class", "1.2.840.10008.5.1.4.33" },
            { "Intraocular Lens Calculations Storage", "1.2.840.10008.5.1.4.1.1.78.8" },
            { "Intravascular Optical Coherence Tomography Image Storage - For Presentation", "1.2.840.10008.5.1.4.1.1.14.1" },
            { "Intravascular Optical Coherence Tomography Image Storage - For Processing", "1.2.840.10008.5.1.4.1.1.14.2" },
            { "JPEG 2000 Image Compression", "1.2.840.10008.1.2.4.91" },
            { "JPEG 2000 Image Compression (Lossless Only)", "1.2.840.10008.1.2.4.90" },
            { "JPEG 2000 Part 2 Multi-component Image Compression", "1.2.840.10008.1.2.4.93" },
            { "JPEG 2000 Part 2 Multi-component Image Compression (Lossless Only)", "1.2.840.10008.1.2.4.92" },
            { "JPEG Baseline (Process 1): Default Transfer Syntax for Lossy JPEG 8 Bit Image Compression", "1.2.840.10008.1.2.4.50" },
            { "JPEG Extended (Process 2 & 4): Default Transfer Syntax for Lossy JPEG 12 Bit Image Compression (Process 4 only)", "1.2.840.10008.1.2.4.51" },
            { "JPEG Lossless, Non-Hierarchical (Process 14)", "1.2.840.10008.1.2.4.57" },
            { "JPEG Lossless, Non-Hierarchical, First-Order Prediction (Process 14 [Selection Value 1]): Default Transfer Syntax for Lossless JPEG Image Compression", "1.2.840.10008.1.2.4.70" },
            { "JPEG-LS Lossless Image Compression", "1.2.840.10008.1.2.4.80" },
            { "JPEG-LS Lossy (Near-Lossless) Image Compression", "1.2.840.10008.1.2.4.81" },
            { "JPIP Referenced", "1.2.840.10008.1.2.4.94" },
            { "JPIP Referenced Deflate", "1.2.840.10008.1.2.4.95" },
            { "Keratometry Measurements Storage", "1.2.840.10008.5.1.4.1.1.78.3" },
            { "Key Object Selection Document Storage", "1.2.840.10008.5.1.4.1.1.88.59" },
            { "Legacy Converted Enhanced CT Image Storage", "1.2.840.10008.5.1.4.1.1.2.2" },
            { "Legacy Converted Enhanced MR Image Storage", "1.2.840.10008.5.1.4.1.1.4.4" },
            { "Legacy Converted Enhanced PET Image Storage", "1.2.840.10008.5.1.4.1.1.128.1" },
            { "Lensometry Measurements Storage", "1.2.840.10008.5.1.4.1.1.78.1" },
            { "Macular Grid Thickness and Volume Report Storage", "1.2.840.10008.5.1.4.1.1.79.1" },
            { "Mammography CAD SR Storage", "1.2.840.10008.5.1.4.1.1.88.50" },
            { "Media Creation Management SOP Class UID", "1.2.840.10008.5.1.1.33" },
            { "Media Storage Directory Storage", "1.2.840.10008.1.3.10" },
            { "Modality Performed Procedure Step Notification SOP Class", "1.2.840.10008.3.1.2.3.5" },
            { "Modality Performed Procedure Step Retrieve SOP Class", "1.2.840.10008.3.1.2.3.4" },
            { "Modality Performed Procedure Step SOP Class", "1.2.840.10008.3.1.2.3.3" },
            { "Modality Worklist Information Model - FIND", "1.2.840.10008.5.1.4.31" },
            { "MPEG2 Main Profile / High Level", "1.2.840.10008.1.2.4.101" },
            { "MPEG2 Main Profile / Main Level", "1.2.840.10008.1.2.4.100" },
            { "MPEG-4 AVC/H.264 BD-compatible High Profile / Level 4.1", "1.2.840.10008.1.2.4.103" },
            { "MPEG-4 AVC/H.264 High Profile / Level 4.1", "1.2.840.10008.1.2.4.102" },
            { "MPEG-4 AVC/H.264 High Profile / Level 4.2 For 2D Video", "1.2.840.10008.1.2.4.104" },
            { "MPEG-4 AVC/H.264 High Profile / Level 4.2 For 3D Video", "1.2.840.10008.1.2.4.105" },
            { "MPEG-4 AVC/H.264 Stereo High Profile / Level 4.2", "1.2.840.10008.1.2.4.106" },
            { "MR Image Storage", "1.2.840.10008.5.1.4.1.1.4" },
            { "MR Spectroscopy Storage", "1.2.840.10008.5.1.4.1.1.4.2" },
            { "Multi-channel Respiratory Waveform Storage", "1.2.840.10008.5.1.4.1.1.9.6.2" },
            { "Multi-frame Grayscale Byte Secondary Capture Image Storage", "1.2.840.10008.5.1.4.1.1.7.2" },
            { "Multi-frame Grayscale Word Secondary Capture Image Storage", "1.2.840.10008.5.1.4.1.1.7.3" },
            { "Multi-frame Single Bit Secondary Capture Image Storage", "1.2.840.10008.5.1.4.1.1.7.1" },
            { "Multi-frame True Color Secondary Capture Image Storage", "1.2.840.10008.5.1.4.1.1.7.4" },
            { "Multiple Volume Rendering Volumetric Presentation State Storage", "1.2.840.10008.5.1.4.1.1.11.11" },
            { "Nuclear Medicine Image Storage", "1.2.840.10008.5.1.4.1.1.20" },
            { "Ophthalmic Axial Measurements Storage", "1.2.840.10008.5.1.4.1.1.78.7" },
            { "Ophthalmic Optical Coherence Tomography B-scan Volume Analysis Storage", "1.2.840.10008.5.1.4.1.1.77.1.5.8" },
            { "Ophthalmic Optical Coherence Tomography En Face Image Storage", "1.2.840.10008.5.1.4.1.1.77.1.5.7" },
            { "Ophthalmic Photography 16 Bit Image Storage", "1.2.840.10008.5.1.4.1.1.77.1.5.2" },
            { "Ophthalmic Photography 8 Bit Image Storage", "1.2.840.10008.5.1.4.1.1.77.1.5.1" },
            { "Ophthalmic Thickness Map Storage", "1.2.840.10008.5.1.4.1.1.81.1" },
            { "Ophthalmic Tomography Image Storage", "1.2.840.10008.5.1.4.1.1.77.1.5.4" },
            { "Ophthalmic Visual Field Static Perimetry Measurements Storage", "1.2.840.10008.5.1.4.1.1.80.1" },
            { "Parametric Map Storage", "1.2.840.10008.5.1.4.1.1.30" },
            { "Patient Radiation Dose SR Storage", "1.2.840.10008.5.1.4.1.1.88.73" },
            { "Patient Root Query/Retrieve Information Model - FIND", "1.2.840.10008.5.1.4.1.2.1.1" },
            { "Patient Root Query/Retrieve Information Model - GET", "1.2.840.10008.5.1.4.1.2.1.3" },
            { "Patient Root Query/Retrieve Information Model - MOVE", "1.2.840.10008.5.1.4.1.2.1.2" },
            { "Performed Imaging Agent Administration SR Storage", "1.2.840.10008.5.1.4.1.1.88.75" },
            { "Planned Imaging Agent Administration SR Storage", "1.2.840.10008.5.1.4.1.1.88.74" },
            { "Positron Emission Tomography Image Storage", "1.2.840.10008.5.1.4.1.1.128" },
            { "Presentation LUT SOP Class", "1.2.840.10008.5.1.1.23" },
            { "Print Job SOP Class", "1.2.840.10008.5.1.1.14" },
            { "Printer Configuration Retrieval SOP Class", "1.2.840.10008.5.1.1.16.376" },
            { "Printer SOP Class", "1.2.840.10008.5.1.1.16" },
            { "Procedural Event Logging SOP Class", "1.2.840.10008.1.40" },
            { "Procedure Log Storage", "1.2.840.10008.5.1.4.1.1.88.40" },
            { "Product Characteristics Query SOP Class", "1.2.840.10008.5.1.4.41" },
            { "Protocol Approval Information Model - FIND", "1.2.840.10008.5.1.4.1.1.200.4" },
            { "Protocol Approval Information Model - GET", "1.2.840.10008.5.1.4.1.1.200.6" },
            { "Protocol Approval Information Model - MOVE", "1.2.840.10008.5.1.4.1.1.200.5" },
            { "Protocol Approval Storage", "1.2.840.10008.5.1.4.1.1.200.3" },
            { "Pseudo-Color Softcopy Presentation State Storage", "1.2.840.10008.5.1.4.1.1.11.3" },
            { "Radiopharmaceutical Radiation Dose SR Storage", "1.2.840.10008.5.1.4.1.1.88.68" },
            { "Raw Data Storage", "1.2.840.10008.5.1.4.1.1.66" },
            { "Real World Value Mapping Storage", "1.2.840.10008.5.1.4.1.1.67" },
            { "Rendition Selection Document Real-Time Communication", "1.2.840.10008.10.4" },
            { "Respiratory Waveform Storage", "1.2.840.10008.5.1.4.1.1.9.6.1" },
            { "RLE Lossless", "1.2.840.10008.1.2.5" },
            { "Robotic Radiation Record Storage", "1.2.840.10008.5.1.4.1.1.481.20" },
            { "Robotic-Arm Radiation Storage", "1.2.840.10008.5.1.4.1.1.481.15" },
            { "Routine Scalp Electroencephalogram Waveform Storage", "1.2.840.10008.5.1.4.1.1.9.7.1" },
            { "RT Beams Delivery Instruction Storage", "1.2.840.10008.5.1.4.34.7" },
            { "RT Beams Treatment Record Storage", "1.2.840.10008.5.1.4.1.1.481.4" },
            { "RT Brachy Application Setup Delivery Instruction Storage", "1.2.840.10008.5.1.4.34.10" },
            { "RT Brachy Treatment Record Storage", "1.2.840.10008.5.1.4.1.1.481.6" },
            { "RT Conventional Machine Verification", "1.2.840.10008.5.1.4.34.8" },
            { "RT Dose Storage", "1.2.840.10008.5.1.4.1.1.481.2" },
            { "RT Image Storage", "1.2.840.10008.5.1.4.1.1.481.1" },
            { "RT Ion Beams Treatment Record Storage", "1.2.840.10008.5.1.4.1.1.481.9" },
            { "RT Ion Machine Verification", "1.2.840.10008.5.1.4.34.9" },
            { "RT Ion Plan Storage", "1.2.840.10008.5.1.4.1.1.481.8" },
            { "RT Physician Intent Storage", "1.2.840.10008.5.1.4.1.1.481.10" },
            { "RT Plan Storage", "1.2.840.10008.5.1.4.1.1.481.5" },
            { "RT Radiation Record Set Storage", "1.2.840.10008.5.1.4.1.1.481.16" },
            { "RT Radiation Salvage Record Storage", "1.2.840.10008.5.1.4.1.1.481.17" },
            { "RT Radiation Set Storage", "1.2.840.10008.5.1.4.1.1.481.12" },
            { "RT Segment Annotation Storage", "1.2.840.10008.5.1.4.1.1.481.11" },
            { "RT Structure Set Storage", "1.2.840.10008.5.1.4.1.1.481.3" },
            { "RT Treatment Summary Record Storage", "1.2.840.10008.5.1.4.1.1.481.7" },
            { "Secondary Capture Image Storage", "1.2.840.10008.5.1.4.1.1.7" },
            { "Segmentation Storage", "1.2.840.10008.5.1.4.1.1.66.4" },
            { "Segmented Volume Rendering Volumetric Presentation State Storage", "1.2.840.10008.5.1.4.1.1.11.10" },
            { "Simplified Adult Echo SR Storage", "1.2.840.10008.5.1.4.1.1.88.72" },
            { "Sleep Electroencephalogram Waveform Storage", "1.2.840.10008.5.1.4.1.1.9.7.4" },
            { "SMPTE ST 2110-20 Uncompressed Interlaced Active Video", "1.2.840.10008.1.2.7.2" },
            { "SMPTE ST 2110-20 Uncompressed Progressive Active Video", "1.2.840.10008.1.2.7.1" },
            { "SMPTE ST 2110-30 PCM Digital Audio", "1.2.840.10008.1.2.7.3" },
            { "Spatial Fiducials Storage", "1.2.840.10008.5.1.4.1.1.66.2" },
            { "Spatial Registration Storage", "1.2.840.10008.5.1.4.1.1.66.1" },
            { "Spectacle Prescription Report Storage", "1.2.840.10008.5.1.4.1.1.78.6" },
            { "Stereometric Relationship Storage", "1.2.840.10008.5.1.4.1.1.77.1.5.3" },
            { "Storage Commitment Push Model SOP Class", "1.2.840.10008.1.20.1" },
            { "Study Root Query/Retrieve Information Model - FIND", "1.2.840.10008.5.1.4.1.2.2.1" },
            { "Study Root Query/Retrieve Information Model - GET", "1.2.840.10008.5.1.4.1.2.2.3" },
            { "Study Root Query/Retrieve Information Model - MOVE", "1.2.840.10008.5.1.4.1.2.2.2" },
            { "Subjective Refraction Measurements Storage", "1.2.840.10008.5.1.4.1.1.78.4" },
            { "Substance Administration Logging SOP Class", "1.2.840.10008.1.42" },
            { "Substance Approval Query SOP Class", "1.2.840.10008.5.1.4.42" },
            { "Surface Scan Mesh Storage", "1.2.840.10008.5.1.4.1.1.68.1" },
            { "Surface Scan Point Cloud Storage", "1.2.840.10008.5.1.4.1.1.68.2" },
            { "Surface Segmentation Storage", "1.2.840.10008.5.1.4.1.1.66.5" },
            { "Tomotherapeutic Radiation Record Storage", "1.2.840.10008.5.1.4.1.1.481.18" },
            { "Tomotherapeutic Radiation Storage", "1.2.840.10008.5.1.4.1.1.481.14" },
            { "Tractography Results Storage", "1.2.840.10008.5.1.4.1.1.66.6" },
            { "Ultrasound Image Storage", "1.2.840.10008.5.1.4.1.1.6.1" },
            { "Ultrasound Multi-frame Image Storage", "1.2.840.10008.5.1.4.1.1.3.1" },
            { "Unified Procedure Step - Event SOP Class", "1.2.840.10008.5.1.4.34.6.4" },
            { "Unified Procedure Step - Pull SOP Class", "1.2.840.10008.5.1.4.34.6.3" },
            { "Unified Procedure Step - Push SOP Class", "1.2.840.10008.5.1.4.34.6.1" },
            { "Unified Procedure Step - Query SOP Class", "1.2.840.10008.5.1.4.34.6.5" },
            { "Unified Procedure Step - Watch SOP Class", "1.2.840.10008.5.1.4.34.6.2" },
            { "Verification SOP Class", "1.2.840.10008.1.1" },
            { "Video Endoscopic Image Real-Time Communication", "1.2.840.10008.10.1" },
            { "Video Endoscopic Image Storage", "1.2.840.10008.5.1.4.1.1.77.1.1.1" },
            { "Video Microscopic Image Storage", "1.2.840.10008.5.1.4.1.1.77.1.2.1" },
            { "Video Photographic Image Real-Time Communication", "1.2.840.10008.10.2" },
            { "Video Photographic Image Storage", "1.2.840.10008.5.1.4.1.1.77.1.4.1" },
            { "Visual Acuity Measurements Storage", "1.2.840.10008.5.1.4.1.1.78.5" },
            { "VL Endoscopic Image Storage", "1.2.840.10008.5.1.4.1.1.77.1.1" },
            { "VL Microscopic Image Storage", "1.2.840.10008.5.1.4.1.1.77.1.2" },
            { "VL Photographic Image Storage", "1.2.840.10008.5.1.4.1.1.77.1.4" },
            { "VL Slide-Coordinates Microscopic Image Storage", "1.2.840.10008.5.1.4.1.1.77.1.3" },
            { "VL Whole Slide Microscopy Image Storage", "1.2.840.10008.5.1.4.1.1.77.1.6" },
            { "VOI LUT Box SOP Class", "1.2.840.10008.5.1.1.22" },
            { "Volume Rendering Volumetric Presentation State Storage", "1.2.840.10008.5.1.4.1.1.11.9" },
            { "Wide Field Ophthalmic Photography 3D Coordinates Image Storage", "1.2.840.10008.5.1.4.1.1.77.1.5.6" },
            { "Wide Field Ophthalmic Photography Stereographic Projection Image Storage", "1.2.840.10008.5.1.4.1.1.77.1.5.5" },
            { "XA Defined Procedure Protocol Storage", "1.2.840.10008.5.1.4.1.1.200.7" },
            { "XA Performed Procedure Protocol Storage", "1.2.840.10008.5.1.4.1.1.200.8" },
            { "XA/XRF Grayscale Softcopy Presentation State Storage", "1.2.840.10008.5.1.4.1.1.11.5" },
            { "X-Ray 3D Angiographic Image Storage", "1.2.840.10008.5.1.4.1.1.13.1.1" },
            { "X-Ray 3D Craniofacial Image Storage", "1.2.840.10008.5.1.4.1.1.13.1.2" },
            { "X-Ray Angiographic Image Storage", "1.2.840.10008.5.1.4.1.1.12.1" },
            { "X-Ray Radiation Dose SR Storage", "1.2.840.10008.5.1.4.1.1.88.67" },
            { "X-Ray Radiofluoroscopic Image Storage", "1.2.840.10008.5.1.4.1.1.12.2" },
        };

        public List<string> KnownUids { get; }

        private string selectedUidName;

        public string SelectedUidName
        {
            get => selectedUidName;
            set
            {
                if (Set(ref selectedUidName, value))
                {
                    CurrentUid = myDict[value];
                }
            }
        }

        private string currentUid;

        public string CurrentUid
        {
            get => currentUid;
            set => Set(ref currentUid, value);
        }

        private RelayCommand copyCommand;
        private RelayCommand generateCommand;

        public RelayCommand CopyCommand => copyCommand ??= new RelayCommand(CopyToClipboard, CanCopyToClipboard);
        public RelayCommand GenerateCommand => generateCommand ??= new RelayCommand(GenerateNewUid);

        public MainViewModel()
        {
            KnownUids = myDict.Keys.ToList();
            SelectedUidName = KnownUids.First();
        }

        private bool CanCopyToClipboard() => !string.IsNullOrEmpty(CurrentUid);

        private void CopyToClipboard()
        {
            Clipboard.SetText(CurrentUid);
        }

        private void GenerateNewUid()
        {
            var guid = Guid.NewGuid();
            CurrentUid = ConvertGuidToUuidInteger(ref guid);
        }

        /// <summary>
        /// Converts a .NET Guid to a UUID in OID format.
        /// </summary>
        /// <remarks>Method is internal to support access to unit tests.</remarks>
        internal static string ConvertGuidToUuidInteger(ref Guid value)
        {
            // ISO/IEC 9834-8, paragraph 6.3 (referenced by DICOM PS 3.5, B.2) defines how
            // to convert a UUID to a single integer value that can be converted back into a UUID.

            // The Guid.ToByteArray Method returns the array in a strange order (see .NET docs),
            // BigInteger expects the input array in little endian order.
            // The last byte controls the sign, add an additional zero to ensure
            // the array is parsed as a positive number.
            var octets = value.ToByteArray();
            var littleEndianOrder = new byte[]
                { octets[15], octets[14], octets[13], octets[12], octets[11], octets[10], octets[9], octets[8],
                  octets[6], octets[7], octets[4], octets[5], octets[0], octets[1], octets[2], octets[3], 0 };

            return "2.25." + new BigInteger(littleEndianOrder).ToString(CultureInfo.InvariantCulture);
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets a backing field value and if it's changed raise a notifcation.
        /// </summary>
        /// <typeparam name="T">The type of the value being set.</typeparam>
        /// <param name="oldValue">A reference to the field to update.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyName">The name of the property for change notifications.</param>
        /// <returns></returns>
        public virtual bool Set<T>(ref T oldValue, T newValue, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                return false;
            }

            oldValue = newValue;

            NotifyOfPropertyChanged(propertyName ?? string.Empty);

            return true;
        }

        public virtual void NotifyOfPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
