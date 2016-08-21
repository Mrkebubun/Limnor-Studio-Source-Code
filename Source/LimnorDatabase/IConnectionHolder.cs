using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LimnorDatabase
{
    public interface IConnectionHolder
    {
        Guid ConnectionID { get; set; }
        ConnectionItem DatabaseConnection { get; set; }
    }
}
