/*
	Microsoft Dynamics AX for Retail POS Upgrade Database Script
	DbVersion: 6.2.8.0
*/

/*
 * Update RetailParameters with gift card product field
 */
ALTER TABLE [dbo].[RETAILPARAMETERS]
ADD
	[GIFTCARDITEM] [nvarchar](20) NOT NULL DEFAULT((''))
GO
