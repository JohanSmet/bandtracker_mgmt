using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandtrackerMgmt
{
    public class Band
    {
          // nested types
        public enum Status
        {
            Revoked = -1,
            New = 0,
            Released = 1
        };

        // properties
        public string MBID          { get; set; }
        public string discogsId     { get; set; }
        public string name          { get; set; }
        public string genre         { get; set; }
        public string imageUrl      { get; set; }
        public string biography     { get; set; }
        public string bioSource     { get; set; }
        public Status   recordStatus  { get; set; }
        public DateTime lastChanged { get; set; } 
    }
}
