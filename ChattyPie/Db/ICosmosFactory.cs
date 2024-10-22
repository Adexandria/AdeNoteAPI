using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattPie.Db
{
    public interface ICosmosFactory
    {
        Database InitialiseDatabase();
    }
}
