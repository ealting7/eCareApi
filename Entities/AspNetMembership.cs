using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCareApi.Entities
{
    [Table("aspnet_Membership")]
    public class AspNetMembership
    {
        public Guid ApplicationId { get; set; }
        public Guid UserId { get; set; }
        public string Password { get; set; }
        public int PasswordFormat { get; set; }
        public string PasswordSalt { get; set; }
        public string? MobilePIN { get; set; }
        public string? Email { get; set; }
        public string? LoweredEmail { get; set; }
        public string? PasswordQuestion { get; set; }
        public string? PasswordAnswer { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsLockedOut { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public DateTime? LastLockoutDate { get; set; }
        public int? FailedPasswordAttemptCount { get; set; }
        public DateTime? FailedPasswordAttemptWindowStart { get; set; }
        public int? FailedPasswordAnswerAttemptCount { get; set; }
        public DateTime? FailedPasswordAnswerAttemptWindowStart { get; set; }
        public string? Comment { get; set; }
        public Guid? userEntryID { get; set; }
        public byte? is_forja { get; set; }
        public byte? added_by_download_file { get; set; }
        public DateTime? download_file_date { get; set; }


    }
}
