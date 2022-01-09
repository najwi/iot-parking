using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace iot_parking.Models
{
    [Index(nameof(ScannedCard.CardNumber), IsUnique = true)]
    public class ScannedCard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string CardNumber { get; set; }

        [Required]
        public DateTime ScanDate { get; set; }
    }
}
