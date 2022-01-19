Use ugkps
GO

-- Add CreatedDate, ModifiedDate columns in Users
ALTER TABLE Users
	ADD CreatedDate datetime NOT NULL DEFAULT(GETDATE()), ModifiedDate datetime NOT NULL DEFAULT(GETDATE()), Age int NOT NULL DEFAULT(0)
GO

-- Update Age values
Update Users Set Age = DATEDIFF(MONTH, DateOfBirth, GETDATE())/ 12

--Alter DateOfBirth column to be nullable
Alter Table Users
Alter Column DateOfBirth date NULL

-- Remove All data from DateOfBirth Column
--Update Users Set DateOfBirth = NULL


----------------------------------------------------------------------------------------------------------------------------------------------------------------

-- Add CreatedDate, ModifiedDate columns in FamilyMembers
ALTER TABLE FamilyMembers
	ADD CreatedDate datetime NOT NULL DEFAULT(GETDATE()), ModifiedDate datetime NOT NULL DEFAULT(GETDATE()), Age int NOT NULL DEFAULT(0)
GO

-- Update Age values
Update FamilyMembers Set Age = DATEDIFF(MONTH, DateOfBirth, GETDATE())/ 12

--Alter DateOfBirth column to be nullable
Alter Table FamilyMembers
Alter Column DateOfBirth date NULL

-- Remove All data from DateOfBirth Column
--Update FamilyMembers Set DateOfBirth = NULL

----------------------------------------------------------------------------------------------------------------------------------------------------------------


-- Add CreatedDate, ModifiedDate columns in Inquiries
ALTER TABLE Inquiries
	ADD CreatedDate datetime NOT NULL DEFAULT(GETDATE()), ModifiedDate datetime NOT NULL DEFAULT(GETDATE())
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------

-- Add CreatedDate, ModifiedDate columns in InvestTab_User_Specification
ALTER TABLE InvestTab_User_Specification
	ADD CreatedDate datetime NOT NULL DEFAULT(GETDATE()), ModifiedDate datetime NOT NULL DEFAULT(GETDATE())
GO

----------------------------------------------------------------------------------------------------------------------------------------------------------------

-- Add CreatedDate, ModifiedDate columns in Stocks_Investment
ALTER TABLE Stocks_Investment
	ADD CreatedDate datetime NOT NULL DEFAULT(GETDATE()), ModifiedDate datetime NOT NULL DEFAULT(GETDATE())
GO


----------------------------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[sp_ActivateUserAccount]    Script Date: 4/24/2020 12:00:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_ActivateUserAccount]
	@ActivationCode uniqueidentifier, @UserID int
AS
BEGIN

	UPDATE [dbo].Users SET IsEmailVerified = 'true', ActivationCode = @ActivationCode, ModifiedDate = GETDATE()
	where UserID = @UserID
END



----------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[sp_ApproveDisapproveUser]    Script Date: 4/24/2020 12:00:44 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_ApproveDisapproveUser]
	@IsUserActivated bit, @UserID int, @IsDeactivated bit
AS
BEGIN
    Update  [dbo].[Users] Set IsUserActivated=@IsUserActivated, IsNewRequest = 'false', IsDeactivated = @IsDeactivated, ModifiedDate = GETDATE()
	where UserID=@UserID
END


----------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[sp_InsertEventRegistrationDetails]    Script Date: 4/24/2020 12:01:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Batch submitted through debugger: SQLQuery4.sql|7|0|C:\Users\ampa\AppData\Local\Temp\~vs6C29.sql
-- Batch submitted through debugger: SQLQuery19.sql|7|0|C:\Users\ampa\AppData\Local\Temp\~vsCBC.sql
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_InsertEventRegistrationDetails]
	-- Add the parameters for the stored procedure here
	@UserID int, @EventID int, @DateRegistered datetime, @FamilyMemberID int, @Age int
AS
BEGIN
	Declare @IsChild bit

	If (@Age <= 5) SET @IsChild = 1 Else SET @IsChild = 0

	If @FamilyMemberID Not In (SELECT Event_Registration.FamilyMemberID from Event_Registration where EventID = @EventID)
	Begin
		Insert Into dbo.Event_Registration(EventID, UserID, FamilyMemberID, DateRegistered, IsChild)
		VALUES (@EventID, @UserID, @FamilyMemberID, @DateRegistered, @IsChild)
	END
END


----------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[sp_InsertFamilyMemberDetails]    Script Date: 4/24/2020 12:01:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_InsertFamilyMemberDetails]
	@UserID int, @FirstName varchar(50), @LastName varchar(50), @Age int, @Relation varchar(50)
AS
BEGIN
	
	Insert Into dbo.FamilyMembers(UserID, FirstName, LastName, Age, Relation, CreatedDate, ModifiedDate)
    VALUES (@UserID, @FirstName, @LastName, @Age, @Relation, GETDATE(), GETDATE())
