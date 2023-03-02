﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MySqlConnector;
using Smartstore.Data.Providers;

namespace Smartstore.Data.MySql
{
    public class MySqlDataProvider : DataProvider
    {
        public MySqlDataProvider(DatabaseFacade database)
            : base(database)
        {
        }

        public override DbSystemType ProviderType => DbSystemType.MySql;

        public override string ProviderFriendlyName
        {
            get => "MySQL " + Database.ExecuteScalarRaw<string>("SELECT @@version");
        }

        public override DataProviderFeatures Features
            => DataProviderFeatures.Shrink
            | DataProviderFeatures.ReIndex
            | DataProviderFeatures.ComputeSize
            | DataProviderFeatures.AccessIncrement
            | DataProviderFeatures.ExecuteSqlScript
            | DataProviderFeatures.StoredProcedures
            | DataProviderFeatures.ReadSequential;

        public override DbParameter CreateParameter()
            => new MySqlParameter();

        public override bool MARSEnabled => false;

        public override string EncloseIdentifier(string identifier)
        {
            Guard.NotEmpty(identifier, nameof(identifier));
            return identifier.EnsureStartsWith('`').EnsureEndsWith('`');
        }

        public override string ApplyPaging(string sql, int skip, int take)
        {
            Guard.NotNegative(skip);
            Guard.NotNegative(take);

            return $@"{sql}
LIMIT {take} OFFSET {skip}";
        }

        protected override ValueTask<bool> HasDatabaseCore(string databaseName, bool async)
        {
            FormattableString sql = $"SELECT SCHEMA_NAME FROM information_schema.schemata WHERE SCHEMA_NAME = {databaseName}";
            return async
                ? Database.ExecuteQueryInterpolatedAsync<string>(sql).AnyAsync()
                : ValueTask.FromResult(Database.ExecuteQueryInterpolated<string>(sql).Any());
        }

        protected override ValueTask<bool> HasTableCore(string tableName, bool async)
        {
            FormattableString sql = $@"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA = {DatabaseName} AND TABLE_NAME = {tableName}";
            return async
                ? Database.ExecuteQueryInterpolatedAsync<string>(sql).AnyAsync()
                : ValueTask.FromResult(Database.ExecuteQueryInterpolated<string>(sql).Any());
        }

        protected override ValueTask<bool> HasColumnCore(string tableName, string columnName, bool async)
        {
            FormattableString sql = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = {DatabaseName} AND TABLE_NAME = {tableName} And COLUMN_NAME = {columnName}";
            return async
                ? Database.ExecuteQueryInterpolatedAsync<string>(sql).AnyAsync()
                : ValueTask.FromResult(Database.ExecuteQueryInterpolated<string>(sql).Any());
        }

        protected override ValueTask<string[]> GetTableNamesCore(bool async)
        {
            FormattableString sql = $"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_SCHEMA = {DatabaseName}";
            return async
                ? Database.ExecuteQueryInterpolatedAsync<string>(sql).AsyncToArray()
                : ValueTask.FromResult(Database.ExecuteQueryInterpolated<string>(sql).ToArray());
        }

        protected override Task<int> TruncateTableCore(string tableName, bool async)
        {
            var sql = $"TRUNCATE TABLE `{tableName}`";
            return async
                ? Database.ExecuteSqlRawAsync(sql)
                : Task.FromResult(Database.ExecuteSqlRaw(sql));
        }

        protected override async Task<int> InsertIntoCore(string sql, bool async, params object[] parameters)
        {
            sql += "; SELECT LAST_INSERT_ID();";
            return async
                ? (await Database.ExecuteQueryRawAsync<decimal>(sql, parameters).FirstOrDefaultAsync()).Convert<int>()
                : Database.ExecuteQueryRaw<decimal>(sql, parameters).FirstOrDefault().Convert<int>();
        }

        public override bool IsTransientException(Exception ex)
            => ex is MySqlException mySqlException
                ? mySqlException.IsTransient
                : ex is TimeoutException;

