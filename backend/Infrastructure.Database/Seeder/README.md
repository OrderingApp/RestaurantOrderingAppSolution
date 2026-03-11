# Seeding system

## Overview

Database seeding is handled by typed C# seeders that read from JSON files.
Excel (`SeedData.xlsx`) was the original data source and is now a **migration tool only**.

---

## Folder structure

```
Infrastructure.Database/
  Seeder/
    SeedData/          ← JSON files = source of truth for seed data
    SeedModels/        ← C# models used to deserialize each JSON file
    Seeders/           ← Seeders that write data to the database
    Tools/             ← Dev-only helpers (Excel exporter)
    SeedDataReader.cs  ← Reads and deserializes JSON files

tools/
  exporter-runner/     ← Dev-only CLI: runs ExcelToJsonExporter
  seeder-runner/       ← Dev-only CLI: runs DatabaseSeeder against a test SQLite DB
```

---

## How seeding works

1. `SeedDataReader` reads a JSON file from `Seeder/SeedData/` and deserializes it
   into the matching `SeedModel`.
2. Each seeder (e.g. `IngredientSeeder`) checks **if its table is empty**.
   If it is, it loads the JSON, maps models to domain entities and saves them.
3. `DatabaseSeeder` orchestrates all seeders in dependency order
   (parent entities before children, base entities before relation tables).
4. `ServiceCollectionExtensions.AddDatabaseSeeders()` registers everything in DI.

> **Seeders operate in "if empty" mode.**
> They will not overwrite or update existing rows.
> See [update behaviour](#update-behaviour) below.

---

## JSON files (Seeder/SeedData)

| File | Seeder |
|------|--------|
| `menu-categories.json` | `MenuCategorySeeder` |
| `sub-categories.json` | `SubCategorySeeder` |
| `ingredient-categories.json` | `IngredientCategorySeeder` |
| `ingredients.json` | `IngredientSeeder` |
| `tags.json` | `TagSeeder` |
| `allergens.json` | `AllergenSeeder` |
| `areas.json` | `AreaSeeder` |
| `tables.json` | `TableSeeder` |
| `menu-items.json` | `MenuItemSeeder` |
| `ingredient-tag-rels.json` | `IngredientTagRelSeeder` |
| `ingredient-allergen-rels.json` | `IngredientAllergenRelSeeder` |
| `menu-item-ingredient-rels.json` | `MenuItemIngredientRelSeeder` |

**To edit seed data: edit the JSON files directly.**
Do not edit the Excel file and expect changes to propagate automatically.

---

## Generating JSON from Excel (one-time migration)

Use the dev-only `ExcelToJsonExporter`:

```
dotnet run --project tools/exporter-runner/ExporterRunner.csproj \
  -- "<path-to-SeedData.xlsx>" "<path-to-Infrastructure.Database>"
```

The exporter:
- reads each sheet from the Excel file
- maps rows to typed SeedModels (with FK validation and defaults)
- writes JSON files to `Seeder/SeedData/`
- prints an export report with skipped rows and defaulted values

After exporting, **verify the JSON files** before using them to seed.
The exporter reports which rows were skipped and which fields got default values.

---

## Running a dev seed test

Use the dev-only seeder runner to seed a local SQLite database:

```
# Base entities only
dotnet run --project tools/seeder-runner/SeederRunner.csproj \
  -- "my-test.db" "<path-to-Infrastructure.Database>"

# Base entities + relational tables
dotnet run --project tools/seeder-runner/SeederRunner.csproj \
  -- "my-test.db" "<path-to-Infrastructure.Database>" --with-relations
```

The runner:
- creates the SQLite schema using `EnsureCreated()` (dev only; use migrations in production)
- runs each seeder and prints a per-seeder record count
- with `--with-relations`: also runs `IngredientTagRelSeeder` and `MenuItemIngredientRelSeeder`
  with FK validation, reporting added/skipped counts per relation

---

## Connecting seeding to the application

`DatabaseSeeder` is **not** called automatically on startup.
To enable it, call it in `Program.cs` after `app.Build()`:

```csharp
using var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
await seeder.SeedAsync();
```

Make sure `AddDatabaseSeeders()` is called in the DI setup first.

---

## Update behaviour

Seeders check `if (table.Any()) return;` before doing anything.
**Changing a JSON file does NOT update an already-seeded database.**

This is intentional for production safety. If you need to re-seed:
- drop the database and re-seed from scratch, or
- write a targeted migration or manual update script.

A future improvement would be a sync/upsert seeder for dictionary/reference data
(areas, tags, allergens, ingredient categories) that updates existing rows
and adds missing ones. This has not been implemented yet.

---

## Excel / legacy seeder

`ExcelSeeder` (project root, `Infrastructure.Database/ExcelSeeder.cs`) is the **old approach**.
It reads directly from the Excel file and writes to the DB.

**Do not run it alongside `DatabaseSeeder`.**

It can be deleted once:
- all JSON files in `Seeder/SeedData/` have been verified,
- `DatabaseSeeder` has been successfully run on the production database (or the DB has been reset),
- no code paths in the application call `ExcelSeeder`.

---

## Source of truth

| What | Where |
|------|-------|
| Seed data | `Infrastructure.Database/Seeder/SeedData/*.json` |
| Seed models | `Infrastructure.Database/Seeder/SeedModels/` |
| Seeding logic | `Infrastructure.Database/Seeder/Seeders/` |
| Dev tools | `tools/exporter-runner/`, `tools/seeder-runner/` |
| Legacy (to be removed) | `Infrastructure.Database/ExcelSeeder.cs` |
