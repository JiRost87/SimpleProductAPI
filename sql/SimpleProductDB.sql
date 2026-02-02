-- DB Creation
CREATE DATABASE ProductDB;
GO

-- Sample user
CREATE LOGIN ProductDBUser
	WITH PASSWORD = 'hsRPW@"v641h';
GO

USE [ProductDB]
CREATE USER ProductDBUser
	FOR LOGIN ProductDBUser;
GO

-- Table Creation
USE [ProductDB]
CREATE TABLE [dbo].[Product](
	[ProductId] INT NOT NULL,
	[ProductName] NVARCHAR(100) NOT NULL,
	[ImgUri] NVARCHAR(255) NOT NULL,
	[Price] DECIMAL(12,2) NOT NULL,
	[Description] NVARCHAR(255) NULL,
	CONSTRAINT PK_Product_ProductId PRIMARY KEY CLUSTERED (ProductId)
);
GO

-- SP for getting Product(s) data, if ProductId is provided returns all products
CREATE PROCEDURE [dbo].[GetProducts](
	@ProductId INT = NULL
)
AS
BEGIN
	IF (@ProductId IS NULL)
		SELECT [ProductId],
			[ProductName],
			[ImgUri],
			[Price],
			[Description]
		FROM [dbo].[Product]
	ELSE
		SELECT [ProductId],
			[ProductName],
			[ImgUri],
			[Price],
			[Description]
		FROM [dbo].[Product]
		WHERE [ProductId] = @ProductId;
END
GO

-- SP for updating description for Product
CREATE PROCEDURE [dbo].[InsertOrUpdateProductDescription](
	@ProductId INT = NULL,
	@Description NVARCHAR(255) = NULL
)
AS
BEGIN
	UPDATE [dbo].[Product]
	SET [Description] = @Description
	WHERE [ProductId] = @ProductId;
END
GO

-- granting privileges for sample user to execute needed SPs
USE [ProductDB]
GRANT EXECUTE ON [dbo].[GetProducts]
	TO ProductDBUser;

GRANT EXECUTE ON [dbo].[InsertOrUpdateProductDescription]
	TO ProductDBUser;
GO

-- sample data seeding
USE [ProductDB]
INSERT INTO [Product](
	[ProductId], 
	[ProductName],
	[ImgUri],
	[Price],
	[Description]
)
VALUES
(45632, N'TestProduct', N'http://TestImg.png', 45.99, NULL);
GO


