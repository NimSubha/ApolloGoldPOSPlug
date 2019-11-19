/*
	Microsoft Dynamics AX for Retail POS Upgrade Database Script
	DbVersion: 6.2.22.0
*/

/*
 * Update RETAILBARCODEMASKTABLE table
 */
ALTER TABLE [dbo].RETAILBARCODEMASKTABLE ALTER COLUMN [MASK] nvarchar(80) NULL
GO
