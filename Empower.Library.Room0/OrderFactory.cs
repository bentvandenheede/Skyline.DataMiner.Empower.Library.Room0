using Skyline.DataMiner.Empower.Library.Room0;

using System;
using System.Collections.Generic;
using System.Text;

namespace Empower.Library.Room0
{
    /// <summary>
    /// Factory to create Orders.
    /// </summary>
    public static class OrderFactory
    {
        /// <summary>
        /// Create an instance of <see cref="IOrder"/>.
        /// </summary>
        /// <param name="orderValue">Value of the order.</param>
        /// <returns>An instance of <see cref="IOrder"/>.</returns>
        public static IOrder CreateOrder(string orderValue)
        {
            return new Order(orderValue);
        }
    }
}
