/*
	Microsoft Dynamics AX for Retail POS Upgrade Database Script
	DbVersion: 6.2.12.0
*/

/****** adding new columns to [LOGISTICSADDRESSDISTRICT] for Russia ******/
ALTER TABLE [dbo].[LOGISTICSADDRESSDISTRICT]
ADD 
	[COUNTRYREGIONID_RU] [nvarchar](10) NOT NULL DEFAULT (''),
	[STATEID_RU] [nvarchar](10) NOT NULL DEFAULT (''),
	[COUNTYID_RU] [nvarchar](10) NOT NULL DEFAULT ('')
GO
