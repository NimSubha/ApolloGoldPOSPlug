/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Dynamics.Retail.Pos.Services.InfoCodes
{
    internal partial class FormInfoCodeSubCodeBase : LSRetailPosis.POSProcesses.frmTouchBase
    {
        protected string       infoCodePrompt;
        protected string       infoCodeId;
        protected string       selectedDescription;
        protected string       selectedSubCodeId;
        protected int          triggerFunction;
        protected string       triggerCode;
        protected int          priceType;
        protected decimal      amountPercent;
        protected bool         inputRequired;
        protected const string ItemDescriptionFormat = "{0} : {1}"; // <ItemID> : <ItemDescription>

        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Grandfather")]
        protected List<LSRetailPosis.POSProcesses.SubcodeInfo> SubCodes;

        public string InfoCodePrompt
        {
            set { infoCodePrompt = value; }
        }

        public string InfoCodeId
        {
            set { infoCodeId = value; }
        }

        public string SelectedDescription
        {
            get { return selectedDescription; }
        }

        public string SelectedSubcodeId
        {
            get { return selectedSubCodeId; }
        }

        public int SelectedTriggerFunction
        {
            get { return triggerFunction; }
        }

        public string SelectedTriggerCode
        {
            get { return triggerCode; }
        }

        public int SelectedPriceType
        {
            get { return priceType; }
        }

        public decimal SelectedAmountPercent
        {
            get { return amountPercent; }
        }

        public bool InputRequired
        {
            set { inputRequired = value; }
        }

        public Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.ISaleLineItem Item
        {
            get;
            set;
        }

        public FormInfoCodeSubCodeBase()
        {
            InitializeComponent();
        }
    }
}