using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Microsoft.Dynamics.Retail.Pos.SimulatedFiscalPrinter
{
    /// <summary>
    /// Class that contains line item information for the fiscal printer receipt
    /// </summary>
    public sealed class LineItem
    {
        /// <summary>
        /// Gets or sets the item lookup code.
        /// </summary>
        /// <value>The item lookup code.</value>
        public string ItemLookupCode { get; private set;  }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; private set;  }
        
        /// <summary>
        /// Gets or sets the tax rate id.
        /// </summary>
        /// <value>The tax rate id.</value>
        public string TaxRateId { get; private set; }
        
        /// <summary>
        /// Gets or sets the price.
        /// </summary>
        /// <value>The price.</value>
        public decimal Price { get; private set; }
        
        /// <summary>
        /// Gets or sets the adjusted price (this is the price with discounts applied)
        /// </summary>
        /// <value>The adjusted price.</value>
        public decimal AdjustedPrice { get; private set;  }
        
        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        /// <value>The quantity.</value>
        public decimal Quantity { get; private set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="LineItem"/> is voided.
        /// </summary>
        /// <value><c>true</c> if voided; otherwise, <c>false</c>.</value>
        public bool Voided { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineItem"/> class.
        /// </summary>
        /// <param name="itemLookupCode">The item lookup code.</param>
        /// <param name="itemDescription">The item description.</param>
        /// <param name="itemPrice">The item price.</param>
        /// <param name="taxRateId">The tax rate id.</param>
        /// <param name="itemQuantity">The item quantity.</param>
        public LineItem(string itemLookupCode, string itemDescription, decimal itemPrice, string taxRateId, int itemQuantity)
        {
            this.ItemLookupCode = itemLookupCode;
            this.Description = itemDescription;
            this.Price = itemPrice;
            this.AdjustedPrice = this.Price;
            this.TaxRateId = taxRateId;
            this.Quantity = itemQuantity;
            this.Voided = false;
        }

        /// <summary>
        /// Applies the discount percent.
        /// </summary>
        /// <param name="percentDiscount">The percent discount.</param>
        public void ApplyDiscountPercent(decimal percentDiscount)
        {
            Debug.Assert((percentDiscount >= 0m) && (percentDiscount <= 100m));

            Decimal tempAdjustedPrice = Price - Price * (percentDiscount / 100m);
            
            if (tempAdjustedPrice < 0m)
            {
                tempAdjustedPrice = 0m;
            }

            this.AdjustedPrice = tempAdjustedPrice;
        }

        /// <summary>
        /// Applies the discount amount.
        /// </summary>
        /// <param name="amountDiscount">The amount discount.</param>
        public void ApplyDiscountAmount(decimal amountDiscount)
        {
            Debug.Assert((amountDiscount >= 0m) && (amountDiscount <= Price));

            Decimal tempAdjustedPrice = Price - amountDiscount;

            if (tempAdjustedPrice < 0m)
            {
                tempAdjustedPrice = 0m;
            }

            this.AdjustedPrice = tempAdjustedPrice;
        }


    }
}
