using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Request
{
    public class ApplyChangesSimulationRequest
        : SimulationRequest
    {
	    // sistema di filtri
        public int? MinCapacity { get; set; }
        public int? MaxCapacity { get; set; }

        public Guid? Sim_CarMAtchId { get; set; }

        // valori per il prezzo al km
        public decimal? BasePriceLimit { get; set; }
        public decimal? BasePriceNewVAl { get; set; }
        public decimal? BasePriceAddVal { get; set; }
        public float? BasePriceFacto { get; set; }
        //  valori per il prezzo al km extra
        public decimal? ExtraPriceLimit { get; set; }
        public decimal? ExtraPriceNewVAl { get; set; }
        public decimal? ExtraPriceAddVal { get; set; }
        public float? ExtraPriceFacto { get; set; }

        // valori predefiniti per il costo al chilometro
        public decimal? DefKmPrice { get; set; }
        public decimal? DefKmExtraPrice { get; set; }

        public bool Overwrite { get; set; }
    }
}
