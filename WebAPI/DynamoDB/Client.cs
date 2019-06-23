using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using RestAPI.Common;

namespace RestAPI.DAL
{
    public class Client : IDisposable
    {
        public DynamoDBContext DbContext;
        public AmazonDynamoDBClient DynamoClient;

        public Client(bool useDynamoDbLocal)
        {
            if (useDynamoDbLocal)
            {
                // First, check to see whether anyone is listening on the DynamoDB local port
                // (by default, this is port 8000, so if you are using a different port, modify this accordingly)
                try
                {
                    using (var tcpClient = new TcpClient())
                    {
                        var result = tcpClient.BeginConnect("localhost", 8000, null, null);
                        result.AsyncWaitHandle.WaitOne(3000); // Wait 3 seconds
                        tcpClient.EndConnect(result);
                    }
                }
                catch
                {
                    //Do nothing
                }

                // If DynamoDB-Local does seem to be running, so create a DynamoClient
                Console.WriteLine("  -- Setting up a DynamoDB-Local DynamoClient (DynamoDB Local seems to be running)");
                var ddbConfig = new AmazonDynamoDBConfig
                {
                    ServiceURL = "http://localhost:8000"
                };
                try
                {
                    DynamoClient = new AmazonDynamoDBClient(ddbConfig);
                }
                catch
                {
                    //FAILED to create a DynamoDBLocal DynamoClient
                    
                    throw new Exception("Failed to create database client.");
                }
            }

            else
            {
                try
                {
                    DynamoClient = new AmazonDynamoDBClient(new AppConfigAWSCredentials(), RegionEndpoint.EUCentral1);

                }
                catch
                {
                    throw new Exception("Failed to create database client.");
                }
            }

            if (DynamoClient != null)
                DbContext = new DynamoDBContext(DynamoClient);
           
        }

        internal void DeleteEntities(EntityType tableName)
        {
            try
            {
                if (CheckingTableExistence(tableName.ToString()))
                {
                    // Delete Existing Table...
                    DynamoClient.DeleteTableAsync(tableName.ToString()).Wait();
                    //Deleted Existing Table...
                }
            }
            catch
            {
                throw new Exception("Require collection not exists. Please try after a while");
            }
        }

        public void Dispose()
        {
            if (DynamoClient != null)
                ((IDisposable) DynamoClient).Dispose();
            if (DbContext != null)
                ((IDisposable) DbContext).Dispose();
        }

        /// <summary>
        ///     The BatchStore Method allows you to store a list of items of type T to dynamoDb
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        public void BatchStore<T>(IEnumerable<T> items) where T : class
        {
            try
            {
                var itemBatch = DbContext.CreateBatchWrite<T>();

                foreach (var item in items) itemBatch.AddPutItem(item);

                itemBatch.Execute();
            }
            catch
            {
                throw new Exception("Require collection not exists. Please try after a while");
            }
         
        }

        /// <summary>
        ///     Retrieves an item based on a search key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public T GetItem<T>(int key, long range) where T : class
        {
            return DbContext.Load<T>(key, range);
        }


        /*--------------------------------------------------------------------------
     *                       CreatingTable
     *--------------------------------------------------------------------------*/
        public void CreatingTable(string newTableName,
            List<AttributeDefinition> tableAttributes,
            List<KeySchemaElement> tableKeySchema,
            ProvisionedThroughput provisionedThroughput)
        {
           
            if (CheckingTableExistence(newTableName) && CheckTableIsReady(newTableName))
            {
                return;
            }
            //Creating a new table

            CreateNewTable(newTableName,
                tableAttributes,
                tableKeySchema,
                provisionedThroughput);
        }


        /*--------------------------------------------------------------------------
         *                      checkingTableExistence
         *--------------------------------------------------------------------------*/

        public bool CheckTableIsReady(string tableName)
        {
            try
            {
                var result = DynamoClient.DescribeTable(tableName);
                return result?.Table != null && result.Table.TableStatus == TableStatus.ACTIVE;
            }
            catch 
            {
                return false;
            }
        }
        public bool CheckingTableExistence(string tableName)
        {
            
            var tblResponse = DynamoClient.ListTables();
            return tblResponse.TableNames.Contains(tableName);
        }


        /*--------------------------------------------------------------------------
         *                CreateNewTable
         *--------------------------------------------------------------------------*/
        public bool CreateNewTable(string tableName,
            List<AttributeDefinition> tableAttributes,
            List<KeySchemaElement> tableKeySchema,
            ProvisionedThroughput provisionedThroughput)
        {
            // Build the 'CreateTableRequest' structure for the new table
            var request = new CreateTableRequest
            {
                TableName = tableName,
                AttributeDefinitions = tableAttributes,
                KeySchema = tableKeySchema,
                // Provisioned-throughput settings are always required,
                // although the local test version of DynamoDB ignores them.
                ProvisionedThroughput = provisionedThroughput
            };

           
            try
            {
                DynamoClient.CreateTable(request);
                //Created the {tableName} table successfully!
            }
            catch
            {
                // FAILED to create the new table, because: {0}.", ex.Message);

                throw new Exception("Failed creating collection. Please try after a while");

            }

            return true;
        }
    }
}