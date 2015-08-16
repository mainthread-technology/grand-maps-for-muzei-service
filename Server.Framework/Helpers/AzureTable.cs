namespace Server.Framework.Helpers
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Microsoft.WindowsAzure.Storage.Table.Queryable;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Helper class to access an Azure Storage Table.
    /// </summary>
    /// <typeparam name="T">The table entity to reference.</typeparam>
    public class AzureTable<T> where T : TableEntity, new()
    {
        /// <summary>
        /// The Azure storage account.
        /// </summary>
        private CloudStorageAccount storageAccount;

        /// <summary>
        /// The table client.
        /// </summary>
        private CloudTableClient tableClient;

        /// <summary>
        /// The Azure Storage Table.
        /// </summary>
        private CloudTable table;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTable{T}" /> class.
        /// </summary>
        /// <param name="tableName">The name of the table to reference.</param>
        /// <param name="connectionString">The storage connection string.</param>
        public AzureTable(string tableName, string connectionString)
        {
            Guard.NotNullOrEmpty(() => tableName);
            Guard.NotNullOrEmpty(() => connectionString);

            this.storageAccount = CloudStorageAccount.Parse(connectionString);

            this.tableClient = this.storageAccount.CreateCloudTableClient();

            this.table = this.tableClient.GetTableReference(tableName);

            this.table.CreateIfNotExists();
        }

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <returns>The stored entity.</returns>
        public async Task<T> Insert(T entity)
        {
            Guard.NotNull(() => entity);

            TableOperation insertOperation = TableOperation.Insert(entity);

            return await this.ExecuteOperation(insertOperation);
        }

        /// <summary>
        /// Replaces an entity.
        /// </summary>
        /// <param name="entity">The entity to replace.</param>
        /// <returns>The stored entity.</returns>
        public async Task<T> Replace(T entity)
        {
            Guard.NotNull(() => entity);

            TableOperation replaceOperation = TableOperation.Replace(entity);

            return await this.ExecuteOperation(replaceOperation);
        }

        /// <summary>
        /// Inserts or replaces a new entity.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <returns>The stored entity.</returns>
        public async Task<T> InsertOrReplace(T entity)
        {
            Guard.NotNull(() => entity);

            TableOperation insertOperation = TableOperation.InsertOrReplace(entity);

            return await this.ExecuteOperation(insertOperation);
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>The stored entity.</returns>
        public async Task<T> Delete(T entity)
        {
            Guard.NotNull(() => entity);

            TableOperation deleteOperation = TableOperation.Delete(entity);

            return await this.ExecuteOperation(deleteOperation);
        }

        /// <summary>
        /// Gets a single entity.
        /// </summary>
        /// <param name="partitionKey">The entities partition key.</param>
        /// <param name="rowKey">The entities row key.</param>
        /// <returns>The requested entity.</returns>
        public async Task<T> Get(string partitionKey, string rowKey)
        {
            Guard.NotNullOrEmpty(() => partitionKey);
            Guard.NotNullOrEmpty(() => rowKey);

            TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);

            return await this.ExecuteOperation(retrieveOperation);
        }

        /// <summary>
        /// Gets a list of entities returned by a query.
        /// </summary>
        /// <param name="query">The query to run.</param>
        /// <returns>The resulting entities.</returns>
        public async Task<IEnumerable<T>> GetFromQuery(TableQuery<T> query)
        {
            Guard.NotNull(() => query);

            Func<TableContinuationToken, Task<TableQuerySegment<T>>> queryExecute = (token) => this.table.ExecuteQuerySegmentedAsync(query, token);

            return await ExecuteQuery(queryExecute);
        }

        /// <summary>
        /// Gets all entities from the table.
        /// </summary>
        /// <returns>An enumerable of all stored entities.</returns>
        public async Task<IEnumerable<T>> GetAll()
        {
            TableQuery<T> query = (from map in this.table.CreateQuery<T>() select map).AsTableQuery<T>();

            Func<TableContinuationToken, Task<TableQuerySegment<T>>> queryExecute = (token) => query.ExecuteSegmentedAsync(token);

            return await ExecuteQuery(queryExecute);
        }

        /// <summary>
        /// Executes a Table Query.
        /// </summary>
        /// <param name="queryExecute">The delegate function with the query to run.</param>
        /// <returns>A list of entities.</returns>
        private static async Task<IEnumerable<T>> ExecuteQuery(Func<TableContinuationToken, Task<TableQuerySegment<T>>> queryExecute)
        {
            Guard.NotNull(() => queryExecute);

            List<T> entities = new List<T>();

            TableContinuationToken token = null;

            do
            {
                var result = await queryExecute(token);

                entities.AddRange(result);

                token = result.ContinuationToken;
            }
            while (token != null);

            return entities;
        }

        /// <summary>
        /// Executes a given operation on the referenced table.
        /// </summary>
        /// <param name="operation">The operation to execute.</param>
        /// <returns>The resulting entity.</returns>
        private async Task<T> ExecuteOperation(TableOperation operation)
        {
            Guard.NotNull(() => operation);

            var result = await this.table.ExecuteAsync(operation);

            return result.Result as T;
        }
    }
}
