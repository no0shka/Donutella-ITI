USE DonutellaDb;
GO

-- ============ Categories ============
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = N'Dessert')
    INSERT INTO Categories (Name, Description, IsDeleted)
    VALUES (N'Dessert', N'Cookies, cupcakes and sweet treats', 0);

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = N'Ice Cream')
    INSERT INTO Categories (Name, Description, IsDeleted)
    VALUES (N'Ice Cream', N'Creamy ice cream scoops in many flavors', 0);

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = N'Donut')
    INSERT INTO Categories (Name, Description, IsDeleted)
    VALUES (N'Donut', N'Fresh glazed and filled donuts', 0);
GO

DECLARE @dessertId INT = (SELECT Id FROM Categories WHERE Name = N'Dessert');
DECLARE @iceCreamId INT = (SELECT Id FROM Categories WHERE Name = N'Ice Cream');
DECLARE @donutId INT = (SELECT Id FROM Categories WHERE Name = N'Donut');

-- ============ SubCategories ============
IF NOT EXISTS (SELECT 1 FROM SubCategories WHERE Name = N'Cookies')
    INSERT INTO SubCategories (Name, Description, CategoryId)
    VALUES (N'Cookies', N'Buttery cookies', @dessertId);

IF NOT EXISTS (SELECT 1 FROM SubCategories WHERE Name = N'Cupcakes')
    INSERT INTO SubCategories (Name, Description, CategoryId)
    VALUES (N'Cupcakes', N'Soft cupcakes', @dessertId);

IF NOT EXISTS (SELECT 1 FROM SubCategories WHERE Name = N'Scoops')
    INSERT INTO SubCategories (Name, Description, CategoryId)
    VALUES (N'Scoops', N'Ice cream scoops', @iceCreamId);

IF NOT EXISTS (SELECT 1 FROM SubCategories WHERE Name = N'Donuts')
    INSERT INTO SubCategories (Name, Description, CategoryId)
    VALUES (N'Donuts', N'Classic donuts', @donutId);
GO

DECLARE @cookiesId INT = (SELECT Id FROM SubCategories WHERE Name = N'Cookies');
DECLARE @cupcakesId INT = (SELECT Id FROM SubCategories WHERE Name = N'Cupcakes');
DECLARE @scoopsId INT = (SELECT Id FROM SubCategories WHERE Name = N'Scoops');
DECLARE @donutsId INT = (SELECT Id FROM SubCategories WHERE Name = N'Donuts');
DECLARE @dessertId2 INT = (SELECT Id FROM Categories WHERE Name = N'Dessert');
DECLARE @iceCreamId2 INT = (SELECT Id FROM Categories WHERE Name = N'Ice Cream');
DECLARE @donutId2 INT = (SELECT Id FROM Categories WHERE Name = N'Donut');

-- ============ Products (Dessert / Cookies) ============
IF NOT EXISTS (SELECT 1 FROM Products WHERE Name = N'Butter Cookies')
    INSERT INTO Products (Name, Description, Price, Image, CategoryId, IsDeleted, Stock, SubCategoryId)
    VALUES (N'Butter Cookies', N'Crisp golden butter cookies baked fresh daily.', 45,
            N'/Images/Products/Butter Cookies.jpg', @dessertId2, 0, 50, @cookiesId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE Name = N'Chocolate Cookies')
    INSERT INTO Products (Name, Description, Price, Image, CategoryId, IsDeleted, Stock, SubCategoryId)
    VALUES (N'Chocolate Cookies', N'Rich chocolate cookies with a soft chewy center.', 55,
            N'/Images/Products/chocolate-cookies.jpg', @dessertId2, 0, 50, @cookiesId);

-- ============ Products (Dessert / Cupcakes) ============
IF NOT EXISTS (SELECT 1 FROM Products WHERE Name = N'Chocolate Cupcake')
    INSERT INTO Products (Name, Description, Price, Image, CategoryId, IsDeleted, Stock, SubCategoryId)
    VALUES (N'Chocolate Cupcake', N'Moist chocolate cupcake topped with creamy frosting.', 65,
            N'/Images/Products/Chocolate Cupcake.jpg', @dessertId2, 0, 50, @cupcakesId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE Name = N'Red Velvet Cupcake')
    INSERT INTO Products (Name, Description, Price, Image, CategoryId, IsDeleted, Stock, SubCategoryId)
    VALUES (N'Red Velvet Cupcake', N'Classic red velvet cupcake with smooth cream cheese frosting.', 70,
            N'/Images/Products/red-velvet-cupcake.jpg', @dessertId2, 0, 50, @cupcakesId);

