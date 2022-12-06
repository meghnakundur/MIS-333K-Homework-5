using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Kundur_Meghna_HW5.Models;

namespace Kundur_Meghna_HW5.Models
{
    public class OrderDetail
    {
        public Int32 OrderDetailID { get; set; }

        [Display(Name = "Quantity:")]
        [Range(0, 1000000, ErrorMessage = "Quantity must be at least 0")]
        public Int32 Quantity { get; set; }

        [Display(Name = "Product Price:")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public Decimal Price { get; set; }

        [Display(Name = "Extended Price:")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public Decimal ExtendedPrice { get; set; }

        //navigational properties
        public Order Order { get; set; }
        public Product Product { get; set; }

    }
}
