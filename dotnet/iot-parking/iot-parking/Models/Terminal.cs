using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace iot_parking.Models
{
    public enum TerminalTypes
    {
        EntryGate,
        ExitGate,
        CardReader
    }


    [Index(nameof(Terminal.TerminalNumber), IsUnique = true)]
    public class Terminal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string TerminalNumber { get; set; }

        [Required]
        public TerminalTypes Type { get; set; }
    }
}