END


----------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[sp_InsertInquiryDetails]    Script Date: 4/24/2020 12:03:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_InsertInquiryDetails]
	-- Add the parameters for the stored procedure here
	@IsMember bit, @FirstName nvarchar(50), @LastName nvarchar(50), 
	@EmailID nvarchar(50), @contactNumber nvarchar(50), @Message nvarchar(MAX)
AS
BEGIN
	
	Insert Into dbo.Inquiries (IsMember, FirstName, LastName, ContactNumber, EmailID, Message, DidReply, CreatedDate, ModifiedDate)
    VALUES (@IsMember, @FirstName, @LastName, @ContactNumber, @EmailID, @Message, 0, GETDATE(), GETDATE())
END


----------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[sp_InsertPWResetCode]    Script Date: 4/24/2020 12:03:38 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_InsertPWResetCode]
	@pwresetcode uniqueidentifier, @EmailID nvarchar(50)
AS
BEGIN

	UPDATE [dbo].[Users] SET PWResetCode = @pwresetcode, ModifiedDate = GETDATE() where EmailID = @EmailID
END


----------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[sp_InsertStock]    Script Date: 4/24/2020 12:03:52 PM ******/
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
	@DividendPercent float, @DividendYieldPercent float, @Comment text
	 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	

    -- Insert statements for procedure here
	INSERT INTO dbo.Stocks_Investment(UserID, UserName, Ticker, Exchange, CompanyName, LastUpdated, Price, Price_High, Price_Low, P_E_Ratio, EPS, DividendPerShare, DividendPercent, DividendYieldPercent, Comment, CreatedDate, ModifiedDate) 
	VALUES (@UserID, @UserName, @Ticker, @Exchange, @CompanyName, @LastUpdated, @Price, @Price_High, @Price_Low, @P_E_Ratio, @EPS, @DividendPerShare, @DividendPercent, @DividendYieldPercent, @Comment, GETDATE(), GETDATE())
END



----------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[sp_InsertUserInvestAccessRecord]    Script Date: 4/24/2020 12:04:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_InsertUserInvestAccessRecord]
	@UserID int, @IsInvestTabVisible bit, @HasAcceptedDisclaimer bit
AS
BEGIN
	
	Insert Into dbo.InvestTab_User_Specification(UserID, IsInvestTabVisible, HasAcceptedDisclaimer, CreatedDate, ModifiedDate)
    VALUES (@UserID, @IsInvestTabVisible, @HasAcceptedDisclaimer, GETDATE(), GETDATE())
END


----------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[sp_RegisterUser]    Script Date: 4/24/2020 12:35:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Amit Patel
-- ALTER date: May 29, 2019
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_RegisterUser]
	@FirstName nvarchar(50), @LastName nvarchar(50), @EmailID nvarchar(50), @Password nvarchar(50), 
	@Address nvarchar(MAX), @City nvarchar(50), @State nvarchar(50), @Country nvarchar(50), @ZipCode nvarchar(50),
	@PhoneNumber nvarchar(50), @Native nvarchar(50), @IsEmailVerified bit, @ActivationCode uniqueidentifier, @IsNewRequest bit,
	@Age int
	 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	

    -- Insert statements for procedure here
	INSERT INTO dbo.Users (FirstName, LastName, EmailID, Password, Address, City, State, Country, ZipCode, PhoneNumber, Native, IsEmailVerified, ActivationCode, IsNewRequest, IsDeactivated, CreatedDate, ModifiedDate, Age) 
	VALUES (@FirstName, @LastName, @EMailID, @Password, @Address, @City, @State, @Country, @ZipCode, @PhoneNumber, @Native, @IsEmailVerified, @ActivationCode, @IsNewRequest, 'false', GETDATE(), GETDATE(), @Age)
END


----------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[sp_UpdateFamilyMemberDetails]    Script Date: 4/24/2020 12:04:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_UpdateFamilyMemberDetails]
	@FamilyMemberID int, @FirstName varchar(50), @LastName varchar(50), @Relation varchar(50)
AS
BEGIN
	
	UPDATE [dbo].[FamilyMembers] SET FirstName = @FirstName, LastName = @LastName,
	Relation = @Relation, ModifiedDate = GETDATE()
	where FamilyMemberID= @FamilyMemberID
END


----------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[sp_UpdateInquiryRecord]    Script Date: 4/24/2020 12:05:00 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_UpdateInquiryRecord]
	@InquiryID int
AS
BEGIN
	
	UPDATE [dbo].Inquiries SET DidReply = 1 , ModifiedDate = GETDATE()
	where InquiryID = @InquiryID
END


----------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[sp_UpdatePassword]    Script Date: 4/24/2020 12:05:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_UpdatePassword]
	@password nvarchar(50), @RandomCode uniqueidentifier, @pwresetcode uniqueidentifier
