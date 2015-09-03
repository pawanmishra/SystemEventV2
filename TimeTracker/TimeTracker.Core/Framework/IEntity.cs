using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.Core.Framework
{
    public interface IEntity
    {
        int Id { get; }

        DateTime DateCreated { get; set; }

        DateTime DateModified { get; set; }

        int SyncRoot { get; set; }
    }
}
