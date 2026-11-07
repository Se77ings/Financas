using Microsoft.Data.Sqlite;

namespace Financas.Desktop.Backups
{
    public static class SqliteBackup
    {
        /// <summary>
        /// Sem pool: uma conexão guardada manteria o arquivo preso e impediria movê-lo logo em seguida.
        /// </summary>
        public static string ConnectionString(string path, SqliteOpenMode mode = SqliteOpenMode.ReadWriteCreate)
        {
            return new SqliteConnectionStringBuilder { DataSource = path, Mode = mode, Pooling = false }.ToString();
        }

        /// <summary>
        /// O EF cria o banco em WAL: copiar só o .db deixaria de fora o que ainda está no -wal.
        /// </summary>
        public static void Copy(string sourcePath, string targetPath)
        {
            using var source = new SqliteConnection(ConnectionString(sourcePath, SqliteOpenMode.ReadOnly));
            using var target = new SqliteConnection(ConnectionString(targetPath));

            source.Open();
            source.BackupDatabase(target);
        }
    }
}
