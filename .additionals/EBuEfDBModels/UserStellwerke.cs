using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class UserStellwerke
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StellwerkId { get; set; }
        public int? Lupe { get; set; }
        public int? Berue { get; set; }
        public int? Streckenspiegel { get; set; }
        public int? Zn { get; set; }
        public int? Zwl { get; set; }
        public int? Idplan { get; set; }
        public string Recht { get; set; }

        public virtual Stellwerke Stellwerk { get; set; }
    }
}
