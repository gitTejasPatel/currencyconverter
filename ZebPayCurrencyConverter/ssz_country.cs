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
    
    public partial class ssz_country
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ssz_country()
        {
            this.ssz_exchange_rate = new HashSet<ssz_exchange_rate>();
        }
    
        public byte zcountry_id { get; set; }
        public string zcountry_name { get; set; }
        public string zcountry_currency_code { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ssz_exchange_rate> ssz_exchange_rate { get; set; }
    }
}
