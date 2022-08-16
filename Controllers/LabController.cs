using eCareApi.Models;
using eCareApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LabController : ControllerBase
    {
        private readonly ILab _labInterface;

        public LabController(ILab labInterface)
        {
            _labInterface = labInterface ?? throw new ArgumentNullException(nameof(labInterface));
        }

        
        [HttpGet("labqsearch/hospitals/labtypes/all")]
        public IActionResult GetAllLabTypes()
        {
            var labTypes = _labInterface.getAllLabTypes();

            if (labTypes == null)
            {
                return NoContent();
            }

            return Ok(labTypes);
        }

        [HttpGet("labqsearch/hospitals/labtypes")]
        public IActionResult GetHospitalLabTypes()
        {
            var labTypes = _labInterface.GetHospitalLabTypes();

            if (labTypes == null)
            {
                return NoContent();
            }

            var returnLabs = new List<Lab>();

            foreach (var lab in labTypes)
            {
                returnLabs.Add(new Lab
                {
                    labId = lab.labId,
                    labName = lab.labName
                });
            }


            return Ok(returnLabs);
        }


        
        [HttpGet("labqsearch/hospitals/resultflags/all")]
        public IActionResult getAllLabResultFlags()
        {
            var flags = _labInterface.getAllLabResultFlags();

            if (flags == null)
            {
                return NoContent();
            }

            return Ok(flags);
        }


        [HttpGet("labqsearch/hospitals/labtypes/{id}/tests")]
        public IActionResult GetLabTypeTests(string id)
        {
            var tests = _labInterface.GetLabTypeTests(id);

            if (tests == null)
            {
                return NoContent();
            }

            var returnLabs = tests;         

            return Ok(returnLabs);
        }


        [HttpGet("labqsearch/hospitals/collectiondepartment/{hospId}")]
        public IActionResult GetHospitalCollectionDeparments(int hospId)
        {
            var collDepts = _labInterface.GetHospitalCollectionDepartments(hospId);

            if (collDepts == null)
            {
                return NoContent();
            }

            var returnDepts = collDepts;

            return Ok(returnDepts);
        }


        [HttpGet("labqsearch/labtype/{labTypeId}/appTypes")]
        public IActionResult GetLabTypeAppointmentTypes(int labTypeId)
        {
            var apptTypes = _labInterface.GetLabTypeAppointmentTypes(labTypeId);

            if (apptTypes == null)
            {
                return NoContent();
            }

            var returnDepts = apptTypes;

            return Ok(returnDepts);
        }

        [HttpGet("labqsearch/labtype/{labTypeId}/icd10s")]
        public IActionResult GetLabTypeIcd10s(int labTypeId)
        {
            var apptTypes = _labInterface.GetLabTypeIcd10s(labTypeId);

            if (apptTypes == null)
            {
                return NoContent();
            }

            var returnDepts = apptTypes;

            return Ok(returnDepts);
        }


        [HttpPost("labqsearch/apptsearch")]
        public IActionResult GetLabAvailableAppointments(Appointment lab)
        {
            DateTime srchFromDate = _labInterface.getSearchFromDate(lab);
            List<DateTime> twoWeekSchedule = _labInterface.getTwoWeeksOutDate(srchFromDate);

            var appointments = _labInterface.GetLabAvailableAppointments(lab, srchFromDate);


            if (appointments == null)
            {
                return NoContent();
            }
            else
            {
                var returnAppts = new List<Appointment>();


                foreach (var appt in appointments)
                {
                    returnAppts.Add(new Appointment
                    {
                        HospitalName = appt.HospitalName,
                        HospitalId = appt.HospitalId,
                        DepartmentName = appt.DepartmentName,
                        DepartmentId = appt.DepartmentId,
                        RoomName = appt.RoomName,
                        RoomId = appt.RoomId,
                        RoomOccupancy = appt.RoomOccupancy,
                        appointmentStartDateTime = appt.appointmentStartDateTime,
                        appointmentEndDateTime = appt.appointmentEndDateTime,
                        returnSearchFromDate = srchFromDate,
                        returnTwoWeekSchedule = twoWeekSchedule,
                        departmentRooms = appt.departmentRooms,
                        departmentWorkday = appt.departmentWorkday,
                        selectedAppointmentType = appt.selectedAppointmentType
                    });
                }


                return Ok(returnAppts);
            }            
        }


        [HttpPost("labqsearch/apptprint")]
        public IActionResult GetLabAppointmentPrint(Appointment lab)
        {            
            
            var appointment = _labInterface.GetAppointmentScheduleData(lab);


            if (appointment == null)
            {
                return NoContent();
            }
            else
            {
                var returnAppts = new Appointment();

                returnAppts.patient = appointment.patient;

                returnAppts.HospitalName = appointment.HospitalName;
                returnAppts.HospitalId = appointment.HospitalId;
                returnAppts.TestingHospitalId = appointment.TestingHospitalId;
                returnAppts.TestingHospitalName = appointment.TestingHospitalName;

                returnAppts.dr = appointment.dr;

                returnAppts.scheduleIcd10s = appointment.scheduleIcd10s;
                returnAppts.scheduleResultIcd10s = appointment.scheduleResultIcd10s;

                returnAppts.appointmentAppointmentTypeId = appointment.appointmentAppointmentTypeId;
                returnAppts.appointmentAppointmentTypeName = appointment.appointmentAppointmentTypeName;
                returnAppts.appointmentLabTypeId = appointment.appointmentLabTypeId;
                returnAppts.appointmentLabTypeName = appointment.appointmentLabTypeName;

                returnAppts.DepartmentId = lab.scheduleCollectionDepartmentId;
                returnAppts.RoomId = lab.scheduleRoomId;
                
                returnAppts.appointmentStartDateTime = lab.scheduleCollectionDateTime;
                returnAppts.appointmentEndDateTime = lab.scheduleCollectionEndDateTime;

                return Ok(returnAppts);
            }
        }

        [HttpPost("labqsearch/apptschedule")]
        public IActionResult ScheduleLabAppointment(Appointment lab)
        {
            var appointment = _labInterface.GetAppointmentScheduleData(lab);


            if (appointment == null)
            {
                return NoContent();
            }
            else
            {
                var returnAdmission = _labInterface.CreateLabAppointment(lab);

                //var returnAppts = new Appointment();               

                //if (_labInterface.CreateLabAppointment(lab))
                //{
                //    returnAppts.patient = appointment.patient;

                //    returnAppts.HospitalName = appointment.HospitalName;
                //    returnAppts.HospitalId = appointment.HospitalId;
                //    returnAppts.TestingHospitalId = appointment.TestingHospitalId;
                //    returnAppts.TestingHospitalName = appointment.TestingHospitalName;

                //    returnAppts.dr = appointment.dr;

                //    returnAppts.scheduleIcd10s = appointment.scheduleIcd10s;
                //    returnAppts.scheduleResultIcd10s = appointment.scheduleResultIcd10s;

                //    returnAppts.appointmentAppointmentTypeId = appointment.appointmentAppointmentTypeId;
                //    returnAppts.appointmentAppointmentTypeName = appointment.appointmentAppointmentTypeName;
                //    returnAppts.appointmentLabTypeId = appointment.appointmentLabTypeId;
                //    returnAppts.appointmentLabTypeName = appointment.appointmentLabTypeName;

                //    returnAppts.DepartmentId = lab.scheduleCollectionDepartmentId;
                //    returnAppts.RoomId = lab.scheduleRoomId;

                //    returnAppts.appointmentStartDateTime = lab.scheduleCollectionDateTime;
                //    returnAppts.appointmentEndDateTime = lab.scheduleCollectionEndDateTime;
                //}

                return Ok(returnAdmission);
            }
        }



        [HttpGet("labqsearch/patients/labs/{first}/{last}/{dob}")]
        public IActionResult GetPatientWithLabs(string first, string last, string dob)
        {
            var patients = _labInterface.GetPatientWithLabs(first, last, dob);

            if (patients == null)
            {
                return NoContent();
            }


            var returnPatients = new List<Patient>();

            foreach (var pat in patients)
            {
                returnPatients.Add(new Patient
                {
                    PatientId = pat.member_id,
                    firstName = pat.member_first_name,
                    lastName = pat.member_last_name,
                    dateOfBirth = pat.member_birth,
                    dateOfBirthDisplay = pat.member_birth.ToString(),
                    ssn = pat.member_ssn
                });
            }



            return Ok(returnPatients);
        }

        [HttpGet("labqsearch/patients/{id}/labs/")]
        public IActionResult GetPatientLabs(string id)
        {
            var labs = _labInterface.GetPatientLabs(id);

            if (labs == null)
            {
                return NoContent();
            }

            return Ok(labs);
        }

        [HttpGet("labqsearch/patients/labs/{id}")]
        public IActionResult GetPatientLab(string id)
        {
            var lab = _labInterface.GetPatientLab(id);

            if (lab == null)
            {
                return NoContent();
            }


            var returnLab = new Lab();

            returnLab.labId = lab.labId;
            returnLab.labName = (!string.IsNullOrEmpty(lab.labName)) ? lab.labName : "N/A";
            returnLab.accessionNumber = (!string.IsNullOrEmpty(lab.accessionNumber)) ? lab.accessionNumber : "N/A";
            returnLab.collectionDate = lab.collectionDate;
            returnLab.displayCollectionDate = (!lab.collectionDate.Equals(null)) ? lab.collectionDate.ToString() : "N/A";
            returnLab.collectionSite = (!string.IsNullOrEmpty(lab.collectionSite)) ? lab.collectionSite : "N/A";


            return Ok(returnLab);
        }

        [HttpGet("labqsearch/todays/labs/{today}/{hospitalId}")]
        public IActionResult GetTodyasLabs(string today, string hospitalId)
        {
            var labs = _labInterface.GetTodaysLabs(today, hospitalId);

            if (labs == null)
            {
                return NoContent();
            }


            var returnLabs = new List<Lab>();

            foreach (var lab in labs)
            {
                returnLabs.Add(new Lab
                {
                    labId = lab.labId,
                    patient = lab.patient,
                    labName = (!string.IsNullOrEmpty(lab.labName)) ? lab.labName : "N/A",
                    accessionNumber = (!string.IsNullOrEmpty(lab.accessionNumber)) ? lab.accessionNumber : "N/A",
                    collectionDate = lab.collectionDate,
                    displayCollectionDate = (!lab.collectionDate.Equals(null)) ? lab.collectionDate.ToString() : "N/A",
                    collectionSite = (!string.IsNullOrEmpty(lab.collectionSite)) ? lab.collectionSite : "N/A"
                });
            }



            return Ok(returnLabs);
        }


        [HttpGet("labqsearch/patients/labs/results/{labId}")]
        public IActionResult getLabResult(int labId)
        {
            var result = _labInterface.getAdmissionLabResult(labId);

            if (result == null)
            {
                return NoContent();
            }

            return Ok(result);
        }



        [HttpPost("dbms/update/inpatient/admissions/labs")]
        public IActionResult updateAdmissionLabs(Lab lab)
        {
            var labs = _labInterface.updateAdmissionLabs(lab);

            if (labs == null)
            {
                return NotFound();
            }

            Admission admit = new Admission();
            admit.labs = labs;

            return Ok(admit);
        }

        
        [HttpPost("dbms/update/inpatient/admissions/labs/results")]
        public IActionResult updateAdmissionLabResults(LabResult result)
        {
            var labResult = _labInterface.updateAdmissionLabResults(result);

            if (labResult == null)
            {
                return NotFound();
            }

            return Ok(labResult);
        }
    }
}