-- ============ Products (Ice Cream / Scoops) ============
IF NOT EXISTS (SELECT 1 FROM Products WHERE Name = N'Chocolate Ice Cream')
    INSERT INTO Products (Name, Description, Price, Image, CategoryId, IsDeleted, Stock, SubCategoryId)
    VALUES (N'Chocolate Ice Cream', N'Creamy Belgian chocolate ice cream.', 50,
            N'/Images/Products/Chocolate Ice Cream.jpg', @iceCreamId2, 0, 50, @scoopsId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE Name = N'Chocolate Vanilla Ice Cream')
    INSERT INTO Products (Name, Description, Price, Image, CategoryId, IsDeleted, Stock, SubCategoryId)
    VALUES (N'Chocolate Vanilla Ice Cream', N'Double delight mix of chocolate and vanilla.', 55,
            N'/Images/Products/Chocolate_and_vanilla Ice Cream.jpg', @iceCreamId2, 0, 50, @scoopsId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE Name = N'Mango Ice Cream')
    INSERT INTO Products (Name, Description, Price, Image, CategoryId, IsDeleted, Stock, SubCategoryId)
    VALUES (N'Mango Ice Cream', N'Fresh mango ice cream made with real fruit.', 50,
            N'/Images/Products/mango-ice-cream.jpg', @iceCreamId2, 0, 50, @scoopsId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE Name = N'Pistachio Ice Cream')
    INSERT INTO Products (Name, Description, Price, Image, CategoryId, IsDeleted, Stock, SubCategoryId)
    VALUES (N'Pistachio Ice Cream', N'Premium pistachio ice cream with roasted nuts.', 75,
            N'/Images/Products/pistachio-ice-cream.jpg', @iceCreamId2, 0, 50, @scoopsId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE Name = N'Vanilla Ice Cream')
    INSERT INTO Products (Name, Description, Price, Image, CategoryId, IsDeleted, Stock, SubCategoryId)
    VALUES (N'Vanilla Ice Cream', N'Classic Madagascar vanilla ice cream.', 45,
            N'/Images/Products/Vanilla Ice Cream.jpg', @iceCreamId2, 0, 50, @scoopsId);

-- ============ Products (Donut / Donuts) ============
IF NOT EXISTS (SELECT 1 FROM Products WHERE Name = N'Chocolate Donut')
    INSERT INTO Products (Name, Description, Price, Image, CategoryId, IsDeleted, Stock, SubCategoryId)
    VALUES (N'Chocolate Donut', N'Fluffy donut coated with rich chocolate glaze.', 35,
            N'/Images/Products/chocolate-donut.jpg', @donutId2, 0, 50, @donutsId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE Name = N'Glazed Donut')
    INSERT INTO Products (Name, Description, Price, Image, CategoryId, IsDeleted, Stock, SubCategoryId)
    VALUES (N'Glazed Donut', N'Our signature light glazed donut.', 30,
            N'/Images/Products/glazed-donut.jpg', @donutId2, 0, 50, @donutsId);

IF NOT EXISTS (SELECT 1 FROM Products WHERE Name = N'Strawberry Donut')
    INSERT INTO Products (Name, Description, Price, Image, CategoryId, IsDeleted, Stock, SubCategoryId)
    VALUES (N'Strawberry Donut', N'Sweet strawberry glazed donut with sprinkles.', 40,
            N'/Images/Products/Strawberry Donut.jpg', @donutId2, 0, 50, @donutsId);
GO

-- ============ Extra product images ============
DECLARE @img TABLE (ProductName NVARCHAR(100), ImageUrl NVARCHAR(500));

INSERT INTO @img VALUES
    (N'Butter Cookies', N'/Images/Products/Butter Cookies1.jpg'),
    (N'Chocolate Cookies', N'/Images/Products/Chocolate Cookies11.jpg'),
    (N'Chocolate Cupcake', N'/Images/Products/Chocolate Cupcake1.jpg'),
    (N'Red Velvet Cupcake', N'/Images/Products/red-velvet-cupcake1.jpg'),
    (N'Mango Ice Cream', N'/Images/Products/mango-ice-cream1.jpg'),
    (N'Mango Ice Cream', N'/Images/Products/MangoIce Cream.jpg'),
    (N'Vanilla Ice Cream', N'/Images/Products/Vanilla Ice Cream1.jpg'),
    (N'Chocolate Donut', N'/Images/Products/chocolate-donut1.jpg'),
    (N'Glazed Donut', N'/Images/Products/glazed-donut1.jpg'),
    (N'Glazed Donut', N'/Images/Products/glazed-donut11.jpg'),
    (N'Strawberry Donut', N'/Images/Products/Strawberry Donut11.jpg'),
    (N'Strawberry Donut', N'/Images/Products/Strawberry Donut111.jpg');

INSERT INTO ProductImages (ImageUrl, ProductId)
SELECT i.ImageUrl, p.Id
FROM @img i
JOIN Products p ON p.Name = i.ProductName
WHERE NOT EXISTS (
    SELECT 1 FROM ProductImages pi
    WHERE pi.ProductId = p.Id AND pi.ImageUrl = i.ImageUrl
);
GO
