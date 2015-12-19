using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandtrackerMgmt
{
    public class ServerTask
    {
        // nested types
        public enum Status
        {
            Open,
            Running,
            FinishedOk,
            Failed
        }

        // properties
        public string       _id             { get; set; }
        public string       taskType        { get; set; }
        public List<string> taskParams      { get; set; }
        public DateTime     dateCreated     { get; set; }
        public DateTime     dateStarted     { get; set; }
        public DateTime     dateExecuted    { get; set; }
        public bool         resultOk        { get; set; }

        public Status status
        {
            get
            {
                if (dateStarted == null)
                    return Status.Open;
                else if (dateExecuted == null)
                    return Status.Running;
                else if (resultOk)
                    return Status.FinishedOk;
                else
                    return Status.Failed;
            }
        }
    }
}
