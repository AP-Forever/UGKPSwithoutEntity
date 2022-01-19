USE [ugkps]
GO
/****** Object:  Table [dbo].[InvestTab_User_Specification]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvestTab_User_Specification](
	[InvestTab_User_Spec_ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[IsInvestTabVisible] [bit] NOT NULL,
	[HasAcceptedDisclaimer] [bit] NOT NULL,
	[DateAccepted] [date] NULL
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_ActivateEvent]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ActivateEvent]
	@EventID int
AS
BEGIN
	
	Update Events SET IsActive='false'

	Update Events SET IsActive='true' WHERE EventID=@EventID
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ActivateUserAccount]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ActivateUserAccount]
	@ActivationCode uniqueidentifier, @UserID int
AS
BEGIN

	UPDATE [dbo].Users SET IsEmailVerified = 'true', ActivationCode = @ActivationCode
	where UserID = @UserID
END
GO
/****** Object:  StoredProcedure [dbo].[sp_AddEvent]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Amit Patel
-- Create date: Feb 14, 2020
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_AddEvent] 
	@Event_Name nvarchar(100), @Event_Location nvarchar(100), @Event_Description nvarchar(100), @Event_StartDate datetime, @Event_EndDate datetime,
	@Date_Created datetime, @Date_Updated datetime, @Price_Adult float, @Price_Child float, @Fees_Membership float
	 
AS
BEGIN

    -- Insert statements for procedure here
	INSERT INTO dbo.Events(Event_Name, Event_Location, Event_Description, Event_StartDate, Event_EndDate, Date_Created, Date_Updated, Price_Adult, Price_Child, Fees_Membership, IsActive, IsCanceled) 
	VALUES (@Event_Name, @Event_Location, @Event_Description, @Event_StartDate, @Event_EndDate, @Date_Created, @Date_Updated, @Price_Adult, @Price_Child, @Fees_Membership, 0, 0)
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ApproveDisapproveUser]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_ApproveDisapproveUser]
	@IsUserActivated bit, @UserID int, @IsDeactivated bit
AS
BEGIN

    Update  [dbo].[Users] Set IsUserActivated=@IsUserActivated, IsNewRequest = 'false', IsDeactivated = @IsDeactivated where UserID=@UserID

END
GO
/****** Object:  StoredProcedure [dbo].[sp_CheckFamilyMemberExists]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_CheckFamilyMemberExists]
	@UserID int, @FirstName varchar(50), @LastName varchar(50), @DateOfBirth date, @Relation varchar(50)
AS
BEGIN
	
    -- Insert statements for procedure here
	SELECT * from dbo.FamilyMembers where UserID = @UserID AND FirstName = @FirstName AND LastName = @LastName AND DateOfBirth = @DateOfBirth AND Relation = @Relation;
	return @@ROWCOUNT
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CheckUser]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_CheckUser]
	@column nvarchar(100) = '', 
	@value varchar(MAX) = ''
AS
BEGIN

	Declare @sql nvarchar(max)
	set @sql = 'SELECT * From dbo.Users Where ' + @column + ' = ''' + @value  + ''''
	print @sql
	EXEC (@sql)
END
GO
/****** Object:  StoredProcedure [dbo].[sp_CheckUserInvestAccessRecord]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_CheckUserInvestAccessRecord]
	@UserID int
AS
BEGIN
	SELECT * from dbo.InvestTab_User_Specification where UserID = @UserID;
	return @@ROWCOUNT
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteFamilyMemberRecord]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_DeleteFamilyMemberRecord] 
	@FamilyMemberID int
AS
BEGIN

	Delete FROM FamilyMembers WHERE FamilyMemberID = @FamilyMemberID
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DeleteStockRecord]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_DeleteStockRecord]
	@SI_ID int
AS
BEGIN
	Delete FROM dbo.Stocks_Investment WHERE SI_ID = @SI_ID
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAttendingUserFamilyMember]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetAttendingUserFamilyMember]
	@FamilyMemberID int
AS
BEGIN
	
    -- Insert statements for procedure here
	SELECT * from dbo.Event_Registration where FamilyMemberID = @FamilyMemberID;
	return @@ROWCOUNT
END
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEventRegistrationsDetails]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_GetEventRegistrationsDetails]

AS
BEGIN
	SELECT e.[Event_Name], e.Price_Adult, e.Price_Child, e.Fees_Membership
	, r.[UserID], Mem_FirstName = u.[FirstName], Mem_LastName = u.[LastName], u.[EmailID], u.[Address], u.[City], u.[State], u.Country, u.[ZipCode], u.[PhoneNumber], u.[Native]
	, r.[FamilyMemberID], f.[FirstName], f.[LastName], f.[DateOfBirth], f.[Relation]
	, Age = DATEDIFF(YEAR, f.[DateOfBirth], e.[Event_StartDate]), r.[DateRegistered]
	INTO #tmp
	FROM [UGKPS].[dbo].[Events] e
	JOIN [UGKPS].[dbo].[Event_Registration] r ON r.EventID = e.EventID
	JOIN [UGKPS].[dbo].[Users] u ON u.UserID = r.UserID
	JOIN [UGKPS].[dbo].[FamilyMembers] f ON f.UserID = u.UserID AND f.[FamilyMemberID] = r.[FamilyMemberID]
	WHERE e.IsActive = 'true'
	ORDER BY u.UserID, f.[FamilyMemberID]

	SELECT * FROM #tmp ORDER BY UserID, [FamilyMemberID]
END
GO
/****** Object:  StoredProcedure [dbo].[sp_HasUserAlreadyAttendedEvent]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_HasUserAlreadyAttendedEvent]
	@UserID int
AS
BEGIN
	SELECT * from dbo.Event_Attended where UserID = @UserID;
	return @@ROWCOUNT
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertEventRegistrationDetails]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Batch submitted through debugger: SQLQuery19.sql|7|0|C:\Users\ampa\AppData\Local\Temp\~vsCBC.sql
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_InsertEventRegistrationDetails]
	-- Add the parameters for the stored procedure here
	@UserID int, @EventID int, @DateRegistered datetime, @FamilyMemberID int, @DateOfBirth date
AS
BEGIN
	Declare @IsChild bit

	If (DATEDIFF(YEAR,@DateOfBirth,GETDATE()) <= 5) SET @IsChild = 1 Else SET @IsChild = 0

	If @FamilyMemberID Not In (SELECT Event_Registration.FamilyMemberID from Event_Registration)
	Begin
		Insert Into dbo.Event_Registration(EventID, UserID, FamilyMemberID, DateRegistered, IsChild)
		VALUES (@EventID, @UserID, @FamilyMemberID, @DateRegistered, @IsChild)
	END
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertFamilyMemberDetails]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_InsertFamilyMemberDetails]
	@UserID int, @FirstName varchar(50), @LastName varchar(50), @DateOfBirth date, @Relation varchar(50)
AS
BEGIN
	
	Insert Into dbo.FamilyMembers(UserID, FirstName, LastName, DateOfBirth, Relation)
    VALUES (@UserID, @FirstName, @LastName, @DateOfBirth, @Relation)
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertInquiryDetails]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_InsertInquiryDetails]
	-- Add the parameters for the stored procedure here
	@IsMember bit, @FirstName nvarchar(50), @LastName nvarchar(50), 
	@EmailID nvarchar(50), @contactNumber nvarchar(50), @Message nvarchar(MAX)
AS
BEGIN
	
	Insert Into dbo.Inquiries (IsMember, FirstName, LastName, ContactNumber, EmailID, Message)
    VALUES (@IsMember, @FirstName, @LastName, @ContactNumber, @EmailID, @Message)
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertPWResetCode]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_InsertPWResetCode]
	@pwresetcode uniqueidentifier, @EmailID nvarchar(50)
AS
BEGIN

	UPDATE [dbo].[Users] SET PWResetCode = @pwresetcode where EmailID = @EmailID
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertStock]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Amit Patel
-- Create date: Feb 14, 2020
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_InsertStock] 
	@UserID int, @UserName nvarchar(100), @Ticker nvarchar(100), @CompanyName nvarchar(100), @LastUpdated datetime,
	@Price float, @Price_High float, @Price_Low float, @P_E_Ratio float, @EPS float, @DividendPerShare float, 
	@DividendPercent float, @DividendYieldPercent float, @Comment text
	 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	

    -- Insert statements for procedure here
	INSERT INTO dbo.Stocks_Investment(UserID, UserName, Ticker, CompanyName, LastUpdated, Price, Price_High, Price_Low, P_E_Ratio, EPS, DividendPerShare, DividendPercent, DividendYieldPercent, Comment) 
	VALUES (@UserID, @UserName, @Ticker, @CompanyName, @LastUpdated, @Price, @Price_High, @Price_Low, @P_E_Ratio, @EPS, @DividendPerShare, @DividendPercent, @DividendYieldPercent, @Comment)
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertUserInvestAccessRecord]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_InsertUserInvestAccessRecord]
	@UserID int, @IsInvestTabVisible bit, @HasAcceptedDisclaimer bit
AS
BEGIN
	
	Insert Into dbo.InvestTab_User_Specification(UserID, IsInvestTabVisible, HasAcceptedDisclaimer)
    VALUES (@UserID, @IsInvestTabVisible, @HasAcceptedDisclaimer)
END
GO
/****** Object:  StoredProcedure [dbo].[sp_MarkUserAsAttendedEvent]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_MarkUserAsAttendedEvent]
@UserID int, @DateAttended datetime
AS
BEGIN
	Insert INTO Event_Attended (EventID, UserID, FamilyMemberID, DateAttended)
	SELECT EventID, UserID, FamilyMemberID, @DateAttended as DateAttended
	FROM Event_Registration
	WHERE UserID = @UserID
    
END
GO
/****** Object:  StoredProcedure [dbo].[sp_RegisterUser]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Amit Patel
-- Create date: May 29, 2019
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_RegisterUser]
	@FirstName nvarchar(50), @LastName nvarchar(50), @EmailID nvarchar(50), @Password nvarchar(50), @DateOfBirth date,
	@Address nvarchar(MAX), @City nvarchar(50), @State nvarchar(50), @Country nvarchar(50), @ZipCode nvarchar(50),
	@PhoneNumber nvarchar(50), @Native nvarchar(50), @IsEmailVerified bit, @ActivationCode uniqueidentifier, @IsNewRequest bit
	 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	

    -- Insert statements for procedure here
	INSERT INTO dbo.Users (FirstName, LastName, EmailID, Password, DateOfBirth, Address, City, State, Country, ZipCode, PhoneNumber, Native, IsEmailVerified, ActivationCode, IsNewRequest, IsDeactivated) 
	VALUES (@FirstName, @LastName, @EMailID, @Password, @DateOfBirth, @Address, @City, @State, @Country, @ZipCode, @PhoneNumber, @Native, @IsEmailVerified, @ActivationCode, @IsNewRequest, 'false')
END
GO
/****** Object:  StoredProcedure [dbo].[sp_RemoveFamilyMemberFromEventRegistration]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_RemoveFamilyMemberFromEventRegistration] 
	@FamilyMemberID int, @EventID int
AS
BEGIN

	Delete FROM Event_Registration WHERE FamilyMemberID = @FamilyMemberID AND EventID = @EventID
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateFamilyMemberDetails]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_UpdateFamilyMemberDetails]
	@FamilyMemberID int, @FirstName varchar(50), @LastName varchar(50), @Relation varchar(50)
AS
BEGIN
	
	UPDATE [dbo].[FamilyMembers] SET FirstName = @FirstName, LastName = @LastName,
	Relation = @Relation
	where FamilyMemberID= @FamilyMemberID
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdatePassword]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_UpdatePassword]
	@password nvarchar(50), @RandomCode uniqueidentifier, @pwresetcode uniqueidentifier
AS
BEGIN

	UPDATE [dbo].[Users] SET Password = @password, PWResetCode = @RandomCode where PWResetCode = @pwresetcode
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateStockDetails]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_UpdateStockDetails]
	@SI_ID int, @LastUpdated datetime, @Price float, @Price_High float, @Price_Low float, @P_E_Ratio float, @EPS float, @DividendPerShare float, 
	@DividendPercent float, @DividendYieldPercent float, @Comment text
AS
BEGIN
	UPDATE [dbo].Stocks_Investment SET LastUpdated = @LastUpdated, Price = @Price, Price_High = @Price_High, Price_Low = @Price_Low, P_E_Ratio = @P_E_Ratio, EPS =  @EPS, DividendPerShare = @DividendPerShare,
	DividendPercent =  @DividendPercent, DividendYieldPercent = @DividendYieldPercent, Comment = @Comment
	where SI_ID= @SI_ID
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateUserInvestmentAccessRecord]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_UpdateUserInvestmentAccessRecord] 
	@InvestTab_User_Spec_ID int, @UserID int, @IsInvestTabVisible bit, @HasAcceptedDisclaimer bit, @DateAccepted date
AS
BEGIN	
	UPDATE [dbo].InvestTab_User_Specification SET IsInvestTabVisible = @IsInvestTabVisible, HasAcceptedDisclaimer = @HasAcceptedDisclaimer, DateAccepted = @DateAccepted
	where InvestTab_User_Spec_ID= @InvestTab_User_Spec_ID
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateUserProfile]    Script Date: 3/21/2020 8:22:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_UpdateUserProfile]
	@UserID int, @address nvarchar(MAX), @city nvarchar(50), @state nvarchar(50),
	@country nvarchar(50), @zipCode nvarchar(50), @phoneNumber nvarchar(50),
	@native nvarchar(50)
AS
BEGIN
	
	UPDATE [dbo].[Users] SET Address = @address, City = @city, State = @state,
	Country = @country, ZipCode = @zipCode, PhoneNumber = @phoneNumber, Native = @native
	where UserID= @UserID
END
GO
