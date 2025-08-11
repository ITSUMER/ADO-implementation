USE POS_DB;

CREATE TABLE Customers (

    Id INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(50) NOT NULL,
    Contact VARCHAR(50) NOT NULL UNIQUE

);

CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name VARCHAR(50) NOT NULL,
    Price DECIMAL(10,2) NOT NULL CHECK (Price > 0),
    Stock INT NOT NULL CHECK (Stock >= 0),
    Category VARCHAR(50) NOT NULL,
    Size VARCHAR(50) NOT NULL
);

CREATE TABLE Orders (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT NOT NULL,
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);

CREATE TABLE OrderItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

INSERT INTO Customers(Name, Contact)
VALUES ('umer', '03099991261'), ('ahmad', '03123456789');

INSERT INTO Products (Name, Price, Stock, Category, Size)
VALUES 
('Laptop', 120, 5, 'Electronics', 'Medium'),
('pent', 200, 5, 'Clothing', 'Large');

INSERT INTO Orders (CustomerId) 
VALUES (1);

INSERT INTO OrderItems (OrderId, ProductId, Quantity)
VALUES (1, 1, 1), (1, 2, 3);

SELECT * FROM Customers;
SELECT * FROM Products;
SELECT * FROM Orders;
SELECT * FROM OrderItems;

SELECT 
    o.Id AS OrderId,
    c.Name AS CustomerName,
    p.Name AS ProductName,
    oi.Quantity,
    p.Price,
    (oi.Quantity * p.Price) AS Total
FROM Orders o
JOIN Customers c ON o.CustomerId = c.Id
JOIN OrderItems oi ON oi.OrderId = o.Id
JOIN Products p ON p.Id = oi.ProductId;

UPDATE Products SET Stock = Stock - 3 WHERE Id = 2;
DELETE FROM OrderItems WHERE Id = 1;

GO
CREATE PROCEDURE AddProduct
    @Name  VARCHAR(100),
    @Price DECIMAL(10,2),
    @Stock INT,
    @Category VARCHAR(50), 
    @Size VARCHAR(50)
AS
BEGIN
    BEGIN TRY
        INSERT INTO Products (Name, Price, Stock, Category, Size)
        VALUES (@Name, @Price, @Stock, @Category, @Size);

        PRINT 'Product added successfully.';
    END TRY
    BEGIN CATCH
        PRINT 'Error adding product: ' + ERROR_MESSAGE();
    END CATCH
END;
GO

GO
CREATE PROCEDURE ViewAllProducts
AS
BEGIN
    BEGIN TRY
        SELECT * FROM Products;
        PRINT 'Products listed successfully.';
    END TRY
    BEGIN CATCH
        PRINT 'Error: ' + ERROR_MESSAGE();
    END CATCH
END;
GO

CREATE PROCEDURE UpdateProductStock
    @ProductId INT,
    @NewStock INT
AS
BEGIN
    BEGIN TRY
        UPDATE Products
        SET Stock = @NewStock
        WHERE Id = @ProductId;

        IF @@ROWCOUNT = 0
            PRINT 'Product not found.';
        ELSE
            PRINT 'Stock updated successfully.';
    END TRY
    BEGIN CATCH
        PRINT 'Error: ' + ERROR_MESSAGE();
    END CATCH
END;
GO

CREATE PROCEDURE DeleteProduct
    @ProductId INT
AS
BEGIN
    BEGIN TRY
        DELETE FROM Products
        WHERE Id = @ProductId;

        IF @@ROWCOUNT = 0
            PRINT 'Product not found.';
        ELSE
            PRINT 'Product deleted successfully.';
    END TRY
    BEGIN CATCH
        PRINT 'Error: ' + ERROR_MESSAGE();
    END CATCH
END;
GO

CREATE PROCEDURE ViewAllCustomers
AS
BEGIN
    BEGIN TRY
        SELECT * FROM Customers;
        PRINT 'Customers listed successfully.';
    END TRY
    BEGIN CATCH
        PRINT 'Error: ' + ERROR_MESSAGE();
    END CATCH
END;
GO

CREATE FUNCTION GetOrderTotal(@OrderId INT)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @Total DECIMAL(10,2);
    
    SELECT @Total = SUM(p.Price * oi.Quantity)
    FROM OrderItems oi
    JOIN Products p ON oi.ProductId = p.Id
    WHERE oi.OrderId = @OrderId;

    RETURN ISNULL(@Total, 0);
END;
GO
