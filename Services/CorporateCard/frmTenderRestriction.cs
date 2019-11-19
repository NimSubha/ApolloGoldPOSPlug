/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using DevExpress.XtraEditors;
using LSRetailPosis.POSProcesses;

namespace Microsoft.Dynamics.Retail.Pos.CorporateCard
{
	public partial class frmTenderRestriction : frmTouchBase
	{
		#region Variables and Properties

		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Msg", Justification = "Cannot change public API.")]
		public string DisplayMsg
		{
			set { memMessage.Text = value; }
		}

		#endregion

		public frmTenderRestriction(RetailTransaction transaction)
		{
			RetailTransaction retailTransaction = transaction as RetailTransaction;
			if (retailTransaction == null)
				throw new ArgumentNullException("transaction");

			try
			{
				InitializeComponent();

				btnNo.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(50154); //No
				btnYes.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(50155); //Yes
				lblContinue.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(50156); //Do you want to continue?
				lblExcluded.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(50157); //Items excluded from payment

				for (int i = 0; i < retailTransaction.SaleItems.Count; i++)
				{
					SaleLineItem lineItem = retailTransaction.GetItem(i + 1);

					if (string.IsNullOrEmpty(lineItem.TenderRestrictionId))
					{
						lstExcluded.Items.Add(lineItem.Description);
					}
				}
				btnYes.Focus();
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
