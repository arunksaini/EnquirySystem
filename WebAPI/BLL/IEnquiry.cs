using System.Collections.Generic;
using System.IO;
using RestAPI.Common;

namespace RestAPI.BLL
{
    public interface IEnquiry
    {
        void  ReadFromStream(StreamReader streamReader);
        Entity ConvertCsvToEntity(string csv);

        bool SaveToDatabase(IList<Entity> entities);
        Entity SearchEntity(Entity entity);
        void ClearEntities();
    }
}