//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Stok_Takip.Models.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class Müşteri
    {
        public int ID { get; set; }
        public string AdSoyad { get; set; }
        public string Telefon { get; set; }
        public string Adres { get; set; }
        public string Email { get; set; }
        public string Resim { get; set; }
        public Nullable<System.DateTime> KayıtTarihi { get; set; }
        public Nullable<decimal> Puani { get; set; }
        public byte[] Aciklama { get; set; }
    }
}
