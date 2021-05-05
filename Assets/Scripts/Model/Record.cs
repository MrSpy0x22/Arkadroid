using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Model
{
    public class Record
    {
        public string Name { get; set; }
        public int Scores { get; set; }
        public long Timestamp { get; set; }

        public override string ToString()
        {
            return string.Format("{0}|{1}|{2}" , Name , Scores , Timestamp);
        }
    }
}
