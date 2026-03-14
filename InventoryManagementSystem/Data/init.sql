-- Database Initialization Script for Inventory Management System (PostgreSQL)

-- Users Table
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" SERIAL PRIMARY KEY,
    "Username" VARCHAR(50) NOT NULL UNIQUE,
    "PasswordHash" TEXT NOT NULL
);

-- Categories Table
CREATE TABLE IF NOT EXISTS "Categories" (
    "Id" SERIAL PRIMARY KEY,
    "CategoryName" VARCHAR(100) NOT NULL,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT (NOW() AT TIME ZONE 'utc'),
    "UserId" INTEGER NOT NULL,
    CONSTRAINT "FK_Categories_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

-- Products Table
CREATE TABLE IF NOT EXISTS "Products" (
    "Id" SERIAL PRIMARY KEY,
    "ProductName" VARCHAR(200) NOT NULL,
    "SKU" VARCHAR(50) NOT NULL,
    "CategoryId" INTEGER NOT NULL,
    "PurchasePrice" DECIMAL(18,2) NOT NULL,
    "SellingPrice" DECIMAL(18,2) NOT NULL,
    "Quantity" INTEGER NOT NULL DEFAULT 0,
    "Description" VARCHAR(500),
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT (NOW() AT TIME ZONE 'utc'),
    "UserId" INTEGER NOT NULL,
    CONSTRAINT "FK_Products_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "Categories" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_Products_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

-- Unique index for SKU per user
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Products_UserId_SKU" ON "Products" ("UserId", "SKU");

-- Orders Table
CREATE TABLE IF NOT EXISTS "Orders" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INTEGER NOT NULL,
    "ProductId" INTEGER NOT NULL,
    "Quantity" INTEGER NOT NULL,
    "UnitSellingPrice" DECIMAL(18,2) NOT NULL,
    "UnitPurchasePrice" DECIMAL(18,2) NOT NULL,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE DEFAULT (NOW() AT TIME ZONE 'utc'),
    CONSTRAINT "FK_Orders_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Orders_Products_ProductId" FOREIGN KEY ("ProductId") REFERENCES "Products" ("Id") ON DELETE RESTRICT
);

-- Seed Initial Admin User
-- PasswordHash is for 'password'
INSERT INTO "Users" ("Id", "Username", "PasswordHash")
VALUES (1, 'admin', '$2b$10$S5M9J1T4ERzGUwjPPurRKuCEJm5yoBze5thgw9/MqwZtNwLrAtUIi')
ON CONFLICT ("Id") DO NOTHING;

-- Reset PK sequence for Users if needed
SELECT setval(pg_get_serial_sequence('"Users"', 'Id'), COALESCE(MAX("Id"), 1)) FROM "Users";
