SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[template](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [char](50) NOT NULL,
	[Text] [text] NULL,
	[CreateDate] [datetime] NULL,
	[EditDate] [datetime] NULL,
	[Version] [int] NULL,
 CONSTRAINT [PK_template] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

GO
CREATE UNIQUE NONCLUSTERED INDEX [UNQ_Name] ON [dbo].[template] 
(
	[Name] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/*3张表的脚本*/
GO
CREATE TABLE [dbo].[task](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskId] [uniqueidentifier] NOT NULL,
	[StartTime] [datetime] NULL,
	[NextTime] [datetime] NULL,
	[FinishTime] [datetime] NULL,
	[Status] [tinyint] NULL,
	[StepCount] [int] NULL,
	[NextStep] [int] NULL,
	[UserSign] [nvarchar](80) NULL,
	[UserSignState] [int] NULL,
	[AttachState] [int] NULL,
	[CommandType] [nvarchar](20) NULL,
	[ProcessPercent] [decimal](18, 2) NULL,
	[CreateDate] [datetime] NULL,
	[EditDate] [datetime] NULL,
	[Version] [int] NULL,
	[PushMessage] [text] NULL,
 CONSTRAINT [PK_task] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

GO
CREATE UNIQUE NONCLUSTERED INDEX [UNQ_TaskId] ON [dbo].[task] 
(
	[TaskId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX [IDX_UserSign] ON [dbo].[task] 
(
	[UserSign] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX [IDX_Status] ON [dbo].[task] 
(
	[Status] ASC
)
INCLUDE ( [StartTime],
[AttachState]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

GO
CREATE TABLE [dbo].[task_node](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RowId] [uniqueidentifier] NOT NULL,
	[TaskId] [uniqueidentifier] NULL,
	[OrderNo] [int] NULL,
	[StartTime] [datetime] NULL,
	[FinishTime] [datetime] NULL,
	[StepCoordinationMode] [tinyint] NULL,
	[FailTimes] [int] NULL,
	[WaitTimes] [int] NULL,
	[StepCount] [int] NULL,
	[StepType] [text] NULL,
	[CreateDate] [datetime] NULL,
	[EditDate] [datetime] NULL,
	[Version] [int] NULL,
	[Removed] [bit] NULL,
	[ExecutingMessage] [text] NULL,
	[ResultMessage] [text] NULL,
 CONSTRAINT [PK_task_node] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

GO
ALTER TABLE [dbo].[task_node] ADD  CONSTRAINT [DF_task_node_Removed]  DEFAULT ((0)) FOR [Removed]
GO

CREATE UNIQUE NONCLUSTERED INDEX [UNQ_RowId] ON [dbo].[task_node] 
(
	[RowId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]


GO
CREATE NONCLUSTERED INDEX [IDX_TaskId] ON [dbo].[task_node] 
(
	[TaskId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY];
GO

CREATE TABLE [dbo].[task_node_error](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RowId] [uniqueidentifier] NULL,
	[CreateDate] [datetime] NULL,
	[StepType] [nvarchar](50) NULL,
	[FailException] [text] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

GO
CREATE NONCLUSTERED INDEX [IDX_RowId] ON [dbo].[task_node_error] 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

SET ANSI_PADDING OFF
GO


/*一张表的脚本*/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[task](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TaskId] [uniqueidentifier] NOT NULL,
	[StartTime] [datetime] NULL,
	[NextTime] [datetime] NULL,
	[FinishTime] [datetime] NULL,
	[Status] [tinyint] NULL,
	[StepCount] [int] NULL,
	[NextStep] [int] NULL,
	[UserSign] [nvarchar](80) NULL,
	[UserSignState] [int] NULL,
	[AttachState] [int] NULL,
	[CommandType] [nvarchar](20) NULL,
	[ProcessPercent] [decimal](18, 2) NULL,
	[CreateDate] [datetime] NULL,
	[EditDate] [datetime] NULL,
	[Version] [int] NULL,
	[FailTimes] [int] NULL,
	[WaitTimes] [int] NULL,
	[PushMessage] [text] NULL,
	[StepType] [text] NULL,
	[CoordinationMode] [text] NULL,
	[ExecutingMessage] [text] NULL,
	[ResultMessage] [text] NULL,
 CONSTRAINT [PK_task] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [UNQ_TaskId] ON [dbo].[task] 
(
	[TaskId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]


GO

CREATE NONCLUSTERED INDEX [IDX_UserSign] ON [dbo].[task] 
(
	[UserSign] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IDX_Status] ON [dbo].[task] 
(
	[Status] ASC
)
INCLUDE ( [StartTime],
[AttachState]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


