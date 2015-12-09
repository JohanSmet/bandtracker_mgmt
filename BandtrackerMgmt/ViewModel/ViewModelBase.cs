using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandtrackerMgmt
{
    public abstract class ViewModelBase : DataBindingNotifier
    {
        abstract public void Initialize();
    }
}
