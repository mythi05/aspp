
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspp.Models
{
    public class Contract
    {
        public int Id { get; set; }

        [Required]
        public string ContractCode { get; set; } = ""; // HD001

        // ===== FK =====
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public int RoomId { get; set; }
        public Room? Room { get; set; }

        // ===== DATA =====
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal MonthlyFee { get; set; }
        public decimal Deposit { get; set; }

        public int Status { get; set; } // 1: active, 0: expired
    }
}
