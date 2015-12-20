using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandtrackerMgmt
{
    public class Band
    {
        public string MBID          { get; set; }
        public string discogsId     { get; set; }
        public string name          { get; set; }
        public string genre         { get; set; }
        public string imageUrl      { get; set; }
        public string biography     { get; set; }
        public string bioSource     { get; set; }
        public int    recordStatus  { get; set; }
        public DateTime lastChanged { get; set; } 
    }
}
