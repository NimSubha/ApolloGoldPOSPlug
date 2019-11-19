/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.InfocodeItem;
using LSRetailPosis.Transaction.Line.SaleItem;
using LSRetailPosis.Transaction.Line.TenderItem;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.Contracts.UI;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace Microsoft.Dynamics.Retail.Pos.Services.InfoCodes
{
    /// <summary>
    /// stub class
    /// </summary>
    [Export(typeof(IInfoCodes))]
    public class InfoCodes : IInfoCodes
    {
        // Get all text through the Translation function in the ApplicationLocalizer
        //
        // TextID's for Infocode are reserved at 3590 - 3609
        // In use now are ID's: 3604

        /// <summary>
        /// Maximum length of the Infocode information database field.
        /// </summary>
        private const int MaxInfocodeInformationLengh = 100;

        /// <summary>
        /// IApplication instance.
        /// </summary>
        private IApplication application;

        /// <summary>
        /// Format used to display item ID then description when prompting for a line item infocode.
        /// </summary>
        private const string ItemDisplayFormat = "{0} : {1}";

        /// <summary>
        /// Gets or sets the IApplication instance.
        /// </summary>
        [Import]
        public IApplication Application
        {
            get
            {
                return this.application;
            }
            set
            {
                this.application = value;
                InternalApplication = value;
            }
        }

        /// <summary>
        /// Gets or sets the static IApplication instance.
        /// </summary>
        internal static IApplication InternalApplication { get; private set; }

        /// <summary>
        /// ProcessInfoCode
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="refRelation"></param>
        /// <param name="refRelation2"></param>
        /// <param name="tableRefId"></param>
        /// <param name="infoCodeType"></param>
        /// <returns></returns>
        public bool ProcessInfoCode(IPosTransaction posTransaction, string refRelation, string refRelation2, InfoCodeTableRefType tableRefId, InfoCodeType infoCodeType)
        {
            return ProcessInfoCode(posTransaction, null, refRelation, refRelation2, tableRefId, infoCodeType);
        }

        /// <summary>
        /// ProcessInfoCode for a sale line item
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="saleLineItem"></param>
        /// <param name="refRelation"></param>
        /// <param name="refRelation2"></param>
        /// <param name="tableRefId"></param>
        /// <param name="infoCodeType"></param>
        /// <returns></returns>
        public bool ProcessInfoCode(IPosTransaction posTransaction, ISaleLineItem saleLineItem, string refRelation, string refRelation2, InfoCodeTableRefType tableRefId, InfoCodeType infoCodeType)
        {
            return ProcessInfoCode(posTransaction, saleLineItem, 0m, 0m, refRelation, refRelation2, string.Empty, tableRefId, string.Empty, null, infoCodeType);
        }

        /// <summary>
        /// ProcessInfoCode
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="quantity"></param>
        /// <param name="amount"></param>
        /// <param name="refRelation"></param>
        /// <param name="refRelation2"></param>
        /// <param name="refRelation3"></param>
        /// <param name="tableRefId"></param>
        /// <param name="linkedInfoCodeId"></param>
        /// <param name="orgInfoCode"></param>
        /// <param name="infoCodeType"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public bool ProcessInfoCode(IPosTransaction posTransaction, decimal quantity, decimal amount, string refRelation, string refRelation2, string refRelation3,
                InfoCodeTableRefType tableRefId, string linkedInfoCodeId, IInfoCodeLineItem orgInfoCode, InfoCodeType infoCodeType)
        {
            return ProcessInfoCode(posTransaction, null, quantity, amount, refRelation, refRelation2, refRelation3, tableRefId, linkedInfoCodeId, orgInfoCode, infoCodeType);
        }

        /// <summary>
        /// ProcessInfoCode for a sale line item
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="saleLineItem"></param>
        /// <param name="quantity"></param>
        /// <param name="amount"></param>
        /// <param name="refRelation"></param>
        /// <param name="refRelation2"></param>
        /// <param name="refRelation3"></param>
        /// <param name="tableRefId"></param>
        /// <param name="linkedInfoCodeId"></param>
        /// <param name="orgInfoCode"></param>
        /// <param name="infoCodeType"></param>    
        /// <returns></returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public bool ProcessInfoCode(IPosTransaction posTransaction, ISaleLineItem saleLineItem, decimal quantity, decimal amount, string refRelation, string refRelation2, string refRelation3,
                InfoCodeTableRefType tableRefId, string linkedInfoCodeId, IInfoCodeLineItem orgInfoCode, InfoCodeType infoCodeType)
        {
            return ProcessInfoCode(posTransaction, saleLineItem, quantity, amount, refRelation, refRelation2, refRelation3,tableRefId, linkedInfoCodeId, orgInfoCode, infoCodeType, 0);
        }

        /// <summary>
        /// ProcessInfoCode for a sale line item
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="saleLineItem"></param>
        /// <param name="quantity"></param>
        /// <param name="amount"></param>
        /// <param name="refRelation"></param>
        /// <param name="refRelation2"></param>
        /// <param name="refRelation3"></param>
        /// <param name="tableRefId"></param>
        /// <param name="linkedInfoCodeId"></param>
        /// <param name="orgInfoCode"></param>
        /// <param name="infoCodeType"></param>
        /// <param name="tenderLineId"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        public bool ProcessInfoCode(IPosTransaction posTransaction, ISaleLineItem saleLineItem, decimal quantity, decimal amount, string refRelation, string refRelation2, string refRelation3,
                InfoCodeTableRefType tableRefId, string linkedInfoCodeId, IInfoCodeLineItem orgInfoCode, InfoCodeType infoCodeType, int tenderLineId)
        {
            RetailTransaction retailTransaction = posTransaction as RetailTransaction;
            TenderCountTransaction tenderCountTransaction = posTransaction as TenderCountTransaction;
            CustomerPaymentTransaction customerPaymentTransaction = posTransaction as CustomerPaymentTransaction;
            NoSaleTransaction noSaleTransaction = posTransaction as NoSaleTransaction;
            // Other possible transaction types at this point include CustomerPayment

            if (refRelation == null)
            {
                refRelation = string.Empty;
            }

            if (refRelation2 == null)
            {
                refRelation2 = string.Empty;
            }

            if (refRelation3 == null)
            {
                refRelation3 = string.Empty;
            }

            //Infocode
            IInfoCodeSystem infoCodeSystem = this.Application.BusinessLogic.InfoCodeSystem;
            IInfoCodeLineItem[] infoCodes = new IInfoCodeLineItem[0];

            if (!string.IsNullOrEmpty(linkedInfoCodeId))
            {
                infoCodes = infoCodeSystem.GetInfocodes(linkedInfoCodeId);
            }
            else if (tableRefId == InfoCodeTableRefType.FunctionalityProfile)
            {
                infoCodes = infoCodeSystem.GetInfocodes(refRelation2);
                refRelation2 = string.Empty;
            }
            else if (tableRefId == InfoCodeTableRefType.PreItem)
            {
                // Pre item is just a table ref id of item, but handled during different processing
                infoCodes = infoCodeSystem.GetInfocodes(refRelation, refRelation2, refRelation3, InfoCodeTableRefType.Item);
            }
            else
            {
                infoCodes = infoCodeSystem.GetInfocodes(refRelation, refRelation2, refRelation3, tableRefId);
            }

            foreach (InfoCodeLineItem infoCode in infoCodes)
            {
                if (infoCode.InfocodeId == null) { return false; } //If no infocode is found

                // Process age limit info codes as pre item.  I.e. stop processing on this info code if it is pre item
                // and not of type age limit.
                // Low impact fix that should be reevaluated if any info code other than age limit is ever added
                // pre item.  Using continue because indentation of if/else sequence already too much.
                if (((tableRefId == InfoCodeTableRefType.PreItem) && (infoCode.InputType != InfoCodeInputType.AgeLimit)))
                {
                    continue;
                }

                //If bug in data, fixes division by zero
                if (infoCode.RandomFactor == 0)
                {
                    infoCode.RandomFactor = 100;
                }

                infoCode.OriginType = infoCodeType;
                infoCode.RefRelation = refRelation;
                infoCode.RefRelation2 = refRelation2;
                infoCode.RefRelation3 = refRelation3;
                switch (infoCode.OriginType)
                {
                    case InfoCodeType.Header:
                        infoCode.Amount = amount;
                        break;
                    case InfoCodeType.Sales:
                        infoCode.Amount = (amount * -1);
                        break;
                    case InfoCodeType.Payment:
                        infoCode.Amount = amount;
                        break;
                    case InfoCodeType.IncomeExpense:
                        infoCode.Amount = amount;
                        break;
                }

                int randomFactor = (int)(100 / infoCode.RandomFactor); //infoCode.RandomFactor = 100 means ask 100% for a infocode
                Random random = new Random();
                int randomNumber = random.Next(randomFactor);  //Creates numbers from 0 to randomFactor-1
                //Only get the infocode if randomfactor is set to zero or generated random number is the sama as the randomfactor-1
                if (infoCode.RandomFactor == 100 || randomNumber == (randomFactor - 1))
                {
                    Boolean infoCodeNeeded = true;
                    if (infoCode.OncePerTransaction)
                    {
                        if (tenderCountTransaction != null)
                        {
                            infoCodeNeeded = tenderCountTransaction.InfoCodeNeeded(infoCode.InfocodeId);
                        }
                        else if (customerPaymentTransaction != null)
                        {
                            infoCodeNeeded = customerPaymentTransaction.InfoCodeNeeded(infoCode.InfocodeId);
                        }
                        else
                        {
                            infoCodeNeeded = retailTransaction.InfoCodeNeeded(infoCode.InfocodeId);
                        }
                    }

                    if (infoCodeNeeded)
                    {
                        // If the required type is negative but the quantity is positive, do not continue
                        if (infoCode.InputRequiredType == InfoCodeInputRequiredType.Negative && quantity > 0)
                        {
                            infoCodeNeeded = false;
                        }

                        // If the required type is positive but the quantity is negative, do not continue
                        if (infoCode.InputRequiredType == InfoCodeInputRequiredType.Positive && quantity < 0)
                        {
                            infoCodeNeeded = false;
                        }
                    }
                    // If there is some infocodeID existing, and infocod is needed
                    if (infoCode.InfocodeId != null && infoCodeNeeded == true)
                    {
                        #region Text and General
                        if (infoCode.InputType == InfoCodeInputType.Text || infoCode.InputType == InfoCodeInputType.General)
                        {
                            Boolean inputValid = true;
                            bool abort = false;
                            // Get a infocode text
                            do
                            {
                                inputValid = true;

                                InputConfirmation inputConfirmation = new InputConfirmation()
                                {
                                    Text = (saleLineItem != null) ? string.Format(ItemDisplayFormat, saleLineItem.ItemId, saleLineItem.Description) : null,
                                    MaxLength = MaxInfocodeInformationLengh,
                                    PromptText = infoCode.Prompt,
                                    RemoveDefaultFocus = true,
                                };

                                InteractionRequestedEventArgs request = new InteractionRequestedEventArgs(inputConfirmation, () =>
                                {
                                    if (inputConfirmation.Confirmed)
                                    {
                                        int textId = 0;
                                        if (infoCode.InputRequired && string.IsNullOrEmpty(inputConfirmation.EnteredText))
                                        {
                                            textId = 3593; //Text is required
                                            inputValid = false;
                                        }

                                        if (inputValid && infoCode.MinimumLength > 0 && inputConfirmation.EnteredText.Length < infoCode.MinimumLength)
                                        {
                                            textId = 3591; //The input text is too short.
                                            inputValid = false;
                                        }

                                        if (inputValid && infoCode.MaximumLength > 0 && inputConfirmation.EnteredText.Length > infoCode.MaximumLength)
                                        {
                                            textId = 3592; //The input text exceeds the maximum length.
                                            inputValid = false;
                                        }

                                        if (inputValid && infoCode.AdditionalCheck == 1)
                                        {
                                            inputValid = CheckKennitala(inputConfirmation.EnteredText);
                                            if (!inputValid)
                                            {
                                                textId = 3603;
                                            }
                                        }

                                        if (inputValid)
                                        {
                                            infoCode.Information = inputConfirmation.EnteredText;
                                        }
                                        else
                                        {
                                            POSFormsManager.ShowPOSMessageDialog(textId);
                                        }
                                    }
                                    else // User Hits Cancel
                                    {
                                        if (infoCode.InputRequired)
                                        {
                                            abort = inputValid = false;
                                        }
                                        else
                                        {
                                            abort = inputValid = true;
                                        }
                                    }
                                }
                                );


                                Application.Services.Interaction.InteractionRequest(request);

                                if (abort)
                                {
                                    return false;
                                }

                            } while (!inputValid);
                            //Adding the result to the infocode
                        }

                        #endregion

                        #region Date
                        else if (infoCode.InputType == InfoCodeInputType.Date)
                        {
                            Boolean inputValid = true;
                            do
                            {
                                inputValid = true;

                                string inputText;
                                //Show the input form
                                using (frmInputNumpad inputDialog = new frmInputNumpad(false))
                                {
                                    if (saleLineItem != null)
                                        inputDialog.Text = string.Format(ItemDisplayFormat, saleLineItem.ItemId, saleLineItem.Description);

                                    inputDialog.EntryTypes = NumpadEntryTypes.Date;
                                    inputDialog.PromptText = infoCode.Prompt;
                                    POSFormsManager.ShowPOSForm(inputDialog);
                                    // Quit if cancel is pressed...
                                    if (inputDialog.DialogResult == System.Windows.Forms.DialogResult.Cancel && !infoCode.InputRequired)
                                    {
                                        return false;
                                    }

                                    inputText = inputDialog.InputText;
                                }

                                //Is input valid? 
                                if (!string.IsNullOrEmpty(inputText))
                                {
                                    int textId = 0;
                                    bool isDate = true;
                                    DateTime infoDate = DateTime.Now;

                                    try
                                    {
                                        infoDate = Convert.ToDateTime(inputText, (IFormatProvider)Thread.CurrentThread.CurrentCulture.DateTimeFormat);
                                    }
                                    catch
                                    {
                                        isDate = false;
                                    }

                                    if (!isDate)
                                    {
                                        textId = 3602; //Date entered is not valid
                                        inputValid = false;
                                    }

                                    if (inputText.Length == 0 && infoCode.InputRequired)
                                    {
                                        textId = 3594; //A number input is required
                                        inputValid = false;
                                    }

                                    if (!inputValid)
                                    {
                                        POSFormsManager.ShowPOSMessageDialog(textId);
                                    }
                                    else
                                    {
                                        //Setting the result to the infocode
                                        infoCode.Information = infoDate.ToString(Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern);
                                    }
                                }
                                else if (infoCode.InputRequired)
                                {
                                    inputValid = false;
                                    POSFormsManager.ShowPOSMessageDialog(3597);//A number input is required
                                }
                            } while (!inputValid);

                        }

                        #endregion

                        #region Numeric and Operator/Staff

                        else if (infoCode.InputType == InfoCodeInputType.Numeric || infoCode.InputType == InfoCodeInputType.Operator)
                        {
                            Boolean inputValid = true;

                            do
                            {
                                inputValid = true;

                                string inputText = string.Empty;
                                //Show the input form
                                using (frmInputNumpad inputDialog = new frmInputNumpad(false))
                                {
                                    if (saleLineItem != null)
                                        inputDialog.Text = string.Format(ItemDisplayFormat, saleLineItem.ItemId, saleLineItem.Description);

                                    inputDialog.EntryTypes = NumpadEntryTypes.Price;
                                    inputDialog.PromptText = infoCode.Prompt;
                                    POSFormsManager.ShowPOSForm(inputDialog);
                                    // Quit if cancel is pressed and input not required...
                                    if (inputDialog.DialogResult == System.Windows.Forms.DialogResult.Cancel && !infoCode.InputRequired)
                                    {
                                        return false;
                                    }

                                    // If input required then only valid result is Ok.
                                    if (inputDialog.DialogResult == System.Windows.Forms.DialogResult.OK)
                                    {
                                        inputText = inputDialog.InputText;
                                    }
                                }

                                //Is input empty?
                                if (!string.IsNullOrWhiteSpace(inputText))
                                {
                                    int textId = 0;
                                    if (inputText.Length == 0 && infoCode.InputRequired)
                                    {
                                        textId = 3594; //A number input is required
                                        inputValid = false;
                                    }

                                    if (infoCode.MinimumValue != 0 && Convert.ToDecimal(inputText) < infoCode.MinimumValue)
                                    {
                                        textId = 3595; //The number is lower than the minimum value
                                        inputValid = false;
                                    }

                                    if (infoCode.MaximumValue != 0 && Convert.ToDecimal(inputText) > infoCode.MaximumValue)
                                    {
                                        textId = 3596; //The number exceeds the maximum value
                                        inputValid = false;
                                    }

                                    if (!inputValid)
                                    {
                                        POSFormsManager.ShowPOSMessageDialog(textId);
                                    }
                                }
                                else
                                {
                                    inputValid = false;
                                    POSFormsManager.ShowPOSMessageDialog(3597); //A number input is required 
                                }

                                //Setting the result to the infocode
                                infoCode.Information = inputText;
                            } while (!inputValid);
                        }

                        #endregion

                        #region Customer

                        else if (infoCode.InputType == InfoCodeInputType.Customer)
                        {
                            bool inputValid = true;
                            do
                            {
                                inputValid = true;
                                Contracts.DataEntity.ICustomer customer = this.Application.Services.Customer.Search();
                                if (customer != null)
                                {
                                    infoCode.Information = customer.CustomerId;
                                    inputValid = true;
                                }
                                else
                                {
                                    if (infoCode.InputRequired)
                                    {
                                        inputValid = false;
                                        POSFormsManager.ShowPOSMessageDialog(3598); //A customer needs to be selected
                                    }
                                }
                            } while (!inputValid);
                        }
                        #endregion

                        #region Item

                        else if (infoCode.InputType == InfoCodeInputType.Item)
                        {
                            Boolean inputValid = true;
                            do
                            {
                                string selectedItemID = string.Empty;
                                DialogResult dialogResult = this.Application.Services.Dialog.ItemSearch(500, ref selectedItemID);
                                // Quit if cancel is pressed...
                                if (dialogResult == System.Windows.Forms.DialogResult.Cancel && !infoCode.InputRequired)
                                {
                                    return false;
                                }

                                if (!string.IsNullOrEmpty(selectedItemID))
                                {
                                    infoCode.Information = selectedItemID;
                                    inputValid = true;
                                }
                                else
                                {
                                    if (infoCode.InputRequired)
                                    {
                                        inputValid = false;
                                        POSFormsManager.ShowPOSMessageDialog(3599);//A items needs to be selected
                                    }
                                }
                            } while (!inputValid);
                        }
                        #endregion

                        #region SubCode

                        else if ((infoCode.InputType == InfoCodeInputType.SubCodeList) || (infoCode.InputType == InfoCodeInputType.SubCodeButtons))
                        {
                            FormInfoCodeSubCodeBase infoSubCodeDialog;
                            if (infoCode.InputType == InfoCodeInputType.SubCodeList)
                            {
                                infoSubCodeDialog = new FormInfoCodeSubCodeList();
                            }
                            else
                            {
                                infoSubCodeDialog = new FormInfoSubCode(false);
                            }

                            using (infoSubCodeDialog)
                            {
                                bool inputValid = false;
                                do
                                {
                                    infoSubCodeDialog.InfoCodePrompt = infoCode.Prompt;
                                    infoSubCodeDialog.InfoCodeId = infoCode.InfocodeId;
                                    infoSubCodeDialog.InputRequired = infoCode.InputRequired;
                                    infoSubCodeDialog.Item = saleLineItem;

                                    POSFormsManager.ShowPOSForm(infoSubCodeDialog);
                                    switch (infoSubCodeDialog.DialogResult)
                                    {
                                        case DialogResult.OK:
                                            if (!string.IsNullOrWhiteSpace(infoSubCodeDialog.SelectedSubcodeId))
                                            {
                                                inputValid = true;
                                            }
                                            else
                                            {
                                                POSFormsManager.ShowPOSMessageDialog(1113); //The value entered is not valid
                                            }
                                            break;

                                        case DialogResult.No:
                                            return false;

                                        default:
                                            if (!infoCode.InputRequired)
                                            {
                                                return false;
                                            }
                                            break;
                                    }

                                    //if InputValid is false then nothing was selected in the dialog and there is no point in going through this code
                                    if (inputValid)
                                    {
                                        infoCode.Information = infoSubCodeDialog.SelectedDescription;
                                        infoCode.Subcode = infoSubCodeDialog.SelectedSubcodeId;

                                        //TenderDeclarationTransaction also has infocodes but it can't have any sale items so no need
                                        //to go through this code.
                                        if (retailTransaction != null)
                                        {
                                            //Look through the information on the subcode that was selected
                                            //foreach (SubcodeInfo subcodeInfo in infoSubCodeDialog.SubCodes)
                                            //{
                                            switch (infoSubCodeDialog.SelectedTriggerFunction)
                                            {
                                                //See if an item is to be sold and if a discount and/or price needs to be modified
                                                case (int)TriggerFunctionEnum.Item:
                                                    if (infoSubCodeDialog.SelectedTriggerCode.Length != 0)
                                                    {
                                                        OperationInfo opInfo = new OperationInfo();

                                                        ItemSale itemSale = new ItemSale();
                                                        itemSale.OperationID = PosisOperations.ItemSale;
                                                        itemSale.OperationInfo = opInfo;
                                                        itemSale.Barcode = infoSubCodeDialog.SelectedTriggerCode;
                                                        itemSale.POSTransaction = retailTransaction;
                                                        itemSale.IsInfocodeItem = true;
                                                        itemSale.RunOperation();

                                                        //The infocode item has been added. 
                                                        //Look at the last item on the transaction and if it is the same item that the ItemSale was supposed to sell
                                                        //then check to see if it is to have a special price or discount
                                                        if (retailTransaction.SaleItems.Last.Value.ItemId == infoSubCodeDialog.SelectedTriggerCode)
                                                        {
                                                            //set the property of that item to being an infocode item such that if the same item is added again, then 
                                                            //these will not be aggregated (since the item as infocode item might receive a different discount than the same
                                                            //item added regularly.
                                                            retailTransaction.SaleItems.Last.Value.IsInfoCodeItem = true;

                                                            infoCode.SubcodeSaleLineId = retailTransaction.SaleItems.Last.Value.LineId;

                                                            if (infoSubCodeDialog.SelectedPriceType == (int)PriceTypeEnum.Price)
                                                            {
                                                                PriceOverride priceOverride = new PriceOverride();

                                                                opInfo.NumpadValue = infoSubCodeDialog.SelectedAmountPercent.ToString("n2");
                                                                opInfo.ItemLineId = retailTransaction.SaleItems.Last.Value.LineId;

                                                                priceOverride.OperationInfo = opInfo;
                                                                priceOverride.OperationID = PosisOperations.PriceOverride;
                                                                priceOverride.POSTransaction = retailTransaction;
                                                                priceOverride.LineId = retailTransaction.SaleItems.Last.Value.LineId;
                                                                priceOverride.RunOperation();
                                                            }
                                                            else if (infoSubCodeDialog.SelectedPriceType == (int)PriceTypeEnum.Percent)
                                                            {
                                                                LineDiscountPercent lineDiscount = new LineDiscountPercent();

                                                                opInfo.NumpadValue = infoSubCodeDialog.SelectedAmountPercent.ToString("n2");
                                                                opInfo.ItemLineId = retailTransaction.SaleItems.Last.Value.LineId;

                                                                lineDiscount.OperationInfo = opInfo;
                                                                lineDiscount.OperationID = PosisOperations.LineDiscountPercent;
                                                                lineDiscount.POSTransaction = retailTransaction;
                                                                lineDiscount.RunOperation();
                                                            }
                                                        }
                                                    }
                                                    break;

                                                // Update warehouse in the transaction for return purposes
                                                case (int)TriggerFunctionEnum.InventLocationReturns:
                                                    
                                                    if (!string.IsNullOrWhiteSpace(infoSubCodeDialog.SelectedTriggerCode))
                                                    {
                                                        retailTransaction.InventLocationIdReturns = infoSubCodeDialog.SelectedTriggerCode;
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                } while (!inputValid);
                            }
                        }
                        #endregion

                        #region Age limit

                        else if ((infoCode.InputType == InfoCodeInputType.AgeLimit))
                        {
                            int ageLimit = (int)infoCode.MinimumValue;
                            if (ageLimit >= 0)
                            {
                                //Calculate birthdate corresponding to minimum age limit
                                DateTime dtBirthDate = DateTime.Today.AddYears(-ageLimit);

                                //Convert the date time value according to the current culture information
                                string birthDate = dtBirthDate.ToString(System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern);
                                string msg = ApplicationLocalizer.Language.Translate(3606, ageLimit, birthDate);

                                // Process age limit info codes if it is pre item and prompt the message to user for processing.
                                // Also Info Code type is an item and Age Limit is a link to reason code then process the info code with prompt to user.
                                if (((tableRefId == InfoCodeTableRefType.PreItem) && (infoCode.InputType == InfoCodeInputType.AgeLimit)) || (!string.IsNullOrEmpty(linkedInfoCodeId) && tableRefId == InfoCodeTableRefType.Item))
                                {
                                    using (frmMessage frmMsg = new frmMessage(msg, MessageBoxButtons.YesNo, MessageBoxIcon.Information, false))
                                    {
                                        POSFormsManager.ShowPOSForm(frmMsg);
                                        if (frmMsg.DialogResult != DialogResult.Yes)
                                        {
                                            return false;
                                        }
                                    }
                                }
                                infoCode.Information = msg;
                            }
                        }
                        #endregion

                        else
                        {
                            POSFormsManager.ShowPOSMessageDialog(3589);
                            return false;
                        }

                        // Add the infocode to the transaction
                        if (infoCode.Information != null && infoCode.Information.Length > 0)
                        {
                            string infoComment = string.Empty;
                            if (infoCode.PrintPromptOnReceipt || infoCode.PrintInputOnReceipt || infoCode.PrintInputNameOnReceipt)
                            {
                                if (infoCode.PrintPromptOnReceipt && (infoCode.Prompt.Length != 0))
                                {
                                    infoComment = infoCode.Prompt;
                                }

                                if (infoCode.PrintInputOnReceipt && (infoCode.Subcode != null) && (infoCode.Subcode.Length != 0))
                                {
                                    if (infoComment.Length != 0)
                                    {
                                        infoComment += " - " + infoCode.Subcode;
                                    }
                                    else
                                    {
                                        infoComment = infoCode.Subcode;
                                    }
                                }

                                if (infoCode.PrintInputNameOnReceipt && (infoCode.Information.Length != 0))
                                {
                                    if (infoComment.Length != 0)
                                    {
                                        infoComment += " - " + infoCode.Information;
                                    }
                                    else
                                    {
                                        infoComment = infoCode.Information;
                                    }
                                }
                            }

                            if (infoCodeType == InfoCodeType.Sales)
                            {
                                int lineId = 0;
                                if (infoCode.SubcodeSaleLineId != -1 && retailTransaction != null)
                                {
                                    SaleLineItem lineItem = retailTransaction.GetItem(infoCode.SubcodeSaleLineId - 1);
                                    lineItem.Add(infoCode);
                                    lineId = lineItem.LineId;
                                }
                                else
                                {
                                    // don't save if this is pre-item. the same infocodes will be found next pass for normal item infocodes.
                                    if (tableRefId == InfoCodeTableRefType.PreItem)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        SaleLineItem lineItem = saleLineItem as SaleLineItem;
                                        if (lineItem != null)
                                        {
                                            lineItem.Add(infoCode);
                                            lineId = lineItem.LineId;
                                        }
                                        else if (retailTransaction != null)
                                        {
                                            LinkedListNode<SaleLineItem> lastLineItem = retailTransaction.SaleItems.Last;
                                            lastLineItem.Value.Add(infoCode);
                                            lineId = lastLineItem.Value.LineId;
                                        }
                                    }
                                }

                                if (infoComment.Trim().Length != 0)
                                {
                                    OperationInfo opInfo = new OperationInfo();
                                    opInfo.NumpadValue = infoComment.Trim();
                                    opInfo.ItemLineId = lineId;

                                    ItemComment itemComment = new ItemComment();
                                    itemComment.OperationID = PosisOperations.ItemComment;
                                    itemComment.POSTransaction = (PosTransaction)posTransaction;
                                    itemComment.OperationInfo = opInfo;
                                    itemComment.RunOperation();
                                }
                            }
                            else if (infoCodeType == InfoCodeType.Payment)
                            {
                                LinkedListNode<TenderLineItem> tenderLineItem = null;
                                if (retailTransaction != null)
                                {
                                    if (tenderLineId > 0)
                                    {
                                        tenderLineItem = new LinkedListNode<TenderLineItem>(retailTransaction.GetTenderItem(tenderLineId));
                                    }
                                    else
                                    {
                                        tenderLineItem = retailTransaction.TenderLines.Last;
                                    }
                                }
                                else if (customerPaymentTransaction != null)
                                {
                                    if (tenderLineId > 0)
                                    {
                                        tenderLineItem = new LinkedListNode<TenderLineItem>(customerPaymentTransaction.GetTenderItem(tenderLineId));
                                    }
                                    else
                                    {
                                        tenderLineItem = customerPaymentTransaction.TenderLines.Last;
                                    }
                                }

                                if (tenderLineItem != null)
                                {
                                    tenderLineItem.Value.Add(infoCode);

                                    if (infoComment.Trim().Length != 0)
                                    {
                                        tenderLineItem.Value.Comment += infoComment.Trim();
                                    }
                                }
                            }
                            else
                            {
                                if (retailTransaction != null)
                                {
                                    retailTransaction.Add(infoCode);
                                    if (infoComment.Trim().Length != 0)
                                    {
                                        retailTransaction.InvoiceComment += infoComment.Trim();
                                    }
                                }
                                else if (customerPaymentTransaction != null)
                                {
                                    customerPaymentTransaction.Add(infoCode);
                                    //No invoice comment on CustomerPaymentTransaction
                                }
                                else if (infoCodeType == InfoCodeType.NoSale)
                                {
                                    noSaleTransaction.Add(infoCode);
                                }
                                else
                                {
                                    tenderCountTransaction.Add(infoCode);
                                    //No invoice comment on TenderDeclarationTransaction
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Process Linked InfoCodes for SaleLineItem
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="saleLineItem"></param>
        /// <param name="tableRefId"></param>
        /// <param name="infoCodeType"></param>
        public void ProcessLinkedInfoCodes(IPosTransaction posTransaction, ISaleLineItem saleLineItem, InfoCodeTableRefType tableRefId, InfoCodeType infoCodeType)
        {
            SaleLineItem lineItem = saleLineItem as SaleLineItem;
            if (lineItem == null)
            {
                NetTracer.Warning("lineItem parameter is null");
                throw new ArgumentNullException("saleLineItem");
            }

            //Loop while a linkedInfoCode is found
            int i = 0; //Set as a stop for a infinite loop
            LinkedListNode<InfoCodeLineItem> infoCodeItem = lineItem.InfoCodeLines.Last;
            if (infoCodeItem != null)
            {
                while (!string.IsNullOrEmpty(infoCodeItem.Value.LinkedInfoCodeId) && (i < 10))
                {
                    ProcessInfoCode(posTransaction, lineItem.Quantity, lineItem.GrossAmount, lineItem.ItemId, string.Empty, string.Empty, tableRefId, infoCodeItem.Value.LinkedInfoCodeId, infoCodeItem.Value, infoCodeType);
                    // This is to prevent an infinite loop when infocodes link to themselves..
                    if (infoCodeItem.Value.LinkedInfoCodeId == lineItem.InfoCodeLines.Last.Value.LinkedInfoCodeId)
                    {
                        break;
                    }

                    infoCodeItem = lineItem.InfoCodeLines.Last;
                    i++;
                }
            }
        }

        /// <summary>
        /// Process Linked InfoCodes for TenderLineItem
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="tenderLineItem"></param>
        /// <param name="storeId"></param>
        /// <param name="tableRefId"></param>
        /// <param name="infoCodeType"></param>
        public void ProcessLinkedInfoCodes(IPosTransaction posTransaction, ITenderLineItem tenderLineItem, string storeId, InfoCodeTableRefType tableRefId, InfoCodeType infoCodeType)
        {
            TenderLineItem lineItem = tenderLineItem as TenderLineItem;
            if (lineItem == null)
            {
                NetTracer.Warning("tenderLineItem parameter is null");
                throw new ArgumentNullException("tenderLineItem");
            }

            //Loop while a linkedInfoCode is found
            int i = 0; //Set as a stop for a infinite loop
            LinkedListNode<InfoCodeLineItem> infoCodeItem = lineItem.InfoCodeLines.Last;
            if (infoCodeItem != null)
            {
                while (!string.IsNullOrEmpty(infoCodeItem.Value.LinkedInfoCodeId) && (i < 10))
                {
                    ProcessInfoCode(posTransaction, 0, lineItem.Amount, storeId, lineItem.TenderTypeId, lineItem.CurrencyCode, tableRefId, infoCodeItem.Value.LinkedInfoCodeId, infoCodeItem.Value, infoCodeType);
                    // This is to prevent an infinite loop when infocodes link to themselves..
                    if (infoCodeItem.Value.LinkedInfoCodeId == lineItem.InfoCodeLines.Last.Value.LinkedInfoCodeId)
                    {
                        break;
                    }

                    infoCodeItem = lineItem.InfoCodeLines.Last;
                    i++;
                }
            }
        }

        /// <summary>
        /// Process Linked InfoCodes for InfoCodeLineItem
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="tableRefId"></param>
        /// <param name="infoCodeType"></param>
        public void ProcessLinkedInfoCodes(IPosTransaction posTransaction, InfoCodeTableRefType tableRefId, InfoCodeType infoCodeType)
        {
            CustomerPaymentTransaction customerTransaction = posTransaction as CustomerPaymentTransaction;
            TenderCountTransaction tenderTransaction = posTransaction as TenderCountTransaction;

            if (customerTransaction != null)
            {
                //Loop while a linkedInfoCode is found
                int i = 0; //Set as a stop for a infinite loop
                LinkedListNode<InfoCodeLineItem> infoCodeItem = customerTransaction.InfoCodeLines.Last;
                if (infoCodeItem != null)
                {
                    while (!string.IsNullOrEmpty(infoCodeItem.Value.LinkedInfoCodeId) && (i < 10))
                    {
                        ProcessInfoCode(posTransaction, 0, 0, customerTransaction.Customer.CustomerId, string.Empty, string.Empty, InfoCodeTableRefType.Customer, infoCodeItem.Value.LinkedInfoCodeId, (IInfoCodeLineItem)infoCodeItem.Value, InfoCodeType.Header);
                        // This is to prevent an infinite loop when infocodes link to themselves..
                        if (infoCodeItem.Value.LinkedInfoCodeId == customerTransaction.InfoCodeLines.Last.Value.LinkedInfoCodeId)
                        {
                            break;
                        }

                        infoCodeItem = customerTransaction.InfoCodeLines.Last;
                        i++;
                    }
                }
            }
            else if (tenderTransaction != null)
            {
                //Loop while a linkedInfoCode is found
                int i = 0; //Set as a stop for a infinite loop
                LinkedListNode<InfoCodeLineItem> infoCodeItem = tenderTransaction.InfoCodeLines.Last;
                if (infoCodeItem != null)
                {
                    while (!string.IsNullOrEmpty(infoCodeItem.Value.LinkedInfoCodeId) && (i < 10))
                    {
                        infoCodeItem = tenderTransaction.InfoCodeLines.Last;
                        i++;
                    }
                }
            }
            else
            {
                RetailTransaction asRetailTransaction = (RetailTransaction)posTransaction;

                //Loop while a linkedInfoCode is found
                int i = 0; //Set as a stop for a infinite loop
                LinkedListNode<InfoCodeLineItem> infoCodeItem = asRetailTransaction.InfoCodeLines.Last;
                if (infoCodeItem != null)
                {
                    while (!string.IsNullOrEmpty(infoCodeItem.Value.LinkedInfoCodeId) && (i < 10))
                    {
                        ProcessInfoCode(posTransaction, 0, 0, asRetailTransaction.Customer.CustomerId, string.Empty, string.Empty, tableRefId, infoCodeItem.Value.LinkedInfoCodeId, (IInfoCodeLineItem)infoCodeItem.Value, infoCodeType);
                        // This is to prevent an infinite loop when infocodes link to themselves..
                        if (infoCodeItem.Value.LinkedInfoCodeId == asRetailTransaction.InfoCodeLines.Last.Value.LinkedInfoCodeId)
                        {
                            break;
                        }

                        infoCodeItem = asRetailTransaction.InfoCodeLines.Last;
                        i++;
                    }
                }
            }
        }

        /// <summary>
        /// Checks to see if "Kennitala" is a valid number. Only used in Iceland.
        /// </summary>
        /// <param name="kt"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        private static bool CheckKennitala(string kt)
        {
            try
            {
                string[] ktParts = kt.Split(Convert.ToChar("-"));
                string newKt = string.Empty;

                //Ef að það er bandstrik í kennitölunni þá taka það úr strengnum
                foreach (string part in ktParts)
                {
                    newKt += part;
                }

                try
                {
                    Convert.ToInt64(newKt);
                }
                catch
                {
                    return false;
                }

                int i1; int i2; int i3;
                int i4; int i5; int i6;
                int i8; int i9; int i10;
                int iTot; int iRest;

                i1 = Convert.ToInt16(newKt.Substring(0, 1));
                i2 = Convert.ToInt16(newKt.Substring(1, 1));
                i3 = Convert.ToInt16(newKt.Substring(2, 1));
                i4 = Convert.ToInt16(newKt.Substring(3, 1));
                i5 = Convert.ToInt16(newKt.Substring(4, 1));
                i6 = Convert.ToInt16(newKt.Substring(5, 1));
                //Upprunalegi kódinn gerði ráð fyrir að í sæti 7 væri bandstrik
                i8 = Convert.ToInt16(newKt.Substring(6, 1));
                i9 = Convert.ToInt16(newKt.Substring(7, 1));
                i10 = Convert.ToInt16(newKt.Substring(8, 1));

                iTot = (3 * i1) + (2 * i2) + (7 * i3) + (6 * i4) + (5 * i5) + (4 * i6) + (3 * i8) + (2 * i9);

                iRest = iTot % 11; //Modular deiling
                iRest = 11 - iRest;
                if (iRest == 11)   //ef það er ekki stafur í 10. sæti
                    iRest = 0;
                if (iRest != i10)
                    return false;

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
