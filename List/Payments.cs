using System;

namespace Nationalization.List
{
    class Payment
    {
        public double paymentLC { get; set; }
        public double paymentSC { get; set; }
        public double ActualAmount { get; set; }
        public double exRate { get; set; }
        public int line { get; set; }
        public int TransId { get; set; }
        public int TransRowId { get; set; }
        public string TransType { get; set; }
        public string Branch { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime Date { get; set; }

    }

}
