using LibraryManagementSystem.DataAccess.Contract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.DataAccess.Data.Interceptors
{
    internal class SoftDeleteInterceptor: SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges
            (DbContextEventData eventData, InterceptionResult<int> result)
        {
            if(eventData.Context is null)
                return result;


            foreach(var entry in eventData.Context.ChangeTracker.Entries())
            {
                if(entry is not { State : EntityState.Deleted,Entity : ISoftDeletable entity } )
                   continue;

                entry.State = EntityState.Modified;

                entity.Delete();
            }


            return result;
        }
    }
}
