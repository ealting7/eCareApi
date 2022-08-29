using eCareApi.Context;
using eCareApi.Entities;
using eCareApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace eCareApi.Services
{
    public class NoteService : INote
    {

        private readonly IcmsContext _icmsContext;
        private readonly AspNetContext _aspNetContext;
        private readonly IcmsDataStagingContext _dataStagingContext;


        public NoteService(IcmsContext icmsContext, AspNetContext aspNetContext, IcmsDataStagingContext dataStagingContext)
        {
            _icmsContext = icmsContext ?? throw new ArgumentNullException(nameof(icmsContext));
            _aspNetContext = aspNetContext ?? throw new ArgumentNullException(nameof(aspNetContext));
            _dataStagingContext = dataStagingContext ?? throw new ArgumentNullException(nameof(dataStagingContext));
        }


        public List<Note> getPatientCmNotes(string patId)
        {

            List<Note> cmNotes = null;
            List<Note> tempNotes = null;

            Guid memberId = Guid.Empty;

            if (Guid.TryParse(patId, out memberId))
            {

                List<Note> dbNotes = (

                        from notes in _icmsContext.MemberNotes

                        join systemUsr in _icmsContext.SystemUsers
                        on notes.user_updated equals systemUsr.system_user_id into sysUsrs
                        from systemUsrs in sysUsrs.DefaultIfEmpty()

                        where notes.member_id.Equals(memberId)

                        orderby notes.record_date descending

                        select new Note
                        {
                            recordDate = notes.record_date,
                            displayRecordDate = (notes.record_date != null) ?
                                notes.record_date.ToShortDateString() + " " + notes.record_date.ToShortTimeString() :
                                "",
                            caseOwnerId = notes.user_updated,
                            caseOwnerName = systemUsrs.system_user_first_name + " " + systemUsrs.system_user_last_name
                        }
                    )
                    .Distinct()
                    .ToList();

                if (dbNotes != null)
                {

                    foreach (Note note in dbNotes)
                    {

                        List<Note> dbNote = (

                                from notes in _icmsContext.MemberNotes
                                where notes.member_id.Equals(memberId)
                                && notes.record_date.Equals(note.recordDate)
                                orderby notes.record_seq_num
                                select new Note
                                {
                                    noteSequenceNumber = notes.record_seq_num,
                                    noteText = notes.evaluation_text,                                    
                                }
                            )
                            .ToList();

                        if (dbNote != null)
                        {

                            if (tempNotes == null)
                            {
                                tempNotes = new List<Note>();
                            }

                            string noteText = "";

                            foreach (Note singleNote in dbNote)
                            {
                                noteText += singleNote.noteText;                                                                                              
                            }

                            Note addNote = new Note();

                            addNote.recordDate = note.recordDate;
                            addNote.displayRecordDate = note.displayRecordDate;
                            addNote.caseOwnerId = note.caseOwnerId;
                            addNote.caseOwnerName = note.caseOwnerName;
                            addNote.noteText = noteText;

                            tempNotes.Add(addNote);
                        }
                    }                    
                }


                if (tempNotes != null && tempNotes.Count > 0)
                {

                    foreach (Note nte in tempNotes)
                    {

                        if (string.IsNullOrEmpty(nte.caseOwnerName.Trim()) && !nte.caseOwnerId.Equals(Guid.Empty))
                        {

                            AspNetUsers aspUsr = (

                                    from aspNetUsr in _aspNetContext.AspNetUsers
                                    where aspNetUsr.UserId.Equals(nte.caseOwnerId)
                                    select aspNetUsr
                                )
                                .FirstOrDefault();

                            if (aspUsr != null && !string.IsNullOrEmpty(aspUsr.UserName))
                            {
                                nte.caseOwnerName = aspUsr.UserName;
                            }
                        }
                    }

                    cmNotes = tempNotes.OrderByDescending(nte => nte.recordDate).ToList();
                }
            }

            return cmNotes;
        }

        public List<DocumentForm> getPatientCmAttachments(string patId)
        {
            
            List<DocumentForm> attachments = null;

            Guid memberId = Guid.Empty;

            if (Guid.TryParse(patId, out memberId))
            {

                attachments = (

                    from cmAttach in _icmsContext.MemberNotesAttachments
                    where cmAttach.member_id.Equals(memberId)
                    orderby cmAttach.record_date descending
                    select new DocumentForm
                    {
                        documentId = cmAttach.member_notes_attachment_id,
                        documentFileName = cmAttach.file_identifier,
                        creationDate = cmAttach.record_date,
                        displayCreationDate = (cmAttach.record_date != null) ? 
                            cmAttach.record_date.ToShortDateString() + " " + cmAttach.record_date.ToShortTimeString() : 
                            ""

                    }
                )
                .ToList();

            }


            return attachments;
        }

        public DocumentForm getPatientCmAttachment(string patId, int attachId)
        {

            DocumentForm attachment = null;

            Guid memberId = Guid.Empty;

            if (Guid.TryParse(patId, out memberId) && attachId > 0)
            {

                attachment = (

                    from cmAttach in _icmsContext.MemberNotesAttachments
                    where cmAttach.member_notes_attachment_id.Equals(attachId)
                    orderby cmAttach.record_date descending
                    select new DocumentForm
                    {
                        documentId = cmAttach.member_notes_attachment_id,
                        documentFileName = cmAttach.file_identifier,
                        creationDate = cmAttach.record_date,
                        displayCreationDate = (cmAttach.record_date != null) ?
                            cmAttach.record_date.ToShortDateString() + " " + cmAttach.record_date.ToShortTimeString() :
                            "",
                        documentImage = cmAttach.file_blob,
                        documentBase64 = Convert.ToBase64String(cmAttach.file_blob)
                    }
                )
                .FirstOrDefault();


                if (attachment != null && !string.IsNullOrEmpty(attachment.documentFileName))
                {
                    switch(Path.GetExtension(attachment.documentFileName))
                    {
                        case ".gif":
                            attachment.documentContentType = "image/gif";
                            break;
                        case ".jpg":
                            attachment.documentContentType = "image/jpeg";
                            break;
                        case ".bmp":
                            attachment.documentContentType = "image/x-windows-bmp";
                            break;
                        case ".png":
                            attachment.documentContentType = "image/png";
                            break;
                        case ".csv":
                            attachment.documentContentType = "text/csv";
                            break;
                        case ".pdf":
                            attachment.documentContentType = "application/pdf";
                            break;
                        case ".txt":
                            attachment.documentContentType = "text/plain";
                            break;
                        case ".xls":
                            attachment.documentContentType = "application/vnd.ms-excel";
                            break;
                        case ".xlsx":
                            attachment.documentContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            break;
                        case ".doc":
                            attachment.documentContentType = "application/msword";
                            break;
                        case ".docx":
                            attachment.documentContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                            break;
                        default:
                            attachment.documentContentType = "application/pdf";
                            break;
                    }
                }
            }


            return attachment;
        }

        public List<Note> getPatientSuspendedNotes(string patId)
        {

            List<Note> suspendedNotes = null;

            Guid memberId = Guid.Empty;

            if (Guid.TryParse(patId, out memberId))
            {

                suspendedNotes = (

                        from suspNotes in _icmsContext.SuspendedNoteses

                        join sysUsr in _icmsContext.SystemUsers
                        on suspNotes.creation_user_id equals sysUsr.system_user_id into icmsUsr
                        from icmsUsers in icmsUsr.DefaultIfEmpty()

                        where suspNotes.member_id.Equals(memberId)

                        orderby suspNotes.creation_date descending

                        select new Note
                        {
                            noteId = suspNotes.suspended_note_id,
                            memberId = (Guid)suspNotes.member_id,
                            recordDate = suspNotes.creation_date.Value,
                            displayRecordDate = (suspNotes.creation_date.HasValue) ? 
                                suspNotes.creation_date.Value.ToShortDateString() + " " + suspNotes.creation_date.Value.ToShortTimeString()  : 
                                "",
                            caseOwnerId = suspNotes.creation_user_id,
                            caseOwnerName = icmsUsers.system_user_first_name + " " + icmsUsers.system_user_last_name,
                            noteText = suspNotes.note_text
                        }
                    )
                    .ToList();

                if (suspendedNotes != null && suspendedNotes.Count > 0)
                {

                    foreach(Note nte in suspendedNotes)
                    {

                        if (string.IsNullOrEmpty(nte.caseOwnerName.Trim()) && !nte.caseOwnerId.Equals(Guid.Empty))
                        {

                            AspNetUsers aspUsr = (

                                    from aspNetUsr in _aspNetContext.AspNetUsers
                                    where aspNetUsr.UserId.Equals(nte.caseOwnerId)
                                    select aspNetUsr
                                )
                                .FirstOrDefault();

                            if (aspUsr != null && !string.IsNullOrEmpty(aspUsr.UserName))
                            {
                                nte.caseOwnerName = aspUsr.UserName;
                            }
                        }
                    }
                }
            }

            return suspendedNotes;
        }

        public List<Note> getReferralSuspendedNotes(string referralNumber)
        {

            List<Note> suspendedNotes = null;

            if (!string.IsNullOrEmpty(referralNumber))
            {

                suspendedNotes = (

                        from suspNotes in _icmsContext.SuspendedNoteses

                        join sysUsr in _icmsContext.SystemUsers
                        on suspNotes.creation_user_id equals sysUsr.system_user_id into icmsUsr
                        from icmsUsers in icmsUsr.DefaultIfEmpty()

                        where suspNotes.referral_number == referralNumber

                        orderby suspNotes.creation_date descending

                        select new Note
                        {
                            noteId = suspNotes.suspended_note_id,
                            referralNumber = suspNotes.referral_number,
                            recordDate = suspNotes.creation_date.Value,
                            displayRecordDate = (suspNotes.creation_date.HasValue) ?
                                suspNotes.creation_date.Value.ToShortDateString() + " " + suspNotes.creation_date.Value.ToShortTimeString() :
                                "",
                            caseOwnerId = suspNotes.creation_user_id,
                            caseOwnerName = icmsUsers.system_user_first_name + " " + icmsUsers.system_user_last_name,
                            noteText = suspNotes.note_text
                        }
                    )
                    .ToList();

                if (suspendedNotes != null && suspendedNotes.Count > 0)
                {

                    foreach (Note nte in suspendedNotes)
                    {

                        if (string.IsNullOrEmpty(nte.caseOwnerName.Trim()) && !nte.caseOwnerId.Equals(Guid.Empty))
                        {

                            AspNetUsers aspUsr = (

                                    from aspNetUsr in _aspNetContext.AspNetUsers
                                    where aspNetUsr.UserId.Equals(nte.caseOwnerId)
                                    select aspNetUsr
                                )
                                .FirstOrDefault();

                            if (aspUsr != null && !string.IsNullOrEmpty(aspUsr.UserName))
                            {
                                nte.caseOwnerName = aspUsr.UserName;
                            }
                        }
                    }
                }
            }

            return suspendedNotes;
        }

        public Note getReferralSuspendedNote(int suspendNoteId)
        {

            Note suspendedNote = null;

            if (suspendNoteId > 0)
            {

                suspendedNote = (

                        from suspNotes in _icmsContext.SuspendedNoteses

                        join sysUsr in _icmsContext.SystemUsers
                        on suspNotes.creation_user_id equals sysUsr.system_user_id into icmsUsr
                        from icmsUsers in icmsUsr.DefaultIfEmpty()

                        where suspNotes.suspended_note_id.Equals(suspendNoteId)

                        orderby suspNotes.creation_date descending

                        select new Note
                        {
                            noteId = suspNotes.suspended_note_id,
                            referralNumber = suspNotes.referral_number,
                            recordDate = suspNotes.creation_date.Value,
                            displayRecordDate = (suspNotes.creation_date.HasValue) ?
                                suspNotes.creation_date.Value.ToShortDateString() + " " + suspNotes.creation_date.Value.ToShortTimeString() :
                                "",
                            caseOwnerId = suspNotes.creation_user_id,
                            caseOwnerName = icmsUsers.system_user_first_name + " " + icmsUsers.system_user_last_name,
                            noteText = suspNotes.note_text
                        }
                    )
                    .FirstOrDefault();

                if (suspendedNote != null)
                {

                    if (string.IsNullOrEmpty(suspendedNote.caseOwnerName.Trim()) && !suspendedNote.caseOwnerId.Equals(Guid.Empty))
                    {

                        AspNetUsers aspUsr = (

                                from aspNetUsr in _aspNetContext.AspNetUsers
                                where aspNetUsr.UserId.Equals(suspendedNote.caseOwnerId)
                                select aspNetUsr
                            )
                            .FirstOrDefault();

                        if (aspUsr != null && !string.IsNullOrEmpty(aspUsr.UserName))
                        {
                            suspendedNote.caseOwnerName = aspUsr.UserName;
                        }
                    }
                }
            }

            return suspendedNote;
        }

        public Note getPatientSuspendedNote(string patId, int noteId)
        {

            Note suspendedNote = null;

            Guid memberId = Guid.Empty;

            if (Guid.TryParse(patId, out memberId) && noteId > 0)
            {

                suspendedNote = (

                        from suspNotes in _icmsContext.SuspendedNoteses

                        join sysUsr in _icmsContext.SystemUsers
                        on suspNotes.creation_user_id equals sysUsr.system_user_id into icmsUsr
                        from icmsUsers in icmsUsr.DefaultIfEmpty()

                        where suspNotes.suspended_note_id.Equals(noteId)

                        orderby suspNotes.creation_date descending

                        select new Note
                        {
                            noteId = suspNotes.suspended_note_id,
                            memberId = (Guid)suspNotes.member_id,
                            recordDate = suspNotes.creation_date.Value,
                            displayRecordDate = (suspNotes.creation_date.HasValue) ?
                                suspNotes.creation_date.Value.ToShortDateString() + " " + suspNotes.creation_date.Value.ToShortTimeString() :
                                "",
                            caseOwnerId = suspNotes.creation_user_id,
                            caseOwnerName = icmsUsers.system_user_first_name + " " + icmsUsers.system_user_last_name,
                            noteText = suspNotes.note_text,
                            midMonthBill = (suspNotes.mid_month_suspend.HasValue && suspNotes.mid_month_suspend.Value > 0) ?
                                true : false,
                        }
                    )
                    .FirstOrDefault();

                if (suspendedNote != null)
                {

                    if ((bool)suspendedNote.midMonthBill)
                    {
                        if (validateSuspendedNoteInMidMonthRange(suspendedNote))
                        {
                            
                            suspendedNote = new Note();
                            suspendedNote.notPassedMidMonth = true;

                            return suspendedNote;
                        }
                    }

                    if (string.IsNullOrEmpty(suspendedNote.caseOwnerName) && !suspendedNote.caseOwnerId.Equals(Guid.Empty))
                    {

                        AspNetUsers aspUsr = (

                                from aspNetUsr in _aspNetContext.AspNetUsers
                                where aspNetUsr.UserId.Equals(suspendedNote.caseOwnerId)
                                select aspNetUsr
                            )
                            .FirstOrDefault();

                        if (aspUsr != null && !string.IsNullOrEmpty(aspUsr.UserName))
                        {
                            suspendedNote.caseOwnerName = aspUsr.UserName;
                        }
                    }
                }
            }

            return suspendedNote;
        }

        private bool validateSuspendedNoteInMidMonthRange(Note suspendedNote)
        {

            DateTime dteNow = DateTime.Now;

            switch (dteNow.Day)
            {

                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                    return false;
            }

            return true;
        }

        public Note getPatientCmSummary(string patId, string recordDate)
        {
            
            Note cmSummary = null;

            Guid memberId = Guid.Empty;
            DateTime summaryDate = DateTime.MinValue;

            if ((!string.IsNullOrEmpty(patId) && Guid.TryParse(patId, out memberId)) 
                && (!string.IsNullOrEmpty(recordDate) && DateTime.TryParse(recordDate, out summaryDate)))
            {

                try
                {
                    DateTime firstOfMonth = Convert.ToDateTime(summaryDate.Year.ToString()
                                                                + "-" +
                                                                summaryDate.Month.ToString()
                                                                + "-01 00:00:00");

                    cmSummary = (
                            from memNteSum in _icmsContext.MemberNotesSummaries
                            where memNteSum.member_id.Equals(memberId)
                            && memNteSum.record_date.Equals(firstOfMonth)
                            && memNteSum.month_closed.Equals(false)
                            orderby memNteSum.record_date descending
                            select new Note
                            {
                                noteId = memNteSum.member_notes_summary_id,
                                recordDate = (memNteSum.record_date.HasValue) ? memNteSum.record_date.Value : DateTime.MinValue,
                                noteText = memNteSum.evaluation_text
                            }
                        )
                        .Take(1)
                        .FirstOrDefault();
                }
                catch(Exception ex)
                {
                    Console.WriteLine("error: " + ex.Message);
                }
                
            }

            return cmSummary;
        }



        public List<Note> addSuspendedNote(Note suspendedNote)
        {

            List<Note> suspendedNotes = null;

            SuspendedNotes newSuspend = new SuspendedNotes();

            newSuspend.member_id = suspendedNote.memberId;
            newSuspend.creation_date = DateTime.Now;
            newSuspend.creation_user_id = suspendedNote.caseOwnerId;
            newSuspend.note_text = suspendedNote.noteText;

            if ((bool)suspendedNote.midMonthBill)
            {
                newSuspend.mid_month_suspend = 1;
            }

            _icmsContext.SuspendedNoteses.Add(newSuspend);
            int result = _icmsContext.SaveChanges();

            suspendedNotes = getPatientSuspendedNotes(suspendedNote.memberId.ToString());

            return suspendedNotes;
        }

        public bool removeSuspendNote(Note utilNote)
        {

            SuspendedNotes dbSuspendNote = null;

            if (utilNote.suspendNoteId > 0)
            {

                dbSuspendNote = (

                        from suspnte in _icmsContext.SuspendedNoteses
                        where suspnte.suspended_note_id.Equals(utilNote.suspendNoteId)
                        select suspnte
                    )
                    .FirstOrDefault();

                if (dbSuspendNote != null)
                {
                    _icmsContext.Remove(dbSuspendNote);
                    int result = _icmsContext.SaveChanges();

                    if (result > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }



        public List<Note> addCmNote(Note cmNote)
        {

            List<Note> cmNotes = null;

            if ((bool)cmNote.midMonthBill)
            {
                List<Note> suspendedNotes = addSuspendedNote(cmNote);

                return suspendedNotes;
            } 
            else
            {

                double lengthOfNote = cmNote.noteText.Length;

                if (lengthOfNote <= 512)
                {
                    cmNotes = addCmNoteSingle(cmNote);
                }
                else
                {
                    cmNotes = addCmNoteMultiple(cmNote, lengthOfNote);
                }
            }


            return cmNotes;
        }

        private List<Note> addCmNoteSingle(Note cmNote)
        {

            List<Note> cmNotes = null;            

            MemberNotes newNote = new MemberNotes();

            newNote.member_id = cmNote.memberId;
            newNote.record_date = cmNote.recordDate;
            newNote.record_seq_num = 1;
            newNote.user_updated = cmNote.caseOwnerId;
            newNote.evaluation_text = cmNote.noteText;
            newNote.billing_id = cmNote.billingId;
            newNote.RN_notes = cmNote.billingMinutes;
            newNote.entered_via_web = 1;

            _icmsContext.MemberNotes.Add(newNote);
            int result = _icmsContext.SaveChanges();

            if (result > 0)
            {

                if (cmNote.suspendNoteId > 0)
                {
                    removeSuspendedCmNote(cmNote.suspendNoteId);
                }

                cmNotes = getPatientCmNotes(cmNote.memberId.ToString());
            }

            return cmNotes;
        }

        private List<Note> addCmNoteMultiple(Note cmNote, double noteLength)
        {

            List<Note> cmNotes = null;

            int result = 0;
            double numberOfLines = 0;
            int totalNumberOfLines = 0;
            int starting = 0;
            double remainingLengthOfNote = 0;

            numberOfLines = noteLength / 512;

            if (numberOfLines > 0)
            {

                totalNumberOfLines = getCmNoteTotalNumberOfLines(numberOfLines);

                if (totalNumberOfLines > 0)
                {

                    for (int i = 0; i < totalNumberOfLines; i++)
                    {

                        MemberNotes newNote = new MemberNotes();


                        newNote.member_id = cmNote.memberId;
                        newNote.record_date = cmNote.recordDate;
                        newNote.record_seq_num = i + 1;
                        newNote.user_updated = cmNote.caseOwnerId;
                        newNote.evaluation_text = cmNote.noteText;
                        newNote.billing_id = cmNote.billingId;
                        newNote.RN_notes = cmNote.billingMinutes;
                        newNote.entered_via_web = 1;

                        starting = i * 512 + 1;

                        if (starting > 512)
                        {

                            remainingLengthOfNote = noteLength - starting;
                            newNote.evaluation_text = cmNote.noteText.Substring(starting, Convert.ToInt32(remainingLengthOfNote));
                        }
                        else
                        {
                            newNote.evaluation_text = cmNote.noteText.Substring(starting, 512);
                        }

                        _icmsContext.MemberNotes.Add(newNote);
                        result = _icmsContext.SaveChanges();
                    }
                }
            }


            if (result > 0)
            {
                
                if (cmNote.suspendNoteId > 0)
                {
                    removeSuspendedCmNote(cmNote.suspendNoteId);
                }

                cmNotes = getPatientCmNotes(cmNote.memberId.ToString());
            }

            return cmNotes;
        }

        
        private int getCmNoteTotalNumberOfLines(double numberOfLines)
        {

            if (numberOfLines == Convert.ToInt32(numberOfLines))
            {
                return Convert.ToInt32(numberOfLines);
            }
            else
            {
                return Convert.ToInt32(numberOfLines) + 1;
            }
        }



        private bool removeSuspendedCmNote(int suspendNoteId)
        {

            SuspendedNotes suspNote = (

                    from susNte in _icmsContext.SuspendedNoteses
                    where susNte.suspended_note_id.Equals(suspendNoteId)
                    select susNte
                )
                .FirstOrDefault();

            if (suspNote != null)
            {

                _icmsContext.Remove(suspNote);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    return true;
                }
            }

            return false;
        }



        public Note saveCmSummary(Note cmSummary)
        {

            Note summary = null;


            if (!cmSummary.recordDate.Equals(DateTime.MinValue))
            {

                DateTime firstOfMonth = Convert.ToDateTime(cmSummary.recordDate.Year.ToString()
                                                            + "-" +
                                                            cmSummary.recordDate.Month.ToString()
                                                            + "-01 00:00:00");

                MemberNotesSummary dbSummary = (

                        from memNteSum in _icmsContext.MemberNotesSummaries
                        where memNteSum.member_id.Equals(cmSummary.memberId)
                        && memNteSum.record_date.Equals(firstOfMonth)
                        && memNteSum.month_closed.Equals(false)
                        select memNteSum
                    )
                    .FirstOrDefault();

                if (dbSummary != null)
                {
                    summary = updateCmSummary(dbSummary, cmSummary);
                }
                else
                {
                    summary = addCmSummary(cmSummary);
                }
            }

            return summary;
        }

        private Note addCmSummary(Note cmSummary)
        {
            Note summary = null;

            if (!cmSummary.recordDate.Equals(DateTime.MinValue))
            {

                DateTime firstOfMonth = Convert.ToDateTime(cmSummary.recordDate.Year.ToString()
                                                            + "-" +
                                                            cmSummary.recordDate.Month.ToString()
                                                            + "-01 00:00:00");

                MemberNotesSummary newSummary = new MemberNotesSummary();
                newSummary.member_id = cmSummary.memberId;
                newSummary.record_date = firstOfMonth;
                newSummary.evaluation_text = cmSummary.noteText;
                newSummary.month_closed = false;
                newSummary.creation_date = DateTime.Now;
                newSummary.creation_system_user_id = cmSummary.caseOwnerId;

                _icmsContext.MemberNotesSummaries.Add(newSummary);

                if (_icmsContext.SaveChanges() > 0)
                {

                    summary = new Note();
                    summary = cmSummary;
                }
            }

            return summary;
        }

        private Note updateCmSummary(MemberNotesSummary dbSummary, Note cmSummary)
        {
            Note summary = null;

            dbSummary.evaluation_text = cmSummary.noteText;

            _icmsContext.MemberNotesSummaries.Update(dbSummary);

            if (_icmsContext.SaveChanges() > 0)
            {

                summary = new Note();
                summary = cmSummary;
            }            

            return summary;
        }



        public Note addCcmNote(Note cmNote)
        {

            Note cmNotes = null;

            if (addNewMemberLcmFollowupNotes(cmNote))
            {
                cmNotes = updateMemberLcmFollowup(cmNote);
            }

            return cmNotes;
        }

        private Note updateMemberLcmFollowup(Note cmNote)
        {

            Note cmNotes = null;

            MemberLcmFollowup dbMemberFollowup = getMemberFollowup(cmNote);

            if (dbMemberFollowup != null)
            {

                switch (cmNote.lcmNoteType)
                {

                    case "current medical status":
                        dbMemberFollowup.current_treatments = cmNote.noteText;
                        break;

                    case "case history":
                        dbMemberFollowup.previous_treatments = cmNote.noteText;
                        break;

                    case "medications":
                        dbMemberFollowup.future_treatments = cmNote.noteText;
                        break;

                    case "newly identified cm":
                        dbMemberFollowup.newly_identified_cm_summary = cmNote.noteText;
                        break;

                    case "service authorized":
                        dbMemberFollowup.nurse_summary = cmNote.noteText;
                        break;

                    case "family dynamics":
                        dbMemberFollowup.psycho_social_summary = cmNote.noteText;
                        break;

                    case "recommendations":
                        dbMemberFollowup.physician_prognosis = cmNote.noteText;
                        break;
                }

                _icmsContext.MemberLcmFollowups.Update(dbMemberFollowup);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    CaseManagementService caseServ = new CaseManagementService(_icmsContext, _aspNetContext, _dataStagingContext);
                    cmNotes = caseServ.getLcmReportNotes((int)cmNote.lcmFollowupId);
                }
            }

            return cmNotes;
        }

        private bool addNewMemberLcmFollowupNotes(Note cmNote)
        {

            MemberLcmFollowupNotes dbFollowupNotes = getFollowupNotes(cmNote);

            if (dbFollowupNotes != null)
            {

                switch (cmNote.lcmNoteType)
                {

                    case "current medical status":
                        dbFollowupNotes.current_treatments = cmNote.noteText;
                        break;

                    case "case history":
                        dbFollowupNotes.previous_treatments = cmNote.noteText;
                        break;

                    case "medications":
                        dbFollowupNotes.future_treatments = cmNote.noteText;
                        break;

                    case "newly identified cm":
                        dbFollowupNotes.newly_identified_cm = cmNote.noteText;
                        break;

                    case "service authorized":
                        dbFollowupNotes.nurse_summary = cmNote.noteText;
                        break;

                    case "family dynamics":
                        dbFollowupNotes.psycho_social_summary = cmNote.noteText;
                        break;

                    case "recommendations":
                        dbFollowupNotes.physician_prognosis = cmNote.noteText;
                        break;
                }

                _icmsContext.MemberLcmFollowupNoteses.Update(dbFollowupNotes);
                int result = _icmsContext.SaveChanges();

                if (result > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private MemberLcmFollowup getMemberFollowup(Note cmNote)
        {

            MemberLcmFollowup dbMemberFollowup = null;

            dbMemberFollowup = (

                    from followup in _icmsContext.MemberLcmFollowups
                    where followup.lcm_followup_id.Equals(cmNote.lcmFollowupId)
                    select followup
                )
                .FirstOrDefault();

            return dbMemberFollowup;
        }

        private MemberLcmFollowupNotes getFollowupNotes(Note cmNote)
        {

            MemberLcmFollowupNotes dbNewFollowupNotes = null;

            MemberLcmFollowupNotes oldNotes = removeCurrentFollowupNote(cmNote);
              
            dbNewFollowupNotes = createNewFollowupNote(cmNote, oldNotes);
            
            return dbNewFollowupNotes;
        }

        private MemberLcmFollowupNotes removeCurrentFollowupNote(Note cmNote)
        {

            MemberLcmFollowupNotes dbCurrentNote = null;

            dbCurrentNote = (

                    from followup in _icmsContext.MemberLcmFollowupNoteses
                    where followup.lcm_followup_id.Equals(cmNote.lcmFollowupId)
                    && followup.current_note > 0
                    orderby followup.creation_date descending
                    select followup
                )
                .FirstOrDefault();

            if (dbCurrentNote == null)
            {

                dbCurrentNote.current_note = 0;

                _icmsContext.MemberLcmFollowupNoteses.Update(dbCurrentNote);
                _icmsContext.SaveChanges();
            }

            return dbCurrentNote;
        }

        private MemberLcmFollowupNotes createNewFollowupNote(Note cmNote, MemberLcmFollowupNotes oldNotes)
        {

            MemberLcmFollowupNotes dbFollowupNote = new MemberLcmFollowupNotes();
            dbFollowupNote.lcn_case_number = cmNote.lcnCaseNumber;
            dbFollowupNote.lcm_followup_id = cmNote.lcmFollowupId;
            dbFollowupNote.member_id = cmNote.memberId;
            dbFollowupNote.current_note = 1;
            dbFollowupNote.moved_to_previous_treatment = 0;
            dbFollowupNote.creation_date = cmNote.recordDate;
            dbFollowupNote.creation_user_id = cmNote.caseOwnerId;
            dbFollowupNote.current_treatments = oldNotes.current_treatments;
            dbFollowupNote.future_treatments = oldNotes.future_treatments;
            dbFollowupNote.newly_identified_cm = oldNotes.newly_identified_cm;
            dbFollowupNote.nurse_summary = oldNotes.nurse_summary;
            dbFollowupNote.previous_treatments = oldNotes.previous_treatments;
            dbFollowupNote.psycho_social_summary = oldNotes.psycho_social_summary;

            _icmsContext.MemberLcmFollowupNoteses.Add(dbFollowupNote);
            int result = _icmsContext.SaveChanges();

            if (result > 0)
            {
                return dbFollowupNote;
            }
            else
            {
                return null;
            }
        }

        public List<DocumentForm> addCmAttachment(Note cmAttach)
        {

            List<DocumentForm> attachments = null;

            MemberNotesAttachment newAttach = new MemberNotesAttachment();

            newAttach.member_id = cmAttach.memberId;
            newAttach.record_date = cmAttach.recordDate;
            newAttach.file_identifier = cmAttach.attachments[0].documentFileName;
            newAttach.file_blob = cmAttach.attachments[0].documentImage;
            newAttach.creation_user_id = cmAttach.caseOwnerId;
            newAttach.entered_via_web = 1;

            _icmsContext.MemberNotesAttachments.Add(newAttach);
            int result = _icmsContext.SaveChanges();

            if (result > 0)
            {
                attachments = getPatientCmAttachments(cmAttach.memberId.ToString());
            }

            return attachments;
        }


    }
}
