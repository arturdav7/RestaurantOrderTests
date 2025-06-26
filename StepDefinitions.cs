using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TechTalk.SpecFlow;
using NUnit.Framework;

namespace RestaurantOrderTests
{
    [Binding]
    public class StepDefinitions
    {
        private class OrderCollection
        {
            public int Starters { get; set; }
            public int Mains { get; set; }
            public int Drinks { get; set; }
            public DateTime Time { get; set; }
        }

        private List<OrderCollection> _orders = new();
        private decimal _total;
        private decimal _intermediateTotal;
        private OrderCalculator _calculator = new OrderCalculator();

        // This step initializes an order; parameters are parsed from scenario
        [Given(@"the order is (\d+) starter[s]?, (\d+) main[s]?, (\d+) drink[s]? at ""(.*)""")]
        public void CreateOrder(int starters, int mains, int drinks, string time)
        {
            DateTime orderTime = DateTime.ParseExact(time, "HH:mm", CultureInfo.InvariantCulture);
            _orders.Add(new OrderCollection
            {
                Starters = starters,
                Mains = mains,
                Drinks = drinks,
                Time = orderTime
            });
        }

        // Scenario step: more people join, so add new order with new parameters
        [When(@"the party grows to (.*) people at ""(.*)"" and (.*) mains and (.*) drinks are added")]
        public void JoinParty(int people, string time, int mains, int drinks)
        {
            DateTime joinTime = DateTime.ParseExact(time, "HH:mm", CultureInfo.InvariantCulture);
            _orders.Add(new OrderCollection
            {
                Starters = 0,
                Mains = mains,
                Drinks = drinks,
                Time = joinTime
            });
        }

        // Scenario step: a member cancels some items
        [When(@"a member cancels (.*) starter, (.*) main, and (.*) drink")]
        public void CancelItems(int starters, int mains, int drinks)
        {
            var lastOrder = _orders.LastOrDefault();
            if (lastOrder == null) return;

            Console.WriteLine($"Before cancel: {lastOrder.Starters} starters, {lastOrder.Mains} mains, {lastOrder.Drinks} drinks");

            // Decrease items from the last order (cancelled items)
            lastOrder.Starters -= starters;
            lastOrder.Mains -= mains;
            lastOrder.Drinks -= drinks;

            // Ensuring no negative values
            lastOrder.Starters = Math.Max(0, lastOrder.Starters);
            lastOrder.Mains = Math.Max(0, lastOrder.Mains);
            lastOrder.Drinks = Math.Max(0, lastOrder.Drinks);

            Console.WriteLine($"After cancel: {lastOrder.Starters} starters, {lastOrder.Mains} mains, {lastOrder.Drinks} drinks");
        }

        // Calculate total for all orders
        [When("I calculate the total")]
        public void CalculateTotal()
        {
            // Sum by calculating each order's price and adding them up
            decimal totalSum = 0;
            foreach (var order in _orders)
            {
                Console.WriteLine($"[TOTAL] Calculating for order at {order.Time:HH:mm} → " +
                          $"Starters: {order.Starters}, Mains: {order.Mains}, Drinks: {order.Drinks}");

                totalSum += _calculator.CalculateBill(order.Starters, order.Mains, order.Drinks, order.Time);
            }
            _total = Math.Round(totalSum, 2);
            Console.WriteLine($"[TOTAL] Final Total: £{_total}");
        }

        // Calculate total and store as a temporary/intermediate total
        [When("I calculate the total and store it temporarily")]
        public void CalculateIntermediateTotal()
        {
            decimal intermediateSum = 0;
            foreach (var order in _orders)
            {
                Console.WriteLine($"[INTERMEDIATE] Calculating for order at {order.Time:HH:mm} → " +
                          $"Starters: {order.Starters}, Mains: {order.Mains}, Drinks: {order.Drinks}");

                intermediateSum += _calculator.CalculateBill(order.Starters, order.Mains, order.Drinks, order.Time);
            }
            _intermediateTotal = Math.Round(intermediateSum, 2);
            Console.WriteLine($"[INTERMEDIATE] Stored Intermediate Total: £{_intermediateTotal}");
        }

        // Assert: check if the total matches expected
        [Then(@"the total should be (.*)")]
        public void CheckTotal(decimal expected)
        {
            Assert.That(_total, Is.EqualTo(expected));
        }

        // Assert: check if the intermediate total matches expected
        [Then(@"the intermediate total should be (.*)")]
        public void CheckIntermediateTotal(decimal expected)
        {
            Assert.That(_intermediateTotal, Is.EqualTo(expected));
        }
    }
}