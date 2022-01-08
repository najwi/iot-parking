using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
