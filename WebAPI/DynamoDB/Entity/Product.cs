using Amazon.DynamoDBv2.DataModel;

namespace RestAPI.DAL.Entity
{
    [DynamoDBTable("Product")]
    public class Product
    {
        [DynamoDBHashKey] public int Id { get; set; }

        [DynamoDBProperty] public string Changes { get; set; }

        [DynamoDBRangeKey] public long TimeStamp { get; set; }
    }
}