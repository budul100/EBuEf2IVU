using System;
using System.Collections.Generic;

namespace EBuEfDBModels
{
    public partial class StellwerkeMerkspeicher
    {
        public int Id { get; set; }
        public int StellwerkUz { get; set; }
        public int Position { get; set; }
        public string Text { get; set; }
        public string Bearbeiter { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual StellwerkeUz StellwerkUzNavigation { get; set; }
    }
}
