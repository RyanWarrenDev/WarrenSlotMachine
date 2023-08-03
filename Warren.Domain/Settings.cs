using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Xsl;

namespace Warren.Domain
{
    public class Settings
    {
        public double MinDeposit { get; set; }

        public double MaxDeposit { get; set; }

        public double MinStake { get; set; }

        public double MaxStake { get; set; }

        public SlotSettings SlotSettings { get; set; }
    }
}
