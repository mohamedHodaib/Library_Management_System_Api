using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.DataAccess.Contract
{
    public interface ISoftDeletable
    {
        public bool IsDeleted { get; set; }
        public DateTime? DeleteDate { get; set; }

        public void Delete()
        {
            DeleteDate = DateTime.Now;
        }


        public void UndoDelete()
        {
            DeleteDate = null;
        }
    }
}
