-- DB Creation
CREATE DATABASE ProductDB;
GO

-- Sample user
CREATE LOGIN ProductDBUser
	WITH PASSWORD = 'hsRPW@mv641h';
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
		SELECT [ProductId] AS Id,
			[ProductName] AS Name,
			[ImgUri] AS ImageUri,
			[Price],
			[Description]
		FROM [dbo].[Product]
	ELSE
		SELECT [ProductId] AS Id,
			[ProductName] AS Name,
			[ImgUri] AS ImageUri,
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
(1,  N'Bezdrátová sluchátka SoundPro X1', N'/img/products/headphones_x1.png', 1299.00, N'Bezdrátová Bluetooth sluchátka s dlouhou výdrží baterie'),
(2,  N'Chytré hodinky FitWatch Active', N'/img/products/smartwatch_active.png', 2499.20, N'Chytré hodinky se sledováním sportovních aktivit'),
(3,  N'Bluetooth reproduktor BoomMini', N'/img/products/speaker_boommini.png', 999.00,  N'Kompaktní přenosný reproduktor s čistým zvukem'),
(4,  N'Powerbanka 20000 mAh PowerMax', N'/img/products/powerbank_20000.png', 899.50, NULL),
(5,  N'USB-C nabíječka FastCharge 65W', N'/img/products/charger_65w.png', 699.70,  N'Rychlonabíječka pro notebooky a telefony'),

(6,  N'Bezdrátová myš ErgoClick', N'/img/products/mouse_ergoclick.png', 499.23,  N'Ergonomická myš pro každodenní práci'),
(7,  N'Mechanická klávesnice KeyMaster', N'/img/products/keyboard_keymaster.png', 1899.15, N'Mechanická klávesnice s RGB podsvícením'),
(8,  N'27" monitor UltraView QHD', N'/img/products/monitor_27qhd.png', 6999.16, N'Monitor s QHD rozlišením a IPS panelem'),
(9,  N'Externí SSD 1TB SpeedDrive', N'/img/products/ssd_1tb.png', 2499.56, N'Rychlý externí SSD disk s USB-C'),
(10, N'Webkamera Full HD StreamCam', N'/img/products/webcam_fhd.png', 1199.82, NULL),

(11, N'Robotický vysavač CleanBot 3000', N'/img/products/vacuum_robot.png', 8999.93, N'Robotický vysavač s chytrou navigací'),
(12, N'Tyčový vysavač PowerClean', N'/img/products/vacuum_stick.png', 4299.55, N'Lehký a výkonný tyčový vysavač'),
(13, N'Kávovar EspressoPlus', N'/img/products/coffee_espresso.png', 5499.10, N'Espresso kávovar s tlakem 15 bar'),
(14, N'Rychlovarná konvice Steel 1.7L', N'/img/products/kettle_steel.png', 799.00,  N'Nerezová rychlovarná konvice'),
(15, N'Topinkovač ToastMaster', N'/img/products/toaster.png', 699.14, NULL),

(16, N'Fitness náramek MoveBand', N'/img/products/fitness_band.png', 899.18,  N'Fitness náramek s měřením tepu'),
(17, N'Podložka na jógu YogaFlex', N'/img/products/yoga_mat.png', 499.20, NULL),
(18, N'Nastavitelné činky 20 kg', N'/img/products/dumbbells_20kg.png', 1799.25, N'Sada činek pro domácí trénink'),
(19, N'Sportovní láhev 1 l HydroPlus', N'/img/products/bottle_1l.png', 299.93,  N'Odolná sportovní láhev na pití'),
(20, N'Běžecký pás HomeRun 100', N'/img/products/treadmill.png', 14999.99, N'Skládací běžecký pás pro domácí použití'),

(21, N'Kancelářská židle ComfortSeat', N'/img/products/chair_office.png', 3999.17, N'Ergonomická kancelářská židle'),
(22, N'Psací stůl WorkDesk 120 cm', N'/img/products/desk_120.png', 2999.22, N'Pracovní stůl s kovovou konstrukcí'),
(23, N'Stolní LED lampa BrightLight', N'/img/products/lamp_led.png', 599.10,  N'LED lampa s nastavitelným jasem'),
(24, N'Bezdrátová nabíječka QiPad', N'/img/products/wireless_charger.png', 499.00,  N'Bezdrátové nabíjení pro smartphony'),
(25, N'Prodlužovací kabel 5 zásuvek', N'/img/products/extension_5.png', 349.23,  N'Prodlužovací kabel s ochranou'),

(26, N'Chytrá žárovka SmartLight E27', N'/img/products/smart_bulb.png', 399.25,  N'Chytrá LED žárovka ovládaná aplikací'),
(27, N'Domácí meteostanice WeatherPro', N'/img/products/weather_station.png', 1299.16, NULL),
(28, N'Bezpečnostní kamera HomeCam', N'/img/products/security_camera.png', 1599.32, N'Vnitřní kamera s nočním viděním'),
(29, N'Router Wi-Fi 6 NetSpeed', N'/img/products/router_wifi6.png', 2199.87, N'Výkonný router s podporou Wi-Fi 6'),
(30, N'Síťový kabel CAT6 10 m', N'/img/products/cable_cat6.png', 199.15,  N'Kvalitní síťový kabel CAT6'),

(31, N'Batoh na notebook 15.6"', N'/img/products/backpack_15.png', 899.16, NULL),
(32, N'Cestovní kufr TravelCase M', N'/img/products/suitcase_m.png', 2499.77, N'Odolný kufr střední velikosti'),
(33, N'Bezdrátová sluchátka do uší AirBeats', N'/img/products/earbuds.png', 1499.72, NULL),
(34, N'Elektrický holicí strojek SmoothCut', N'/img/products/shaver.png', 1299.18, N'Holicí strojek s plovoucími hlavami'),
(35, N'Fén na vlasy DryFast 2200W', N'/img/products/hair_dryer.png', 899.95,  N'Výkonný fén s ionizací'),

(36, N'Kuchyňská váha SlimScale', N'/img/products/kitchen_scale.png', 399.21,  N'Digitální kuchyňská váha'),
(37, N'Sada nožů ChefSet 5 ks', N'/img/products/knife_set.png', 1499.34, N'Nerezové kuchyňské nože'),
(38, N'Pánev StoneCook 28 cm', N'/img/products/pan_28.png', 799.77, NULL),
(39, N'Elektrický gril GrillMaster', N'/img/products/grill_electric.png', 3499.00, N'Elektrický gril pro domácí použití'),
(40, N'Rýžovar RiceEasy', N'/img/products/rice_cooker.png', 1299.93, N'Rýžovar s automatickým vypnutím'),

(41, N'Čistička vzduchu AirPure', N'/img/products/air_purifier.png', 4999.45, N'Čistička vzduchu s HEPA filtrem'),
(42, N'Zvlhčovač vzduchu MistCare', N'/img/products/humidifier.png', 1999.82, N'Ultrazvukový zvlhčovač'),
(43, N'Elektrická koloběžka CityRide', N'/img/products/scooter.png', 12999.44, NULL),
(44, N'Autokamera DashCam Pro', N'/img/products/dashcam.png', 1799.11, N'Autokamera s Full HD záznamem'),
(45, N'Navigace GPS DriveNav', N'/img/products/gps_nav.png', 2999.05, N'Samostatná GPS navigace do auta');

GO


