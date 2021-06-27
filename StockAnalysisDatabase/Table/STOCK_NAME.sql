﻿CREATE TABLE [dbo].[STOCK_NAME](
	[STOCK_ID] [int] IDENTITY(1,1) NOT NULL,
	[STOCK_NAME] [varchar](1000) NULL,
	[STOCK_SYMBOL] [varchar](1000) NOT NULL,
	[STOCK_TYPE] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[STOCK_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO