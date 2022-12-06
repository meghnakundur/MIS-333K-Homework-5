using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kundur_Meghna_HW5.Models
{
    public class Supplier
    {
        [Display(Name = "Supplier ID:")]
        public Int32 SupplierID { get; set; }

        [Display(Name = "Supplier Name:")]
        [Required(ErrorMessage = "Supplier Name is required.")]
        public String SupplierName { get; set; }

        [Display(Name = "Email:")]
        public String Email { get; set; }

        [Display(Name = "Phone Number:")]
        public String PhoneNumber { get; set; }

        //navigational property
        public List<Product> Products { get; set; }


        public Supplier()
        {
            if (Products == null)
            {
                Products = new List<Product>();
            }
        }

    }
}
