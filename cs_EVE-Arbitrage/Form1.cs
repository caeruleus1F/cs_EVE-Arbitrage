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

            if (sourceid != null && destinationid != null)
            {
                Thread t = new Thread(new EVECentralInterfacer(this, sourceid, destinationid, _all_marketables).Begin);
                t.IsBackground = true;
                t.Start();
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
            decimal minprofitpervolume = 0M;
            float maxvolume = 2000000F;

            if (chbExclude.Checked == true)
            {
                minprofitpervolume = Convert.ToDecimal(txbExclude.Text);
            }

            if (chbMaxVolume.Checked == true)
            {
                maxvolume = Convert.ToSingle(txbMaxVolume.Text);
            }

            foreach (MarketableItem m in marketables)
            {
                if (m.ProfitPerM3 >= minprofitpervolume &&
                    m.Volume <= maxvolume)
                {
                    decimal unitprofit = (m.BuyOrderHighest * .98M - m.SellOrderLowest);
                    dgvDisplay.Rows.Add(m.TypeName, m.ProfitPerM3, unitprofit,
                        m.SellOrderLowest, m.BuyOrderHighest, m.Volume,
                        m.SellOrderStation, m.BuyOrderStation);
                }
            }

            rtbDisplay.Text = "Complete!";
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
    }
}
