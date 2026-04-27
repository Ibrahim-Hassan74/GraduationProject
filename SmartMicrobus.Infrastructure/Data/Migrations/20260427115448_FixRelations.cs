using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMicrobus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.foreign_keys 
                    WHERE name = 'FK_Routes_Stations_StationId'
                )
                BEGIN
                    ALTER TABLE Routes DROP CONSTRAINT FK_Routes_Stations_StationId
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.indexes 
                    WHERE name = 'IX_Routes_StationId'
                )
                BEGIN
                    DROP INDEX IX_Routes_StationId ON Routes
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (
                    SELECT 1 FROM sys.columns 
                    WHERE Name = 'StationId' AND Object_ID = Object_ID('Routes')
                )
                BEGIN
                    ALTER TABLE Routes DROP COLUMN StationId
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.columns 
                    WHERE Name = 'StationId' AND Object_ID = Object_ID('Routes')
                )
                BEGIN
                    ALTER TABLE Routes ADD StationId UNIQUEIDENTIFIER NULL
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes 
                    WHERE name = 'IX_Routes_StationId'
                )
                BEGIN
                    CREATE INDEX IX_Routes_StationId ON Routes (StationId)
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.foreign_keys 
                    WHERE name = 'FK_Routes_Stations_StationId'
                )
                BEGIN
                    ALTER TABLE Routes
                    ADD CONSTRAINT FK_Routes_Stations_StationId
                    FOREIGN KEY (StationId) REFERENCES Stations(Id)
                END
            ");
        }
    }
}
