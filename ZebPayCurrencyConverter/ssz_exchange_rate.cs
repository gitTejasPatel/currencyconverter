//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ZebPayCurrencyConverter
{
    using System;
    using System.Collections.Generic;
    
    public partial class ssz_exchange_rate
    {
        public int exchange_rate_id { get; set; }
        public Nullable<byte> zcountry_id { get; set; }
        public Nullable<decimal> exchange_rate_amount { get; set; }
        public Nullable<System.DateTime> exchange_date { get; set; }
    
        public virtual ssz_country ssz_country { get; set; }
        public virtual ssz_exchange_rate ssz_exchange_rate1 { get; set; }
        public virtual ssz_exchange_rate ssz_exchange_rate2 { get; set; }
    }
}
