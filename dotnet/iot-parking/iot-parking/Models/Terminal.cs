using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

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
