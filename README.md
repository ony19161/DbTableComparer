# Db Table Comparer
A simple DB schema and data comparison tool that finds the new rows in destination table, removed rows from source table and also lists the mutated data in destination table.


# Tool Setup Steps
1. Change the connection string in App.Config file
```
<connectionStrings>
<add name="DefaultConnectionString" 
		 connectionString="server=YOUR_SERVER_NAME;database=YOUR_DATABASE_NAME;uid=YOUR_DATABASE_USERNAME;password=YOUR_DATABASE_PASSWORD;Trusted_Connection=True;"
		 providerName="System.Data.SqlClient"/>
</connectionStrings>
```
2. Create tables in your database, you can use your preffered IDE or use the following script for example
```
CREATE TABLE [dbo].[SourceTable1](
   [SocialSecurityNumber] [nvarchar](50) NOT NULL,
   [FirstName] [nvarchar](50) NULL,
   [LastName] [nvarchar](50) NULL,
   [Department] [nvarchar](50) NULL,
CONSTRAINT [PK_SourceTable1] PRIMARY KEY CLUSTERED 
(
   [SocialSecurityNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[SourceTable2](
   [SocialSecurityNumber] [nvarchar](50) NOT NULL,
   [FirstName] [nvarchar](50) NULL,
   [LastName] [nvarchar](50) NULL,
   [Department] [nvarchar](50) NULL,
CONSTRAINT [PK_SourceTable2] PRIMARY KEY CLUSTERED 
(
   [SocialSecurityNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
```

# Run Application
Use the following command to run the application
```
DbTableComparer.exe -TableName1 SourceTable1 -TableName2 SourceTable2 -Primarykey SocialSecurityNumber
```
-TableName1, -TableName2, -Primarykey is required parameter to run the application successfully.
