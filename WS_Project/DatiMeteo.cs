//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WS_Project
{
    using System;
    using System.Collections.Generic;
    
    public partial class DatiMeteo
    {
        public int ID { get; set; }
        public System.DateTime Data { get; set; }
        public double Temperatura { get; set; }
        public double Umidità { get; set; }
        public string Aria { get; set; }
        public string Pressione { get; set; }
        public double Pioggia { get; set; }
    }
}
