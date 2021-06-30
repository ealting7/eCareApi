using eCareApi.Models;
using eCareApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCareApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabController : ControllerBase
    {
        private readonly ILab _labInterface;

        public LabController(ILab labInterface)
        {
            _labInterface = labInterface ?? throw new ArgumentNullException(nameof(labInterface));
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
    }
}
