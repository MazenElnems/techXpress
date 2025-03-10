using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities.Enums
{
    public enum OrderStatus
    {
        /// <summary>
        /// The order has been placed but is awaiting payment confirmation or further processing.
        /// </summary>
        Pending,

        /// <summary>
        ///  is confirmation that the order is valid and will be processed.
        /// </summary>
        Approved,

        /// <summary>
        /// order is not valid and it will not be processed
        /// </summary>
        Failed,

        /// <summary>
        /// the order is being prepared for shipment.
        /// </summary>
        InProgress,

        /// <summary>
        /// customer can cancel the order before it shipped
        /// </summary>
        Canceled,

        /// <summary>
        /// the order shipped successfully.
        /// </summary>
        Shipped
    }
}
