using System;
using System.ComponentModel.DataAnnotations;

namespace ClubManagment.ViewModels
{
    public class ServiceViewModel
    {
        //public int Id { get; set; }

        //[Required(ErrorMessage = "Service name is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Service name must be between 2 and 200 characters")]
        //public string Name { get; set; }

        //public string Description { get; set; }

        //public decimal Price { get; set; }

        public int Id { get; set; }

        [Required(ErrorMessage = "Service name is required"), StringLength(200, MinimumLength = 2, ErrorMessage = "Service name must be between 2 and 200 characters")]
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

    }
}
