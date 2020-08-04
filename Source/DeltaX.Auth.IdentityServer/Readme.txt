
# Generar codigos de Migracion:
dotnet ef migrations add Application -c ApplicationDbContext -o Data/Migrations/ApplicationDb

# Generar Script de Migracion
dotnet ef migrations script -c ApplicationDbContext -o Data/Migrations/ApplicationDb.sql


# Subir el esquema a Database
dotnet ef database update -c ApplicationDbContext 