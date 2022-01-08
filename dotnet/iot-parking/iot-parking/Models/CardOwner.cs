using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace iot_parking.Models
{
    public class CardOwner
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Firstname { get; set; }

        [Required]
        [MaxLength(50)]
        public string Lastname { get; set; }

        [Required]
        [MaxLength(50)]
        public string Email { get; set; }

        [Required]
        public DateTime IssueDate { get; set; }

        public DateTime ValidDate { get; set; }

        [Required]
        public int CardId { get; set; }

        [ForeignKey("CardId")]
        public RFIDCard RFIDCard { get; set; }
    }
}
