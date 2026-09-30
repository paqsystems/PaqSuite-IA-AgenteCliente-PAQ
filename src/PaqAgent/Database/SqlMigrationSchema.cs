namespace PaqAgent.Database;

public static class SqlMigrationSchema
{
    public const string EnsureTableSql = """
        IF OBJECT_ID(N'dbo.paq_sp_migrations', N'U') IS NULL
        BEGIN
            CREATE TABLE dbo.paq_sp_migrations (
                id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_paq_sp_migrations PRIMARY KEY,
                migration NVARCHAR(150) NOT NULL,
                batch INT NOT NULL CONSTRAINT DF_paq_sp_migrations_batch DEFAULT (1),
                applied_at DATETIME2(3) NOT NULL,
                checksum_sha256 CHAR(64) NOT NULL,
                CONSTRAINT UQ_paq_sp_migrations_migration_checksum UNIQUE (migration, checksum_sha256)
            );
        END
        """;
}
