using ACME.LearningCenterPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using Microsoft.EntityFrameworkCore;

namespace RentalPeAPI.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
/// Provides extension methods for ModelBuilder to apply naming conventions.
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Applies snake_case naming convention to the model.
    /// </summary>
    /// <param name="builder">The model builder.</param>
    public static void UseSnakeCaseNamingConvention(this ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName();
            if (!string.IsNullOrEmpty(tableName))
                entity.SetTableName(tableName.ToPlural().ToSnakeCase());

            foreach (var property in entity.GetProperties())
                property.SetColumnName(property.GetColumnName().ToSnakeCase());
            foreach (var key in entity.GetKeys())
            {
                var keyName = key.GetName();
                if (!string.IsNullOrEmpty(keyName))
                    key.SetName(keyName.ToSnakeCase());
            }

            foreach (var foreignKey in entity.GetForeignKeys())
            {
                var fkName = foreignKey.GetConstraintName();
                if (!string.IsNullOrEmpty(fkName))
                    foreignKey.SetConstraintName(fkName.ToSnakeCase());
            }

            foreach (var index in entity.GetIndexes())
            {
                var idx = index.GetDatabaseName();
                if (!string.IsNullOrEmpty(idx))
                    index.SetDatabaseName(idx.ToSnakeCase());
            }
        }
    }
}