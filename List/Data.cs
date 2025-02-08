using System;

namespace Nationalization.List
{
    class Data
    {
        public String Origin { get; set; }
        public string Branch { get; set; }
        public string OriginNo { get; set; }
        public double AmountLc { get; set; }
        public double ActualAmount { get; set; }
        public double AmountSc { get; set; }
        public double exRate { get; set; }
        public int line { get; set; }
        public int TransId { get; set; }
        public string TransType { get; set; }
        public int TransRowId { get; set; }
        public string Type { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public DateTime Date { get; set; }
    }
}