        public override bool IsUniquenessViolationException(DbUpdateException updateException)
        {
            if (updateException?.InnerException is MySqlException ex)
            {
                switch (ex.ErrorCode)
                {
                    case MySqlErrorCode.DuplicateEntryWithKeyName:
                    case MySqlErrorCode.DuplicateKey:
                    case MySqlErrorCode.DuplicateKeyEntry:
                    case MySqlErrorCode.DuplicateKeyName:
                    case MySqlErrorCode.DuplicateUnique:
                    case MySqlErrorCode.ForeignDuplicateKey:
                    case MySqlErrorCode.DuplicateEntryAutoIncrementCase:
                    case MySqlErrorCode.NonUnique:
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }
        
        protected override Task<long> GetDatabaseSizeCore(bool async)
        {
            var sql = $@"SELECT SUM(data_length + index_length) AS 'size'
                FROM information_schema.TABLES
                WHERE table_schema = '{DatabaseName}'";
            return async
                ? Database.ExecuteScalarRawAsync<long>(sql)
                : Task.FromResult(Database.ExecuteScalarRaw<long>(sql));
        }

        protected override Task<int> ShrinkDatabaseCore(bool async, CancellationToken cancelToken = default)
        {
            return async
                ? ReIndexTablesAsync(cancelToken)
                : Task.FromResult(ReIndexTables());
        }

        protected override async Task<int> ReIndexTablesCore(bool async, CancellationToken cancelToken = default)
        {
            var sqlTables = $"SHOW TABLES FROM `{DatabaseName}`";
            var tables = async 
                ? await Database.ExecuteQueryRawAsync<string>(sqlTables, cancelToken).ToListAsync(cancelToken)
                : Database.ExecuteQueryRaw<string>(sqlTables).ToList();

            if (tables.Count > 0)
            {
                var sql = $"OPTIMIZE TABLE `{string.Join("`, `", tables)}`";
                return async 
                    ? await Database.ExecuteSqlRawAsync(sql, cancelToken)
                    : Database.ExecuteSqlRaw(sql);
            }

            return 0;
        }

        protected override async Task<int?> GetTableIncrementCore(string tableName, bool async)
        {
            FormattableString sql = $"SELECT AUTO_INCREMENT FROM information_schema.TABLES WHERE TABLE_SCHEMA = {DatabaseName} AND TABLE_NAME = {tableName}";
            return async
               ? (await Database.ExecuteScalarInterpolatedAsync<ulong>(sql)).Convert<int?>()
               : Database.ExecuteScalarInterpolated<ulong>(sql).Convert<int?>();
        }

        protected override Task SetTableIncrementCore(string tableName, int ident, bool async)
        {
            var sql = $"ALTER TABLE `{tableName}` AUTO_INCREMENT = {ident}";
            return async
               ? Database.ExecuteSqlRawAsync(sql)
               : Task.FromResult(Database.ExecuteSqlRaw(sql));
        }

        protected override IList<string> SplitSqlScript(string sqlScript)
        {
            var commands = new List<string>();
            var lines = sqlScript.GetLines(true);
            var delimiter = ";";
            var command = string.Empty;

            foreach (var line in lines)
            {
                // Ignore comments
                if (line.StartsWith("--") || line.StartsWith("#"))
                {
                    continue;
                }

                // In MySQL scripts, you can change the delimiter using the DELIMITER statement.
                // To handle this scenario, we need to track the current delimiter
                // and change it whenever we encounter a DELIMITER statement
                if (line.StartsWithNoCase("DELIMITER"))
                {
                    delimiter = line.Split(' ')[1].Trim();
                    continue;
                }

                if (!line.EndsWithNoCase(delimiter))
                {
                    command += line + Environment.NewLine;
                }
                else
                {
                    command += line[..^delimiter.Length];
                    commands.Add(command);
                    command = string.Empty;
                }
            }

            return commands;
        }

        protected override Stream OpenBlobStreamCore(string tableName, string blobColumnName, string pkColumnName, object pkColumnValue)
        {
            return new SqlBlobStream(this, tableName, blobColumnName, pkColumnName, pkColumnValue);
        }
    }
}
