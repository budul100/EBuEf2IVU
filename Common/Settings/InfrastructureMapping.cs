using Common.BusinessObjects;

namespace Common.Settings
{
    public class InfrastructureMapping
    {
        #region Public Properties

        public string EBuEfNachBetriebsstelle { get; set; }

        public string EBuEfNachVerschiebungSekunden { get; set; }

        public string EBuEfVonBetriebsstelle { get; set; }

        public string EBuEfVonVerschiebungSekunden { get; set; }

        public string IVUGleis { get; set; }

        public string IVUNetzpunkt { get; set; }

        public TrainPositionType IVUTrainPositionType { get; set; }

        public string IVUVerschiebungSekunden { get; set; }

        public string MessageBetriebsstelle { get; set; }

        public string MessageEndGleis { get; set; }

        public string MessageStartGleis { get; set; }

        #endregion Public Properties
    }
}