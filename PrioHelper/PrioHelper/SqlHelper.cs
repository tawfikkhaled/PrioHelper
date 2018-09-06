using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrioHelper
{
    static class SqlHelper
    {
        static public bool AddSubject(SUJET toDo)
        {
            using (var context = new CLASSEUREntities1())
            {
                // Perform data access using the context
                context.SUJET.Add(toDo);
                context.SaveChanges();
            }
            return true;
        }
        static public List<SUJET> GetAllToDos()
        {
            using (var context = new CLASSEUREntities1())
            {
                // Perform data access using the context
                return (from r in context.SUJET select r).ToList();
            }
        }
    }
}
