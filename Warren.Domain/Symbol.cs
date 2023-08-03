using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warren.Domain
{
    public class Symbol
    {
        public string Icon { get; set; }

        public double Probability { get; set; }

        public double Coefficient { get; set; }

        public bool IsWildcard { get; set; } = false;

        public Symbol(string icon, double probability, double coefficient, bool isWildcard = false)
        {
            this.Icon = icon;
            this.Probability = probability;
            this.Coefficient = coefficient;
            this.IsWildcard = isWildcard;
        }
    }
}
