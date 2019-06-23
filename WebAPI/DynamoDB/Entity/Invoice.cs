using Amazon.DynamoDBv2.DataModel;

namespace RestAPI.DAL.Entity
{
    [DynamoDBTable("Invoice")]
    public class Invoice
    {
        [DynamoDBHashKey] public int Id { get; set; }

        [DynamoDBProperty] public string Changes { get; set; }

        [DynamoDBRangeKey] public long TimeStamp { get; set; }
    }
}