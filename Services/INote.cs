using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;

namespace eCareApi.Services
{
    public interface INote
    {

        public List<Note> getPatientCmNotes(string patId);

        public List<DocumentForm> getPatientCmAttachments(string patId);

        public DocumentForm getPatientCmAttachment(string patId, int attachId);

        public List<Note> getPatientSuspendedNotes(string patId);

        public Note getPatientSuspendedNote(string patId, int noteId);

        public Note getPatientCmSummary(string patId, string recordDate);

        public List<Note> addSuspendedNote(Note suspendedNote);        

        public List<Note> addCmNote(Note cmNote);

        public Note saveCmSummary(Note cmSummary);

        public Note addCcmNote(Note cmNote); 

        public List<DocumentForm> addCmAttachment(Note cmAttach);
    }
}
