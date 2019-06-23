using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.Model;
using Newtonsoft.Json;
using RestAPI.Common;
using RestAPI.DAL.Entity;

namespace RestAPI.DAL
{
    public class DynamoDb : IDal
    {
        public Client Client;
        private readonly object _lockObj = new object();
        public DynamoDb()
        {
            Client = new Client(false);
        }

        public bool InsertEntities(IList<Common.Entity> entities)
        {
            CreateTables();

            WaitForTableCreation(EntityType.Product.ToString());
            Client.BatchStore(
                ConvertEntityToProduct(entities.Where(e => e.Type == EntityType.Product.ToString()).ToList()));
            WaitForTableCreation(EntityType.Invoice.ToString());
            Client.BatchStore(
                ConvertEntityToInvoice(entities.Where(e => e.Type == EntityType.Invoice.ToString()).ToList()));
            WaitForTableCreation(EntityType.Order.ToString());
            Client.BatchStore(
                ConvertEntityToOrder(entities.Where(e => e.Type == EntityType.Order.ToString()).ToList()));

            return true;
        }

        private void WaitForTableCreation(string tableName)
        {
            while (!Client.CheckingTableExistence(tableName) && !Client.CheckTableIsReady(tableName))
            {
                System.Threading.Thread.Sleep(1000);
            } 

        }

        private void WaitForTableDeletion()
        {

            while (Client.CheckingTableExistence(EntityType.Product.ToString())
                   ||
                   Client.CheckingTableExistence(EntityType.Order.ToString())
                   ||
                   Client.CheckingTableExistence(EntityType.Invoice.ToString())
            )
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        public void ClearEntities()
        {
            Client.DeleteEntities(EntityType.Product);
            Client.DeleteEntities(EntityType.Order);
            Client.DeleteEntities(EntityType.Invoice);

            WaitForTableDeletion();

            CreateTables();
        }
        public Common.Entity QueryEntity(EntityType type, int id, long timeStamp)
        {
            
            Common.Entity entity = null;
            switch (type)
            {
                case EntityType.Order:
                    if (Client.CheckingTableExistence(EntityType.Order.ToString()) &&
                        Client.CheckTableIsReady(EntityType.Order.ToString()))
                    {
                        entity = ConvertEntityToDynamoEntity<Order>(Client.GetItem<Order>(id, timeStamp));
                    }
                    break;
                case EntityType.Product:
                    if (Client.CheckingTableExistence(EntityType.Product.ToString()) &&
                        Client.CheckTableIsReady(EntityType.Product.ToString()))
                    {
                        entity = ConvertEntityToDynamoEntity<Product>(Client.GetItem<Product>(id, timeStamp));
                    }
                    break;
                case EntityType.Invoice:
                    if (Client.CheckingTableExistence(EntityType.Invoice.ToString()) &&
                        Client.CheckTableIsReady(EntityType.Invoice.ToString()))
                    {
                        entity = ConvertEntityToDynamoEntity<Invoice>(Client.GetItem<Invoice>(id, timeStamp));
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Common.Entity type not supported");
            }

            return entity;
        }

        public void Dispose()
        {
            ((IDisposable) Client).Dispose();
        }

        private void CreateTables()
        {
            lock (_lockObj)
            {
                CreateTable(EntityType.Product);
                CreateTable(EntityType.Invoice);
                CreateTable(EntityType.Order);
            }
        }

        private void CreateTable(EntityType table)
        {
            // table_name
            var tableName = table.ToString();

            // key names for the table
            const string partitionKeyName = "Id";
            const string sortKeyName = "TimeStamp";

            // items_attributes
            var itemsAttributes
                = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = partitionKeyName,
                        AttributeType = "N"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = sortKeyName,
                        AttributeType = "N"
                    }
                 
                };

            // key_schema
            var keySchema
                = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = partitionKeyName,
                        KeyType = "HASH"
                    },
                    new KeySchemaElement
                    {
                        AttributeName = sortKeyName,
                        KeyType = "RANGE"
                    }
                };

 
            var tableProvisionedThroughput
                = new ProvisionedThroughput(2, 1);

            Client.CreatingTable(tableName,
                itemsAttributes,
                keySchema,
                tableProvisionedThroughput);
        }

        private static IEnumerable<Product> ConvertEntityToProduct(IEnumerable<Common.Entity> entities)
        {
            return entities.Select(entity => new Product {Id = entity.Id, TimeStamp = entity.TimeStamp, Changes = JsonConvert.SerializeObject(entity.Changes)}).ToList();
        }

        private static IEnumerable<Invoice> ConvertEntityToInvoice(IEnumerable<Common.Entity> entities)
        {
            return entities.Select(entity => new Invoice {Id = entity.Id, TimeStamp = entity.TimeStamp, Changes = JsonConvert.SerializeObject(entity.Changes)}).ToList();
        }

        private static IEnumerable<Order> ConvertEntityToOrder(IEnumerable<Common.Entity> entities)
        {
            return entities.Select(entity => new Order {Id = entity.Id, TimeStamp = entity.TimeStamp, Changes = JsonConvert.SerializeObject(entity.Changes)}).ToList();
        }

        private static Common.Entity ConvertEntityToDynamoEntity<T>(dynamic entity)
        {
            Common.Entity item = null;
            if (entity != null)
                item = new Common.Entity(entity.Id.ToString(), typeof(T).Name, entity.TimeStamp.ToString(),
                    entity.Changes);
            return item;
        }
    }
}