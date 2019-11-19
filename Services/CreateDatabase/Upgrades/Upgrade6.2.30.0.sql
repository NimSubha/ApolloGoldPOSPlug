/****** Recreating View [dbo].[ASSORTEDINVENTITEMS] ******/
/** Note: This has to be done after droping/adding columns from/to INVENTTABLE **/

/****** creating View [dbo].[DELIVERYMODESEXPLODEDVIEW] ******/
IF EXISTS(SELECT * FROM sys.views join sys.schemas on sys.views.schema_id = sys.schemas.schema_id WHERE sys.views.name ='DELIVERYMODESEXPLODEDVIEW' AND sys.schemas.name = 'dbo')
    DROP VIEW [dbo].DELIVERYMODESEXPLODEDVIEW
GO
CREATE VIEW [dbo].DELIVERYMODESEXPLODEDVIEW
AS
    SELECT
        DM.RecId,
        RCT.[RECID] as CHANNELID,        
        DM.Code,
        DM.Txt,
        DM.MarkupGroup,
        DMProduct.ItemId,
        DMProduct.InventDim,
        DMAddress.CountryRegion,
        DMAddress.State
    FROM DLVMODE DM
    INNER JOIN RETAILDLVMODECHANNELEXPLODED DMChannel on DM.RecId = DMChannel.DlvMode
    INNER JOIN RETAILCHANNELTABLE RCT ON RCT.OMOPERATINGUNITID = DMChannel.OMOPERATINGUNIT
    LEFT OUTER JOIN RETAILDLVMODEPRODUCTEXPLODED DMProduct on DM.RecId = DMProduct.DlvMode
    LEFT OUTER JOIN RETAILDLVMODEADDRESSEXPLODED DMAddress on DM.RecId = DMAddress.DlvMode
GO
