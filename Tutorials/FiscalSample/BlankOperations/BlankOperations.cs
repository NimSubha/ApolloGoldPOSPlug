using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessObjects;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations
{
    [Export(typeof(IBlankOperations))]
    public class BlankOperationsService : IBlankOperations
    {
        // Get all text through the Translation function in the ApplicationLocalizer
        // TextID's for BlankOperations are reserved at 50700 - 50999

        [Import]
        public IApplication Application { get; set; }

        #region IBlankOperations Members

        [CLSCompliant(false)]
        public void BlankOperation(IBlankOperationInfo operationInfo, IPosTransaction posTransaction)
        {
            if (operationInfo == null)
            {
                throw new ArgumentNullException("operationInfo");
            }

            switch (operationInfo.OperationId)
            {
                case "2":
                    using (Form theForm = new ItemPriceListForm(Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID))
                    {
                        theForm.ShowDialog();
                    }                    
                    break;
                case "1":
                    // Note: can also use  operationInfo.Parameter
                    using (Form theForm = new GeneralFiscalForm())
                    {
                        theForm.ShowDialog();
                    } break;
                default:
                    using (Form theForm = new GeneralFiscalForm())
                    {
                        theForm.ShowDialog();
                    }

                    break;
            }
        }

        #endregion
    }
}
