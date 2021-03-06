using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace iot_parking.Models
{

    [Index(nameof(RFIDCard.CardNumber), IsUnique = true)]
    public class RFIDCard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string CardNumber { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public CardOwner CardOwner { get; set; }

        public ICollection<Parking> Parkings { get; set; }
    }
}
