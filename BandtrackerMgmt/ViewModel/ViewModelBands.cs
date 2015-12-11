using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandtrackerMgmt
{
    public class ViewModelBands : ViewModelBase
    {
        // initalization
        public ViewModelBands()
        {
            m_cmd_refresh = new SimpleCommand(Refresh);
        }

        override public void Initialize()
        {
            Refresh();
        }

        public void Refresh()
        {
            new Task(async () =>
            {
                var f_bands = BandTrackerClient.Instance.BandList(100, 0);
                DataCentral.Context.BandsClear();
                DataCentral.Context.BandsMergeList(await f_bands);
            }).Start();
        }

        // properties
        public static string Id { get { return "Bands"; } }
        public string PageTitle { get { return m_page_title; } set { SetField(ref m_page_title, value); } }

        public ObservableCollection<Band> Bands { get { return DataCentral.Context.Bands;  } }

        public SimpleCommand CommandRefresh { get { return m_cmd_refresh; } }

        // variables
        private string          m_page_title = "Bands";
        private SimpleCommand   m_cmd_refresh;
    }
}
