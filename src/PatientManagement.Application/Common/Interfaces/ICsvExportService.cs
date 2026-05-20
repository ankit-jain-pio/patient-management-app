using PatientManagement.Application.DTOs.Appointments;
using PatientManagement.Application.DTOs.Consultations;
using PatientManagement.Application.DTOs.Patients;

namespace PatientManagement.Application.Common.Interfaces;

public interface ICsvExportService
{
    byte[] ExportPatients(List<PatientDto> patients);
    byte[] ExportConsultations(List<ConsultationDto> consultations);
    byte[] ExportAppointments(List<AppointmentDto> appointments);
}
