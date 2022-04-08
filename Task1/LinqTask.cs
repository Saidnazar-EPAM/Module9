using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Task1.DoNotChange;

namespace Task1
{
    public static class LinqTask
    {
        public static IEnumerable<Customer> Linq1(IEnumerable<Customer> customers, decimal limit)
        {
            return customers.Where(b => b.Orders.Sum(c => c.Total) > limit);
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            return customers.Select(b => (b, suppliers.Where(c => c.Country == b.Country && c.City == b.City)));
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2UsingGroup(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            return customers.GroupBy(b => b).Select(b => (b.Key, suppliers.Where(c => c.SupplierName == b.Key.Country && b.Key.City == b.Key.City)));
        }

        public static IEnumerable<Customer> Linq3(IEnumerable<Customer> customers, decimal limit)
        {
            return customers.Where(b => b.Orders.Count(c => c.Total > limit) > 0);
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq4(
            IEnumerable<Customer> customers
        )
        {
            return customers
                .Where(b => b.Orders.Any())
                .Select(b => (b, b.Orders.OrderBy(c => c.OrderDate).Select(c => c.OrderDate).FirstOrDefault()));
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq5(
            IEnumerable<Customer> customers
        )
        {
            return customers
                .Where(b => b.Orders.Any())
                .Select(b => (customer: b, dateOfEntry: b.Orders.OrderBy(c => c.OrderDate).Select(c => c.OrderDate).FirstOrDefault()))
                .OrderBy(b => b.dateOfEntry.Year)
                .ThenBy(b => b.dateOfEntry.Month)
                .ThenByDescending(b => b.customer.Orders.Sum(c => c.Total))
                .ThenBy(b => b.customer.CompanyName);
        }

        public static IEnumerable<Customer> Linq6(IEnumerable<Customer> customers)
        {
            var haveOperatorCode = new Regex(@"\(\d+\)");
            var onlyDigit = new Regex(@"^\s*\d+\s*$");
            return customers
                .Where(b => !onlyDigit.IsMatch(b.PostalCode) || string.IsNullOrWhiteSpace(b.Region) || !haveOperatorCode.IsMatch(b.Phone));
        }

        public static IEnumerable<Linq7CategoryGroup> Linq7(IEnumerable<Product> products)
        {
            /* example of Linq7result

             category - Beverages
	            UnitsInStock - 39
		            price - 18.0000
		            price - 19.0000
	            UnitsInStock - 17
		            price - 18.0000
		            price - 19.0000
             */

            return products.GroupBy(products => products.Category).Select(b => new Linq7CategoryGroup
            {
                Category = b.Key,
                UnitsInStockGroup =
                b.GroupBy(c => c.UnitsInStock).Select(c => new Linq7UnitsInStockGroup { UnitsInStock = c.Key, Prices = c.OrderBy(d => d.UnitPrice).Select(d => d.UnitPrice) })
            });
        }

        public static IEnumerable<(decimal category, IEnumerable<Product> products)> Linq8(
            IEnumerable<Product> products,
            decimal cheap,
            decimal middle,
            decimal expensive
        )
        {
            return products.GroupBy(b =>
            {
                if (b.UnitPrice <= cheap)
                {
                    return cheap;
                }
                else if (b.UnitPrice > cheap && b.UnitPrice <= middle)
                {
                    return middle;
                }
                else
                {
                    return expensive;
                }
            }).Select(b => (b.Key, b.AsEnumerable()));
            
        }

        public static IEnumerable<(string city, int averageIncome, int averageIntensity)> Linq9(
            IEnumerable<Customer> customers
        )
        {
            return customers.GroupBy(b => b.City).Select(b => (b.Key, 
                (int)Math.Round(b.Average(c => c.Orders.Any() ? c.Orders.Sum(d => d.Total) : 0)), 
                (int)Math.Round(b.Average(c => c.Orders.Any()? c.Orders.Count() : 0))));
        }

        public static string Linq10(IEnumerable<Supplier> suppliers)
        {
            return suppliers.Select(b => b.Country)
                .Distinct()
                .OrderBy(b => b.Length)
                .ThenBy(b => b)
                .Aggregate((b, c) => b + c);
        }
    }
}