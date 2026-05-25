using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financeiro.Infra.Migrations
{
    /// <inheritdoc />
    public partial class EffectiveDateMonthAsEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Month_New",
                table: "EffectiveDate",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(MonthTextToEnumConversionSql);

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM [EffectiveDate] WHERE [Month_New] IS NULL)
                BEGIN
                    RAISERROR (N'Existem registros em EffectiveDate.Month que não puderam ser convertidos para int (1-12). Verifique os dados.', 16, 1);
                END
            ");

            migrationBuilder.AlterColumn<int>(
                name: "Month_New",
                table: "EffectiveDate",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "Month",
                table: "EffectiveDate");

            migrationBuilder.RenameColumn(
                name: "Month_New",
                table: "EffectiveDate",
                newName: "Month");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Month_Old",
                table: "EffectiveDate",
                type: "varchar(100)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE [EffectiveDate]
                SET [Month_Old] = CASE [Month]
                    WHEN 1 THEN 'Jan'
                    WHEN 2 THEN 'Fev'
                    WHEN 3 THEN 'Mar'
                    WHEN 4 THEN 'Abr'
                    WHEN 5 THEN 'Mai'
                    WHEN 6 THEN 'Jun'
                    WHEN 7 THEN 'Jul'
                    WHEN 8 THEN 'Ago'
                    WHEN 9 THEN 'Set'
                    WHEN 10 THEN 'Out'
                    WHEN 11 THEN 'Nov'
                    WHEN 12 THEN 'Dez'
                    ELSE NULL
                END
            ");

            migrationBuilder.AlterColumn<string>(
                name: "Month_Old",
                table: "EffectiveDate",
                type: "varchar(100)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "Month",
                table: "EffectiveDate");

            migrationBuilder.RenameColumn(
                name: "Month_Old",
                table: "EffectiveDate",
                newName: "Month");
        }

        /// <summary>
        /// Converte Month (texto legado simplificado) para Month_New (int 1-12).
        /// </summary>
        private const string MonthTextToEnumConversionSql = @"
            UPDATE [EffectiveDate]
            SET [Month_New] = CASE LOWER(TRIM([Month]))
                WHEN 'jan' THEN 1
                WHEN 'fev' THEN 2
                WHEN 'mar' THEN 3
                WHEN 'abr' THEN 4
                WHEN 'mai' THEN 5
                WHEN 'jun' THEN 6
                WHEN 'jul' THEN 7
                WHEN 'ago' THEN 8
                WHEN 'set' THEN 9
                WHEN 'out' THEN 10
                WHEN 'nov' THEN 11
                WHEN 'dez' THEN 12
                ELSE NULL
            END
            ";
    }
}