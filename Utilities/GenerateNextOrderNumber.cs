using Kundur_Meghna_HW5.DAL;
using System;
using System.Linq;

namespace Kundur_Meghna_HW5.Utilities
{
    internal class GenerateNextOrderNumber
    {
        public static Int32 GetNextOrderNumber(AppDbContext _context)
        {
            //set a constant to designate where the order numbers 
            //should start
            const Int32 START_NUMBER = 70000;

            Int32 intMaxOrderNumber; 
            Int32 intNextOrderNumber; 

            if (_context.Orders.Count() == 0) 
            {
                intMaxOrderNumber = START_NUMBER; 
            }
            else
            {
                intMaxOrderNumber = _context.Orders.Max(c => c.OrderNumber); 
            }

            if (intMaxOrderNumber < START_NUMBER)
            {
                intMaxOrderNumber = START_NUMBER;
            }

            //add one to the current max to find the next one
            intNextOrderNumber = intMaxOrderNumber + 1;

            //return the value
            return intNextOrderNumber;
        }
    }
}