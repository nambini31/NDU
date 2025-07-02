using System;
using System.Collections.Generic;

namespace Core.Entity.Exorabilis
{
    public class Chrono
    {
        public string? Racine { get; set; }
        public long? NbRemiseId { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int? DatabaseSave { get; set; }
        public int? DatabaseSaveImage { get; set; }
    }
}
