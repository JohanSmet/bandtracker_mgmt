using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandtrackerMgmt
{
    public static class DataCentral
    {
        // properties
        public static DataContext Context {get {return m_context; } }

        // variables
        private static DataContext m_context = new DataContext();
    }
}
