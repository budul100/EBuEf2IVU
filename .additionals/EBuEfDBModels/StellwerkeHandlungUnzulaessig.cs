using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class StellwerkeHandlungUnzulaessig
    {
        public int Id { get; set; }
        public int? StellwerkId { get; set; }
        public int? UserId { get; set; }
        public int InfraId { get; set; }
        public int? Dir { get; set; }

        public virtual InfraZustand Infra { get; set; }
        public virtual Stellwerke Stellwerk { get; set; }
    }
}
