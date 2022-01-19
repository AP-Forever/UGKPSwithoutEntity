Use ugkps

ALTER TABLE Stocks_Investment
	ADD Day_Price_High float NULL, Day_Price_Low float NULL, Sector nvarchar(100) NULL
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Amit Patel
-- ALTER date: Feb 14, 2020
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_InsertStock] 
	@UserID int, @UserName nvarchar(100), @Ticker nvarchar(100), @Exchange nvarchar(100), @CompanyName nvarchar(100), @LastUpdated datetime,
	@Price float, @Price_High float, @Price_Low float, @P_E_Ratio float, @EPS float, @DividendPerShare float, 
	@DividendPercent float, @DividendYieldPercent float, @Comment text, @Day_Price_High float, @Day_Price_Low float, @Sector nvarchar(100)
	 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	

    -- Insert statements for procedure here
	INSERT INTO dbo.Stocks_Investment(UserID, UserName, Ticker, Exchange, CompanyName, LastUpdated, Price, Price_High, Price_Low, P_E_Ratio, EPS, DividendPerShare, DividendPercent, DividendYieldPercent, Comment, CreatedDate, ModifiedDate, Day_Price_High, Day_Price_Low, Sector) 
	VALUES (@UserID, @UserName, @Ticker, @Exchange, @CompanyName, @LastUpdated, @Price, @Price_High, @Price_Low, @P_E_Ratio, @EPS, @DividendPerShare, @DividendPercent, @DividendYieldPercent, @Comment, GETDATE(), GETDATE(), @Day_Price_High, @Day_Price_Low, @Sector)
END



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Batch submitted through debugger: SQLQuery9.sql|7|0|C:\Users\ampa\AppData\Local\Temp\~vsCD8C.sql
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_UpdateStockDetails]
	@SI_ID int, @LastUpdated datetime, @Price float, @Price_High float, @Price_Low float, @P_E_Ratio float, @EPS float, @DividendPerShare float, 
	@DividendPercent float, @DividendYieldPercent float, @Comment text, @Day_Price_High float, @Day_Price_Low float, @Sector nvarchar(100)
AS
BEGIN
	UPDATE [dbo].Stocks_Investment SET LastUpdated = @LastUpdated, Price = @Price, Price_High = @Price_High, Price_Low = @Price_Low, P_E_Ratio = @P_E_Ratio, EPS =  @EPS, DividendPerShare = @DividendPerShare,
	DividendPercent =  @DividendPercent, DividendYieldPercent = @DividendYieldPercent, Comment = @Comment, ModifiedDate = GETDATE(), Day_Price_High = @Day_Price_High, Day_Price_Low = @Day_Price_Low,
	Sector = @Sector
	where SI_ID= @SI_ID
END