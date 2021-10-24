using PassengerApi.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PassengerApi.Model
{
    public class Passenger
    {
        [Key]
        public Guid UniquePassengerId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public int DocumentNo { get; set; }
        [Required]
        public DocumentType DocumentType { get; set; }
        [Required]
        public DateTime IssueDate { get; set; }
    }
}
