using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EBuEfDBModels
{
    public partial class ebuef_fahrplanContext : DbContext
    {
        public ebuef_fahrplanContext()
        {
        }

        public ebuef_fahrplanContext(DbContextOptions<ebuef_fahrplanContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Bahnuebergaenge> Bahnuebergaenge { get; set; }
        public virtual DbSet<BahnuebergaengeEinstellung> BahnuebergaengeEinstellung { get; set; }
        public virtual DbSet<BahnuebergaengeElemente> BahnuebergaengeElemente { get; set; }
        public virtual DbSet<BahnuebergaengeStellauftraege> BahnuebergaengeStellauftraege { get; set; }
        public virtual DbSet<Betra> Betra { get; set; }
        public virtual DbSet<BetriebsstellenArt> BetriebsstellenArt { get; set; }
        public virtual DbSet<BetriebsstellenErsetzung> BetriebsstellenErsetzung { get; set; }
        public virtual DbSet<BetriebsstellenFahrtrichtung> BetriebsstellenFahrtrichtung { get; set; }
        public virtual DbSet<BetriebsstellenHinweise> BetriebsstellenHinweise { get; set; }
        public virtual DbSet<BetriebsstellenListe> BetriebsstellenListe { get; set; }
        public virtual DbSet<BetriebsstellenStatus> BetriebsstellenStatus { get; set; }
        public virtual DbSet<BetriebsstellenStatusElemente> BetriebsstellenStatusElemente { get; set; }
        public virtual DbSet<Blockfelder> Blockfelder { get; set; }
        public virtual DbSet<BlockfelderEinstellung> BlockfelderEinstellung { get; set; }
        public virtual DbSet<BlockfelderElemente> BlockfelderElemente { get; set; }
        public virtual DbSet<Dwege> Dwege { get; set; }
        public virtual DbSet<DwegeEinstellung> DwegeEinstellung { get; set; }
        public virtual DbSet<DwegeEinstellungElemente> DwegeEinstellungElemente { get; set; }
        public virtual DbSet<DwegeElemente> DwegeElemente { get; set; }
        public virtual DbSet<Erlaubnisfelder> Erlaubnisfelder { get; set; }
        public virtual DbSet<ErlaubnisfelderAufhebung> ErlaubnisfelderAufhebung { get; set; }
        public virtual DbSet<Fahrplan> Fahrplan { get; set; }
        public virtual DbSet<FahrplanGrundaufstellung> FahrplanGrundaufstellung { get; set; }
        public virtual DbSet<FahrplanMindesthaltezeiten> FahrplanMindesthaltezeiten { get; set; }
        public virtual DbSet<FahrplanSession> FahrplanSession { get; set; }
        public virtual DbSet<FahrplanSessionfahrplanTracker> FahrplanSessionfahrplanTracker { get; set; }
        public virtual DbSet<FahrplanSessionzuege> FahrplanSessionzuege { get; set; }
        public virtual DbSet<FahrplanZuege> FahrplanZuege { get; set; }
        public virtual DbSet<Fahrstrassen> Fahrstrassen { get; set; }
        public virtual DbSet<FahrstrassenAnstoss> FahrstrassenAnstoss { get; set; }
        public virtual DbSet<FahrstrassenDwege> FahrstrassenDwege { get; set; }
        public virtual DbSet<FahrstrassenEinstellung> FahrstrassenEinstellung { get; set; }
        public virtual DbSet<FahrstrassenEinstellungElemente> FahrstrassenEinstellungElemente { get; set; }
        public virtual DbSet<FahrstrassenElemente> FahrstrassenElemente { get; set; }
        public virtual DbSet<FahrstrassenElementtypen> FahrstrassenElementtypen { get; set; }
        public virtual DbSet<FahrstrassenFolgen> FahrstrassenFolgen { get; set; }
        public virtual DbSet<Fahrzeuge> Fahrzeuge { get; set; }
        public virtual DbSet<FahrzeugeAbschnitte> FahrzeugeAbschnitte { get; set; }
        public virtual DbSet<FahrzeugeBaureiheGruppe> FahrzeugeBaureiheGruppe { get; set; }
        public virtual DbSet<FahrzeugeBaureihen> FahrzeugeBaureihen { get; set; }
        public virtual DbSet<FahrzeugeDaten> FahrzeugeDaten { get; set; }
        public virtual DbSet<FahrzeugeGruppen> FahrzeugeGruppen { get; set; }
        public virtual DbSet<FahrzeugeSteuerung> FahrzeugeSteuerung { get; set; }
        public virtual DbSet<Fma> Fma { get; set; }
        public virtual DbSet<Fplo> Fplo { get; set; }
        public virtual DbSet<Gbt> Gbt { get; set; }
        public virtual DbSet<GbtAnbietefelder> GbtAnbietefelder { get; set; }
        public virtual DbSet<GbtBetriebsstellen> GbtBetriebsstellen { get; set; }
        public virtual DbSet<GbtFma> GbtFma { get; set; }
        public virtual DbSet<HaltabschnitteEinstellung> HaltabschnitteEinstellung { get; set; }
        public virtual DbSet<InfraDaten> InfraDaten { get; set; }
        public virtual DbSet<InfraFestlegung> InfraFestlegung { get; set; }
        public virtual DbSet<InfraTypen> InfraTypen { get; set; }
        public virtual DbSet<InfraZustand> InfraZustand { get; set; }
        public virtual DbSet<Protokoll> Protokoll { get; set; }
        public virtual DbSet<ProtokollTrainer> ProtokollTrainer { get; set; }
        public virtual DbSet<Signale> Signale { get; set; }
        public virtual DbSet<SignaleBegriffe> SignaleBegriffe { get; set; }
        public virtual DbSet<SignaleEinstellung> SignaleEinstellung { get; set; }
        public virtual DbSet<SignaleEinstellungErhalt> SignaleEinstellungErhalt { get; set; }
        public virtual DbSet<SignaleElemente> SignaleElemente { get; set; }
        public virtual DbSet<SignaleNachlauf> SignaleNachlauf { get; set; }
        public virtual DbSet<SignaleStellauftraege> SignaleStellauftraege { get; set; }
        public virtual DbSet<SignaleVorsignale> SignaleVorsignale { get; set; }
        public virtual DbSet<SignaleWenden> SignaleWenden { get; set; }
        public virtual DbSet<SignaleZielpunkte> SignaleZielpunkte { get; set; }
        public virtual DbSet<Stellwerke> Stellwerke { get; set; }
        public virtual DbSet<StellwerkeBereiche> StellwerkeBereiche { get; set; }
        public virtual DbSet<StellwerkeBetriebsstellen> StellwerkeBetriebsstellen { get; set; }
        public virtual DbSet<StellwerkeHandlungUnzulaessig> StellwerkeHandlungUnzulaessig { get; set; }
        public virtual DbSet<StellwerkeMerkschilder> StellwerkeMerkschilder { get; set; }
        public virtual DbSet<StellwerkeMerkspeicher> StellwerkeMerkspeicher { get; set; }
        public virtual DbSet<StellwerkeProtokoll> StellwerkeProtokoll { get; set; }
        public virtual DbSet<StellwerkeStellauftraege> StellwerkeStellauftraege { get; set; }
        public virtual DbSet<StellwerkeUz> StellwerkeUz { get; set; }
        public virtual DbSet<StellwerkeZuordnung> StellwerkeZuordnung { get; set; }
        public virtual DbSet<Stoerungen> Stoerungen { get; set; }
        public virtual DbSet<Strecken> Strecken { get; set; }
        public virtual DbSet<StreckenAbschnitte> StreckenAbschnitte { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserAnmeldungenAktiv> UserAnmeldungenAktiv { get; set; }
        public virtual DbSet<UserRechteliste> UserRechteliste { get; set; }
        public virtual DbSet<UserStellwerke> UserStellwerke { get; set; }
        public virtual DbSet<UserZugriffsrechte> UserZugriffsrechte { get; set; }
        public virtual DbSet<WeichenAbhaengigkeit> WeichenAbhaengigkeit { get; set; }
        public virtual DbSet<WeichenStellauftraege> WeichenStellauftraege { get; set; }
        public virtual DbSet<WeichenVorzugslage> WeichenVorzugslage { get; set; }
        public virtual DbSet<ZuegeVerkehrsarten> ZuegeVerkehrsarten { get; set; }
        public virtual DbSet<ZuegeZuggattungen> ZuegeZuggattungen { get; set; }
        public virtual DbSet<ZuglenkungAktuell> ZuglenkungAktuell { get; set; }
        public virtual DbSet<ZuglenkungSelbststellbetrieb> ZuglenkungSelbststellbetrieb { get; set; }
        public virtual DbSet<ZuglenkungTemp> ZuglenkungTemp { get; set; }

        // Unable to generate entity type for table 'config'. Please see the warning messages.
        // Unable to generate entity type for table 'fahrplan_sessionfahrplan'. Please see the warning messages.
        // Unable to generate entity type for table 'fahrplan_umlaeufe'. Please see the warning messages.
        // Unable to generate entity type for table 'fahrplan_umlaeufe_elemente'. Please see the warning messages.
        // Unable to generate entity type for table 'fplo_fzm'. Please see the warning messages.
        // Unable to generate entity type for table 'fzm'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Add Connection String
                optionsBuilder.UseMySql("");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bahnuebergaenge>(entity =>
            {
                entity.ToTable("bahnuebergaenge");

                entity.HasIndex(e => e.Betriebsstelle)
                    .HasName("betriebsstelle");

                entity.HasIndex(e => e.Freimeldeart)
                    .HasName("freimeldeart");

                entity.HasIndex(e => e.Stoerung)
                    .HasName("stoerung");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Betriebsstelle)
                    .IsRequired()
                    .HasColumnName("betriebsstelle")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Bezeichnung)
                    .IsRequired()
                    .HasColumnName("bezeichnung")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Dauereinschaltung)
                    .HasColumnName("dauereinschaltung")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Freimeldeart)
                    .IsRequired()
                    .HasColumnName("freimeldeart")
                    .HasColumnType("enum('gfa','manuell','halbschranke')");

                entity.Property(e => e.Lock)
                    .HasColumnName("lock")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Sicherung)
                    .HasColumnName("sicherung")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Stoerung)
                    .IsRequired()
                    .HasColumnName("stoerung")
                    .HasColumnType("enum('keine','gfaverzoegerung')");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.HasOne(d => d.BetriebsstelleNavigation)
                    .WithMany(p => p.Bahnuebergaenge)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Betriebsstelle)
                    .HasConstraintName("bahnuebergaenge_ibfk_1");
            });

            modelBuilder.Entity<BahnuebergaengeEinstellung>(entity =>
            {
                entity.ToTable("bahnuebergaenge_einstellung");

                entity.HasIndex(e => e.AufloesungId)
                    .HasName("aufloesung_id");

                entity.HasIndex(e => e.BahnuebergangId)
                    .HasName("bahnuebergang_id");

                entity.HasIndex(e => e.EinstellungTimestamp)
                    .HasName("einstellung_timestamp");

                entity.HasIndex(e => e.FahrstrasseId)
                    .HasName("fahrstrasse_id");

                entity.HasIndex(e => e.GleisId)
                    .HasName("gleis_id");

                entity.HasIndex(e => e.Status)
                    .HasName("status");

                entity.HasIndex(e => e.Timestamp)
                    .HasName("timestamp");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Art)
                    .IsRequired()
                    .HasColumnName("art")
                    .HasColumnType("enum('zug','manuell','dauer')");

                entity.Property(e => e.AufloesungId)
                    .HasColumnName("aufloesung_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BahnuebergangId)
                    .HasColumnName("bahnuebergang_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.EinstellungTimestamp)
                    .HasColumnName("einstellung_timestamp")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FahrstrasseId)
                    .HasColumnName("fahrstrasse_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.GleisId)
                    .HasColumnName("gleis_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<BahnuebergaengeElemente>(entity =>
            {
                entity.ToTable("bahnuebergaenge_elemente");

                entity.HasIndex(e => e.AufloesungId)
                    .HasName("aufloesung_id");

                entity.HasIndex(e => e.BahnuebergangId)
                    .HasName("bahnuebergang_id");

                entity.HasIndex(e => e.GrundstellungDir)
                    .HasName("grundstellung_dir");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.HasIndex(e => e.ZielDir)
                    .HasName("ziel_dir");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AufloesungId)
                    .HasColumnName("aufloesung_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BahnuebergangId)
                    .HasColumnName("bahnuebergang_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.GrundstellungDir)
                    .HasColumnName("grundstellung_dir")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ZielDir)
                    .HasColumnName("ziel_dir")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Aufloesung)
                    .WithMany(p => p.BahnuebergaengeElementeAufloesung)
                    .HasForeignKey(d => d.AufloesungId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("bahnuebergaenge_elemente_ibfk_3");

                entity.HasOne(d => d.Bahnuebergang)
                    .WithMany(p => p.BahnuebergaengeElemente)
                    .HasForeignKey(d => d.BahnuebergangId)
                    .HasConstraintName("bahnuebergaenge_elemente_ibfk_1");

                entity.HasOne(d => d.Infra)
                    .WithMany(p => p.BahnuebergaengeElementeInfra)
                    .HasForeignKey(d => d.InfraId)
                    .HasConstraintName("bahnuebergaenge_elemente_ibfk_2");
            });

            modelBuilder.Entity<BahnuebergaengeStellauftraege>(entity =>
            {
                entity.ToTable("bahnuebergaenge_stellauftraege");

                entity.HasIndex(e => e.Ausfuehrung)
                    .HasName("ausfuehrung");

                entity.HasIndex(e => e.BahnuebergangId)
                    .HasName("bahnuebergang_id");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.HasIndex(e => e.Zeitpunkt)
                    .HasName("zeitpunkt");

                entity.HasIndex(e => e.ZielDir)
                    .HasName("ziel_dir");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Ausfuehrung)
                    .HasColumnName("ausfuehrung")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BahnuebergangId)
                    .HasColumnName("bahnuebergang_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Zeitpunkt)
                    .HasColumnName("zeitpunkt")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ZielDir)
                    .HasColumnName("ziel_dir")
                    .HasColumnType("int(1)");
            });

            modelBuilder.Entity<Betra>(entity =>
            {
                entity.ToTable("betra");

                entity.HasIndex(e => e.Nummer)
                    .HasName("nummer")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Arbeitszeit)
                    .IsRequired()
                    .HasColumnName("arbeitszeit")
                    .HasColumnType("mediumtext");

                entity.Property(e => e.Evu)
                    .IsRequired()
                    .HasColumnName("evu")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Grund)
                    .IsRequired()
                    .HasColumnName("grund")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.GueltigAb)
                    .HasColumnName("gueltig_ab")
                    .HasColumnType("datetime");

                entity.Property(e => e.GueltigBis)
                    .HasColumnName("gueltig_bis")
                    .HasColumnType("datetime");

                entity.Property(e => e.GvAusfall)
                    .IsRequired()
                    .HasColumnName("gv_ausfall")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.GvBedarfsperre)
                    .IsRequired()
                    .HasColumnName("gv_bedarfsperre")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.GvNeu)
                    .IsRequired()
                    .HasColumnName("gv_neu")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.GvUebrige)
                    .IsRequired()
                    .HasColumnName("gv_uebrige")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.GvUmleitung)
                    .IsRequired()
                    .HasColumnName("gv_umleitung")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.LageBahnhof)
                    .IsRequired()
                    .HasColumnName("lage_bahnhof")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.LagePlan)
                    .IsRequired()
                    .HasColumnName("lage_plan")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.LageStrecke)
                    .IsRequired()
                    .HasColumnName("lage_strecke")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Nummer)
                    .IsRequired()
                    .HasColumnName("nummer")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.PvAusfall)
                    .IsRequired()
                    .HasColumnName("pv_ausfall")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.PvBedarfsperre)
                    .IsRequired()
                    .HasColumnName("pv_bedarfsperre")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.PvNeu)
                    .IsRequired()
                    .HasColumnName("pv_neu")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.PvUebrige)
                    .IsRequired()
                    .HasColumnName("pv_uebrige")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.PvUmleitung)
                    .IsRequired()
                    .HasColumnName("pv_umleitung")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Sonstiges)
                    .IsRequired()
                    .HasColumnName("sonstiges")
                    .HasColumnType("text");

                entity.Property(e => e.Sperrung)
                    .IsRequired()
                    .HasColumnName("sperrung")
                    .HasColumnType("mediumtext");

                entity.Property(e => e.SperrungHinweis)
                    .IsRequired()
                    .HasColumnName("sperrung_hinweis")
                    .HasColumnType("mediumtext");

                entity.Property(e => e.Verfasser)
                    .IsRequired()
                    .HasColumnName("verfasser")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Weitere)
                    .IsRequired()
                    .HasColumnName("weitere")
                    .HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<BetriebsstellenArt>(entity =>
            {
                entity.ToTable("betriebsstellen_art");

                entity.HasIndex(e => e.Art)
                    .HasName("art")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Art)
                    .IsRequired()
                    .HasColumnName("art")
                    .HasColumnType("char(6)");

                entity.Property(e => e.Artname)
                    .IsRequired()
                    .HasColumnName("artname")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Zuganfang)
                    .HasColumnName("zuganfang")
                    .HasColumnType("int(1)");
            });

            modelBuilder.Entity<BetriebsstellenErsetzung>(entity =>
            {
                entity.ToTable("betriebsstellen_ersetzung");

                entity.HasIndex(e => e.Art)
                    .HasName("art");

                entity.HasIndex(e => e.Bezug)
                    .HasName("bezug");

                entity.HasIndex(e => e.Ersatz)
                    .HasName("ersatz");

                entity.HasIndex(e => e.Ursprung)
                    .HasName("ursprung_2");

                entity.HasIndex(e => e.Ursprung2)
                    .HasName("ursprung2");

                entity.HasIndex(e => e.Ursprung3)
                    .HasName("ursprung3");

                entity.HasIndex(e => new { e.Ursprung, e.Bezug, e.Ursprung2, e.Ursprung3 })
                    .HasName("ursprung")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Art)
                    .IsRequired()
                    .HasColumnName("art")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Bezug)
                    .IsRequired()
                    .HasColumnName("bezug")
                    .HasColumnType("char(10)");

                entity.Property(e => e.Ersatz)
                    .IsRequired()
                    .HasColumnName("ersatz")
                    .HasColumnType("char(10)");

                entity.Property(e => e.Ursprung)
                    .IsRequired()
                    .HasColumnName("ursprung")
                    .HasColumnType("char(10)");

                entity.Property(e => e.Ursprung2)
                    .HasColumnName("ursprung2")
                    .HasColumnType("char(10)");

                entity.Property(e => e.Ursprung3)
                    .HasColumnName("ursprung3")
                    .HasColumnType("char(10)");

                entity.HasOne(d => d.BezugNavigation)
                    .WithMany(p => p.BetriebsstellenErsetzungBezugNavigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Bezug)
                    .HasConstraintName("betriebsstellen_ersetzung_ibfk_3");

                entity.HasOne(d => d.ErsatzNavigation)
                    .WithMany(p => p.BetriebsstellenErsetzungErsatzNavigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Ersatz)
                    .HasConstraintName("betriebsstellen_ersetzung_ibfk_2");

                entity.HasOne(d => d.UrsprungNavigation)
                    .WithMany(p => p.BetriebsstellenErsetzungUrsprungNavigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Ursprung)
                    .HasConstraintName("betriebsstellen_ersetzung_ibfk_1");

                entity.HasOne(d => d.Ursprung2Navigation)
                    .WithMany(p => p.BetriebsstellenErsetzungUrsprung2Navigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Ursprung2)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("betriebsstellen_ersetzung_ibfk_4");

                entity.HasOne(d => d.Ursprung3Navigation)
                    .WithMany(p => p.BetriebsstellenErsetzungUrsprung3Navigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Ursprung3)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("betriebsstellen_ersetzung_ibfk_5");
            });

            modelBuilder.Entity<BetriebsstellenFahrtrichtung>(entity =>
            {
                entity.ToTable("betriebsstellen_fahrtrichtung");

                entity.HasIndex(e => e.Bezug)
                    .HasName("bezug");

                entity.HasIndex(e => e.Fahrtrichtung)
                    .HasName("fahrtrichtung");

                entity.HasIndex(e => e.Nachbar)
                    .HasName("nachbar");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bezug)
                    .IsRequired()
                    .HasColumnName("bezug")
                    .HasColumnType("char(10)");

                entity.Property(e => e.Fahrtrichtung)
                    .HasColumnName("fahrtrichtung")
                    .HasColumnType("int(2)");

                entity.Property(e => e.Nachbar)
                    .IsRequired()
                    .HasColumnName("nachbar")
                    .HasColumnType("char(10)");

                entity.HasOne(d => d.BezugNavigation)
                    .WithMany(p => p.BetriebsstellenFahrtrichtungBezugNavigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Bezug)
                    .HasConstraintName("betriebsstellen_fahrtrichtung_ibfk_2");

                entity.HasOne(d => d.NachbarNavigation)
                    .WithMany(p => p.BetriebsstellenFahrtrichtungNachbarNavigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Nachbar)
                    .HasConstraintName("betriebsstellen_fahrtrichtung_ibfk_1");
            });

            modelBuilder.Entity<BetriebsstellenHinweise>(entity =>
            {
                entity.ToTable("betriebsstellen_hinweise");

                entity.HasIndex(e => e.Bezug)
                    .HasName("bezug");

                entity.HasIndex(e => e.Nach)
                    .HasName("nach");

                entity.HasIndex(e => e.Von)
                    .HasName("von");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bezug)
                    .IsRequired()
                    .HasColumnName("bezug")
                    .HasColumnType("char(6)");

                entity.Property(e => e.Gleis)
                    .IsRequired()
                    .HasColumnName("gleis")
                    .HasColumnType("char(6)");

                entity.Property(e => e.Hinweistext)
                    .IsRequired()
                    .HasColumnName("hinweistext")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Nach)
                    .IsRequired()
                    .HasColumnName("nach")
                    .HasColumnType("char(6)");

                entity.Property(e => e.Von)
                    .IsRequired()
                    .HasColumnName("von")
                    .HasColumnType("char(6)");

                entity.Property(e => e.ZielStreckengleis)
                    .HasColumnName("ziel_streckengleis")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.BezugNavigation)
                    .WithMany(p => p.BetriebsstellenHinweiseBezugNavigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Bezug)
                    .HasConstraintName("betriebsstellen_hinweise_ibfk_3");

                entity.HasOne(d => d.NachNavigation)
                    .WithMany(p => p.BetriebsstellenHinweiseNachNavigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Nach)
                    .HasConstraintName("betriebsstellen_hinweise_ibfk_2");

                entity.HasOne(d => d.VonNavigation)
                    .WithMany(p => p.BetriebsstellenHinweiseVonNavigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Von)
                    .HasConstraintName("betriebsstellen_hinweise_ibfk_1");
            });

            modelBuilder.Entity<BetriebsstellenListe>(entity =>
            {
                entity.ToTable("betriebsstellen_liste");

                entity.HasIndex(e => e.Art)
                    .HasName("art");

                entity.HasIndex(e => e.Bstart)
                    .HasName("bstart");

                entity.HasIndex(e => e.Kuerzel)
                    .HasName("kuerzel")
                    .IsUnique();

                entity.HasIndex(e => e.KuerzelGleis)
                    .HasName("kuerzel_gleis");

                entity.HasIndex(e => e.Nummer)
                    .HasName("nummer");

                entity.HasIndex(e => e.ParentKuerzel)
                    .HasName("parent_kuerzel");

                entity.HasIndex(e => e.Wirkrichtung)
                    .HasName("wirkrichtung");

                entity.HasIndex(e => e.Zl)
                    .HasName("zl");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Art)
                    .IsRequired()
                    .HasColumnName("art")
                    .HasColumnType("char(3)");

                entity.Property(e => e.Bstart)
                    .IsRequired()
                    .HasColumnName("bstart")
                    .HasColumnType("enum('bahnhof','abzweig','blockstelle','haltepunkt','--')");

                entity.Property(e => e.Kuerzel)
                    .IsRequired()
                    .HasColumnName("kuerzel")
                    .HasColumnType("char(10)");

                entity.Property(e => e.KuerzelGleis)
                    .IsRequired()
                    .HasColumnName("kuerzel_gleis")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.Latitude).HasColumnName("latitude");

                entity.Property(e => e.Longitude).HasColumnName("longitude");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Nummer)
                    .HasColumnName("nummer")
                    .HasColumnType("int(3)");

                entity.Property(e => e.ParentKuerzel)
                    .HasColumnName("parent_kuerzel")
                    .HasColumnType("char(11)");

                entity.Property(e => e.Wirkrichtung)
                    .HasColumnName("wirkrichtung")
                    .HasColumnType("int(2)");

                entity.Property(e => e.Zl)
                    .HasColumnName("zl")
                    .HasColumnType("int(1)");

                entity.HasOne(d => d.ArtNavigation)
                    .WithMany(p => p.BetriebsstellenListe)
                    .HasPrincipalKey(p => p.Art)
                    .HasForeignKey(d => d.Art)
                    .HasConstraintName("betriebsstellen_liste_ibfk_1");

                entity.HasOne(d => d.KuerzelGleisNavigation)
                    .WithMany(p => p.InverseKuerzelGleisNavigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.KuerzelGleis)
                    .HasConstraintName("betriebsstellen_liste_ibfk_3");

                entity.HasOne(d => d.ParentKuerzelNavigation)
                    .WithMany(p => p.InverseParentKuerzelNavigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.ParentKuerzel)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("betriebsstellen_liste_ibfk_2");
            });

            modelBuilder.Entity<BetriebsstellenStatus>(entity =>
            {
                entity.ToTable("betriebsstellen_status");

                entity.HasIndex(e => e.Kuerzel)
                    .HasName("kuerzel");

                entity.HasIndex(e => e.Status)
                    .HasName("status");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Kuerzel)
                    .IsRequired()
                    .HasColumnName("kuerzel")
                    .HasColumnType("char(10)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasColumnType("enum('özF','selbststellbetrieb','durchgeschaltet')")
                    .HasDefaultValueSql("'özF'");

                entity.HasOne(d => d.KuerzelNavigation)
                    .WithMany(p => p.BetriebsstellenStatus)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Kuerzel)
                    .HasConstraintName("betriebsstellen_status_ibfk_1");
            });

            modelBuilder.Entity<BetriebsstellenStatusElemente>(entity =>
            {
                entity.ToTable("betriebsstellen_status_elemente");

                entity.HasIndex(e => e.FahrstrasseId)
                    .HasName("fahrstrasse_id");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.HasIndex(e => e.SignalId)
                    .HasName("signal_id");

                entity.HasIndex(e => e.StatusId)
                    .HasName("status_id");

                entity.HasIndex(e => e.ZielfeldId)
                    .HasName("zielfeld_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FahrstrasseId)
                    .HasColumnName("fahrstrasse_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraDir)
                    .HasColumnName("infra_dir")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SignalId)
                    .HasColumnName("signal_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.StatusId)
                    .HasColumnName("status_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ZielfeldId)
                    .HasColumnName("zielfeld_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Fahrstrasse)
                    .WithMany(p => p.BetriebsstellenStatusElemente)
                    .HasForeignKey(d => d.FahrstrasseId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("betriebsstellen_status_elemente_ibfk_4");

                entity.HasOne(d => d.Infra)
                    .WithMany(p => p.BetriebsstellenStatusElemente)
                    .HasForeignKey(d => d.InfraId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("betriebsstellen_status_elemente_ibfk_2");

                entity.HasOne(d => d.Signal)
                    .WithMany(p => p.BetriebsstellenStatusElemente)
                    .HasForeignKey(d => d.SignalId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("betriebsstellen_status_elemente_ibfk_3");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.BetriebsstellenStatusElemente)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("betriebsstellen_status_elemente_ibfk_1");

                entity.HasOne(d => d.Zielfeld)
                    .WithMany(p => p.BetriebsstellenStatusElemente)
                    .HasForeignKey(d => d.ZielfeldId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("betriebsstellen_status_elemente_ibfk_5");
            });

            modelBuilder.Entity<Blockfelder>(entity =>
            {
                entity.ToTable("blockfelder");

                entity.HasIndex(e => e.BlockfeldId)
                    .HasName("blockfeld_id");

                entity.HasIndex(e => e.ErlaubnisId)
                    .HasName("erlaubnis_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bezeichnung)
                    .IsRequired()
                    .HasColumnName("bezeichnung")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.BlockfeldId)
                    .HasColumnName("blockfeld_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ErlaubnisDir)
                    .HasColumnName("erlaubnis_dir")
                    .HasColumnType("int(1)");

                entity.Property(e => e.ErlaubnisId)
                    .HasColumnName("erlaubnis_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Blockfeld)
                    .WithMany(p => p.BlockfelderBlockfeld)
                    .HasForeignKey(d => d.BlockfeldId)
                    .HasConstraintName("blockfelder_ibfk_1");

                entity.HasOne(d => d.Erlaubnis)
                    .WithMany(p => p.BlockfelderErlaubnis)
                    .HasForeignKey(d => d.ErlaubnisId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("blockfelder_ibfk_2");
            });

            modelBuilder.Entity<BlockfelderEinstellung>(entity =>
            {
                entity.ToTable("blockfelder_einstellung");

                entity.HasIndex(e => e.Bluem)
                    .HasName("bluem");

                entity.HasIndex(e => e.FahrstrasseId)
                    .HasName("fahrstrasse_id");

                entity.HasIndex(e => e.Raeumungsmelder)
                    .HasName("raeumungsmelder");

                entity.HasIndex(e => e.RueckblockfeldId)
                    .HasName("rueckblockfeld_id");

                entity.HasIndex(e => e.SignalId)
                    .HasName("signal_id");

                entity.HasIndex(e => e.Timestamp)
                    .HasName("timestamp");

                entity.HasIndex(e => e.VorblockfeldId)
                    .HasName("vorblockfeld_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BlockId1)
                    .HasColumnName("block_id1")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BlockId2)
                    .HasColumnName("block_id2")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bluem)
                    .IsRequired()
                    .HasColumnName("bluem")
                    .HasColumnType("enum('0','1')");

                entity.Property(e => e.FahrstrasseId)
                    .HasColumnName("fahrstrasse_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Raeumungsmelder)
                    .HasColumnName("raeumungsmelder")
                    .HasColumnType("enum('0','1')");

                entity.Property(e => e.RueckblockfeldId)
                    .HasColumnName("rueckblockfeld_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SignalId)
                    .HasColumnName("signal_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.StreckenwiederholungssperreId)
                    .HasColumnName("streckenwiederholungssperre_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.VorblockfeldId)
                    .HasColumnName("vorblockfeld_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<BlockfelderElemente>(entity =>
            {
                entity.ToTable("blockfelder_elemente");

                entity.HasIndex(e => e.BlockfeldId)
                    .HasName("blockfeld_id");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BlockfeldId)
                    .HasColumnName("blockfeld_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Blockfeld)
                    .WithMany(p => p.BlockfelderElemente)
                    .HasForeignKey(d => d.BlockfeldId)
                    .HasConstraintName("blockfelder_elemente_ibfk_2");

                entity.HasOne(d => d.Infra)
                    .WithMany(p => p.BlockfelderElemente)
                    .HasForeignKey(d => d.InfraId)
                    .HasConstraintName("blockfelder_elemente_ibfk_1");
            });

            modelBuilder.Entity<Dwege>(entity =>
            {
                entity.ToTable("dwege");

                entity.HasIndex(e => e.AufloeseId)
                    .HasName("aufloese_id");

                entity.HasIndex(e => e.AufloeseVerzoegerung)
                    .HasName("aufloese_verzoegerung");

                entity.HasIndex(e => e.Betriebsstelle)
                    .HasName("betriebsstelle");

                entity.HasIndex(e => e.SignalId)
                    .HasName("signal_id");

                entity.HasIndex(e => e.Vmax)
                    .HasName("vmax");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AufloeseId)
                    .HasColumnName("aufloese_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AufloeseVerzoegerung)
                    .HasColumnName("aufloese_verzoegerung")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Betriebsstelle)
                    .IsRequired()
                    .HasColumnName("betriebsstelle")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Bezeichnung)
                    .IsRequired()
                    .HasColumnName("bezeichnung")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Ende)
                    .IsRequired()
                    .HasColumnName("ende")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.LaengeIst)
                    .HasColumnName("laenge_ist")
                    .HasColumnType("int(11)");

                entity.Property(e => e.LaengeSoll)
                    .HasColumnName("laenge_soll")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Neigung).HasColumnName("neigung");

                entity.Property(e => e.SignalId)
                    .HasColumnName("signal_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Vmax)
                    .IsRequired()
                    .HasColumnName("vmax")
                    .HasColumnType("varchar(255)");

                entity.HasOne(d => d.Aufloese)
                    .WithMany(p => p.Dwege)
                    .HasForeignKey(d => d.AufloeseId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("dwege_ibfk_1");

                entity.HasOne(d => d.BetriebsstelleNavigation)
                    .WithMany(p => p.Dwege)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Betriebsstelle)
                    .HasConstraintName("dwege_ibfk_2");

                entity.HasOne(d => d.Signal)
                    .WithMany(p => p.Dwege)
                    .HasForeignKey(d => d.SignalId)
                    .HasConstraintName("dwege_ibfk_3");
            });

            modelBuilder.Entity<DwegeEinstellung>(entity =>
            {
                entity.ToTable("dwege_einstellung");

                entity.HasIndex(e => e.AufloeseId)
                    .HasName("aufloese_id");

                entity.HasIndex(e => e.Dfm)
                    .HasName("dfm");

                entity.HasIndex(e => e.DwegId)
                    .HasName("dweg_id");

                entity.HasIndex(e => e.IsDurchfahrt)
                    .HasName("is_durchfahrt");

                entity.HasIndex(e => e.SignalId)
                    .HasName("signal_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AufloeseId)
                    .HasColumnName("aufloese_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AufloeseVerzoegerung)
                    .HasColumnName("aufloese_verzoegerung")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Dfm)
                    .HasColumnName("dfm")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DwegId)
                    .HasColumnName("dweg_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FahrstrasseId)
                    .HasColumnName("fahrstrasse_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IsDurchfahrt)
                    .HasColumnName("is_durchfahrt")
                    .HasColumnType("int(1)");

                entity.Property(e => e.SignalId)
                    .HasColumnName("signal_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'");
            });

            modelBuilder.Entity<DwegeEinstellungElemente>(entity =>
            {
                entity.ToTable("dwege_einstellung_elemente");

                entity.HasIndex(e => e.DwegId)
                    .HasName("dweg_id");

                entity.HasIndex(e => e.InfraDir)
                    .HasName("infra_dir");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.HasIndex(e => e.SignalId)
                    .HasName("signal_id");

                entity.HasIndex(e => e.Timestamp)
                    .HasName("timestamp");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DwegId)
                    .HasColumnName("dweg_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraDir)
                    .HasColumnName("infra_dir")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SignalId)
                    .HasColumnName("signal_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'");
            });

            modelBuilder.Entity<DwegeElemente>(entity =>
            {
                entity.ToTable("dwege_elemente");

                entity.HasIndex(e => e.Dir)
                    .HasName("dir");

                entity.HasIndex(e => e.DwegId)
                    .HasName("dweg_id");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.HasIndex(e => e.Reihenfolge)
                    .HasName("reihenfolge");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Dir)
                    .HasColumnName("dir")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DwegId)
                    .HasColumnName("dweg_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Reihenfolge)
                    .HasColumnName("reihenfolge")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.DirNavigation)
                    .WithMany(p => p.DwegeElemente)
                    .HasPrincipalKey(p => p.Wert)
                    .HasForeignKey(d => d.Dir)
                    .HasConstraintName("dwege_elemente_ibfk_3");

                entity.HasOne(d => d.Dweg)
                    .WithMany(p => p.DwegeElemente)
                    .HasForeignKey(d => d.DwegId)
                    .HasConstraintName("dwege_elemente_ibfk_1");

                entity.HasOne(d => d.Infra)
                    .WithMany(p => p.DwegeElemente)
                    .HasForeignKey(d => d.InfraId)
                    .HasConstraintName("dwege_elemente_ibfk_2");
            });

            modelBuilder.Entity<Erlaubnisfelder>(entity =>
            {
                entity.ToTable("erlaubnisfelder");

                entity.HasIndex(e => e.Betriebsstelle0)
                    .HasName("betriebsstelle0");

                entity.HasIndex(e => e.Betriebsstelle1)
                    .HasName("betriebsstelle1");

                entity.HasIndex(e => e.ErlaubnisfeldId)
                    .HasName("erlaubnisfeld_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Betriebsstelle0)
                    .IsRequired()
                    .HasColumnName("betriebsstelle0")
                    .HasColumnType("char(10)");

                entity.Property(e => e.Betriebsstelle1)
                    .IsRequired()
                    .HasColumnName("betriebsstelle1")
                    .HasColumnType("char(10)");

                entity.Property(e => e.ErlaubnisfeldId)
                    .HasColumnName("erlaubnisfeld_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Betriebsstelle0Navigation)
                    .WithMany(p => p.ErlaubnisfelderBetriebsstelle0Navigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Betriebsstelle0)
                    .HasConstraintName("erlaubnisfelder_ibfk_2");

                entity.HasOne(d => d.Betriebsstelle1Navigation)
                    .WithMany(p => p.ErlaubnisfelderBetriebsstelle1Navigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Betriebsstelle1)
                    .HasConstraintName("erlaubnisfelder_ibfk_3");

                entity.HasOne(d => d.Erlaubnisfeld)
                    .WithMany(p => p.Erlaubnisfelder)
                    .HasForeignKey(d => d.ErlaubnisfeldId)
                    .HasConstraintName("erlaubnisfelder_ibfk_1");
            });

            modelBuilder.Entity<ErlaubnisfelderAufhebung>(entity =>
            {
                entity.ToTable("erlaubnisfelder_aufhebung");

                entity.HasIndex(e => e.ErlaubnisfeldId)
                    .HasName("erlaubnisfeld_id");

                entity.HasIndex(e => e.UserAktivId)
                    .HasName("user_aktiv_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ErlaubnisfeldId)
                    .HasColumnName("erlaubnisfeld_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserAktivId)
                    .HasColumnName("user_aktiv_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.UserAktiv)
                    .WithMany(p => p.ErlaubnisfelderAufhebung)
                    .HasForeignKey(d => d.UserAktivId)
                    .HasConstraintName("erlaubnisfelder_aufhebung_ibfk_2");
            });

            modelBuilder.Entity<Fahrplan>(entity =>
            {
                entity.ToTable("fahrplan");

                entity.HasIndex(e => e.Name)
                    .HasName("name")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Aktiv)
                    .IsRequired()
                    .HasColumnName("aktiv")
                    .HasColumnType("enum('ja','nein')");

                entity.Property(e => e.Autor)
                    .IsRequired()
                    .HasColumnName("autor")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Bezeichnung)
                    .IsRequired()
                    .HasColumnName("bezeichnung")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.GueltigAb)
                    .HasColumnName("gueltig_ab")
                    .HasColumnType("date");

                entity.Property(e => e.GueltigBis)
                    .HasColumnName("gueltig_bis")
                    .HasColumnType("date");

                entity.Property(e => e.Importquelle)
                    .IsRequired()
                    .HasColumnName("importquelle")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Typ)
                    .IsRequired()
                    .HasColumnName("typ")
                    .HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<FahrplanGrundaufstellung>(entity =>
            {
                entity.ToTable("fahrplan_grundaufstellung");

                entity.HasIndex(e => e.UmlaufId)
                    .HasName("umlauf_id");

                entity.HasIndex(e => e.ZugId)
                    .HasName("zug_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Betriebsstelle)
                    .IsRequired()
                    .HasColumnName("betriebsstelle")
                    .HasColumnType("varchar(5)");

                entity.Property(e => e.FahrzeugAdresse)
                    .HasColumnName("fahrzeug_adresse")
                    .HasColumnType("int(11)");

                entity.Property(e => e.GbtId)
                    .HasColumnName("gbt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.Property(e => e.UmlaufId)
                    .HasColumnName("umlauf_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ZugId)
                    .HasColumnName("zug_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Zugnummer)
                    .HasColumnName("zugnummer")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<FahrplanMindesthaltezeiten>(entity =>
            {
                entity.ToTable("fahrplan_mindesthaltezeiten");

                entity.HasIndex(e => e.Betriebsstelle)
                    .HasName("betriebsstelle_kuerzel");

                entity.HasIndex(e => e.Gleis)
                    .HasName("gleis");

                entity.HasIndex(e => e.ZuggattungId)
                    .HasName("zuggattung_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Betriebsstelle)
                    .HasColumnName("betriebsstelle")
                    .HasColumnType("varchar(5)");

                entity.Property(e => e.Gleis)
                    .HasColumnName("gleis")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Mindesthaltezeit)
                    .HasColumnName("mindesthaltezeit")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ZuggattungId)
                    .HasColumnName("zuggattung_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.BetriebsstelleNavigation)
                    .WithMany(p => p.FahrplanMindesthaltezeiten)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Betriebsstelle)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fahrplan_mindesthaltezeiten_ibfk_1");

                entity.HasOne(d => d.Zuggattung)
                    .WithMany(p => p.FahrplanMindesthaltezeiten)
                    .HasForeignKey(d => d.ZuggattungId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fahrplan_mindesthaltezeiten_ibfk_2");
            });

            modelBuilder.Entity<FahrplanSession>(entity =>
            {
                entity.ToTable("fahrplan_session");

                entity.HasIndex(e => e.BetriebsstelleXap)
                    .HasName("betriebsstelle_xap");

                entity.HasIndex(e => e.BetriebsstelleXlg)
                    .HasName("betriebsstelle_xlg");

                entity.HasIndex(e => e.BetriebsstelleXwf)
                    .HasName("betriebsstelle_xwf");

                entity.HasIndex(e => e.FahrplanId)
                    .HasName("fahrplan_id");

                entity.HasIndex(e => e.Sessionkey)
                    .HasName("sessionkey");

                entity.HasIndex(e => e.Status)
                    .HasName("status");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BetriebsstelleXap)
                    .IsRequired()
                    .HasColumnName("betriebsstelle_xap")
                    .HasColumnType("enum('özF','durchgeschaltet')")
                    .HasDefaultValueSql("'özF'");

                entity.Property(e => e.BetriebsstelleXlg)
                    .IsRequired()
                    .HasColumnName("betriebsstelle_xlg")
                    .HasColumnType("enum('özF','selbststellbetrieb')")
                    .HasDefaultValueSql("'özF'");

                entity.Property(e => e.BetriebsstelleXwf)
                    .IsRequired()
                    .HasColumnName("betriebsstelle_xwf")
                    .HasColumnType("enum('özF','selbststellbetrieb')")
                    .HasDefaultValueSql("'özF'");

                entity.Property(e => e.FahrplanId)
                    .HasColumnName("fahrplan_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FzsFahrplanbasiert)
                    .HasColumnName("fzs_fahrplanbasiert")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.RealStartzeit)
                    .HasColumnName("real_startzeit")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Sessionkey)
                    .HasColumnName("sessionkey")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.SimEndzeit)
                    .HasColumnName("sim_endzeit")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SimPausezeit)
                    .HasColumnName("sim_pausezeit")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SimStartzeit)
                    .HasColumnName("sim_startzeit")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SimWochentag)
                    .IsRequired()
                    .HasColumnName("sim_wochentag")
                    .HasColumnType("varchar(7)");

                entity.Property(e => e.Skalierung).HasColumnName("skalierung");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Timeshift)
                    .HasColumnName("timeshift")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.ZnAutowechsel)
                    .HasColumnName("zn_autowechsel")
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("'1'");

                entity.HasOne(d => d.Fahrplan)
                    .WithMany(p => p.FahrplanSession)
                    .HasForeignKey(d => d.FahrplanId)
                    .HasConstraintName("fahrplan_session_ibfk_1");
            });

            modelBuilder.Entity<FahrplanSessionfahrplanTracker>(entity =>
            {
                entity.ToTable("fahrplan_sessionfahrplan_tracker");

                entity.HasIndex(e => e.FahrplanSessionfahrplanId)
                    .HasName("fahrplan_sessionfahrplan_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FahrplanSessionfahrplanId)
                    .HasColumnName("fahrplan_sessionfahrplan_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<FahrplanSessionzuege>(entity =>
            {
                entity.ToTable("fahrplan_sessionzuege");

                entity.HasIndex(e => e.FahrzeugId)
                    .HasName("fahrzeug_id");

                entity.HasIndex(e => e.Triebfahrzeug)
                    .HasName("triebfahrzeug");

                entity.HasIndex(e => e.TriebfahrzeugIst)
                    .HasName("triebfahrzeug_ist");

                entity.HasIndex(e => e.UebergangNachZugId)
                    .HasName("uebergang_nach");

                entity.HasIndex(e => e.UebergangVonZugId)
                    .HasName("uebergang_von");

                entity.HasIndex(e => e.VerkehrstageBin)
                    .HasName("verkehrstage_bin");

                entity.HasIndex(e => e.ZuggattungId)
                    .HasName("zuggattung");

                entity.HasIndex(e => e.Zugnummer)
                    .HasName("zugnummer");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bemerkungen)
                    .IsRequired()
                    .HasColumnName("bemerkungen")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Bremssystem)
                    .IsRequired()
                    .HasColumnName("bremssystem")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.FahrzeugId)
                    .HasColumnName("fahrzeug_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Mbr)
                    .HasColumnName("mbr")
                    .HasColumnType("int(5)");

                entity.Property(e => e.Triebfahrzeug)
                    .HasColumnName("triebfahrzeug")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TriebfahrzeugIst)
                    .HasColumnName("triebfahrzeug_ist")
                    .HasColumnType("int(3)");

                entity.Property(e => e.UebergangNachZugId)
                    .HasColumnName("uebergang_nach_zug_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UebergangVonZugId)
                    .HasColumnName("uebergang_von_zug_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Verkehrstage)
                    .IsRequired()
                    .HasColumnName("verkehrstage")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.VerkehrstageBin)
                    .IsRequired()
                    .HasColumnName("verkehrstage_bin")
                    .HasColumnType("varchar(7)");

                entity.Property(e => e.Vmax)
                    .HasColumnName("vmax")
                    .HasColumnType("int(3)");

                entity.Property(e => e.VmaxIst)
                    .HasColumnName("vmax_ist")
                    .HasColumnType("int(3)");

                entity.Property(e => e.Wendezug)
                    .HasColumnName("wendezug")
                    .HasColumnType("int(1)");

                entity.Property(e => e.ZuggattungId)
                    .HasColumnName("zuggattung_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Zugnummer)
                    .HasColumnName("zugnummer")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<FahrplanZuege>(entity =>
            {
                entity.ToTable("fahrplan_zuege");

                entity.HasIndex(e => e.FahrplanversionId)
                    .HasName("fahrplanversion_id");

                entity.HasIndex(e => e.Triebfahrzeug)
                    .HasName("triebfahrzeug");

                entity.HasIndex(e => e.UebergangNachZugId)
                    .HasName("uebergang_nach_zug_id");

                entity.HasIndex(e => e.UebergangVonZugId)
                    .HasName("uebergang_von_zug_id");

                entity.HasIndex(e => e.VerkehrstageBin)
                    .HasName("verkehrstage_bin");

                entity.HasIndex(e => e.ZuggattungId)
                    .HasName("zuggattung_id");

                entity.HasIndex(e => e.Zugnummer)
                    .HasName("zugnummer");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bemerkungen)
                    .IsRequired()
                    .HasColumnName("bemerkungen")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Bremssystem)
                    .IsRequired()
                    .HasColumnName("bremssystem")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.FahrplanversionId)
                    .HasColumnName("fahrplanversion_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Mbr)
                    .HasColumnName("mbr")
                    .HasColumnType("int(5)");

                entity.Property(e => e.Triebfahrzeug)
                    .HasColumnName("triebfahrzeug")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UebergangNachZugId)
                    .HasColumnName("uebergang_nach_zug_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UebergangVonZugId)
                    .HasColumnName("uebergang_von_zug_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Verkehrstage)
                    .IsRequired()
                    .HasColumnName("verkehrstage")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.VerkehrstageBin)
                    .IsRequired()
                    .HasColumnName("verkehrstage_bin")
                    .HasColumnType("varchar(7)");

                entity.Property(e => e.Vmax)
                    .HasColumnName("vmax")
                    .HasColumnType("int(3)");

                entity.Property(e => e.Wendezug)
                    .IsRequired()
                    .HasColumnName("wendezug")
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.ZuggattungId)
                    .HasColumnName("zuggattung_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Zugnummer)
                    .HasColumnName("zugnummer")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Fahrplanversion)
                    .WithMany(p => p.FahrplanZuege)
                    .HasForeignKey(d => d.FahrplanversionId)
                    .HasConstraintName("fahrplan_zuege_ibfk_1");

                entity.HasOne(d => d.TriebfahrzeugNavigation)
                    .WithMany(p => p.FahrplanZuege)
                    .HasPrincipalKey(p => p.Nummer)
                    .HasForeignKey(d => d.Triebfahrzeug)
                    .HasConstraintName("fahrplan_zuege_ibfk_3");

                entity.HasOne(d => d.UebergangNachZug)
                    .WithMany(p => p.InverseUebergangNachZug)
                    .HasForeignKey(d => d.UebergangNachZugId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fahrplan_zuege_ibfk_5_Uebergang_nach_zug_id");

                entity.HasOne(d => d.UebergangVonZug)
                    .WithMany(p => p.InverseUebergangVonZug)
                    .HasForeignKey(d => d.UebergangVonZugId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fahrplan_zuege_ibfk_4_Uebergang_von_zug_id");

                entity.HasOne(d => d.Zuggattung)
                    .WithMany(p => p.FahrplanZuege)
                    .HasForeignKey(d => d.ZuggattungId)
                    .HasConstraintName("fahrplan_zuege_ibfk_2");
            });

            modelBuilder.Entity<Fahrstrassen>(entity =>
            {
                entity.ToTable("fahrstrassen");

                entity.HasIndex(e => e.Art)
                    .HasName("art");

                entity.HasIndex(e => e.Betriebsstelle)
                    .HasName("betriebsstelle");

                entity.HasIndex(e => e.Fahrtrichtung)
                    .HasName("fahrtrichtung");

                entity.HasIndex(e => e.RegeldwegId)
                    .HasName("regeldweg_id");

                entity.HasIndex(e => e.StartfeldId)
                    .HasName("startfeld_id");

                entity.HasIndex(e => e.StartsignalBegriffId)
                    .HasName("startsignal_begriff_id");

                entity.HasIndex(e => e.StartsignalId)
                    .HasName("startsignal_id");

                entity.HasIndex(e => e.ZielfeldId)
                    .HasName("zielfeld_id");

                entity.HasIndex(e => e.ZielsignalId)
                    .HasName("zielsignal_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Art)
                    .IsRequired()
                    .HasColumnName("art")
                    .HasColumnType("enum('zug','umfahrzug','rangierfahrt')");

                entity.Property(e => e.Betriebsstelle)
                    .IsRequired()
                    .HasColumnName("betriebsstelle")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.F)
                    .HasColumnName("f")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Fahrtrichtung)
                    .HasColumnName("fahrtrichtung")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Kurzbezeichnung)
                    .IsRequired()
                    .HasColumnName("kurzbezeichnung")
                    .HasColumnType("varchar(30)");

                entity.Property(e => e.RegeldwegId)
                    .HasColumnName("regeldweg_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.StartfeldId)
                    .HasColumnName("startfeld_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.StartsignalBegriffId)
                    .HasColumnName("startsignal_begriff_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.StartsignalId)
                    .HasColumnName("startsignal_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UmfahrBezeichnung)
                    .IsRequired()
                    .HasColumnName("umfahr_bezeichnung")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.ZielBezeichnung)
                    .IsRequired()
                    .HasColumnName("ziel_bezeichnung")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ZielfeldId)
                    .HasColumnName("zielfeld_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ZielsignalId)
                    .HasColumnName("zielsignal_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ZlAnstoss1)
                    .HasColumnName("zl_anstoss1")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ZlAnstoss1GbtId)
                    .HasColumnName("zl_anstoss1_gbt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ZlAnstoss2)
                    .HasColumnName("zl_anstoss2")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ZlAnstoss2GbtId)
                    .HasColumnName("zl_anstoss2_gbt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ZlPrioritaet)
                    .HasColumnName("zl_prioritaet")
                    .HasColumnType("int(5)");

                entity.HasOne(d => d.BetriebsstelleNavigation)
                    .WithMany(p => p.Fahrstrassen)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Betriebsstelle)
                    .HasConstraintName("fahrstrassen_ibfk_1");

                entity.HasOne(d => d.Regeldweg)
                    .WithMany(p => p.Fahrstrassen)
                    .HasForeignKey(d => d.RegeldwegId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fahrstrassen_ibfk_4");

                entity.HasOne(d => d.Startfeld)
                    .WithMany(p => p.FahrstrassenStartfeld)
                    .HasForeignKey(d => d.StartfeldId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fahrstrassen_ibfk_5");

                entity.HasOne(d => d.StartsignalBegriff)
                    .WithMany(p => p.Fahrstrassen)
                    .HasForeignKey(d => d.StartsignalBegriffId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fahrstrassen_ibfk_7");

                entity.HasOne(d => d.Startsignal)
                    .WithMany(p => p.FahrstrassenStartsignal)
                    .HasForeignKey(d => d.StartsignalId)
                    .HasConstraintName("fahrstrassen_ibfk_2");

                entity.HasOne(d => d.Zielfeld)
                    .WithMany(p => p.FahrstrassenZielfeld)
                    .HasForeignKey(d => d.ZielfeldId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fahrstrassen_ibfk_6");

                entity.HasOne(d => d.Zielsignal)
                    .WithMany(p => p.FahrstrassenZielsignal)
                    .HasForeignKey(d => d.ZielsignalId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fahrstrassen_ibfk_3");
            });

            modelBuilder.Entity<FahrstrassenAnstoss>(entity =>
            {
                entity.ToTable("fahrstrassen_anstoss");

                entity.HasIndex(e => e.FahrstrasseId)
                    .HasName("fahrstrasse_id");

                entity.HasIndex(e => e.GbtId)
                    .HasName("gbt_id");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.HasIndex(e => e.Verzoegerung)
                    .HasName("verzoegerung");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FahrstrasseId)
                    .HasColumnName("fahrstrasse_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.GbtId)
                    .HasColumnName("gbt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Verzoegerung)
                    .HasColumnName("verzoegerung")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Fahrstrasse)
                    .WithMany(p => p.FahrstrassenAnstoss)
                    .HasForeignKey(d => d.FahrstrasseId)
                    .HasConstraintName("fahrstrassen_anstoss_ibfk_1");

                entity.HasOne(d => d.Gbt)
                    .WithMany(p => p.FahrstrassenAnstoss)
                    .HasForeignKey(d => d.GbtId)
                    .HasConstraintName("fahrstrassen_anstoss_ibfk_3");

                entity.HasOne(d => d.Infra)
                    .WithMany(p => p.FahrstrassenAnstoss)
                    .HasForeignKey(d => d.InfraId)
                    .HasConstraintName("fahrstrassen_anstoss_ibfk_2");
            });

            modelBuilder.Entity<FahrstrassenDwege>(entity =>
            {
                entity.ToTable("fahrstrassen_dwege");

                entity.HasIndex(e => e.DwegId)
                    .HasName("dweg_id");

                entity.HasIndex(e => e.FahrstrasseId)
                    .HasName("fahrstrasse_id");

                entity.HasIndex(e => e.SignalbegriffId)
                    .HasName("signalbegriff_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DwegId)
                    .HasColumnName("dweg_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FahrstrasseId)
                    .HasColumnName("fahrstrasse_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SignalbegriffId)
                    .HasColumnName("signalbegriff_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Dweg)
                    .WithMany(p => p.FahrstrassenDwege)
                    .HasForeignKey(d => d.DwegId)
                    .HasConstraintName("fahrstrassen_dwege_ibfk_2");

                entity.HasOne(d => d.Fahrstrasse)
                    .WithMany(p => p.FahrstrassenDwege)
                    .HasForeignKey(d => d.FahrstrasseId)
                    .HasConstraintName("fahrstrassen_dwege_ibfk_1");

                entity.HasOne(d => d.Signalbegriff)
                    .WithMany(p => p.FahrstrassenDwege)
                    .HasForeignKey(d => d.SignalbegriffId)
                    .HasConstraintName("fahrstrassen_dwege_ibfk_3");
            });

            modelBuilder.Entity<FahrstrassenEinstellung>(entity =>
            {
                entity.ToTable("fahrstrassen_einstellung");

                entity.HasIndex(e => e.FahrstrasseId)
                    .HasName("fahrstrasse_id");

                entity.HasIndex(e => e.Fuem)
                    .HasName("fuem");

                entity.HasIndex(e => e.SignalId)
                    .HasName("signal_id");

                entity.HasIndex(e => e.Status)
                    .HasName("status");

                entity.HasIndex(e => e.Unixtimestamp)
                    .HasName("unixtimestamp");

                entity.HasIndex(e => e.Zfm)
                    .HasName("zfm");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AufloeseId1)
                    .HasColumnName("aufloese_id1")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AufloeseId2)
                    .HasColumnName("aufloese_id2")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FahrstrasseId)
                    .HasColumnName("fahrstrasse_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Fuem)
                    .IsRequired()
                    .HasColumnName("fuem")
                    .HasColumnType("enum('0','1','2')");

                entity.Property(e => e.SignalId)
                    .HasColumnName("signal_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.Unixtimestamp)
                    .HasColumnName("unixtimestamp")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Zfm)
                    .IsRequired()
                    .HasColumnName("zfm")
                    .HasColumnType("enum('0','1')");
            });

            modelBuilder.Entity<FahrstrassenEinstellungElemente>(entity =>
            {
                entity.ToTable("fahrstrassen_einstellung_elemente");

                entity.HasIndex(e => e.EinstellungId)
                    .HasName("einstellung_id");

                entity.HasIndex(e => e.FahrstrasseId)
                    .HasName("fahrstrasse_id");

                entity.HasIndex(e => e.FreimeldeabschnittId)
                    .HasName("freimeldeabschnitt_id");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.HasIndex(e => e.Timestamp)
                    .HasName("timestamp");

                entity.HasIndex(e => e.Unixtimestamp)
                    .HasName("unixtimestamp");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.EinstellungId)
                    .IsRequired()
                    .HasColumnName("einstellung_id")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.FahrstrasseId)
                    .HasColumnName("fahrstrasse_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FreimeldeabschnittId)
                    .HasColumnName("freimeldeabschnitt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Unixtimestamp)
                    .HasColumnName("unixtimestamp")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<FahrstrassenElemente>(entity =>
            {
                entity.ToTable("fahrstrassen_elemente");

                entity.HasIndex(e => e.Dir)
                    .HasName("dir");

                entity.HasIndex(e => e.FahrstrassenId)
                    .HasName("fahrstrassen_id");

                entity.HasIndex(e => e.FreimeldeabschnittId)
                    .HasName("freimeldeabschnitt_id");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.HasIndex(e => e.Reihenfolge)
                    .HasName("reihenfolge");

                entity.HasIndex(e => e.SignalbegriffId)
                    .HasName("signalbegriff_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Dir)
                    .HasColumnName("dir")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FahrstrassenId)
                    .HasColumnName("fahrstrassen_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FreimeldeabschnittId)
                    .HasColumnName("freimeldeabschnitt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Reihenfolge)
                    .HasColumnName("reihenfolge")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SignalbegriffId)
                    .HasColumnName("signalbegriff_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.DirNavigation)
                    .WithMany(p => p.FahrstrassenElemente)
                    .HasPrincipalKey(p => p.Wert)
                    .HasForeignKey(d => d.Dir)
                    .HasConstraintName("fahrstrassen_elemente_ibfk_5");

                entity.HasOne(d => d.Fahrstrassen)
                    .WithMany(p => p.FahrstrassenElemente)
                    .HasForeignKey(d => d.FahrstrassenId)
                    .HasConstraintName("fahrstrassen_elemente_ibfk_1");

                entity.HasOne(d => d.Freimeldeabschnitt)
                    .WithMany(p => p.FahrstrassenElementeFreimeldeabschnitt)
                    .HasForeignKey(d => d.FreimeldeabschnittId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fahrstrassen_elemente_ibfk_4");

                entity.HasOne(d => d.Infra)
                    .WithMany(p => p.FahrstrassenElementeInfra)
                    .HasForeignKey(d => d.InfraId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fahrstrassen_elemente_ibfk_2");

                entity.HasOne(d => d.Signalbegriff)
                    .WithMany(p => p.FahrstrassenElemente)
                    .HasForeignKey(d => d.SignalbegriffId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fahrstrassen_elemente_ibfk_3");
            });

            modelBuilder.Entity<FahrstrassenElementtypen>(entity =>
            {
                entity.ToTable("fahrstrassen_elementtypen");

                entity.HasIndex(e => e.Wert)
                    .HasName("wert")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Beschreibung)
                    .IsRequired()
                    .HasColumnName("beschreibung")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Wert)
                    .HasColumnName("wert")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<FahrstrassenFolgen>(entity =>
            {
                entity.ToTable("fahrstrassen_folgen");

                entity.HasIndex(e => e.AufloeseId)
                    .HasName("aufloese_id");

                entity.HasIndex(e => e.Fahrstrasse1Id)
                    .HasName("fahrstrasse1_id");

                entity.HasIndex(e => e.Fahrstrasse2Id)
                    .HasName("fahrstrasse2_id");

                entity.HasIndex(e => e.Standard)
                    .HasName("standard");

                entity.HasIndex(e => e.StartsignalbegriffId)
                    .HasName("startsignalbegriff_id");

                entity.HasIndex(e => e.ZielsignalbegriffId)
                    .HasName("zielsignalbegriff_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AufloeseId)
                    .HasColumnName("aufloese_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Fahrstrasse1Id)
                    .HasColumnName("fahrstrasse1_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Fahrstrasse2Id)
                    .HasColumnName("fahrstrasse2_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Standard)
                    .HasColumnName("standard")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.StartsignalbegriffId)
                    .HasColumnName("startsignalbegriff_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ZielsignalbegriffId)
                    .HasColumnName("zielsignalbegriff_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Aufloese)
                    .WithMany(p => p.FahrstrassenFolgen)
                    .HasForeignKey(d => d.AufloeseId)
                    .HasConstraintName("fahrstrassen_folgen_ibfk_3");

                entity.HasOne(d => d.Fahrstrasse1)
                    .WithMany(p => p.FahrstrassenFolgenFahrstrasse1)
                    .HasForeignKey(d => d.Fahrstrasse1Id)
                    .HasConstraintName("fahrstrassen_folgen_ibfk_1");

                entity.HasOne(d => d.Fahrstrasse2)
                    .WithMany(p => p.FahrstrassenFolgenFahrstrasse2)
                    .HasForeignKey(d => d.Fahrstrasse2Id)
                    .HasConstraintName("fahrstrassen_folgen_ibfk_2");

                entity.HasOne(d => d.Startsignalbegriff)
                    .WithMany(p => p.FahrstrassenFolgenStartsignalbegriff)
                    .HasForeignKey(d => d.StartsignalbegriffId)
                    .HasConstraintName("fahrstrassen_folgen_ibfk_4");

                entity.HasOne(d => d.Zielsignalbegriff)
                    .WithMany(p => p.FahrstrassenFolgenZielsignalbegriff)
                    .HasForeignKey(d => d.ZielsignalbegriffId)
                    .HasConstraintName("fahrstrassen_folgen_ibfk_5");
            });

            modelBuilder.Entity<Fahrzeuge>(entity =>
            {
                entity.ToTable("fahrzeuge");

                entity.HasIndex(e => e.Adresse)
                    .HasName("adresse")
                    .IsUnique();

                entity.HasIndex(e => e.Dir)
                    .HasName("dir");

                entity.HasIndex(e => e.F0)
                    .HasName("f0");

                entity.HasIndex(e => e.Fzs)
                    .HasName("fzs");

                entity.HasIndex(e => e.Slot)
                    .HasName("slot");

                entity.HasIndex(e => e.Speed)
                    .HasName("speed");

                entity.HasIndex(e => e.Verzoegerung)
                    .HasName("verzoegerung");

                entity.HasIndex(e => e.Zugtyp)
                    .HasName("zugtyp");

                entity.HasIndex(e => e.Zustand)
                    .HasName("zustand");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Adresse)
                    .HasColumnName("adresse")
                    .HasColumnType("int(4)");

                entity.Property(e => e.Dir)
                    .HasColumnName("dir")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.F0)
                    .HasColumnName("f0")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.Fzs)
                    .HasColumnName("fzs")
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.PrevSpeed)
                    .HasColumnName("prev_speed")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Slot)
                    .HasColumnName("slot")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Speed)
                    .HasColumnName("speed")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Verzoegerung)
                    .HasColumnName("verzoegerung")
                    .HasDefaultValueSql("'0.3'");

                entity.Property(e => e.Zuglaenge)
                    .HasColumnName("zuglaenge")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Zugtyp)
                    .IsRequired()
                    .HasColumnName("zugtyp")
                    .HasColumnType("varchar(5)")
                    .HasDefaultValueSql("'pz'");

                entity.Property(e => e.Zustand)
                    .IsRequired()
                    .HasColumnName("zustand")
                    .HasColumnType("enum('0','1','2','3','4','5')");
            });

            modelBuilder.Entity<FahrzeugeAbschnitte>(entity =>
            {
                entity.ToTable("fahrzeuge_abschnitte");

                entity.HasIndex(e => e.AbschnittId)
                    .HasName("abschnitt_id")
                    .IsUnique();

                entity.HasIndex(e => e.FahrzeugId)
                    .HasName("fahrzeug_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AbschnittId)
                    .HasColumnName("abschnitt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FahrzeugId)
                    .HasColumnName("fahrzeug_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<FahrzeugeBaureiheGruppe>(entity =>
            {
                entity.ToTable("fahrzeuge_baureihe_gruppe");

                entity.HasIndex(e => e.BaureiheId)
                    .HasName("baureihe_id");

                entity.HasIndex(e => e.GruppeId)
                    .HasName("gruppe_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BaureiheId)
                    .HasColumnName("baureihe_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.GruppeId)
                    .HasColumnName("gruppe_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Baureihe)
                    .WithMany(p => p.FahrzeugeBaureiheGruppe)
                    .HasForeignKey(d => d.BaureiheId)
                    .HasConstraintName("fahrzeuge_baureihe_gruppe_ibfk_3");

                entity.HasOne(d => d.Gruppe)
                    .WithMany(p => p.FahrzeugeBaureiheGruppe)
                    .HasForeignKey(d => d.GruppeId)
                    .HasConstraintName("fahrzeuge_baureihe_gruppe_ibfk_4");
            });

            modelBuilder.Entity<FahrzeugeBaureihen>(entity =>
            {
                entity.ToTable("fahrzeuge_baureihen");

                entity.HasIndex(e => e.Fahrzeuggruppe)
                    .HasName("fahrzeuggruppe");

                entity.HasIndex(e => e.Laenge)
                    .HasName("laenge");

                entity.HasIndex(e => e.Nummer)
                    .HasName("nummer")
                    .IsUnique();

                entity.HasIndex(e => e.Traktion)
                    .HasName("traktion");

                entity.HasIndex(e => e.Vmax)
                    .HasName("vmax");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bezeichnung)
                    .IsRequired()
                    .HasColumnName("bezeichnung")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Fahrzeuggruppe)
                    .HasColumnName("fahrzeuggruppe")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Laenge)
                    .HasColumnName("laenge")
                    .HasColumnType("int(5)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Nummer)
                    .HasColumnName("nummer")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Traktion)
                    .IsRequired()
                    .HasColumnName("traktion")
                    .HasColumnType("enum('diesel','elektrisch','dampf')");

                entity.Property(e => e.Vmax)
                    .HasColumnName("vmax")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.FahrzeuggruppeNavigation)
                    .WithMany(p => p.FahrzeugeBaureihen)
                    .HasForeignKey(d => d.Fahrzeuggruppe)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fahrzeuge_baureihen_ibfk_1");
            });

            modelBuilder.Entity<FahrzeugeDaten>(entity =>
            {
                entity.ToTable("fahrzeuge_daten");

                entity.HasIndex(e => e.Baureihe)
                    .HasName("baureihe");

                entity.HasIndex(e => e.Railcom)
                    .HasName("railcom");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Baureihe)
                    .HasColumnName("baureihe")
                    .HasColumnType("int(6)");

                entity.Property(e => e.Decodertyp)
                    .IsRequired()
                    .HasColumnName("decodertyp")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Evu)
                    .IsRequired()
                    .HasColumnName("evu")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Radsatzmasse).HasColumnName("radsatzmasse");

                entity.Property(e => e.Railcom)
                    .HasColumnName("railcom")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.BaureiheNavigation)
                    .WithMany(p => p.FahrzeugeDaten)
                    .HasPrincipalKey(p => p.Nummer)
                    .HasForeignKey(d => d.Baureihe)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fahrzeuge_daten_ibfk_2");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.FahrzeugeDaten)
                    .HasForeignKey<FahrzeugeDaten>(d => d.Id)
                    .HasConstraintName("fahrzeuge_daten_ibfk_1");
            });

            modelBuilder.Entity<FahrzeugeGruppen>(entity =>
            {
                entity.ToTable("fahrzeuge_gruppen");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bezeichnung)
                    .IsRequired()
                    .HasColumnName("bezeichnung")
                    .HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<FahrzeugeSteuerung>(entity =>
            {
                entity.ToTable("fahrzeuge_steuerung");

                entity.HasIndex(e => e.Aktion)
                    .HasName("aktion");

                entity.HasIndex(e => e.Ausfuehrung)
                    .HasName("ausfuehrung");

                entity.HasIndex(e => e.Eintragzeit)
                    .HasName("eintragzeit");

                entity.HasIndex(e => e.FahrzeugId)
                    .HasName("fahrzeug_id");

                entity.HasIndex(e => e.FreifahrtId)
                    .HasName("freifahrt_id");

                entity.HasIndex(e => e.Geschwindigkeit)
                    .HasName("geschwindigkeit");

                entity.HasIndex(e => e.Signalstandortid)
                    .HasName("signalstandortid");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Aktion)
                    .IsRequired()
                    .HasColumnName("aktion")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Ausfuehrung)
                    .HasColumnName("ausfuehrung")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bemerkung)
                    .IsRequired()
                    .HasColumnName("bemerkung")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Dir)
                    .HasColumnName("dir")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Eintragzeit)
                    .HasColumnName("eintragzeit")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FahrzeugId)
                    .HasColumnName("fahrzeug_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FreifahrtId)
                    .HasColumnName("freifahrt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Geschwindigkeit)
                    .IsRequired()
                    .HasColumnName("geschwindigkeit")
                    .HasColumnType("varchar(3)");

                entity.Property(e => e.Signalstandortid)
                    .HasColumnName("signalstandortid")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.V0)
                    .HasColumnName("v0")
                    .HasColumnType("int(3)");

                entity.Property(e => e.Vziel)
                    .HasColumnName("vziel")
                    .HasColumnType("int(3)");

                entity.Property(e => e.Wenden)
                    .HasColumnName("wenden")
                    .HasColumnType("int(2)");

                entity.Property(e => e.Zugnummer)
                    .HasColumnName("zugnummer")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Fma>(entity =>
            {
                entity.ToTable("fma");

                entity.HasIndex(e => e.DecoderAdresse)
                    .HasName("decoder_adresse");

                entity.HasIndex(e => e.FmaId)
                    .HasName("fma_id")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bezeichnung)
                    .IsRequired()
                    .HasColumnName("bezeichnung")
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("'#neu#'");

                entity.Property(e => e.DecoderAdresse)
                    .HasColumnName("decoder_adresse")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.FmaId)
                    .HasColumnName("fma_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Laenge)
                    .HasColumnName("laenge")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Marco)
                    .HasColumnName("marco")
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("int(1)");
            });

            modelBuilder.Entity<Fplo>(entity =>
            {
                entity.ToTable("fplo");

                entity.HasIndex(e => e.Nummer)
                    .HasName("nummer")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Evu)
                    .IsRequired()
                    .HasColumnName("evu")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Grund)
                    .IsRequired()
                    .HasColumnName("grund")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.GueltigAb)
                    .HasColumnName("gueltig_ab")
                    .HasColumnType("datetime");

                entity.Property(e => e.GueltigBis)
                    .HasColumnName("gueltig_bis")
                    .HasColumnType("datetime");

                entity.Property(e => e.GvAusfall)
                    .IsRequired()
                    .HasColumnName("gv_ausfall")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.GvBedarfsperre)
                    .IsRequired()
                    .HasColumnName("gv_bedarfsperre")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.GvNeu)
                    .IsRequired()
                    .HasColumnName("gv_neu")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.GvUebrige)
                    .IsRequired()
                    .HasColumnName("gv_uebrige")
                    .HasColumnType("mediumtext");

                entity.Property(e => e.GvUmleitung)
                    .IsRequired()
                    .HasColumnName("gv_umleitung")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Nummer)
                    .IsRequired()
                    .HasColumnName("nummer")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.PvAusfall)
                    .IsRequired()
                    .HasColumnName("pv_ausfall")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.PvBedarfsperre)
                    .IsRequired()
                    .HasColumnName("pv_bedarfsperre")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.PvNeu)
                    .IsRequired()
                    .HasColumnName("pv_neu")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.PvUebrige)
                    .IsRequired()
                    .HasColumnName("pv_uebrige")
                    .HasColumnType("mediumtext");

                entity.Property(e => e.PvUmleitung)
                    .IsRequired()
                    .HasColumnName("pv_umleitung")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Sonstiges)
                    .IsRequired()
                    .HasColumnName("sonstiges")
                    .HasColumnType("text");

                entity.Property(e => e.SperrungArt)
                    .IsRequired()
                    .HasColumnName("sperrung_art")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.SperrungBereich)
                    .IsRequired()
                    .HasColumnName("sperrung_bereich")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Verfasser)
                    .IsRequired()
                    .HasColumnName("verfasser")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Weitere)
                    .IsRequired()
                    .HasColumnName("weitere")
                    .HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<Gbt>(entity =>
            {
                entity.ToTable("gbt");

                entity.HasIndex(e => e.Bearbeiter)
                    .HasName("bearbeiter");

                entity.HasIndex(e => e.Betriebsstelle)
                    .HasName("betriebsstelle");

                entity.HasIndex(e => e.FzgFahrstufe)
                    .HasName("fzg_fahrstufe");

                entity.HasIndex(e => e.Gleis)
                    .HasName("gleis");

                entity.HasIndex(e => e.IsAnbietefeld)
                    .HasName("is_anbietefeld");

                entity.HasIndex(e => e.IsVirtuell)
                    .HasName("is_virtuell");

                entity.HasIndex(e => e.Status)
                    .HasName("status");

                entity.HasIndex(e => e.Timestamp)
                    .HasName("timestamp");

                entity.HasIndex(e => e.Vormeldung0)
                    .HasName("vormeldung_0");

                entity.HasIndex(e => e.Vormeldung1)
                    .HasName("vormeldung_1");

                entity.HasIndex(e => e.Zugnummer)
                    .HasName("Zugnummer");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bearbeiter)
                    .IsRequired()
                    .HasColumnName("bearbeiter")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Betriebsstelle)
                    .HasColumnName("betriebsstelle")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.Decoder)
                    .HasColumnName("decoder")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FzgFahrstufe)
                    .HasColumnName("fzg_fahrstufe")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Gleis)
                    .HasColumnName("gleis")
                    .HasColumnType("int(3)");

                entity.Property(e => e.IsAnbietefeld)
                    .HasColumnName("is_anbietefeld")
                    .HasColumnType("int(1)");

                entity.Property(e => e.IsVirtuell)
                    .HasColumnName("is_virtuell")
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Kurzbezeichnung)
                    .IsRequired()
                    .HasColumnName("kurzbezeichnung")
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.Visible)
                    .HasColumnName("visible")
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Vormeldung0)
                    .IsRequired()
                    .HasColumnName("vormeldung_0")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Vormeldung1)
                    .IsRequired()
                    .HasColumnName("vormeldung_1")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Zugnummer)
                    .IsRequired()
                    .HasColumnType("varchar(11)");

                entity.HasOne(d => d.BetriebsstelleNavigation)
                    .WithMany(p => p.Gbt)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Betriebsstelle)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("gbt_ibfk_1");
            });

            modelBuilder.Entity<GbtAnbietefelder>(entity =>
            {
                entity.ToTable("gbt_anbietefelder");

                entity.HasIndex(e => e.AnbietefeldId)
                    .HasName("anbietefeld_id");

                entity.HasIndex(e => e.GbtId)
                    .HasName("gbt_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AnbietefeldId)
                    .HasColumnName("anbietefeld_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.GbtId)
                    .HasColumnName("gbt_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Anbietefeld)
                    .WithMany(p => p.GbtAnbietefelderAnbietefeld)
                    .HasForeignKey(d => d.AnbietefeldId)
                    .HasConstraintName("gbt_anbietefelder_ibfk_2");

                entity.HasOne(d => d.Gbt)
                    .WithMany(p => p.GbtAnbietefelderGbt)
                    .HasForeignKey(d => d.GbtId)
                    .HasConstraintName("gbt_anbietefelder_ibfk_1");
            });

            modelBuilder.Entity<GbtBetriebsstellen>(entity =>
            {
                entity.ToTable("gbt_betriebsstellen");

                entity.HasIndex(e => e.IstInsGegengleis)
                    .HasName("ist_ins_gegengleis");

                entity.HasIndex(e => e.IstKurzeinfahrt)
                    .HasName("ist_kurzeinfahrt");

                entity.HasIndex(e => e.IstVomGegengleis)
                    .HasName("ist_vom_gegengleis");

                entity.HasIndex(e => e.StartBetriebsstelle)
                    .HasName("start_betriebsstelle");

                entity.HasIndex(e => e.StartGbtId)
                    .HasName("start_gbt_id");

                entity.HasIndex(e => e.ZielBetriebsstelle)
                    .HasName("ziel_betriebsstelle");

                entity.HasIndex(e => e.ZielBetriebsstelle2)
                    .HasName("ziel_betriebsstelle2");

                entity.HasIndex(e => e.ZielGbtId)
                    .HasName("ziel_gbt_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IstInsGegengleis)
                    .HasColumnName("ist_ins_gegengleis")
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.IstKurzeinfahrt)
                    .HasColumnName("ist_kurzeinfahrt")
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.IstVomGegengleis)
                    .HasColumnName("ist_vom_gegengleis")
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.StartBetriebsstelle)
                    .IsRequired()
                    .HasColumnName("start_betriebsstelle")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.StartGbtId)
                    .HasColumnName("start_gbt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ZielBetriebsstelle)
                    .IsRequired()
                    .HasColumnName("ziel_betriebsstelle")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.ZielBetriebsstelle2)
                    .HasColumnName("ziel_betriebsstelle2")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.ZielGbtId)
                    .HasColumnName("ziel_gbt_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.StartBetriebsstelleNavigation)
                    .WithMany(p => p.GbtBetriebsstellenStartBetriebsstelleNavigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.StartBetriebsstelle)
                    .HasConstraintName("gbt_betriebsstellen_ibfk_1");

                entity.HasOne(d => d.StartGbt)
                    .WithMany(p => p.GbtBetriebsstellenStartGbt)
                    .HasForeignKey(d => d.StartGbtId)
                    .HasConstraintName("gbt_betriebsstellen_ibfk_2");

                entity.HasOne(d => d.ZielBetriebsstelleNavigation)
                    .WithMany(p => p.GbtBetriebsstellenZielBetriebsstelleNavigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.ZielBetriebsstelle)
                    .HasConstraintName("gbt_betriebsstellen_ibfk_3");

                entity.HasOne(d => d.ZielBetriebsstelle2Navigation)
                    .WithMany(p => p.GbtBetriebsstellenZielBetriebsstelle2Navigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.ZielBetriebsstelle2)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("gbt_betriebsstellen_ibfk_4");

                entity.HasOne(d => d.ZielGbt)
                    .WithMany(p => p.GbtBetriebsstellenZielGbt)
                    .HasForeignKey(d => d.ZielGbtId)
                    .HasConstraintName("gbt_betriebsstellen_ibfk_5");
            });

            modelBuilder.Entity<GbtFma>(entity =>
            {
                entity.ToTable("gbt_fma");

                entity.HasIndex(e => e.FmaId)
                    .HasName("fma_id")
                    .IsUnique();

                entity.HasIndex(e => e.GbtId)
                    .HasName("gbt_id");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FmaId)
                    .HasColumnName("fma_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.GbtId)
                    .HasColumnName("gbt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Fma)
                    .WithOne(p => p.GbtFma)
                    .HasPrincipalKey<Fma>(p => p.FmaId)
                    .HasForeignKey<GbtFma>(d => d.FmaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("gbt_fma_ibfk_3");

                entity.HasOne(d => d.Gbt)
                    .WithMany(p => p.GbtFma)
                    .HasForeignKey(d => d.GbtId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("gbt_fma_ibfk_1");

                entity.HasOne(d => d.Infra)
                    .WithMany(p => p.GbtFma)
                    .HasForeignKey(d => d.InfraId)
                    .HasConstraintName("gbt_fma_ibfk_2");
            });

            modelBuilder.Entity<HaltabschnitteEinstellung>(entity =>
            {
                entity.ToTable("haltabschnitte_einstellung");

                entity.HasIndex(e => e.FahrstrasseId)
                    .HasName("fahrstrasse_id");

                entity.HasIndex(e => e.FreimeldeabschnittId)
                    .HasName("freimeldeabschnitt_id");

                entity.HasIndex(e => e.HaltabschnittId)
                    .HasName("haltabschnitt_id");

                entity.HasIndex(e => e.Timestamp)
                    .HasName("timestamp");

                entity.HasIndex(e => e.Unixtimestamp)
                    .HasName("unixtimestamp");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FahrstrasseId)
                    .HasColumnName("fahrstrasse_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FreimeldeabschnittId)
                    .HasColumnName("freimeldeabschnitt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HaltabschnittId)
                    .HasColumnName("haltabschnitt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.Unixtimestamp)
                    .HasColumnName("unixtimestamp")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<InfraDaten>(entity =>
            {
                entity.ToTable("infra_daten");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.HasIndex(e => e.LinkedArt)
                    .HasName("linked_art");

                entity.HasIndex(e => e.LinkedId)
                    .HasName("linked_id");

                entity.HasIndex(e => e.StoerungId)
                    .HasName("stoerung_id");

                entity.HasIndex(e => e.WertArt)
                    .HasName("wert_art");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.LinkedArt)
                    .IsRequired()
                    .HasColumnName("linked_art")
                    .HasColumnType("enum('','wsp','gedrueckt','zdsig','folgeabhaengigkeit')");

                entity.Property(e => e.LinkedId)
                    .HasColumnName("linked_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.StoerungId)
                    .HasColumnName("stoerung_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Wert)
                    .IsRequired()
                    .HasColumnName("wert")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.WertArt)
                    .IsRequired()
                    .HasColumnName("wert_art")
                    .HasColumnType("varchar(255)");

                entity.HasOne(d => d.Infra)
                    .WithMany(p => p.InfraDatenInfra)
                    .HasForeignKey(d => d.InfraId)
                    .HasConstraintName("infra_daten_ibfk_2");

                entity.HasOne(d => d.Linked)
                    .WithMany(p => p.InfraDatenLinked)
                    .HasForeignKey(d => d.LinkedId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("infra_daten_ibfk_4");

                entity.HasOne(d => d.Stoerung)
                    .WithMany(p => p.InfraDatenStoerung)
                    .HasForeignKey(d => d.StoerungId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("infra_daten_ibfk_3");
            });

            modelBuilder.Entity<InfraFestlegung>(entity =>
            {
                entity.ToTable("infra_festlegung");

                entity.HasIndex(e => e.Art)
                    .HasName("art");

                entity.HasIndex(e => e.Festlegung)
                    .HasName("festlegung");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.HasIndex(e => e.Sperrung)
                    .HasName("sperrung");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Art)
                    .IsRequired()
                    .HasColumnName("art")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Festlegung)
                    .HasColumnName("festlegung")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Sperrung)
                    .HasColumnName("sperrung")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.Urheber)
                    .IsRequired()
                    .HasColumnName("urheber")
                    .HasColumnType("varchar(255)");

                entity.HasOne(d => d.Infra)
                    .WithMany(p => p.InfraFestlegung)
                    .HasForeignKey(d => d.InfraId)
                    .HasConstraintName("infra_festlegung_ibfk_1");
            });

            modelBuilder.Entity<InfraTypen>(entity =>
            {
                entity.ToTable("infra_typen");

                entity.HasIndex(e => e.Type)
                    .HasName("type")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<InfraZustand>(entity =>
            {
                entity.ToTable("infra_zustand");

                entity.HasIndex(e => e.Address)
                    .HasName("address")
                    .IsUnique();

                entity.HasIndex(e => e.Betriebsstelle)
                    .HasName("betriebsstelle");

                entity.HasIndex(e => e.Festlegung)
                    .HasName("festlegung");

                entity.HasIndex(e => e.FestlegungRa)
                    .HasName("festlegung_ra");

                entity.HasIndex(e => e.FreimeldeabschnittId)
                    .HasName("freimeldeabschnitt_id");

                entity.HasIndex(e => e.Kurzbezeichnung)
                    .HasName("kurzbezeichnung");

                entity.HasIndex(e => e.SignalstandortId)
                    .HasName("signalstandort_id");

                entity.HasIndex(e => e.Timestamp)
                    .HasName("timestamp");

                entity.HasIndex(e => e.Type)
                    .HasName("type");

                entity.HasIndex(e => e.Unixtimestamp)
                    .HasName("unixtimestamp");

                entity.HasIndex(e => e.WeichenabhaengigkeitId)
                    .HasName("weichenabhaengigkeit_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Betriebsstelle)
                    .IsRequired()
                    .HasColumnName("betriebsstelle")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Bezeichnung)
                    .IsRequired()
                    .HasColumnName("bezeichnung")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Blatt)
                    .IsRequired()
                    .HasColumnName("blatt")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Dir)
                    .HasColumnName("dir")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.Festlegung)
                    .HasColumnName("festlegung")
                    .HasColumnType("int(1)");

                entity.Property(e => e.FestlegungBlock)
                    .HasColumnName("festlegung_block")
                    .HasColumnType("int(1)");

                entity.Property(e => e.FestlegungRa)
                    .HasColumnName("festlegung_ra")
                    .HasColumnType("int(1)");

                entity.Property(e => e.FreimeldeabschnittId)
                    .HasColumnName("freimeldeabschnitt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Kurzbezeichnung)
                    .IsRequired()
                    .HasColumnName("kurzbezeichnung")
                    .HasColumnType("varchar(15)");

                entity.Property(e => e.Laenge)
                    .HasColumnName("laenge")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Lock)
                    .HasColumnName("lock")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Plan)
                    .IsRequired()
                    .HasColumnName("plan")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.SignalstandortId)
                    .HasColumnName("signalstandort_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Stellzaehler)
                    .HasColumnName("stellzaehler")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Unixtimestamp)
                    .HasColumnName("unixtimestamp")
                    .HasColumnType("int(11)");

                entity.Property(e => e.WeichenabhaengigkeitId)
                    .HasColumnName("weichenabhaengigkeit_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Wrm)
                    .HasColumnName("wrm")
                    .HasColumnType("int(1)");

                entity.Property(e => e.WrmAktiv)
                    .HasColumnName("wrm_aktiv")
                    .HasColumnType("int(1)");

                entity.HasOne(d => d.BetriebsstelleNavigation)
                    .WithMany(p => p.InfraZustand)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Betriebsstelle)
                    .HasConstraintName("infra_zustand_ibfk_1");

                entity.HasOne(d => d.Freimeldeabschnitt)
                    .WithMany(p => p.InverseFreimeldeabschnitt)
                    .HasForeignKey(d => d.FreimeldeabschnittId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("infra_zustand_ibfk_3");

                entity.HasOne(d => d.Signalstandort)
                    .WithMany(p => p.InfraZustand)
                    .HasForeignKey(d => d.SignalstandortId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("infra_zustand_ibfk_2");

                entity.HasOne(d => d.TypeNavigation)
                    .WithMany(p => p.InfraZustand)
                    .HasPrincipalKey(p => p.Type)
                    .HasForeignKey(d => d.Type)
                    .HasConstraintName("infra_zustand_ibfk_5");
            });

            modelBuilder.Entity<Protokoll>(entity =>
            {
                entity.ToTable("protokoll");

                entity.HasIndex(e => e.Feld)
                    .HasName("feld");

                entity.HasIndex(e => e.IstBetriebsstelle)
                    .HasName("ist_betriebsstelle");

                entity.HasIndex(e => e.SessionId)
                    .HasName("session_id");

                entity.HasIndex(e => e.Signal)
                    .HasName("signal");

                entity.HasIndex(e => e.Zugnummer)
                    .HasName("zugnummer");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bearbeiter)
                    .IsRequired()
                    .HasColumnName("bearbeiter")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Feld)
                    .HasColumnName("feld")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IstArt)
                    .IsRequired()
                    .HasColumnName("ist_art")
                    .HasColumnType("enum('','ankunft','abfahrt','durchfahrt')");

                entity.Property(e => e.IstBetriebsstelle)
                    .IsRequired()
                    .HasColumnName("ist_betriebsstelle")
                    .HasColumnType("varchar(5)");

                entity.Property(e => e.IstVerspaetung)
                    .HasColumnName("ist_verspaetung")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SessionId)
                    .HasColumnName("session_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Signal)
                    .HasColumnName("signal")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Source)
                    .IsRequired()
                    .HasColumnName("source")
                    .HasColumnType("tinytext");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime");

                entity.Property(e => e.Zugnummer)
                    .HasColumnName("zugnummer")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.FeldNavigation)
                    .WithMany(p => p.Protokoll)
                    .HasForeignKey(d => d.Feld)
                    .HasConstraintName("protokoll_ibfk_3");

                entity.HasOne(d => d.Session)
                    .WithMany(p => p.Protokoll)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("protokoll_ibfk_2");

                entity.HasOne(d => d.SignalNavigation)
                    .WithMany(p => p.Protokoll)
                    .HasForeignKey(d => d.Signal)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("protokoll_ibfk_1");
            });

            modelBuilder.Entity<ProtokollTrainer>(entity =>
            {
                entity.ToTable("protokoll_trainer");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Nachricht)
                    .IsRequired()
                    .HasColumnName("nachricht")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnName("type")
                    .HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<Signale>(entity =>
            {
                entity.ToTable("signale");

                entity.HasIndex(e => e.AnhalteId)
                    .HasName("anhalte_id");

                entity.HasIndex(e => e.Betriebsstelle)
                    .HasName("betriebsstelle");

                entity.HasIndex(e => e.Bezeichnung)
                    .HasName("bezeichnung");

                entity.HasIndex(e => e.Fahrplanhalt)
                    .HasName("fahrplanhalt");

                entity.HasIndex(e => e.Folgebetriebsstelle)
                    .HasName("folgebetriebsstelle");

                entity.HasIndex(e => e.FreifahrtId)
                    .HasName("freifahrt_id");

                entity.HasIndex(e => e.FreimeldeId)
                    .HasName("freimelde_id");

                entity.HasIndex(e => e.FreimeldeId2)
                    .HasName("freimelde_id2");

                entity.HasIndex(e => e.GbtId)
                    .HasName("gbt_id");

                entity.HasIndex(e => e.HaltabschnittId)
                    .HasName("haltabschnitt_id");

                entity.HasIndex(e => e.HaltbegriffId)
                    .HasName("haltbegriff_id");

                entity.HasIndex(e => e.HaltfallId)
                    .HasName("haltfall_id");

                entity.HasIndex(e => e.Lock)
                    .HasName("lock");

                entity.HasIndex(e => e.Signaltyp)
                    .HasName("signaltyp");

                entity.HasIndex(e => e.WendenId)
                    .HasName("wenden_id");

                entity.HasIndex(e => e.Wirkart)
                    .HasName("wirkart");

                entity.HasIndex(e => e.Wirkrichtung)
                    .HasName("wirkrichtung");

                entity.HasIndex(e => e.ZlDefault)
                    .HasName("zl_default");

                entity.HasIndex(e => e.Zuglenkung)
                    .HasName("zuglenkung");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AnhalteId)
                    .HasColumnName("anhalte_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Betriebsstelle)
                    .IsRequired()
                    .HasColumnName("betriebsstelle")
                    .HasColumnType("char(10)");

                entity.Property(e => e.Bezeichnung)
                    .IsRequired()
                    .HasColumnName("bezeichnung")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.BezeichnungAlt)
                    .IsRequired()
                    .HasColumnName("bezeichnung_alt")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Fahrplanhalt)
                    .IsRequired()
                    .HasColumnName("fahrplanhalt")
                    .HasColumnType("enum('nein','ja')")
                    .HasDefaultValueSql("'nein'");

                entity.Property(e => e.Folgebetriebsstelle)
                    .HasColumnName("folgebetriebsstelle")
                    .HasColumnType("char(10)");

                entity.Property(e => e.FreifahrtId)
                    .HasColumnName("freifahrt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FreimeldeId)
                    .HasColumnName("freimelde_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FreimeldeId2)
                    .HasColumnName("freimelde_id2")
                    .HasColumnType("int(11)");

                entity.Property(e => e.GbtId)
                    .HasColumnName("gbt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HaltabschnittId)
                    .HasColumnName("haltabschnitt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HaltbegriffId)
                    .HasColumnName("haltbegriff_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HaltfallId)
                    .HasColumnName("haltfall_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Lock)
                    .HasColumnName("lock")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Signaltyp)
                    .IsRequired()
                    .HasColumnName("signaltyp")
                    .HasColumnType("enum('ESig','ASig','BkSig','ZSig','ZdSig','SperrSig','VSig','VirtSig','H-Tafel','Sonstiges')");

                entity.Property(e => e.Standort).HasColumnName("standort");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.WendenId)
                    .HasColumnName("wenden_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Wirkart)
                    .IsRequired()
                    .HasColumnName("wirkart")
                    .HasColumnType("enum('zug','rangierfahrt','alle')");

                entity.Property(e => e.Wirkrichtung)
                    .HasColumnName("wirkrichtung")
                    .HasColumnType("int(1)");

                entity.Property(e => e.ZlDefault)
                    .HasColumnName("zl_default")
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Zuglenkung)
                    .HasColumnName("zuglenkung")
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.Anhalte)
                    .WithMany(p => p.SignaleAnhalte)
                    .HasForeignKey(d => d.AnhalteId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("signale_ibfk_3");

                entity.HasOne(d => d.BetriebsstelleNavigation)
                    .WithMany(p => p.SignaleBetriebsstelleNavigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Betriebsstelle)
                    .HasConstraintName("signale_ibfk_8");

                entity.HasOne(d => d.FolgebetriebsstelleNavigation)
                    .WithMany(p => p.SignaleFolgebetriebsstelleNavigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Folgebetriebsstelle)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("signale_ibfk_10");

                entity.HasOne(d => d.Freifahrt)
                    .WithMany(p => p.SignaleFreifahrt)
                    .HasForeignKey(d => d.FreifahrtId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("signale_ibfk_4");

                entity.HasOne(d => d.Freimelde)
                    .WithMany(p => p.SignaleFreimelde)
                    .HasForeignKey(d => d.FreimeldeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("signale_ibfk_1");

                entity.HasOne(d => d.FreimeldeId2Navigation)
                    .WithMany(p => p.SignaleFreimeldeId2Navigation)
                    .HasForeignKey(d => d.FreimeldeId2)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("signale_ibfk_2");

                entity.HasOne(d => d.Gbt)
                    .WithMany(p => p.Signale)
                    .HasForeignKey(d => d.GbtId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("signale_ibfk_9");

                entity.HasOne(d => d.Haltabschnitt)
                    .WithMany(p => p.SignaleHaltabschnitt)
                    .HasForeignKey(d => d.HaltabschnittId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("signale_ibfk_6");

                entity.HasOne(d => d.Haltbegriff)
                    .WithMany(p => p.Signale)
                    .HasForeignKey(d => d.HaltbegriffId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("signale_ibfk_7");

                entity.HasOne(d => d.Haltfall)
                    .WithMany(p => p.SignaleHaltfall)
                    .HasForeignKey(d => d.HaltfallId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("signale_ibfk_5");
            });

            modelBuilder.Entity<SignaleBegriffe>(entity =>
            {
                entity.ToTable("signale_begriffe");

                entity.HasIndex(e => e.Adresse)
                    .HasName("adresse")
                    .IsUnique();

                entity.HasIndex(e => e.Geschwindigkeit)
                    .HasName("geschwindigkeit");

                entity.HasIndex(e => e.IsZugfahrtbegriff)
                    .HasName("is_zugfahrtbegriff");

                entity.HasIndex(e => e.OriginalBegriffId)
                    .HasName("original_begriff_id");

                entity.HasIndex(e => e.SignalId)
                    .HasName("signal_id");

                entity.HasIndex(e => e.WebstwFarbe)
                    .HasName("webstw_farbe");

                entity.HasIndex(e => e.Zielentfernung)
                    .HasName("zielentfernung");

                entity.HasIndex(e => e.Zielgeschwindigkeit)
                    .HasName("zielgeschwindigkeit");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Adresse)
                    .HasColumnName("adresse")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Begriff)
                    .IsRequired()
                    .HasColumnName("begriff")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Geschwindigkeit)
                    .HasColumnName("geschwindigkeit")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IsZugfahrtbegriff)
                    .HasColumnName("is_zugfahrtbegriff")
                    .HasColumnType("int(1)");

                entity.Property(e => e.OriginalBegriffId)
                    .HasColumnName("original_begriff_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SignalId)
                    .HasColumnName("signal_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.WebstwFarbe)
                    .IsRequired()
                    .HasColumnName("webstw_farbe")
                    .HasColumnType("enum('gruen','gelb','rot','ke','ra12','zs1','zs7','zs8','blau')");

                entity.Property(e => e.Zielentfernung)
                    .HasColumnName("zielentfernung")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Zielgeschwindigkeit)
                    .IsRequired()
                    .HasColumnName("zielgeschwindigkeit")
                    .HasColumnType("enum('','0','10','20','30','40','60','80','90','100','120')")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.OriginalBegriff)
                    .WithMany(p => p.InverseOriginalBegriff)
                    .HasForeignKey(d => d.OriginalBegriffId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("signale_begriffe_ibfk_2");

                entity.HasOne(d => d.Signal)
                    .WithMany(p => p.SignaleBegriffe)
                    .HasForeignKey(d => d.SignalId)
                    .HasConstraintName("signale_begriffe_ibfk_1");
            });

            modelBuilder.Entity<SignaleEinstellung>(entity =>
            {
                entity.ToTable("signale_einstellung");

                entity.HasIndex(e => e.AufloeseId)
                    .HasName("aufloese_id");

                entity.HasIndex(e => e.FahrstrasseId)
                    .HasName("fahrstrasse_id");

                entity.HasIndex(e => e.SignalId)
                    .HasName("signal_id");

                entity.HasIndex(e => e.Timestamp)
                    .HasName("timestamp");

                entity.HasIndex(e => e.ZielsignalbegriffId)
                    .HasName("zielsignalbegriff_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AufloeseId)
                    .HasColumnName("aufloese_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FahrstrasseId)
                    .HasColumnName("fahrstrasse_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SignalId)
                    .HasColumnName("signal_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.ZielsignalbegriffId)
                    .HasColumnName("zielsignalbegriff_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<SignaleEinstellungErhalt>(entity =>
            {
                entity.ToTable("signale_einstellung_erhalt");

                entity.HasIndex(e => e.HaltabschnittId)
                    .HasName("haltabschnitt_id");

                entity.HasIndex(e => e.HaltfallId)
                    .HasName("haltfall_id");

                entity.HasIndex(e => e.SignalId)
                    .HasName("signal_id");

                entity.HasIndex(e => e.SignalbegriffId)
                    .HasName("signalbegriff_id");

                entity.HasIndex(e => e.Signalfahrtstellung)
                    .HasName("signalfahrtstellung");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HaltabschnittId)
                    .HasColumnName("haltabschnitt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HaltfallId)
                    .HasColumnName("haltfall_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SignalId)
                    .HasColumnName("signal_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SignalbegriffId)
                    .HasColumnName("signalbegriff_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Signalfahrtstellung)
                    .HasColumnName("signalfahrtstellung")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'");
            });

            modelBuilder.Entity<SignaleElemente>(entity =>
            {
                entity.ToTable("signale_elemente");

                entity.HasIndex(e => e.Dir)
                    .HasName("dir_2");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.HasIndex(e => e.SignalId)
                    .HasName("signal_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Dir)
                    .HasColumnName("dir")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SignalId)
                    .HasColumnName("signal_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Infra)
                    .WithMany(p => p.SignaleElemente)
                    .HasForeignKey(d => d.InfraId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("signale_elemente_ibfk_2");

                entity.HasOne(d => d.Signal)
                    .WithMany(p => p.SignaleElemente)
                    .HasForeignKey(d => d.SignalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("signale_elemente_ibfk_1");
            });

            modelBuilder.Entity<SignaleNachlauf>(entity =>
            {
                entity.ToTable("signale_nachlauf");

                entity.HasIndex(e => e.FahrstrasseId)
                    .HasName("fahrstrasse_id");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.HasIndex(e => e.Nachlaufart)
                    .HasName("nachlaufart");

                entity.HasIndex(e => e.SignalbegriffId)
                    .HasName("signalbegriff_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FahrstrasseId)
                    .HasColumnName("fahrstrasse_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraDir)
                    .HasColumnName("infra_dir")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Nachlaufart)
                    .IsRequired()
                    .HasColumnName("nachlaufart")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.SignalId)
                    .HasColumnName("signal_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SignalbegriffId)
                    .HasColumnName("signalbegriff_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<SignaleStellauftraege>(entity =>
            {
                entity.ToTable("signale_stellauftraege");

                entity.HasIndex(e => e.FahrstrasseId)
                    .HasName("fahrstrasse_id");

                entity.HasIndex(e => e.IstStartsignal)
                    .HasName("ist_startsignal");

                entity.HasIndex(e => e.SignalbegriffId)
                    .HasName("signalbegriff_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FahrstrasseId)
                    .HasColumnName("fahrstrasse_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IstStartsignal)
                    .HasColumnName("ist_startsignal")
                    .HasColumnType("int(1)");

                entity.Property(e => e.SignalbegriffId)
                    .HasColumnName("signalbegriff_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'")
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<SignaleVorsignale>(entity =>
            {
                entity.ToTable("signale_vorsignale");

                entity.HasIndex(e => e.HauptsignalbegriffId)
                    .HasName("hauptsignalbegriff_id");

                entity.HasIndex(e => e.HauptvorsignalbegriffId)
                    .HasName("hauptvorsignalbegriff_id");

                entity.HasIndex(e => e.VorsignalbegriffId)
                    .HasName("vorsignalbegriff_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HauptsignalbegriffId)
                    .HasColumnName("hauptsignalbegriff_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.HauptvorsignalbegriffId)
                    .HasColumnName("hauptvorsignalbegriff_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.IsFahrstrassenabhaengig)
                    .HasColumnName("is_fahrstrassenabhaengig")
                    .HasColumnType("tinyint(4)");

                entity.Property(e => e.VorsignalbegriffId)
                    .HasColumnName("vorsignalbegriff_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Hauptsignalbegriff)
                    .WithMany(p => p.SignaleVorsignaleHauptsignalbegriff)
                    .HasForeignKey(d => d.HauptsignalbegriffId)
                    .HasConstraintName("signale_vorsignale_ibfk_3");

                entity.HasOne(d => d.Hauptvorsignalbegriff)
                    .WithMany(p => p.SignaleVorsignaleHauptvorsignalbegriff)
                    .HasForeignKey(d => d.HauptvorsignalbegriffId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("signale_vorsignale_ibfk_5");

                entity.HasOne(d => d.Vorsignalbegriff)
                    .WithMany(p => p.SignaleVorsignaleVorsignalbegriff)
                    .HasForeignKey(d => d.VorsignalbegriffId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("signale_vorsignale_ibfk_4");
            });

            modelBuilder.Entity<SignaleWenden>(entity =>
            {
                entity.ToTable("signale_wenden");

                entity.HasIndex(e => e.GegensignalId)
                    .HasName("gegensignal_id");

                entity.HasIndex(e => e.SignalId)
                    .HasName("signal_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.GegensignalId)
                    .HasColumnName("gegensignal_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SignalId)
                    .HasColumnName("signal_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Gegensignal)
                    .WithMany(p => p.SignaleWendenGegensignal)
                    .HasForeignKey(d => d.GegensignalId)
                    .HasConstraintName("signale_wenden_ibfk_2");

                entity.HasOne(d => d.Signal)
                    .WithMany(p => p.SignaleWendenSignal)
                    .HasForeignKey(d => d.SignalId)
                    .HasConstraintName("signale_wenden_ibfk_1");
            });

            modelBuilder.Entity<SignaleZielpunkte>(entity =>
            {
                entity.ToTable("signale_zielpunkte");

                entity.HasIndex(e => e.SignalId)
                    .HasName("signal_id");

                entity.HasIndex(e => e.Zuglaenge)
                    .HasName("zuglaenge");

                entity.HasIndex(e => e.Zugtyp)
                    .HasName("zugtyp");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Entfernung)
                    .HasColumnName("entfernung")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SignalId)
                    .HasColumnName("signal_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Zuglaenge)
                    .HasColumnName("zuglaenge")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Zugtyp)
                    .IsRequired()
                    .HasColumnName("zugtyp")
                    .HasColumnType("enum('alle','pz','gz')");

                entity.HasOne(d => d.Signal)
                    .WithMany(p => p.SignaleZielpunkte)
                    .HasForeignKey(d => d.SignalId)
                    .HasConstraintName("signale_zielpunkte_ibfk_2");
            });

            modelBuilder.Entity<Stellwerke>(entity =>
            {
                entity.ToTable("stellwerke");

                entity.HasIndex(e => e.Berue)
                    .HasName("berue");

                entity.HasIndex(e => e.Betriebsstelle)
                    .HasName("betriebsstelle");

                entity.HasIndex(e => e.HasLenkplan)
                    .HasName("has_lenkplan");

                entity.HasIndex(e => e.Lupe)
                    .HasName("lupe");

                entity.HasIndex(e => e.Streckenspiegel)
                    .HasName("streckenspiegel");

                entity.HasIndex(e => e.Zn)
                    .HasName("zn");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Berue)
                    .HasColumnName("berue")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Betriebsstelle)
                    .HasColumnName("betriebsstelle")
                    .HasColumnType("varchar(4)");

                entity.Property(e => e.Decoderplan)
                    .HasColumnName("decoderplan")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.HasLenkplan)
                    .HasColumnName("has_lenkplan")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Idplan)
                    .HasColumnName("idplan")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Kurzname)
                    .IsRequired()
                    .HasColumnName("kurzname")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Lupe)
                    .HasColumnName("lupe")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Special)
                    .IsRequired()
                    .HasColumnName("special")
                    .HasColumnType("enum('none','snow')");

                entity.Property(e => e.Streckenspiegel)
                    .HasColumnName("streckenspiegel")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Stwname)
                    .IsRequired()
                    .HasColumnName("stwname")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.SvgFile)
                    .IsRequired()
                    .HasColumnName("svg_file")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.SvgHeight)
                    .HasColumnName("svg_height")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SvgWidth)
                    .HasColumnName("svg_width")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Zn)
                    .HasColumnName("zn")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Zwl)
                    .HasColumnName("zwl")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.ZwlLink)
                    .IsRequired()
                    .HasColumnName("zwl_link")
                    .HasColumnType("varchar(255)");

                entity.HasOne(d => d.BetriebsstelleNavigation)
                    .WithMany(p => p.Stellwerke)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Betriebsstelle)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("stellwerke_ibfk_2");
            });

            modelBuilder.Entity<StellwerkeBereiche>(entity =>
            {
                entity.ToTable("stellwerke_bereiche");

                entity.HasIndex(e => e.StellwerkeId)
                    .HasName("stellwerke_id");

                entity.HasIndex(e => e.StellwerkeUzId)
                    .HasName("stellwerke_uz_id");

                entity.HasIndex(e => new { e.StellwerkeUzId, e.StellwerkeId })
                    .HasName("stellwerke_uz_id_2")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.StellwerkeId)
                    .HasColumnName("stellwerke_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.StellwerkeUzId)
                    .HasColumnName("stellwerke_uz_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Stellwerke)
                    .WithMany(p => p.StellwerkeBereiche)
                    .HasForeignKey(d => d.StellwerkeId)
                    .HasConstraintName("stellwerke_bereiche_ibfk_2");

                entity.HasOne(d => d.StellwerkeUz)
                    .WithMany(p => p.StellwerkeBereiche)
                    .HasForeignKey(d => d.StellwerkeUzId)
                    .HasConstraintName("stellwerke_bereiche_ibfk_1");
            });

            modelBuilder.Entity<StellwerkeBetriebsstellen>(entity =>
            {
                entity.ToTable("stellwerke_betriebsstellen");

                entity.HasIndex(e => e.BetriebsstelleKuerzel)
                    .HasName("betriebsstelle_kuerzel");

                entity.HasIndex(e => e.StellwerkId)
                    .HasName("stellwerk_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BetriebsstelleKuerzel)
                    .IsRequired()
                    .HasColumnName("betriebsstelle_kuerzel")
                    .HasColumnType("char(10)");

                entity.Property(e => e.StellwerkId)
                    .HasColumnName("stellwerk_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.BetriebsstelleKuerzelNavigation)
                    .WithMany(p => p.StellwerkeBetriebsstellen)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.BetriebsstelleKuerzel)
                    .HasConstraintName("stellwerke_betriebsstellen_ibfk_2");

                entity.HasOne(d => d.Stellwerk)
                    .WithMany(p => p.StellwerkeBetriebsstellen)
                    .HasForeignKey(d => d.StellwerkId)
                    .HasConstraintName("stellwerke_betriebsstellen_ibfk_1");
            });

            modelBuilder.Entity<StellwerkeHandlungUnzulaessig>(entity =>
            {
                entity.ToTable("stellwerke_handlung_unzulaessig");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.HasIndex(e => e.StellwerkId)
                    .HasName("stellwerk_id");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Dir)
                    .HasColumnName("dir")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.StellwerkId)
                    .HasColumnName("stellwerk_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Infra)
                    .WithMany(p => p.StellwerkeHandlungUnzulaessig)
                    .HasForeignKey(d => d.InfraId)
                    .HasConstraintName("stellwerke_handlung_unzulaessig_ibfk_3");

                entity.HasOne(d => d.Stellwerk)
                    .WithMany(p => p.StellwerkeHandlungUnzulaessig)
                    .HasForeignKey(d => d.StellwerkId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("stellwerke_handlung_unzulaessig_ibfk_4");
            });

            modelBuilder.Entity<StellwerkeMerkschilder>(entity =>
            {
                entity.ToTable("stellwerke_merkschilder");

                entity.HasIndex(e => e.Befahrbarkeitssperre)
                    .HasName("befahrbarkeitssperre");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Befahrbarkeitssperre)
                    .HasColumnName("befahrbarkeitssperre")
                    .HasColumnType("int(1)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Merktext)
                    .IsRequired()
                    .HasColumnName("merktext")
                    .HasColumnType("varchar(5)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.HasOne(d => d.Infra)
                    .WithMany(p => p.StellwerkeMerkschilder)
                    .HasForeignKey(d => d.InfraId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("stellwerke_merkschilder_ibfk_1");
            });

            modelBuilder.Entity<StellwerkeMerkspeicher>(entity =>
            {
                entity.ToTable("stellwerke_merkspeicher");

                entity.HasIndex(e => e.Position)
                    .HasName("position");

                entity.HasIndex(e => e.StellwerkUz)
                    .HasName("stellwerk_uz");

                entity.HasIndex(e => new { e.StellwerkUz, e.Position })
                    .HasName("stellwerk_uz_position")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bearbeiter)
                    .IsRequired()
                    .HasColumnName("bearbeiter")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Position)
                    .HasColumnName("position")
                    .HasColumnType("int(2)");

                entity.Property(e => e.StellwerkUz)
                    .HasColumnName("stellwerk_uz")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasColumnName("text")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(d => d.StellwerkUzNavigation)
                    .WithMany(p => p.StellwerkeMerkspeicher)
                    .HasForeignKey(d => d.StellwerkUz)
                    .HasConstraintName("stellwerke_merkspeicher_ibfk_1");
            });

            modelBuilder.Entity<StellwerkeProtokoll>(entity =>
            {
                entity.ToTable("stellwerke_protokoll");

                entity.HasIndex(e => e.Bediener)
                    .HasName("bediener");

                entity.HasIndex(e => e.Betriebsstelle)
                    .HasName("betriebsstelle");

                entity.HasIndex(e => e.FahrplansessionId)
                    .HasName("fahrplansession_id");

                entity.HasIndex(e => e.FahrstrasseId)
                    .HasName("fahrstrasse_id");

                entity.HasIndex(e => e.GbtId)
                    .HasName("gbt_id");

                entity.HasIndex(e => e.Handlung)
                    .HasName("handlung");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.HasIndex(e => e.Merktext)
                    .HasName("merktext");

                entity.HasIndex(e => e.SignalId)
                    .HasName("signal_id");

                entity.HasIndex(e => e.TimestampSession)
                    .HasName("timestamp_session_2");

                entity.HasIndex(e => e.Zaehlwerk)
                    .HasName("zaehlwerk_2");

                entity.HasIndex(e => e.Zugnummer)
                    .HasName("zugnummer");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bediener)
                    .IsRequired()
                    .HasColumnName("bediener")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Betriebsstelle)
                    .HasColumnName("betriebsstelle")
                    .HasColumnType("char(11)");

                entity.Property(e => e.FahrplansessionId)
                    .HasColumnName("fahrplansession_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FahrstrasseId)
                    .HasColumnName("fahrstrasse_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.GbtId)
                    .HasColumnName("gbt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Handlung)
                    .IsRequired()
                    .HasColumnName("handlung")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Kommentar)
                    .IsRequired()
                    .HasColumnName("kommentar")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Merktext)
                    .IsRequired()
                    .HasColumnName("merktext")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.SignalId)
                    .HasColumnName("signal_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("datetime");

                entity.Property(e => e.TimestampSession)
                    .HasColumnName("timestamp_session")
                    .HasColumnType("timestamp");

                entity.Property(e => e.Zaehlwerk)
                    .HasColumnName("zaehlwerk")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Zugnummer)
                    .HasColumnName("zugnummer")
                    .HasColumnType("int(10)");

                entity.HasOne(d => d.BetriebsstelleNavigation)
                    .WithMany(p => p.StellwerkeProtokoll)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Betriebsstelle)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("stellwerke_protokoll_ibfk_5");

                entity.HasOne(d => d.Fahrplansession)
                    .WithMany(p => p.StellwerkeProtokoll)
                    .HasForeignKey(d => d.FahrplansessionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("stellwerke_protokoll_ibfk_4");

                entity.HasOne(d => d.Fahrstrasse)
                    .WithMany(p => p.StellwerkeProtokoll)
                    .HasForeignKey(d => d.FahrstrasseId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("stellwerke_protokoll_ibfk_3");

                entity.HasOne(d => d.Gbt)
                    .WithMany(p => p.StellwerkeProtokoll)
                    .HasForeignKey(d => d.GbtId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("stellwerke_protokoll_ibfk_6");

                entity.HasOne(d => d.Infra)
                    .WithMany(p => p.StellwerkeProtokoll)
                    .HasForeignKey(d => d.InfraId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("stellwerke_protokoll_ibfk_1");

                entity.HasOne(d => d.Signal)
                    .WithMany(p => p.StellwerkeProtokoll)
                    .HasForeignKey(d => d.SignalId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("stellwerke_protokoll_ibfk_2");
            });

            modelBuilder.Entity<StellwerkeStellauftraege>(entity =>
            {
                entity.ToTable("stellwerke_stellauftraege");

                entity.HasIndex(e => e.DwegId)
                    .HasName("dweg_id");

                entity.HasIndex(e => e.FahrstrasseId)
                    .HasName("fahrstrasse_id");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.HasIndex(e => e.UzId)
                    .HasName("uz_id");

                entity.HasIndex(e => e.ZielDir)
                    .HasName("ziel_dir");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.DwegId)
                    .HasColumnName("dweg_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FahrstrasseId)
                    .HasColumnName("fahrstrasse_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UzId)
                    .HasColumnName("uz_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ZielDir)
                    .HasColumnName("ziel_dir")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<StellwerkeUz>(entity =>
            {
                entity.ToTable("stellwerke_uz");

                entity.HasIndex(e => e.Betriebsstelle)
                    .HasName("betriebsstelle");

                entity.HasIndex(e => e.Elementbezeichnungen)
                    .HasName("elementbezeichnungen");

                entity.HasIndex(e => e.Tanaspannung)
                    .HasName("tanaspannung");

                entity.HasIndex(e => e.Wlk)
                    .HasName("wlk");

                entity.HasIndex(e => e.Zaehlwerk)
                    .HasName("zaehlwerk");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AktivesStellwerkId)
                    .HasColumnName("aktives_stellwerk_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Betriebsstelle)
                    .IsRequired()
                    .HasColumnName("betriebsstelle")
                    .HasColumnType("char(10)");

                entity.Property(e => e.Elementbezeichnungen)
                    .IsRequired()
                    .HasColumnName("elementbezeichnungen")
                    .HasColumnType("enum('aktiv','inaktiv')");

                entity.Property(e => e.Tanaspannung)
                    .IsRequired()
                    .HasColumnName("tanaspannung")
                    .HasColumnType("enum('ta','nawechsel','na','tawechsel')");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.Uzname)
                    .IsRequired()
                    .HasColumnName("uzname")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Wlk)
                    .IsRequired()
                    .HasColumnName("wlk")
                    .HasColumnType("enum('aus','ein')");

                entity.Property(e => e.Zaehlwerk)
                    .HasColumnName("zaehlwerk")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.BetriebsstelleNavigation)
                    .WithMany(p => p.StellwerkeUz)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Betriebsstelle)
                    .HasConstraintName("stellwerke_uz_ibfk_1");
            });

            modelBuilder.Entity<StellwerkeZuordnung>(entity =>
            {
                entity.ToTable("stellwerke_zuordnung");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.HasIndex(e => e.SignalId)
                    .HasName("signal_id");

                entity.HasIndex(e => e.StellwerkUzId)
                    .HasName("stellwerk_uz_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Art)
                    .IsRequired()
                    .HasColumnName("art")
                    .HasColumnType("enum('bedienung','spiegel')");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SignalId)
                    .HasColumnName("signal_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.StellwerkUzId)
                    .HasColumnName("stellwerk_uz_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Infra)
                    .WithMany(p => p.StellwerkeZuordnung)
                    .HasForeignKey(d => d.InfraId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("stellwerke_zuordnung_ibfk_2");

                entity.HasOne(d => d.Signal)
                    .WithMany(p => p.StellwerkeZuordnung)
                    .HasForeignKey(d => d.SignalId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("stellwerke_zuordnung_ibfk_3");

                entity.HasOne(d => d.StellwerkUz)
                    .WithMany(p => p.StellwerkeZuordnung)
                    .HasForeignKey(d => d.StellwerkUzId)
                    .HasConstraintName("stellwerke_zuordnung_ibfk_1");
            });

            modelBuilder.Entity<Stoerungen>(entity =>
            {
                entity.ToTable("stoerungen");

                entity.HasIndex(e => e.Betriebsstelle)
                    .HasName("betriebsstelle");

                entity.HasIndex(e => e.FahrzeugId)
                    .HasName("fahrzeug_id");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Art)
                    .IsRequired()
                    .HasColumnName("art")
                    .HasColumnType("enum('stellwerk','weiche','fahrzeug','sonstige','fahrstrasse','webstw')");

                entity.Property(e => e.Betriebsstelle)
                    .HasColumnName("betriebsstelle")
                    .HasColumnType("char(11)");

                entity.Property(e => e.Eintragzeit)
                    .HasColumnName("eintragzeit")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Erfasser)
                    .IsRequired()
                    .HasColumnName("erfasser")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.FahrzeugId)
                    .HasColumnName("fahrzeug_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Inhalt)
                    .IsRequired()
                    .HasColumnName("inhalt")
                    .HasColumnType("mediumtext");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasColumnType("enum('neu','in Arbeit','erledigt')");

                entity.Property(e => e.StatusMail)
                    .HasColumnName("status_mail")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.HasOne(d => d.BetriebsstelleNavigation)
                    .WithMany(p => p.Stoerungen)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Betriebsstelle)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("stoerungen_ibfk_1");

                entity.HasOne(d => d.Fahrzeug)
                    .WithMany(p => p.Stoerungen)
                    .HasForeignKey(d => d.FahrzeugId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("stoerungen_ibfk_2");

                entity.HasOne(d => d.Infra)
                    .WithMany(p => p.Stoerungen)
                    .HasForeignKey(d => d.InfraId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("stoerungen_ibfk_3");
            });

            modelBuilder.Entity<Strecken>(entity =>
            {
                entity.ToTable("strecken");

                entity.HasIndex(e => e.IstBahnhofsstrecke)
                    .HasName("ist_bahnhofsstrecke");

                entity.HasIndex(e => e.Nummer)
                    .HasName("nummer");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Beschreibung)
                    .IsRequired()
                    .HasColumnName("beschreibung")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.IstBahnhofsstrecke)
                    .HasColumnName("ist_bahnhofsstrecke")
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Nummer)
                    .HasColumnName("nummer")
                    .HasColumnType("char(10)");
            });

            modelBuilder.Entity<StreckenAbschnitte>(entity =>
            {
                entity.ToTable("strecken_abschnitte");

                entity.HasIndex(e => e.Betriebsstelle0)
                    .HasName("betriebsstelle0");

                entity.HasIndex(e => e.Betriebsstelle1)
                    .HasName("betriebsstelle1");

                entity.HasIndex(e => e.Laenge)
                    .HasName("laenge");

                entity.HasIndex(e => e.Reihenfolge)
                    .HasName("reihenfolge");

                entity.HasIndex(e => e.Richtung)
                    .HasName("richtung");

                entity.HasIndex(e => e.StreckenId)
                    .HasName("strecken_id");

                entity.HasIndex(e => new { e.StreckenId, e.Richtung, e.Reihenfolge })
                    .HasName("strecken_id_2")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Betriebsstelle0)
                    .IsRequired()
                    .HasColumnName("betriebsstelle0")
                    .HasColumnType("char(10)");

                entity.Property(e => e.Betriebsstelle1)
                    .IsRequired()
                    .HasColumnName("betriebsstelle1")
                    .HasColumnType("char(10)");

                entity.Property(e => e.Laenge)
                    .HasColumnName("laenge")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Reihenfolge)
                    .HasColumnName("reihenfolge")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Richtung)
                    .IsRequired()
                    .HasColumnName("richtung")
                    .HasColumnType("enum('0','1')");

                entity.Property(e => e.StreckenId)
                    .HasColumnName("strecken_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Betriebsstelle0Navigation)
                    .WithMany(p => p.StreckenAbschnitteBetriebsstelle0Navigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Betriebsstelle0)
                    .HasConstraintName("strecken_abschnitte_ibfk_2");

                entity.HasOne(d => d.Betriebsstelle1Navigation)
                    .WithMany(p => p.StreckenAbschnitteBetriebsstelle1Navigation)
                    .HasPrincipalKey(p => p.Kuerzel)
                    .HasForeignKey(d => d.Betriebsstelle1)
                    .HasConstraintName("strecken_abschnitte_ibfk_3");

                entity.HasOne(d => d.Strecken)
                    .WithMany(p => p.StreckenAbschnitte)
                    .HasForeignKey(d => d.StreckenId)
                    .HasConstraintName("strecken_abschnitte_ibfk_1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.Betriebsstelle)
                    .HasName("betriebsstelle");

                entity.HasIndex(e => e.Password)
                    .HasName("password");

                entity.HasIndex(e => e.Username)
                    .HasName("username");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Betriebsstelle)
                    .IsRequired()
                    .HasColumnName("betriebsstelle")
                    .HasColumnType("varchar(10)");

                entity.Property(e => e.Ipbereich)
                    .IsRequired()
                    .HasColumnName("ipbereich")
                    .HasColumnType("enum('lokal','vpn','alle')");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<UserAnmeldungenAktiv>(entity =>
            {
                entity.ToTable("user_anmeldungen_aktiv");

                entity.HasIndex(e => e.Host)
                    .HasName("host");

                entity.HasIndex(e => e.IsTrainer)
                    .HasName("is_trainer");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Host)
                    .IsRequired()
                    .HasColumnName("host")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.IsTrainer)
                    .HasColumnName("is_trainer")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<UserRechteliste>(entity =>
            {
                entity.ToTable("user_rechteliste");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Beschreibung)
                    .IsRequired()
                    .HasColumnName("beschreibung")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Recht)
                    .IsRequired()
                    .HasColumnName("recht")
                    .HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<UserStellwerke>(entity =>
            {
                entity.ToTable("user_stellwerke");

                entity.HasIndex(e => e.Berue)
                    .HasName("berue");

                entity.HasIndex(e => e.Idplan)
                    .HasName("idplan");

                entity.HasIndex(e => e.Lupe)
                    .HasName("lupe");

                entity.HasIndex(e => e.StellwerkId)
                    .HasName("stellwerk_id");

                entity.HasIndex(e => e.Streckenspiegel)
                    .HasName("streckenspiegel");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_id");

                entity.HasIndex(e => e.Zn)
                    .HasName("zn");

                entity.HasIndex(e => e.Zwl)
                    .HasName("zwl");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Berue)
                    .HasColumnName("berue")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Idplan)
                    .HasColumnName("idplan")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Lupe)
                    .HasColumnName("lupe")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Recht)
                    .IsRequired()
                    .HasColumnName("recht")
                    .HasColumnType("enum('betrachtung','bedienung')");

                entity.Property(e => e.StellwerkId)
                    .HasColumnName("stellwerk_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Streckenspiegel)
                    .HasColumnName("streckenspiegel")
                    .HasColumnType("int(1)");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Zn)
                    .HasColumnName("zn")
                    .HasColumnType("int(1)");

                entity.Property(e => e.Zwl)
                    .HasColumnName("zwl")
                    .HasColumnType("int(1)");

                entity.HasOne(d => d.Stellwerk)
                    .WithMany(p => p.UserStellwerke)
                    .HasForeignKey(d => d.StellwerkId)
                    .HasConstraintName("user_stellwerke_ibfk_2");
            });

            modelBuilder.Entity<UserZugriffsrechte>(entity =>
            {
                entity.ToTable("user_zugriffsrechte");

                entity.HasIndex(e => e.RechtId)
                    .HasName("recht_id");

                entity.HasIndex(e => e.UserId)
                    .HasName("user_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.RechtId)
                    .HasColumnName("recht_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Recht)
                    .WithMany(p => p.UserZugriffsrechte)
                    .HasForeignKey(d => d.RechtId)
                    .HasConstraintName("user_zugriffsrechte_ibfk_4");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserZugriffsrechte)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("user_zugriffsrechte_ibfk_3");
            });

            modelBuilder.Entity<WeichenAbhaengigkeit>(entity =>
            {
                entity.ToTable("weichen_abhaengigkeit");

                entity.HasIndex(e => e.AbhaengigkeitId)
                    .HasName("abhaengigkeit_id");

                entity.HasIndex(e => e.WeicheId)
                    .HasName("weiche_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AbhaengigkeitId)
                    .HasColumnName("abhaengigkeit_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.WeicheDir)
                    .HasColumnName("weiche_dir")
                    .HasColumnType("int(11)");

                entity.Property(e => e.WeicheId)
                    .HasColumnName("weiche_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Weiche)
                    .WithMany(p => p.WeichenAbhaengigkeit)
                    .HasForeignKey(d => d.WeicheId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("weichen_abhaengigkeit_ibfk_1");
            });

            modelBuilder.Entity<WeichenStellauftraege>(entity =>
            {
                entity.ToTable("weichen_stellauftraege");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SollDir)
                    .HasColumnName("soll_dir")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Timestamp)
                    .HasColumnName("timestamp")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'current_timestamp()'");
            });

            modelBuilder.Entity<WeichenVorzugslage>(entity =>
            {
                entity.ToTable("weichen_vorzugslage");

                entity.HasIndex(e => e.Dir)
                    .HasName("dir");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Dir)
                    .HasColumnName("dir")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Infra)
                    .WithMany(p => p.WeichenVorzugslage)
                    .HasForeignKey(d => d.InfraId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("weichen_vorzugslage_ibfk_1");
            });

            modelBuilder.Entity<ZuegeVerkehrsarten>(entity =>
            {
                entity.ToTable("zuege_verkehrsarten");

                entity.HasIndex(e => e.VerkehrsartKuerzel)
                    .HasName("verkehrsart_kuerzel")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Beschreibung)
                    .IsRequired()
                    .HasColumnName("beschreibung")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.VerkehrsartKuerzel)
                    .IsRequired()
                    .HasColumnName("verkehrsart_kuerzel")
                    .HasColumnType("char(2)");
            });

            modelBuilder.Entity<ZuegeZuggattungen>(entity =>
            {
                entity.ToTable("zuege_zuggattungen");

                entity.HasIndex(e => e.Fzs)
                    .HasName("fzs");

                entity.HasIndex(e => e.Verkehrsart)
                    .HasName("verkehrsart");

                entity.HasIndex(e => e.Zuggattung)
                    .HasName("zuggattung")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bezeichnung)
                    .IsRequired()
                    .HasColumnName("bezeichnung")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.BildfahrplanFarbe)
                    .IsRequired()
                    .HasColumnName("bildfahrplan_farbe")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Fzs)
                    .IsRequired()
                    .HasColumnName("fzs")
                    .HasColumnType("enum('pz','gz','ra')");

                entity.Property(e => e.Verkehrsart)
                    .IsRequired()
                    .HasColumnName("verkehrsart")
                    .HasColumnType("char(2)");

                entity.Property(e => e.Zuggattung)
                    .IsRequired()
                    .HasColumnName("zuggattung")
                    .HasColumnType("varchar(11)");

                entity.HasOne(d => d.VerkehrsartNavigation)
                    .WithMany(p => p.ZuegeZuggattungen)
                    .HasPrincipalKey(p => p.VerkehrsartKuerzel)
                    .HasForeignKey(d => d.Verkehrsart)
                    .HasConstraintName("zuege_zuggattungen_ibfk_1");
            });

            modelBuilder.Entity<ZuglenkungAktuell>(entity =>
            {
                entity.ToTable("zuglenkung_aktuell");

                entity.HasIndex(e => e.Abfahrtzeit)
                    .HasName("abfahrtzeit_2");

                entity.HasIndex(e => e.Dispohalt)
                    .HasName("dispohalt_2");

                entity.HasIndex(e => e.EinstellungErfolgt)
                    .HasName("einstellung_erfolgt");

                entity.HasIndex(e => e.FahrstrasseId)
                    .HasName("fahrstrasse_id");

                entity.HasIndex(e => e.FreimeldeabschnittId)
                    .HasName("freimeldeabschnitt_id");

                entity.HasIndex(e => e.GbtId)
                    .HasName("gbt_id");

                entity.HasIndex(e => e.Lu)
                    .HasName("lu_2");

                entity.HasIndex(e => e.Reihenfolge)
                    .HasName("reihenfolge");

                entity.HasIndex(e => e.Wartezeit)
                    .HasName("wartezeit_2");

                entity.HasIndex(e => e.ZugId)
                    .HasName("zug_id");

                entity.HasIndex(e => e.Zugnummer)
                    .HasName("zugnummer");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Abfahrtzeit)
                    .HasColumnName("abfahrtzeit")
                    .HasColumnType("time");

                entity.Property(e => e.Dispohalt)
                    .HasColumnName("dispohalt")
                    .HasColumnType("int(1)");

                entity.Property(e => e.EinstellungErfolgt)
                    .HasColumnName("einstellung_erfolgt")
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.FahrstrasseId)
                    .HasColumnName("fahrstrasse_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FreimeldeabschnittId)
                    .HasColumnName("freimeldeabschnitt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.GbtId)
                    .HasColumnName("gbt_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Lu)
                    .IsRequired()
                    .HasColumnName("lu")
                    .HasColumnType("enum('0','1')")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Reihenfolge)
                    .HasColumnName("reihenfolge")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Wartezeit)
                    .HasColumnName("wartezeit")
                    .HasColumnType("int(11)");

                entity.Property(e => e.ZugId)
                    .HasColumnName("zug_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Zugnummer)
                    .HasColumnName("zugnummer")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<ZuglenkungSelbststellbetrieb>(entity =>
            {
                entity.ToTable("zuglenkung_selbststellbetrieb");

                entity.HasIndex(e => e.FahrstrasseId)
                    .HasName("fahrstrasse_id");

                entity.HasIndex(e => e.SignalId)
                    .HasName("signal_id_2")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FahrstrasseId)
                    .HasColumnName("fahrstrasse_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.SignalId)
                    .HasColumnName("signal_id")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Fahrstrasse)
                    .WithMany(p => p.ZuglenkungSelbststellbetrieb)
                    .HasForeignKey(d => d.FahrstrasseId)
                    .HasConstraintName("zuglenkung_selbststellbetrieb_ibfk_2");

                entity.HasOne(d => d.Signal)
                    .WithOne(p => p.ZuglenkungSelbststellbetrieb)
                    .HasForeignKey<ZuglenkungSelbststellbetrieb>(d => d.SignalId)
                    .HasConstraintName("zuglenkung_selbststellbetrieb_ibfk_1");
            });

            modelBuilder.Entity<ZuglenkungTemp>(entity =>
            {
                entity.ToTable("zuglenkung_temp");

                entity.HasIndex(e => e.AnstosszeitTimestamp)
                    .HasName("anstosszeit_timestamp");

                entity.HasIndex(e => e.BelegungTimestamp)
                    .HasName("belegung_timestamp");

                entity.HasIndex(e => e.EinstellungTimestamp)
                    .HasName("einstellung_timestamp");

                entity.HasIndex(e => e.EinstellungZaehler)
                    .HasName("einstellung_zaehler");

                entity.HasIndex(e => e.FahrstrasseId)
                    .HasName("fahrstrasse_id");

                entity.HasIndex(e => e.InfraId)
                    .HasName("infra_id");

                entity.HasIndex(e => e.LenkplanId)
                    .HasName("lenkplan_id");

                entity.HasIndex(e => e.Reihenfolge)
                    .HasName("reihenfolge");

                entity.HasIndex(e => e.ZugId)
                    .HasName("zug_id");

                entity.HasIndex(e => e.Zugnummer)
                    .HasName("zugnummer");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.AnstosszeitTimestamp)
                    .HasColumnName("anstosszeit_timestamp")
                    .HasColumnType("int(11)");

                entity.Property(e => e.BelegungTimestamp)
                    .HasColumnName("belegung_timestamp")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Dispohalt)
                    .HasColumnName("dispohalt")
                    .HasColumnType("int(1)");

                entity.Property(e => e.EinstellungTimestamp)
                    .HasColumnName("einstellung_timestamp")
                    .HasColumnType("int(11)");

                entity.Property(e => e.EinstellungZaehler)
                    .HasColumnName("einstellung_zaehler")
                    .HasColumnType("int(11)");

                entity.Property(e => e.FahrstrasseId)
                    .HasColumnName("fahrstrasse_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.InfraId)
                    .HasColumnName("infra_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.LenkplanId)
                    .HasColumnName("lenkplan_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Reihenfolge)
                    .HasColumnName("reihenfolge")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ZugId)
                    .HasColumnName("zug_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Zugnummer)
                    .HasColumnName("zugnummer")
                    .HasColumnType("int(11)");
            });
        }
    }
}
