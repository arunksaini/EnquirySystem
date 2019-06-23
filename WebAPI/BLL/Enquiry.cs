using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RestAPI.Common;
using RestAPI.DAL;

namespace RestAPI.BLL
{
    public class Enquiry : IEnquiry
    {
        public Entity SearchEntity(Entity entity)
        {
            //TODO :DI
            Entity result;
            EntityType type;
            Enum.TryParse(entity.Type, out type);
            using (IDal database = new DynamoDb())
            {
                result = database.QueryEntity(type, entity.Id, entity.TimeStamp);
            }

            return result;
        }

        public void ClearEntities()
        {
            //TODO :DI
            using (IDal database = new DynamoDb())
            {
                 database.ClearEntities();
            }

           
        }
        public Entity ConvertCsvToEntity(string csv)
        {
            var rowValues = csv.Split(new[] {','}, 4);

            return new Entity(rowValues[0], rowValues[1], rowValues[2], CorrectJsonString(rowValues[3]));
        }

        public bool SaveToDatabase(IList<Entity> entities)
        {
            bool result;
            //TODO :DI
            using (IDal database = new DynamoDb())
            {
                result = database.InsertEntities(entities);
            }

            return result;
        }

        public bool ReadFromStream(StreamReader streamReader)
        {
            var status = false;
            IList<Entity> entities = new List<Entity>();
            string row = null;
            streamReader.ReadLine(); // Skip header row
            // Read line by line
            while ((row = streamReader.ReadLine()) != null)
            {
                entities.Add(ConvertCsvToEntity(row));
                if (entities.Count() > 1000)
                {
                    // Save to db in batch size of 1000 lets say
                    SaveToDatabase(entities);
                    entities.Clear();
                }
            }

            if (entities.Count > 0)
            {
                // If total rows or end batch was less than 1000 then say that here
                SaveToDatabase(entities);
                entities.Clear();
            }

            status = false;


            return status;
        }

        private string CorrectJsonString(string jsonString)
        {
            var correctedJsonString = jsonString;
            if (jsonString.Length > 1)
                correctedJsonString = correctedJsonString.Remove(correctedJsonString.Length - 1, 1).Remove(0, 1)
                    .Replace("\\\"", "\"");

            return correctedJsonString;
        }
    }
}