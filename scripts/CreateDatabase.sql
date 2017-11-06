USE [master]
GO

IF  EXISTS (
	SELECT name 
		FROM sys.databases 
		WHERE name = N'ViSi'
)
BEGIN
		EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'ViSi'

		ALTER DATABASE ViSi SET  SINGLE_USER WITH ROLLBACK IMMEDIATE

		DROP DATABASE ViSi
END
GO

CREATE DATABASE [ViSi]
GO

USE [ViSi]
GO

ALTER DATABASE [ViSi] SET  READ_WRITE 
GO

USE [ViSi]
GO

/****** Object:  Table [dbo].[Patient]    Script Date: 2-11-2017 10:34:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Patient](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[PatientNumber] [nvarchar](50) NOT NULL,
	[EmailAddress] [nvarchar](100) NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[FamilyName] [nvarchar](100) NOT NULL,
	[DateOfBirth] [date] NOT NULL
) ON [PRIMARY]
GO

USE [ViSi]
GO

/****** Object:  Table [dbo].[BloodPressure]    Script Date: 2-11-2017 10:36:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BloodPressure](
	[Id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	[PatientId] [int] NOT NULL FOREIGN KEY REFERENCES [dbo].[Patient](Id),
	[MeasuredAt] [datetime] NOT NULL,
	[Systolic] [int] NOT NULL,
	[Diastolic] [int] NOT NULL
) ON [PRIMARY]
GO


USE [ViSi]
GO
SET IDENTITY_INSERT [dbo].[Patient] ON 
GO
INSERT [dbo].[Patient] ([Id], [PatientNumber], [EmailAddress], [FirstName], [FamilyName], [DateOfBirth]) VALUES (1, N'34987', N'james@adams.com', N'James', N'Adams', CAST(N'1980-09-30' AS Date))
GO
INSERT [dbo].[Patient] ([Id], [PatientNumber], [EmailAddress], [FirstName], [FamilyName], [DateOfBirth]) VALUES (2, N'34728', N'mary@baker.com', N'Mary', N'Baker', CAST(N'1971-04-28' AS Date))
GO
INSERT [dbo].[Patient] ([Id], [PatientNumber], [EmailAddress], [FirstName], [FamilyName], [DateOfBirth]) VALUES (3, N'98655', N'john@clark.com', N'John', N'Clark', CAST(N'1968-06-21' AS Date))
GO
INSERT [dbo].[Patient] ([Id], [PatientNumber], [EmailAddress], [FirstName], [FamilyName], [DateOfBirth]) VALUES (4, N'85454', N'patricia@davis.com', N'Patricia', N'Davis', CAST(N'1988-02-14' AS Date))
GO
SET IDENTITY_INSERT [dbo].[Patient] OFF
GO

USE [ViSi]
GO
SET IDENTITY_INSERT [dbo].[BloodPressure] ON 
GO
INSERT [dbo].[BloodPressure] ([Id], [PatientId], [MeasuredAt], [Systolic], [Diastolic]) VALUES (1, 1, CAST(N'2016-03-01T09:30:00.000' AS DateTime), 140, 90)
GO
INSERT [dbo].[BloodPressure] ([Id], [PatientId], [MeasuredAt], [Systolic], [Diastolic]) VALUES (2, 1, CAST(N'2016-04-01T10:15:00.000' AS DateTime), 130, 80)
GO
INSERT [dbo].[BloodPressure] ([Id], [PatientId], [MeasuredAt], [Systolic], [Diastolic]) VALUES (3, 2, CAST(N'2016-05-02T11:20:00.000' AS DateTime), 120, 80)
GO
INSERT [dbo].[BloodPressure] ([Id], [PatientId], [MeasuredAt], [Systolic], [Diastolic]) VALUES (4, 2, CAST(N'2016-06-02T09:20:00.000' AS DateTime), 110, 80)
GO
INSERT [dbo].[BloodPressure] ([Id], [PatientId], [MeasuredAt], [Systolic], [Diastolic]) VALUES (5, 3, CAST(N'2016-05-28T09:55:00.000' AS DateTime), 100, 60)
GO
INSERT [dbo].[BloodPressure] ([Id], [PatientId], [MeasuredAt], [Systolic], [Diastolic]) VALUES (6, 3, CAST(N'2016-05-29T09:50:00.000' AS DateTime), 105, 65)
GO
INSERT [dbo].[BloodPressure] ([Id], [PatientId], [MeasuredAt], [Systolic], [Diastolic]) VALUES (7, 3, CAST(N'2016-05-30T09:50:00.000' AS DateTime), 110, 67)
GO
SET IDENTITY_INSERT [dbo].[BloodPressure] OFF 
GO

