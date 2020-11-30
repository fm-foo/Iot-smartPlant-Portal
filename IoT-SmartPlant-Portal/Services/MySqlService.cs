using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace IoT_SmartPlant_Portal.Services {

    public interface IMySqlService {
        /// <summary>
        /// Closes a connection if open
        /// </summary>
        void EnsureConnectionClosed();

        /// <summary>
        /// Executes the scalar command(preferably used to retrieve the last inserted id)
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        Task<int> ExecuteScalar(string commandText, Dictionary<string, object> parameters);

        /// <summary>
        /// Executes a non-query MySQL statement
        /// </summary>
        /// <param name="commandText">The MySQL query to execute</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <returns>The count of records affected by the MySQL statement</returns>
        Task<int> ExecuteAsync(string commandText, Dictionary<string, object> parameters);
        /// <summary>
        /// Executes a non-query MySQL statement
        /// </summary>
        /// <param name="commandText">The MySQL query to execute</param>
        /// <returns>The count of records affected by the MySQL statement</returns>
        Task<int> ExecuteAsync(string commandText);
        /// <summary>
        /// Executes a MySQL query that returns a single scalar value as the result.
        /// </summary>
        /// <param name="commandText">The MySQL query to execute</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <returns></returns>
        Task<object> QueryValueAsync(string commandText, Dictionary<string, object> parameters);

        /// <summary>
        /// Executes a SQL query that returns a list of rows as the result.
        /// </summary>
        /// <param name="commandText">The MySQL query to execute</param>
        /// <param name="parameters">Parameters to pass to the MySQL query</param>
        /// <returns>A list of a Dictionary of Key, values pairs representing the
        /// ColumnName and corresponding value</returns>
        Task<List<Dictionary<string, string>>> QueryAsync(string commandText, Dictionary<string, object> parameters);

        void Dispose();

        /// <summary>
        /// Helper method to return query a string value
        /// </summary>
        /// <param name="commandText">The MySQL query to execute</param>
        /// <param name="parameters">Parameters to pass to the MySQL query</param>
        /// <returns>The string value resulting from the query</returns>
        Task<string> GetStrValueAsync(string commandText, Dictionary<string, object> parameters);

        /// <summary>
        /// Executes a SQL query that returns a list of rows as the result.
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        Task<List<Dictionary<string, string>>> QueryAsync(string commandText);

        /// <summary>
        ///
        /// </summary>
        void StartTransaction();

        /// <summary>
        ///
        /// </summary>
        void Commit();

        /// <summary>
        ///
        /// </summary>
        void Rollback();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsTransactionActive();
    }

    public class MySqlService : IMySqlService {
        private readonly MySqlConnection _connection;
        private MySqlTransaction sqlTransaction;

        public MySqlService(string connectionString) {
            _connection = new MySqlConnection(connectionString);

        }

        public void StartTransaction() {
            EnsureConnectionOpen();
            sqlTransaction = _connection.BeginTransaction();
        }

        public void Commit() {
            EnsureConnectionOpen();
            sqlTransaction.Commit();
            EnsureConnectionClosed();
        }

        public void Rollback() {
            EnsureConnectionOpen();
            sqlTransaction.Rollback();
            EnsureConnectionClosed();
        }

        public bool IsTransactionActive() {
            if (sqlTransaction != null) {
                return true;
            }
            return false;
        }

        public async Task<int> ExecuteAsync(string commandText, Dictionary<string, object> parameters) {
            int result = 0;

            if (String.IsNullOrEmpty(commandText)) {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                result = await command.ExecuteNonQueryAsync();
            } catch (Exception ex) {
                throw ex;
            } finally {
                if (!IsTransactionActive()) {
                    EnsureConnectionClosed();
                }
            }
            return result;
        }

        public async Task<int> ExecuteAsync(string commandText) {
            int result = 0;

            if (String.IsNullOrEmpty(commandText)) {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText);
                result = await command.ExecuteNonQueryAsync();
            } catch (Exception ex) {
                throw ex;
            } finally {
                if (!IsTransactionActive()) {
                    EnsureConnectionClosed();
                }
            }
            return result;
        }

        public async Task<object> QueryValueAsync(string commandText, Dictionary<string, object> parameters) {
            object result = null;

            if (String.IsNullOrEmpty(commandText)) {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                result = await command.ExecuteScalarAsync();
            } catch (Exception ex) {
                throw ex;
            } finally {
                if (!IsTransactionActive()) {
                    EnsureConnectionClosed();
                }
            }
            return result;
        }

        public async Task<List<Dictionary<string, string>>> QueryAsync(string commandText, Dictionary<string, object> parameters) {
            List<Dictionary<string, string>> rows = null;
            if (String.IsNullOrEmpty(commandText)) {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText, parameters);
                using (MySqlDataReader reader = command.ExecuteReader()) {
                    rows = new List<Dictionary<string, string>>();
                    while (await reader.ReadAsync()) {
                        var row = new Dictionary<string, string>();
                        for (var i = 0; i < reader.FieldCount; i++) {
                            var columnName = reader.GetName(i);
                            var columnValue = reader.IsDBNull(i) ? null : reader.GetString(i);
                            row.Add(columnName, columnValue);
                        }
                        rows.Add(row);
                    }
                }
            } catch (Exception ex) {
                throw ex;
            } finally {
                if (!IsTransactionActive()) {
                    EnsureConnectionClosed();
                }
            }
            return rows;
        }

        public async Task<List<Dictionary<string, string>>> QueryAsync(string commandText) {
            List<Dictionary<string, string>> rows = null;
            if (String.IsNullOrEmpty(commandText)) {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try {
                EnsureConnectionOpen();
                var command = CreateCommand(commandText);
                using (MySqlDataReader reader = command.ExecuteReader()) {
                    rows = new List<Dictionary<string, string>>();
                    while (await reader.ReadAsync()) {
                        var row = new Dictionary<string, string>();
                        for (var i = 0; i < reader.FieldCount; i++) {
                            var columnName = reader.GetName(i);
                            var columnValue = reader.IsDBNull(i) ? null : reader.GetString(i);
                            row.Add(columnName, columnValue);
                        }
                        rows.Add(row);
                    }
                }
            } catch (Exception ex) {
                throw ex;
            } finally {
                if (!IsTransactionActive()) {
                    EnsureConnectionClosed();
                }
            }
            return rows;
        }

        /// <summary>
        /// Opens a connection if not open
        /// </summary>
        private void EnsureConnectionOpen() {
            var retries = 3;
            if (_connection.State == ConnectionState.Open) {
                return;
            } else {
                while (retries >= 0 && _connection.State != ConnectionState.Open && _connection != null) {
                    _connection.Open();
                    retries--;
                    Thread.Sleep(30);
                }
            }
        }

        /// <summary>
        /// Closes a connection if open
        /// </summary>
        public void EnsureConnectionClosed() {
            if (_connection.State == ConnectionState.Open && _connection != null) {
                _connection.Close();
                _connection.Dispose();
            }
        }

        /// <summary>
        /// Creates a MySQLCommand with the given parameters
        /// </summary>
        /// <param name="commandText">The MySQL query to execute</param>
        /// <param name="parameters">Parameters to pass to the MySQL query</param>
        /// <returns></returns>
        private MySqlCommand CreateCommand(string commandText, Dictionary<string, object> parameters) {
            MySqlCommand command = _connection.CreateCommand();
            command.CommandText = commandText;
            AddParameters(command, parameters);

            return command;
        }

        /// <summary>
        /// Creates a MySQLCommand
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private MySqlCommand CreateCommand(string commandText) {
            MySqlCommand command = _connection.CreateCommand();
            command.CommandText = commandText;

            return command;
        }

        /// <summary>
        /// Adds the parameters to a MySQL command
        /// </summary>
        /// <param name="commandText">The MySQL query to execute</param>
        /// <param name="parameters">Parameters to pass to the MySQL query</param>
        private static void AddParameters(MySqlCommand command, Dictionary<string, object> parameters) {
            if (parameters == null) {
                return;
            }

            foreach (var param in parameters) {
                var parameter = command.CreateParameter();
                parameter.ParameterName = param.Key;
                parameter.Value = param.Value ?? DBNull.Value;
                command.Parameters.Add(parameter);
            }
        }

        public async Task<string> GetStrValueAsync(string commandText, Dictionary<string, object> parameters) {
            string value = await QueryValueAsync(commandText, parameters) as string;
            return value;
        }

        public void Dispose() {
            if (_connection != null) {
                _connection.Dispose();
            }
        }

        public async Task<int> ExecuteScalar(string commandText, Dictionary<string, object> parameters) {
            if (String.IsNullOrEmpty(commandText)) {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try {
                EnsureConnectionOpen();
                MySqlCommand command = CreateCommand(commandText, parameters);

                object result = await command.ExecuteScalarAsync();
                if ((result == null) || (result == DBNull.Value)) return -1;
                EnsureConnectionClosed(); // TODO: maybe remove this line
                return Convert.ToInt32(result);
            } catch (Exception ex) {
                throw ex;
            } finally {
                if (!IsTransactionActive()) {
                    EnsureConnectionClosed();
                }
            }
        }
    }
}