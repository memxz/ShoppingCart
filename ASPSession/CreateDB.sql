CREATE DATABASE ShoppingCartDB
GO

USE ShoppingCartDB
GO

CREATE TABLE Customers
(
	[CustomerId] INT IDENTITY(1,1) NOT NULL,
	[customerName] NVARCHAR(50) NOT NULL,
	[username] NVARCHAR(20) COLLATE LATIN1_GENERAL_CS_AS NOT NULL,
	[password] NVARCHAR(20) COLLATE LATIN1_GENERAL_CS_AS NOT NULL,
	
    CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([CustomerId] ASC))

GO

CREATE TABLE [Sessions]
(
	[sessionId] UNIQUEIDENTIFIER NOT NULL,
	[customerId] INT UNIQUE NOT NULL,
	[login_time] DATETIME DEFAULT GETDATE()NOT NULL
	
    CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED ([sessionId] ASC)
    CONSTRAINT [FK_SC100] FOREIGN KEY ([customerId]) REFERENCES [Customers] ([CustomerId]))

GO


CREATE TABLE Product
(
	[productId] INT NOT NULL,
	[productName] NVARCHAR (50),
	[productPrice] DECIMAL (10,2),
	[productDesc] NVARCHAR (300),
	[productIMG] NVARCHAR (300)
	
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([productId] ASC))
GO


CREATE TABLE [Orders]
(
	[orderId] INT IDENTITY(10020,1) NOT NULL,
	[customerId] INT NOT NULL,
	[productId] INT NOT NULL,
	[orderDate] DATETIME DEFAULT GETDATE()NOT NULL,
	[productQty] INT NOT NULL
	
    CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([OrderId] ASC)
    CONSTRAINT [FK_SC101] FOREIGN KEY ([customerId]) REFERENCES [Customers] ([CustomerId]),
	CONSTRAINT [FK_SC102] FOREIGN KEY ([productId]) REFERENCES [Product] ([ProductId]))
GO


CREATE TABLE ActivationCodes
(
	[activationID] UNIQUEIDENTIFIER NOT NULL,
	[orderID] INT NOT NULL
	
	CONSTRAINT [PK_ActivationCodes] PRIMARY KEY CLUSTERED ([activationID] ASC)
	CONSTRAINT [FK_SC103] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([OrderId]),
)

GO

CREATE TABLE Rating
(
	[ratingId] INT IDENTITY(1,1) NOT NULL,
	[customerId] INT NOT NULL,
	[productId] INT NOT NULL,
	[ratingValue] FLOAT(4) NOT NULL
	
    CONSTRAINT [PK_Rating] PRIMARY KEY CLUSTERED ([ratingId] ASC)
	CONSTRAINT [FK_SC104] FOREIGN KEY ([customerId]) REFERENCES [Customers] ([customerId]),
	CONSTRAINT [FK_SC105] FOREIGN KEY ([productId]) REFERENCES [Product] ([productId]))
	
GO

CREATE TABLE Cart
(
	[cartId] INT IDENTITY(1,1) NOT NULL,
	[customerId] INT NOT NULL,
	[productId] INT NOT NULL,
	[itemCount] INT NOT NULL
    CONSTRAINT [PK_Cart] PRIMARY KEY CLUSTERED ([cartId] ASC)
    CONSTRAINT [FK_SC106] FOREIGN KEY ([customerId]) REFERENCES [Customers] ([customerId]),
	CONSTRAINT [FK_SC108] FOREIGN KEY ([productId]) REFERENCES [Product] ([productId]))

INSERT INTO dbo.[Customers] (customerName,username,password) VALUES
(N'牛逼',N'牛逼',N'牛逼'),
(N'John Tan',N'Jonjon1',N'Jonjon1'),
(N'Mary Lee',N'MerryMarry',N'MerryMarry'),
(N'Peter Mok',N'ThePetrus',N'ThePetrus'),
(N'flat',N'flat',N'flat'),
(N'cottage',N'cottage',N'cottage'),
(N'headquarters',N'headquarters',N'headquarters'),
(N'temperature',N'temperature',N'temperature'),
(N'colorful',N'colorful',N'colorful'),
(N'salvation',N'salvation',N'salvation'),
(N'hilarious',N'hilarious',N'hilarious'),
(N'fold',N'fold',N'fold'),
(N'medicine',N'medicine',N'medicine'),
(N'border',N'border',N'border'),
(N'look',N'look',N'look'),
(N'advance',N'advance',N'advance'),
(N'sell',N'sell',N'sell')


INSERT INTO dbo.[Product](productid,productName,productPrice,productDesc,productIMG) VALUES
(N'1',N'.NET Charts',N'99',N'Brings powerful charting capabilities to your .NET applications.','/images/ChartNET.png'),
(N'2',N'.NET Paypal',N'69',N'Integrate your .NET apps with PayPal the easy way!','/images/Paypal.png'),
(N'3',N'.NET ML',N'299',N'Supercharged NET machine learning libraries.','/images/ML.png'),
(N'4',N'.NET Analytics',N'299',N'Performs data mining and analytics easily in .NET.','/images/Analytics.png'),
(N'5',N'.NET Logger',N'49',N'Logs and aggregates events easily in your NET apps.','/images/Logger.png'),
(N'6',N'.NET Numerics',N'199',N'Powerful numerical methods for your NET simulations.','/images/Math.png')

INSERT INTO dbo.[Rating](customerId,productId,ratingValue) VALUES
(N'1',N'1',N'1'),
(N'2',N'1',N'5'),
(N'3',N'1',N'5'),
(N'1',N'2',N'3'),
(N'2',N'2',N'5'),
(N'3',N'2',N'4'),
(N'1',N'3',N'3'),
(N'1',N'4',N'1')



insert into rating (customerId,productId,ratingValue) VALUES
	(N'11',N'1',N'5'),
	(N'11',N'3',N'3')
