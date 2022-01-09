using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace iot_parking.Models
{
    public class CardOwnerRFIDCard
    {
        [Required]
        [MaxLength(255)]
        public string CardNumber { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [MaxLength(50)]
        public string Firstname { get; set; }

        [MaxLength(50)]
        public string Lastname { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }

        public DateTime? IssueDate { get; set; }

        public DateTime? ValidDate { get; set; }

        public bool HasOwner { get; set; }
    }
}
