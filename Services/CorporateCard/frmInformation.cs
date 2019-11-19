/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LSRetailPosis.POSProcesses;
using DevExpress.XtraEditors;

namespace Microsoft.Dynamics.Retail.Pos.CorporateCard
{
	public partial class frmInformation : frmTouchBase
	{
		#region Member variables and properties

		private string driverId;
		private string vehicleId;
		private string odometer;

		public string DriverId
		{
			get { return driverId; }
		}

		public string VehicleId
		{
			get { return vehicleId; }
		}

		public string Odometer
		{
			get { return odometer; }
		}

		#endregion

		public frmInformation()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			if (!this.DesignMode)
			{
				btnOK.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(50158); //OK
				lblDriverId.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(50159); //Driver Id
				lblVehicleId.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(50160); //Vehicle Id
				lblOdometer.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(50161);  //Odometer    
                lblHeading.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(50164); //Pay corporate card          
				ResetAllColors();
			}

			base.OnLoad(e);
		}

		private void ResetAllColors()
		{
			try
			{
				Color color = Color.Transparent;
				txtDriverId.BackColor = color;
				txtVehicleId.BackColor = color;
				txtOdometer.BackColor = color;
			}
			catch (Exception x)
			{
				LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
				throw;
			}
		}

		private void Keyboard_EnterButtonPressed()
		{
			try
			{
				if (txtDriverId.BackColor == Color.Gray)
				{
					txtDriverId.Text = txtKeyboardInput.Text;
					driverId = txtDriverId.Text.Trim();
				}
				else if (txtVehicleId.BackColor == Color.Gray)
				{
					txtVehicleId.Text = txtKeyboardInput.Text;
					vehicleId = txtVehicleId.Text.Trim();
				}
				else if (txtOdometer.BackColor == Color.Gray)
				{
					if (IsNumber(txtKeyboardInput.Text))
					{
						txtOdometer.Text = txtKeyboardInput.Text;
						odometer = txtOdometer.Text.Trim();
					}
				}
				txtKeyboardInput.Text = string.Empty;
			}
			catch (Exception x)
			{
				LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
				throw;
			}
		}

		private static bool IsNumber(string str)
		{
			try
			{
				Convert.ToInt64(str);
				return true;
			}
			catch (Exception)
			{
				using (frmMessage dialog = new frmMessage(50163, MessageBoxButtons.OK, MessageBoxIcon.Error)) //Odometer can not have letters in it.
				{
					POSFormsManager.ShowPOSForm(dialog);
				}
			}
			return false;
		}

		private void txtKeyboardInput_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				Keyboard_EnterButtonPressed();
			}
		}
	}
}
