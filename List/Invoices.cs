using System;

namespace Nationalization.List
{
    class Invoices
    {
        public double invoiceLC { get; set; }
        public double invoiceSC { get; set; }
        public double ActualAmount { get; set; }
        public double exRate { get; set; }
        public int line { get; set; }
        public int TransId { get; set; }
        public int TransRowId { get; set; }
        public string TransType { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Branch { get; set; }
        public DateTime Date { get; set; }
    }
}