AS
BEGIN

	UPDATE [dbo].[Users] SET Password = @password, PWResetCode = @RandomCode, ModifiedDate = GETDATE() where PWResetCode = @pwresetcode
END


----------------------------------------------------------------------------------------------------------------------------------------------------------------

/****** Object:  StoredProcedure [dbo].[sp_UpdateStockDetails]    Script Date: 4/24/2020 12:05:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_UpdateStockDetails]
	@SI_ID int, @LastUpdated datetime, @Price float, @Price_High float, @Price_Low float, @P_E_Ratio float, @EPS float, @DividendPerShare float, 
	@DividendPercent float, @DividendYieldPercent float, @Comment text
AS
BEGIN
	UPDATE [dbo].Stocks_Investment SET LastUpdated = @LastUpdated, Price = @Price, Price_High = @Price_High, Price_Low = @Price_Low, P_E_Ratio = @P_E_Ratio, EPS =  @EPS, DividendPerShare = @DividendPerShare,
	DividendPercent =  @DividendPercent, DividendYieldPercent = @DividendYieldPercent, Comment = @Comment, ModifiedDate = GETDATE()
	where SI_ID= @SI_ID
END


----------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[sp_UpdateUserInvestmentAccessRecord]    Script Date: 4/24/2020 12:05:36 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_UpdateUserInvestmentAccessRecord] 
	@InvestTab_User_Spec_ID int, @UserID int, @IsInvestTabVisible bit, @HasAcceptedDisclaimer bit, @DateAccepted date
AS
BEGIN	
	UPDATE [dbo].InvestTab_User_Specification SET IsInvestTabVisible = @IsInvestTabVisible, HasAcceptedDisclaimer = @HasAcceptedDisclaimer, DateAccepted = @DateAccepted,
	ModifiedDate = GETDATE()
	where InvestTab_User_Spec_ID= @InvestTab_User_Spec_ID
END


----------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[sp_UpdateUserProfile]    Script Date: 4/24/2020 12:05:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_UpdateUserProfile]
	@UserID int, @address nvarchar(MAX), @city nvarchar(50), @state nvarchar(50),
	@country nvarchar(50), @zipCode nvarchar(50), @phoneNumber nvarchar(50),
	@native nvarchar(50)
AS
BEGIN
	
	UPDATE [dbo].[Users] SET Address = @address, City = @city, State = @state,
	Country = @country, ZipCode = @zipCode, PhoneNumber = @phoneNumber, Native = @native,
	ModifiedDate = GETDATE()
	where UserID= @UserID
END



----------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[sp_CheckFamilyMemberExists]    Script Date: 4/24/2020 1:49:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_CheckFamilyMemberExists]
	@UserID int, @FirstName varchar(50), @LastName varchar(50), @Age int, @Relation varchar(50)
AS
BEGIN
	
    -- Insert statements for procedure here
	SELECT * from dbo.FamilyMembers where UserID = @UserID AND FirstName = @FirstName AND LastName = @LastName AND Age = @Age AND Relation = @Relation;
	return @@ROWCOUNT
END



----------------------------------------------------------------------------------------------------------------------------------------------------------------
/****** Object:  StoredProcedure [dbo].[sp_GetEventRegistrationsDetails]    Script Date: 4/25/2020 2:58:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Batch submitted through debugger: SQLQuery3.sql|7|0|C:\Users\ampa\AppData\Local\Temp\~vs6446.sql
-- =============================================
-- Author:		<Author,,Name>
-- ALTER date: <ALTER Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_GetEventRegistrationsDetails]

AS
BEGIN
	SELECT e.[EventID], e.[Event_Name], e.Price_Adult, e.Price_Child, e.Fees_Membership
	, r.[UserID], Mem_FirstName = u.[FirstName], Mem_LastName = u.[LastName], u.[EmailID], u.[Address], u.[City], u.[State], u.Country, u.[ZipCode], u.[PhoneNumber], u.[Native]
	, r.[FamilyMemberID], f.[FirstName], f.[LastName], f.[DateOfBirth], f.[Relation]
	, Age = f.Age + (DATEDIFF(MONTH, f.[CreatedDate], e.[Event_StartDate]) / 12), r.[DateRegistered]
	INTO #tmp
	FROM [UGKPS].[dbo].[Events] e
	JOIN [UGKPS].[dbo].[Event_Registration] r ON r.EventID = e.EventID
	JOIN [UGKPS].[dbo].[Users] u ON u.UserID = r.UserID
	JOIN [UGKPS].[dbo].[FamilyMembers] f ON f.UserID = u.UserID AND f.[FamilyMemberID] = r.[FamilyMemberID]
	WHERE e.IsActive = 'true'
	ORDER BY u.UserID, f.[FamilyMemberID]

	SELECT * FROM #tmp ORDER BY UserID, [FamilyMemberID] 
END