
ALTER TABLE dbo.AccountUsers ADD [IsAdmin] [bit] NOT NULL DEFAULT ((0))
UPDATE [dbo].[AccountUsers] SET [IsAdmin] = 1 WHERE UserName = 'BMHUNTER'

CREATE TABLE [dbo].[PageManager](
	[ID] [int] IDENTITY(1,1) NOT NULL,[BackgroundImageFileName] [nvarchar](max) NULL,
	[BackgroundImageFilePath] [nvarchar](max) NULL,[BackgroundImageName] [nvarchar](max) NULL,
 CONSTRAINT [PK_PageManager] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
