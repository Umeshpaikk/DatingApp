using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class RegisterDTO
    {

        [Required] public string username{get;set;}
        [Required] public string KnownAs { get; set; }
        [Required] public string Gender { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
        [Required] public string City { get; set; }
        [Required] public string Country { get; set; }


        [StringLength(maximumLength:8, MinimumLength =4)]
        [Required]
        public string password{get;set;} 
    }
}