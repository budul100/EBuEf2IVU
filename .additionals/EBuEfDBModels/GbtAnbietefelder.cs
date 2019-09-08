using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class GbtAnbietefelder
    {
        public int Id { get; set; }
        public int GbtId { get; set; }
        public int AnbietefeldId { get; set; }

        public virtual Gbt Anbietefeld { get; set; }
        public virtual Gbt Gbt { get; set; }
    }
}
