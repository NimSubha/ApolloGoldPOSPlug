/*
	Microsoft Dynamics AX for Retail POS Upgrade Database Script
	DbVersion: 6.2.32.0
*/


ALTER TABLE [dbo].[RETAILSTAFFTABLE]
ADD
	[BLOCKED] [int] NOT NULL DEFAULT (0)
GO

/****** Recreating View [dbo].[RETAILSTAFFVIEW] ******/
/** Note: This has to be done after droping/adding columns from/to RETAILSTAFFTABLE **/

/****** creating View [dbo].[RETAILSTAFFVIEW] ******/
IF EXISTS(SELECT * FROM sys.views join sys.schemas on sys.views.schema_id = sys.schemas.schema_id WHERE sys.views.name ='RETAILSTAFFVIEW' AND sys.schemas.name = 'dbo')
    DROP VIEW [dbo].RETAILSTAFFVIEW
GO
CREATE VIEW [dbo].RETAILSTAFFVIEW
AS
  SELECT
	T1.STAFFID, T1.BLOCKED, T1.CHANGEPASSWORD, T1.CONTINUEONTSERRORS, T1.EMPLOYMENTTYPE, T1.LAYOUTID, T1.NAMEONRECEIPT, T1.PASSWORD, T1.PASSWORDDATA, T1.RECID, 
    T2.RECID AS RETAILHCMWORKER, T1.VISUALPROFILE, T1.CULTURENAME
  FROM dbo.RETAILSTAFFTABLE AS T1 INNER JOIN dbo.HCMWORKER AS T2 ON T1.STAFFID = T2.PERSONNELNUMBER
GO

