/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/


IF NOT EXISTS(SELECT 1 FROM [STOCK_SETTING] WHERE STOCK_SETTING_UNIQUE_ID = 'SPEV')
BEGIN
INSERT INTO [dbo].[STOCK_SETTING]([STOCK_SETTING_ID],[STOCK_SETTING_NAME],[STOCK_SETTING_VALUE],[STOCK_SETTING_UNIQUE_ID])
     VALUES (1, 'Stock Previous Record Count', 200, 'SPEV')
END
GO