using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace cs_EVE_Arbitrage
{
    public partial class Form1 : Form
    {
        List<string> _raw_marketables = null;
        List<MarketableItem> _all_marketables = new List<MarketableItem>();
        List<string> _solarsystems = null;
        List<string> _regions = null;

        Thread _t = null;
        List<MarketableItem> _filteredlist = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadMarketables();
            LoadSolarSystems();
            LoadRegions();
        }

        private void LoadSolarSystems()
        {
            _solarsystems = Properties.Resources.mapSolarSystems.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            _solarsystems.RemoveAt(0);
        }

        private void LoadRegions()
        {
            _regions = Properties.Resources.mapRegions.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            _regions.RemoveAt(0);
        }

        private void LoadMarketables()
        {
            _raw_marketables = Properties.Resources.invTypes_frostline_10_marketables.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            _raw_marketables.RemoveAt(0);

            foreach (string line in _raw_marketables)
            {
                string[] temp = line.Split(',');

                if (temp.Length == 6)
                {
                    _all_marketables.Add(new MarketableItem(temp[0], temp[2], Convert.ToSingle(temp[3])));
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(temp[2]).Append(temp[3]);
                    _all_marketables.Add(new MarketableItem(temp[0], sb.ToString().Replace("\"", ""), Convert.ToSingle(temp[4])));
                }
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (_t != null && _t.IsAlive)
            {
                _t.Abort();
            }

            string source = txbSource.Text;
            string destination = txbDestination.Text;

            // translate source/destination to sysytemid/regionid
            string sourceid = null;
            string destinationid = null;

            sourceid = FindSolarSystemID(source);
            if (sourceid == null) sourceid = FindRegionID(source);
            if (sourceid == null) MessageBox.Show("Source mispelled?");

            destinationid = FindSolarSystemID(destination);
            if (destinationid == null) destinationid = FindRegionID(destination);
            if (destinationid == null) MessageBox.Show("Destination mispelled?");

            _filteredlist = new List<MarketableItem>();

            foreach (MarketableItem m in _all_marketables)
            {
                _filteredlist.Add(m);
            }

            if (sourceid != null && destinationid != null)
            {
                _t = new Thread(new EVECentralInterfacer(this, sourceid, destinationid, _filteredlist).Begin);
                _t.IsBackground = true;
                _t.Start();
            }

        }

        private string FindSolarSystemID(string inputtext)
        {
            string solarsystemid = null;
            string tempname = null;
            bool found = false;

            int length = _solarsystems.Count;
            for (int i = 0; i < length && !found; ++i)
            {
                tempname = _solarsystems[i].Split(',')[1];
                if (tempname.Equals(inputtext))
                {
                    found = true;
                    solarsystemid = _solarsystems[i].Split(',')[0];
                }
            }

            return solarsystemid;
        }

        private string FindRegionID(string inputtext)
        {
            string regionid = null;
            string tempname = null;
            bool found = false;

            int length = _regions.Count;
            for (int i = 0; i < length && !found; ++i)
            {
                tempname = _regions[i].Split(',')[1];
                if (tempname.Equals(inputtext))
                {
                    found = true;
                    regionid = _regions[i].Split(',')[0];
                }
            }

            return regionid;
        }

        private void btnSwap_Click(object sender, EventArgs e)
        {
            string temp = txbSource.Text;
            txbSource.Text = txbDestination.Text;
            txbDestination.Text = temp;
        }

        public void Display(List<MarketableItem> marketables)
        {
            dgvDisplay.Rows.Clear();

            decimal minprofitpervolume = 0M;
            float maxvolume = 2000000F;
            bool validinput = true;

            if (chbExclude.Checked == true)
            {
                try
                {
                    minprofitpervolume = Convert.ToDecimal(txbExclude.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Minimum profit per volume must be a number.");
                    validinput = false;
                }
            }

            if (chbMaxVolume.Checked == true)
            {
                try
                {
                    maxvolume = Convert.ToSingle(txbMaxVolume.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Max volume must be a number.");
                    validinput = false;
                }
            }

            if (marketables != null && validinput == true)
            {
                foreach (MarketableItem m in marketables)
                {
                    if (m.Volume <= maxvolume && m.ProfitPerM3 >= minprofitpervolume) 
                    {
                        // edit this to reflect the fact that there can be multi buy/sell orders to fill.
                        decimal unitprofit = (m.BuyOrderHighest * .98M - m.SellOrderLowest);
                        foreach (SellOrder s in m.SellOrders)
                        {
                            dgvDisplay.Rows.Add(m.TypeName, m.ProfitPerM3, unitprofit,
                                m.Volume, s.StationName, "S", s.Price, s.RemainingVolume);
                        }

                        foreach (BuyOrder b in m.BuyOrders)
                        {
                            dgvDisplay.Rows.Add(m.TypeName, m.ProfitPerM3, unitprofit,
                                m.Volume, b.StationName, "B", b.Price, b.RemainingVolume);
                        }
                    }

                }

                rtbDisplay.Text = "Complete!";
            }
        }

        public void UpdateStatus(string text)
        {
            rtbDisplay.Text = text;
        }

        private void chbExclude_CheckedChanged(object sender, EventArgs e)
        {
            if (chbExclude.Checked == true)
            {
                txbExclude.Enabled = true;
            }
            else
            {
                txbExclude.Enabled = false;
            }
        }

        private void chbMaxVolume_CheckedChanged(object sender, EventArgs e)
        {
            if (chbMaxVolume.Checked == true)
            {
                txbMaxVolume.Enabled = true;
            }
            else
            {
                txbMaxVolume.Enabled = false;
            }
        }

        public bool ResolveStationNames()
        {
            return chbStationNames.Checked;
        }

        private void btnReapplyFilters_Click(object sender, EventArgs e)
        {
            Display(_filteredlist);
        }
    }
}
