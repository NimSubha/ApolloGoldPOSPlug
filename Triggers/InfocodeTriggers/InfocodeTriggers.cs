//Microsoft Dynamics AX for Retail POS Plug-ins 
//The following project is provided as SAMPLE code. Because this software is "as is," we may not provide support services for it.

using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;

namespace Microsoft.Dynamics.Retail.Pos.InfocodeTriggers
{

    [Export(typeof(IInfocodeTrigger))]
    public class InfocodeTriggers : IInfocodeTrigger
    {

        #region Constructor - Destructor

        public InfocodeTriggers()
        {
            
            // Get all text through the Translation function in the ApplicationLocalizer
            // TextID's for InfocodeTriggers are reserved at 59000 - 59999
        }

        #endregion

        #region IInfocodeTriggers Members

        /// <summary>
        /// Before the infocode is processed this trigger is called. If the infocode should not be processed 
        /// after the trigger has finished running then PreTriggerResults needs to be filled out accordingly.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param> 
        /// <param name="tableRefId">What table does the infocode apply to</param>
        public void PreProcessInfocode(IPreTriggerResult preTriggerResult, IPosTransaction transaction, InfoCodeTableRefType tableRefId)
        {
        }

        /// <summary>
        /// After the infocode has been processed this trigger is called. Any checking and/or additional input validation and etc. can be done here.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="tableRefId">What table does te infode apply to</param>
        public void PostProcessInfocode(IPosTransaction transaction, InfoCodeTableRefType tableRefId)
        {
        }

        #endregion

    }
}
