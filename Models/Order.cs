using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Kundur_Meghna_HW5.Models
{
    public class Order
    {
        public const Decimal TAX_RATE = 0.0825m;

        [Display(Name = "Order ID:")]
        public Int32 OrderID { get; set; }

        [Display(Name = "Order Number:")]
        public Int32 OrderNumber { get; set; }

        [Display(Name = "Order Notes:")]
        public String OrderNotes { get; set; }

        [Display(Name = "Order Date:")]
        [DisplayFormat(DataFormatString = "{0:MMMM d, yyyy}")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Order Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal OrderSubtotal
        {
            get { return OrderDetails.Sum(rd => rd.ExtendedPrice); }
        }

        [Display(Name = "Tax Rate (8.25%)")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal TaxFee
        {
            get { return OrderSubtotal * TAX_RATE; }
        }

        [Display(Name = "Order Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal OrderTotal
        {
            get { return OrderSubtotal + TaxFee; }
        }

        // navigational properties
        public List<OrderDetail> OrderDetails { get; set; }
        public AppUser User { get; set; }

        public Order()
        {
            if (OrderDetails == null)
            {
                OrderDetails = new List<OrderDetail>();
            }
        }
    }
}