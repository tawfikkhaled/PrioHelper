using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace PrioHelper
{
    class ToDo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<byte> CRITERE1 { get; set; }
        public Nullable<byte> CRITERE2 { get; set; }
        public Nullable<byte> CRITERE3 { get; set; }  
    }
    class Subject: DbContext
    {
        public DbSet<ToDo> ToDos { set; get; }
        public Subject()
        {
            
        }
    }
}
