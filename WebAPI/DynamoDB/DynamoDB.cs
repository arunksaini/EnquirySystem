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
        object lockObj = new object();
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

        void WaitForTableCreation(string tableName)
        {

            int tryCount = 0;
            while (!Client.CheckingTableExistence(tableName) && !Client.CheckTableIsReady(tableName)  && tryCount < 10)
            {
                tryCount++;
                System.Threading.Thread.Sleep(5000);
            } 

        }

        public void ClearEntities()
        {
            Client.DeleteEntities(EntityType.Product);
            Client.DeleteEntities(EntityType.Order);
            Client.DeleteEntities(EntityType.Invoice);
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
            }

            return entity;
        }

        public void Dispose()
        {
            ((IDisposable) Client).Dispose();
        }

        private void CreateTables()
        {
            lock (lockObj)
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
            var partitionKeyName = "Id";
            var sortKeyName = "TimeStamp";

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

        private IList<Product> ConvertEntityToProduct(IList<Common.Entity> entities)
        {
            IList<Product> list = new List<Product>();
            Product item;

            foreach (var entity in entities)
            {
                item = new Product();
                item.Id = entity.Id;
                item.TimeStamp = entity.TimeStamp;
                item.Changes = JsonConvert.SerializeObject(entity.Changes);
                list.Add(item);
            }

            return list;
        }

        private IList<Invoice> ConvertEntityToInvoice(IList<Common.Entity> entities)
        {
            IList<Invoice> list = new List<Invoice>();
            Invoice item;

            foreach (var entity in entities)
            {
                item = new Invoice();
                item.Id = entity.Id;
                item.TimeStamp = entity.TimeStamp;
                item.Changes = JsonConvert.SerializeObject(entity.Changes);
                list.Add(item);
            }

            return list;
        }

        private IList<Order> ConvertEntityToOrder(IList<Common.Entity> entities)
        {
            IList<Order> list = new List<Order>();
            Order item;

            foreach (var entity in entities)
            {
                item = new Order();
                item.Id = entity.Id;
                item.TimeStamp = entity.TimeStamp;
                item.Changes = JsonConvert.SerializeObject(entity.Changes);
                list.Add(item);
            }

            return list;
        }

        private Common.Entity ConvertEntityToDynamoEntity<T>(dynamic entity)
        {
            Common.Entity item = null;
            if (entity != null)
                item = new Common.Entity(entity.Id.ToString(), typeof(T).Name, entity.TimeStamp.ToString(),
                    entity.Changes);
            return item;
        }
    }
}