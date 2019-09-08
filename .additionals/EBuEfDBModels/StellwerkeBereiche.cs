using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class StellwerkeBereiche
    {
        public int Id { get; set; }
        public int StellwerkeUzId { get; set; }
        public int StellwerkeId { get; set; }

        public virtual Stellwerke Stellwerke { get; set; }
        public virtual StellwerkeUz StellwerkeUz { get; set; }
    }
}
