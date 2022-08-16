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
    public class NoteController : Controller
    {
        private readonly INote _noteInterface;

        public NoteController(INote noteInterface)
        {
            _noteInterface = noteInterface ?? throw new ArgumentNullException(nameof(noteInterface));
        }


        [HttpGet("dbms/get/notes/patients/{patId}/cm")]
        public IActionResult getPatientCmNotes(string patId)
        {
            var apptTypes = _noteInterface.getPatientCmNotes(patId);

            if (apptTypes == null)
            {
                return NoContent();
            }

            return Ok(apptTypes);
        }

        [HttpGet("dbms/get/notes/patients/{patId}/cm/attachments")]
        public IActionResult getPatientCmAttachments(string patId)
        {
            var apptTypes = _noteInterface.getPatientCmAttachments(patId);

            if (apptTypes == null)
            {
                return NoContent();
            }

            return Ok(apptTypes);
        }

        [HttpGet("dbms/get/notes/patients/{patId}/cm/attachments/{attachId}")]
        public IActionResult getPatientCmAttachments(string patId, int attachId)
        {
            var apptTypes = _noteInterface.getPatientCmAttachment(patId, attachId);

            if (apptTypes == null)
            {
                return NoContent();
            }

            return Ok(apptTypes);
        }

        [HttpGet("dbms/get/notes/patients/{patId}/suspended")]
        public IActionResult getPatientSuspendedNotes(string patId)
        {
            var apptTypes = _noteInterface.getPatientSuspendedNotes(patId);

            if (apptTypes == null)
            {
                return NoContent();
            }

            return Ok(apptTypes);
        }

        [HttpGet("dbms/get/notes/patients/{patId}/suspended/{noteId}")]
        public IActionResult getPatientSuspendedNotes(string patId, int noteId)
        {
            var apptTypes = _noteInterface.getPatientSuspendedNote(patId, noteId);

            if (apptTypes == null)
            {
                return NoContent();
            }

            return Ok(apptTypes);
        }


        
        [HttpPost("dbms/add/notes/suspended")]
        public IActionResult addSuspendedNote(Note suspendedNote)
        {
            var notes = _noteInterface.addSuspendedNote(suspendedNote);

            if (notes == null)
            {
                return NoContent();
            }

            return Ok(notes);
        }

        [HttpPost("dbms/add/notes/cm")]
        public IActionResult addCmNote(Note cmNote)
        {
            var notes = _noteInterface.addCmNote(cmNote);

            if (notes == null)
            {
                return NoContent();
            }

            return Ok(notes);
        }

        [HttpPost("dbms/add/attachment/cm")]
        public IActionResult addCmAttachment(Note cmAttach)
        {
            var notes = _noteInterface.addCmAttachment(cmAttach);

            if (notes == null)
            {
                return NoContent();
            }

            return Ok(notes);
        }

    }
}
