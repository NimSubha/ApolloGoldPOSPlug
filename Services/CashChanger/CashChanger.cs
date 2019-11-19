/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.CashChanger
{
    [Export(typeof(ICashChanger))]
    public class CashChanger : ICashChanger, IInitializeable
    {
        //
        // Get all text through the Translation function in the ApplicationLocalizer
        //
        // TextID's for CashChanger service are reserved at 64000 - 64999
        //

        #region Fields

        public event InsertedAmountDelegate InsertedAmount
        {
            // add empty handlers on unused event to avoid compiler warnings CS0535 and CS0667.
            // See http://blogs.msdn.com/b/trevor/archive/2008/08/14/c-warning-cs0067-the-event-event-is-never-used.aspx
            add { }
            remove { }
        }

        public event LevelStatusDelegate LevelStatusEvent
        {   // add empty handlers on unused event to avoid compiler warnings CS0535 and CS0667.
            // See http://blogs.msdn.com/b/trevor/archive/2008/08/14/c-warning-cs0067-the-event-event-is-never-used.aspx
            add { }
            remove { }
        }

        public event ErrorEventDelegate ErrorEvent
        {   // add empty handlers on unused event to avoid compiler warnings CS0535 and CS0667.
            // See http://blogs.msdn.com/b/trevor/archive/2008/08/14/c-warning-cs0067-the-event-event-is-never-used.aspx
            add { }
            remove { }
        }

        #endregion

        #region ICashChanger Members

        /// <summary>
        /// Get auto mode.
        /// </summary>
        /// <returns></returns>
        public bool GetAutoMode()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Set auto mode.
        /// </summary>
        /// <param name="mode"></param>
        public void SetAutoMode(bool mode)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// The login function is used to register the user with on Cash Changer Device
        /// </summary>
        /// <param name="terminalID"></param>
        /// <param name="operatorID"></param>
        /// <returns>True if successful, false otherwise</returns>
        public bool Login(string terminalID, string operatorID)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Logout user from Cash changer device.
        /// </summary>
        /// <param name="terminalID"></param>
        /// <param name="operatorID"></param>
        /// <returns>True if successful, false otherwise</returns>
        public bool Logout()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// This functions registers in to the transaction the amount that has been entered
        /// into the cash machine.
        /// </summary>
        /// <param name="amountDue">The amount that should be paid by the customer</param>
        /// <param name="receiptID">The receipt ID of the transaction</param>
        /// <param name="amountRest">If not enough amount has been entered, this parameter will specify what is left to pay</param>
        /// <returns>An instance of the CashChangerReturn enum specifying the </returns>
        public CashChangerReturn RegisterAmount(decimal amountDue, string receiptID, ref decimal amountRest)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Return false.
        /// </summary>
        /// <returns></returns>
        public bool Change()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Return false.
        /// </summary>
        /// <returns></returns>
        public bool Reset()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Return false.
        /// </summary>
        /// <param name="regretType"></param>
        /// <returns></returns>
        public bool Regret(CashChangerRegretType regretType)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Return false.
        /// </summary>
        /// <returns></returns>
        public bool Exit()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Conclude transaction.
        /// </summary>
        /// <param name="posTransaction"></param>
        public void ConcludeTransaction(IPosTransaction posTransaction)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        #endregion

        #region IInitializeable Members

        public void Initialize()
        {
        }

        public void Uninitialize()
        {
        }

        #endregion

    }
}
