using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iot_parking.Models
{
    public class Parking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime EntryDate { get; set; }

        public DateTime? ExitDate { get; set; }

        [Required]
        public int CardId { get; set; }

        [ForeignKey("CardId")]
        public RFIDCard RFIDCard { get; set; }
    }
}
