CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "MenuTypes" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_MenuTypes" PRIMARY KEY,
    "Name" TEXT NULL
);

CREATE TABLE "Tables" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Tables" PRIMARY KEY,
    "Name" TEXT NULL,
    "NumberOfPeople" INTEGER NOT NULL,
    "IsOccupied" INTEGER NOT NULL
);

CREATE TABLE "MenuItems" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_MenuItems" PRIMARY KEY,
    "Name" TEXT NULL,
    "Description" TEXT NULL,
    "Price" TEXT NOT NULL,
    "MenuTypeId" TEXT NOT NULL,
    CONSTRAINT "FK_MenuItems_MenuTypes_MenuTypeId" FOREIGN KEY ("MenuTypeId") REFERENCES "MenuTypes" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Orders" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_Orders" PRIMARY KEY,
    "OrderDateTime" TEXT NOT NULL,
    "TotalAmount" TEXT NOT NULL,
    "OrderStatus" INTEGER NOT NULL,
    "TableId" TEXT NOT NULL,
    CONSTRAINT "FK_Orders_Tables_TableId" FOREIGN KEY ("TableId") REFERENCES "Tables" ("Id") ON DELETE CASCADE
);

CREATE TABLE "OrderItems" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_OrderItems" PRIMARY KEY,
    "Price" TEXT NOT NULL,
    "Quantity" INTEGER NOT NULL,
    "SpecialInstructions" TEXT NULL,
    "OrderId" TEXT NOT NULL,
    "MenuItemId" TEXT NOT NULL,
    CONSTRAINT "FK_OrderItems_MenuItems_MenuItemId" FOREIGN KEY ("MenuItemId") REFERENCES "MenuItems" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_OrderItems_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "Orders" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_MenuItems_MenuTypeId" ON "MenuItems" ("MenuTypeId");

CREATE INDEX "IX_OrderItems_MenuItemId" ON "OrderItems" ("MenuItemId");

CREATE INDEX "IX_OrderItems_OrderId" ON "OrderItems" ("OrderId");

CREATE INDEX "IX_Orders_TableId" ON "Orders" ("TableId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241031153159_InitialCreate', '8.0.10');

COMMIT;

