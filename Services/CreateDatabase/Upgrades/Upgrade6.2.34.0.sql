/*
	Microsoft Dynamics AX for Retail POS Upgrade Database Script
	DbVersion: 6.2.34.0
*/

ALTER TABLE [dbo].[RETAILFUNCTIONALITYPROFILE]
ADD
	[CUSTOMERSEARCHMODE]    [int] NOT NULL DEFAULT (0),
	[CUSTOMERSEARCHDEFAULT] [int] NOT NULL DEFAULT (0)
GO

CREATE PROCEDURE [dbo].[GENERICUPSERT]
	-- Input vars
	@tableXml XML
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Local vars
	DECLARE @i_TransactionIsOurs  INT;
	DECLARE @i_Error              INT;
	DECLARE @i_ReturnCode         INT;
	DECLARE @curTableXml          XML;
	DECLARE @tableName            NVARCHAR(128);
	DECLARE @tableSchema          NVARCHAR(128);
	DECLARE @columnCount          INT;
	DECLARE @columnName           NVARCHAR(128);
	DECLARE @columnType           NVARCHAR(128);
	DECLARE @columnValue          NVARCHAR(MAX);
	DECLARE @parsedData           NVARCHAR(MAX);
	DECLARE @updateStatement      NVARCHAR(MAX);
	DECLARE @insertStatement1     NVARCHAR(MAX);
	DECLARE @insertStatement2     NVARCHAR(MAX);
	DECLARE @upsertSql            NVARCHAR(MAX);

	-- initializes the return code and assume the transaction is not ours by default
	SET @i_ReturnCode           = 0;
	SET @i_TransactionIsOurs    = 0;


	IF @@TRANCOUNT = 0
	BEGIN
	    BEGIN TRANSACTION;

	    SELECT @i_Error = @@ERROR;
	    IF @i_Error <> 0
	    BEGIN
	        SET @i_ReturnCode = @i_Error;
	        GOTO error_label;
	    END;

	    SET @i_TransactionIsOurs = 1;
	END;


	-- Iterate the tables
	DECLARE tableCursor CURSOR FOR
	SELECT xmlData.tableData.query('.') FROM @tableXml.nodes('GenericDatabaseTables/Tables/*') xmlData(tableData);
	OPEN tableCursor;
	FETCH NEXT FROM tableCursor INTO @curTableXml;
	WHILE (@@FETCH_STATUS = 0)
	BEGIN
	    SET @parsedData = '';
	    SET @updateStatement = '';
	    SET @insertStatement1 = '';
	    SET @insertStatement2 = '';

	    -- Get the current table's name and schema
	    SELECT 
	        @tableName = xmlData.columnData.value('@Name', 'NVARCHAR(MAX)'),
	        @tableSchema = xmlData.columnData.value('@Schema', 'NVARCHAR(MAX)')
	    FROM @curTableXml.nodes('GenericDatabaseTable') xmlData(columnData)
	    -- Print the table we are upserting
	    PRINT 'Parsing data to upsert into ' + @tableSchema + '.' + @tableName + '.';

	    -- Create a cursor that gets the intersection of the provided data
	    -- and the local schema
	    -- Most of this query's complexity is from getting the column data types
	    DECLARE columnCursor CURSOR FOR
	    SELECT cols.COLUMN_NAME, 
	    cols.DATA_TYPE + 
	    COALESCE(
	    '(' + 
	    -- String data types
	        CASE
	        WHEN cols.CHARACTER_MAXIMUM_LENGTH = -1 THEN 
	            'MAX'
	        WHEN cols.DATA_TYPE <> 'XML' THEN
	            Cast(cols.CHARACTER_MAXIMUM_LENGTH AS NVARCHAR(128))
	        ELSE
	            NULL
	        END 
	    + ')',
	    '(' + 
	    -- Numeric data types
	        CASE
	        WHEN cols.DATA_TYPE <> 'BIGINT' AND cols.DATA_TYPE <> 'INT'
	        THEN
	            Cast(cols.NUMERIC_PRECISION AS NVARCHAR(128))
	            + ',' + 
	            Cast(cols.NUMERIC_SCALE AS NVARCHAR(128)) 
	        ELSE
	            NULL
	        END
	    + ')',
	    -- All other data types dont need a precision or capacity specified
	    '') 
	    as dataType, parsedData.columnValue
	    FROM INFORMATION_SCHEMA.Columns cols
	    INNER JOIN (
	        SELECT 
	            xmlData.columnData.value('@Name', 'NVARCHAR(MAX)') columnName,
	            xmlData.columnData.value('@Value', 'NVARCHAR(MAX)') columnValue
	        FROM @curTableXml.nodes('GenericDatabaseTable/Columns/*') xmlData(columnData)
	        ) AS parsedData
	    ON parsedData.columnName = cols.COLUMN_NAME
	    WHERE cols.TABLE_NAME = @tableName AND cols.TABLE_SCHEMA = @tableSchema;
	    
	    -- Iterate the table's columns to assemble the necessary fields
	    SET @columnCount = 0;
	    OPEN columnCursor;
	    FETCH NEXT FROM columnCursor INTO @columnName, @columnType, @columnValue;
	    WHILE (@@FETCH_STATUS = 0)
	    BEGIN
	        IF(@columnCount <> 0)
	        BEGIN
	            SET @parsedData = ISNULL(@parsedData, '') + ', ';
	            SET @updateStatement = ISNULL(@updateStatement, '') + ', ';
	            SET @insertStatement1 = ISNULL(@insertStatement1, '') + ', ';
	            SET @insertStatement2 = ISNULL(@insertStatement2, '') + ', ';
	        END;
	        SET @parsedData = ISNULL(@parsedData, '') + 'CAST(N''' + ISNULL(@columnValue, '') + ''' AS ' + ISNULL(@columnType, '') + ') AS ' + ISNULL(@columnName, '');
	        SET @updateStatement = ISNULL(@updateStatement, '') + 'dest.' + ISNULL(@columnName, '') + ' = src.' + ISNULL(@columnName, '');
	        SET @insertStatement1 = ISNULL(@insertStatement1, '') + ISNULL(@columnName, '');
	        SET @insertStatement2 = ISNULL(@insertStatement2, '') + 'src.' + ISNULL(@columnName, '');

	        SET @columnCount = @columnCount + 1;
	        FETCH NEXT FROM columnCursor INTO @columnName, @columnType, @columnValue;
	    END;
	    CLOSE columnCursor;
	    DEALLOCATE columnCursor;

	    -- Assemble a merge statement (this performs the upsert)
	    SET @upsertSql = 'MERGE ' + @tableSchema + '.' + @tableName + ' AS dest
	    USING (
	        SELECT ' +
	            @parsedData +
	        ') AS src
	    ON dest.RECID = src.RECID
	    WHEN MATCHED THEN
	    UPDATE SET ' + @updateStatement +
	    ' WHEN NOT MATCHED THEN INSERT (' +
	        @insertStatement1 +
	    ') VALUES (' +
	        @insertStatement2 +
	    ');';

	    -- Ensure there exists data to be inserted
	    IF(@parsedData <> '')
	    BEGIN
	        PRINT 'Upserting into ' + @tableSchema + '.' + @tableName + '.';
	        EXEC (@upsertSql);
	    END
	    ELSE
	    BEGIN
	        PRINT 'No data matching the local schema was found for ' + @tableSchema + '.' + @tableName + '.';
	    END;

	    SELECT @i_Error = @@ERROR;
	    IF @i_Error <> 0
	    BEGIN
	        SET @i_ReturnCode = @i_Error;
			-- CRT ignores messages printed before an error occurs, so this message is added for dubugging in CRT
			PRINT 'The table being upserted when this error occured: ' + @tableSchema + '.' + @tableName + '.';
	        GOTO error_label;
	    END;

	    FETCH NEXT FROM tableCursor INTO @curTableXml;
	END;


	IF @i_TransactionIsOurs = 1
	BEGIN
	    COMMIT TRANSACTION;

	    SET @i_Error = @@ERROR;
	    IF @i_Error <> 0
	    BEGIN
	        SET @i_ReturnCode = @i_Error;
	        GOTO error_label;
	    END;

	    SET @i_TransactionIsOurs = 0;
	    PRINT 'Transaction committed.';
	END;
	PRINT 'Upsert completed successfully.';

	
exit_label:
    IF (SELECT CURSOR_STATUS('global','columnCursor')) >= -1
    BEGIN
        CLOSE columnCursor;
        DEALLOCATE columnCursor;
    END
    CLOSE tableCursor;
    DEALLOCATE tableCursor;
    RETURN @i_ReturnCode;

error_label:
    IF @i_TransactionIsOurs = 1
    BEGIN
        ROLLBACK TRANSACTION;
        PRINT 'Transaction rolled back.';
    END;
    PRINT 'Upsert failed.';
    GOTO exit_label;
END
GO