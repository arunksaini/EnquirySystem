using System;
using System.Collections.Generic;

namespace RestAPI.Common
{
    public interface IDal : IDisposable
    {
        bool InsertEntities(IList<Entity> entities);

        Entity QueryEntity(EntityType type, int id, long timeStamp);
        void ClearEntities();
    }
}