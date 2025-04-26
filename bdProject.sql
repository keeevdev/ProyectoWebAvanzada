CREATE DATABASE proyectoweb;

USE proyectoweb;

CREATE TABLE Roles (
    Id    INT IDENTITY PRIMARY KEY,
    Name  NVARCHAR(50) UNIQUE NOT NULL
);

CREATE TABLE Users (
    Id             INT IDENTITY PRIMARY KEY,
    Username       NVARCHAR(50) UNIQUE NOT NULL,
    Email          NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash   VARBINARY(MAX) NOT NULL,
    CreatedAt      DATETIME    NOT NULL DEFAULT GETDATE()
);

CREATE TABLE UserRoles (
    UserId INT NOT NULL FOREIGN KEY REFERENCES Users(Id),
    RoleId INT NOT NULL FOREIGN KEY REFERENCES Roles(Id),
    CONSTRAINT PK_UserRoles PRIMARY KEY (UserId,RoleId)
);

INSERT INTO Roles (Name) VALUES ('usuario'), ('admin'), ('empleado');


/* Registrar usuario y asignar rol 'usuario' */
CREATE PROCEDURE sp_RegisterUser
    @Username NVARCHAR(50),
    @Email    NVARCHAR(100),
    @PasswordHash VARBINARY(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Users (Username,Email,PasswordHash)
    VALUES (@Username,@Email,@PasswordHash);

    DECLARE @NewUserId INT = SCOPE_IDENTITY();
    DECLARE @UserRoleId INT = (SELECT Id FROM Roles WHERE Name='usuario');

    INSERT INTO UserRoles (UserId,RoleId)
    VALUES (@NewUserId,@UserRoleId);
END
GO

/* Obtener usuario por username */
CREATE PROCEDURE sp_GetUserByUsername
    @Username NVARCHAR(50)
AS
BEGIN
    SELECT * FROM Users WHERE Username=@Username;
END
GO

/* Obtener roles de un usuario */
CREATE PROCEDURE sp_GetRolesByUserId
    @UserId INT
AS
BEGIN
    SELECT R.Name 
    FROM Roles R
    JOIN UserRoles UR ON UR.RoleId=R.Id
    WHERE UR.UserId=@UserId;
END
GO


SELECT * FROM Users;




-- Menu
CREATE TABLE MenuItems (
    Id          INT IDENTITY PRIMARY KEY,
    Name        NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255) NULL,
    Price       DECIMAL(10,2) NOT NULL,
    CreatedAt   DATETIME NOT NULL DEFAULT GETDATE()
);

-- Pedidos
CREATE TABLE Orders (
    Id                   INT IDENTITY PRIMARY KEY,
    UserId               INT NOT NULL REFERENCES Users(Id),
    TableNumber          INT NOT NULL,
    Status               NVARCHAR(50) NOT NULL, -- 'Pending','InProgress','Completed'
    CreatedAt            DATETIME NOT NULL DEFAULT GETDATE(),
    EstimatedTimeMinutes INT NULL
);

CREATE TABLE OrderItems (
    OrderId    INT NOT NULL REFERENCES Orders(Id),
    MenuItemId INT NOT NULL REFERENCES MenuItems(Id),
    Quantity   INT NOT NULL,
    CONSTRAINT PK_OrderItems PRIMARY KEY (OrderId, MenuItemId)
);

-- Menu SPs
CREATE PROCEDURE sp_GetMenuItems AS
SELECT * FROM MenuItems;
GO

CREATE PROCEDURE sp_AddMenuItem
    @Name NVARCHAR(100),
    @Description NVARCHAR(255),
    @Price DECIMAL(10,2)
AS
BEGIN
    INSERT INTO MenuItems(Name,Description,Price)
    VALUES(@Name,@Description,@Price);
END
GO

CREATE PROCEDURE sp_UpdateMenuItem
    @Id INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(255),
    @Price DECIMAL(10,2)
AS
BEGIN
    UPDATE MenuItems
      SET Name=@Name, Description=@Description, Price=@Price
    WHERE Id=@Id;
END
GO

CREATE PROCEDURE sp_DeleteMenuItem
    @Id INT
AS
BEGIN
    DELETE FROM MenuItems WHERE Id=@Id;
END
GO

-- Pedidos SPs
CREATE PROCEDURE sp_CreateOrder
    @UserId INT,
    @TableNumber INT,
    @OrderId INT OUTPUT
AS
BEGIN
    INSERT INTO Orders(UserId,TableNumber,Status)
    VALUES(@UserId,@TableNumber,'Pending');
    SET @OrderId = SCOPE_IDENTITY();
END
GO

CREATE PROCEDURE sp_AddOrderItem
    @OrderId INT,
    @MenuItemId INT,
    @Quantity INT
