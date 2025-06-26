using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantOrderTests
{
    public class OrderCalculator
    {
        const decimal StarterPrice = 4.00m;
        const decimal MainPrice = 7.00m;
        const decimal DrinkPrice = 2.50m;

        public decimal CalculateBill(int starters, int mains, int drinks, DateTime orderTime)
        {
            Console.WriteLine($"Calculating: {starters} starter(s), {mains} main(s), {drinks} drink(s) at {orderTime:HH:mm}");

            decimal foodTotal = starters * StarterPrice + mains * MainPrice;

            decimal drinkCost = DrinkPrice;
            if (orderTime.Hour < 19)
            {
                drinkCost *= 0.7m; // 30% discount
            }

            decimal drinksTotal = drinks * drinkCost;

            decimal serviceCharge = foodTotal * 0.10m; // 10% charge only for food 

            decimal total = foodTotal + drinksTotal + serviceCharge;

            return Math.Round(total, 2);
        }
    }
}