AS
BEGIN
    INSERT INTO OrderItems(OrderId,MenuItemId,Quantity)
    VALUES(@OrderId,@MenuItemId,@Quantity);
END
GO

CREATE PROCEDURE sp_GetOrdersForEmployee AS
SELECT * FROM Orders WHERE Status='Pending';
GO

CREATE PROCEDURE sp_AssignOrderTime
    @OrderId INT,
    @EstimatedTimeMinutes INT
AS
BEGIN
    UPDATE Orders
      SET EstimatedTimeMinutes=@EstimatedTimeMinutes, Status='InProgress'
    WHERE Id=@OrderId;
END
GO

CREATE PROCEDURE sp_CompleteOrder
    @OrderId INT
AS
BEGIN
    UPDATE Orders
      SET Status='Completed'
    WHERE Id=@OrderId;
END
GO

CREATE PROCEDURE sp_GetOrderItems
    @OrderId INT
AS
BEGIN
    SELECT oi.OrderId, oi.MenuItemId, mi.Name, mi.Price, oi.Quantity
    FROM OrderItems oi
    JOIN MenuItems mi ON mi.Id=oi.MenuItemId
    WHERE oi.OrderId=@OrderId;
END
GO

INSERT INTO MenuItems (Name, Description, Price) VALUES
('Pizza Margherita', 'Classic pizza with tomato sauce, mozzarella, and basil', 12.50),
('Hamburger', 'Juicy beef patty with lettuce, tomato, and cheese', 8.99),
('Caesar Salad', 'Fresh romaine lettuce with croutons and Caesar dressing', 7.25),
('Spaghetti Carbonara', 'Pasta with egg, pancetta, Parmesan cheese, and black pepper', 11.00),
('Chicken Tacos', 'Grilled chicken with salsa, guacamole, and sour cream', 9.50),
('Sushi Platter', 'Assortment of fresh sushi rolls and nigiri', 15.75),
('Steak Frites', 'Grilled steak served with crispy French fries', 18.00),
('Mushroom Risotto', 'Creamy risotto with mixed mushrooms and truffle oil', 14.25),
('Fish and Chips', 'Deep-fried fish with thick-cut fries', 10.50),
('Chocolate Brownie', 'Rich chocolate brownie with vanilla ice cream', 6.00);
GO

INSERT INTO Users (Username, Email, PasswordHash) VALUES
('test', 'test2@example.com', HASHBYTES('SHA2_512', 'MySecurePassword'));
GO

BEGIN Transaction;

DECLARE @UserId INT = (SELECT Id FROM Users Where Username = 'test1');
DECLARE @NewRoleId INT = (SELECT Id FROM Roles Where Name = 'admin');

DELETE FROM UserRoles WHERE UserId = @UserId;

INSERT INTO UserRoles (UserId, RoleID) VALUES (@UserId, @NewRoleId);

COMMIT;

SELECT * FROM Users;

BEGIN Transaction;

DECLARE @UserId INT = (SELECT Id FROM Users Where Username = 'kevin5');
DECLARE @NewRoleId INT = (SELECT Id FROM Roles Where Name = 'empleado');

DELETE FROM UserRoles WHERE UserId = @UserId;

INSERT INTO UserRoles (UserId, RoleID) VALUES (@UserId, @NewRoleId);

COMMIT;

CREATE PROCEDURE sp_GetOrdersByUser
    @UserId INT
AS
BEGIN
    SELECT Id, UserId, TableNumber, Status, CreatedAt, EstimatedTimeMinutes
    FROM Orders
    WHERE UserId = @UserId;
END
GO


SELECT * FROM Orders;


SELECT * FROM OrderItems;

SELECT * FROM MenuItems;


CREATE PROCEDURE sp_GetMenuItemById
    @Id INT
AS
BEGIN
    SELECT Id, Name, Description, Price, CreatedAt
    FROM MenuItems
    WHERE Id = @Id;
END
GO





ALTER TABLE MenuItems
ADD ImageUrl NVARCHAR(255) NULL;
GO


ALTER PROCEDURE sp_AddMenuItem
    @Name NVARCHAR(100),
    @Description NVARCHAR(255),
    @Price DECIMAL(10,2),
    @ImageUrl NVARCHAR(255)
AS
BEGIN
    INSERT INTO MenuItems(Name,Description,Price,ImageUrl)
    VALUES(@Name,@Description,@Price,@ImageUrl);
END
GO


ALTER PROCEDURE sp_UpdateMenuItem
    @Id INT,
    @Name NVARCHAR(100),
    @Description NVARCHAR(255),
    @Price DECIMAL(10,2),
    @ImageUrl NVARCHAR(255)
AS
BEGIN
    UPDATE MenuItems
    SET Name=@Name,
        Description=@Description,
        Price=@Price,
        ImageUrl=@ImageUrl
    WHERE Id=@Id;
END
GO

DELETE FROM MenuItems;

