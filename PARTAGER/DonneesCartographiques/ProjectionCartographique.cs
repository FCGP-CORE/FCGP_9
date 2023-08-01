using static FCGP.Enumerations;

namespace FCGP
{
    /// <summary> définit une projection d'un système cartographique </summary>
    internal class ProjectionCartographique
    {
        #region Données constantes
        /// <summary> Retourne le libellé du datum prinpical associé à un site carto </summary>
        /// <param name="SiteCarto"> site cartographique servant de base à une carte ou un regroupement </param>
        internal static string DatumPrincipalLibelle(SitesCartographiques Sitecarto)
        {
            return DatumsLibelles[(int)_Datums[(int)Sitecarto][0]];
        }
        /// <summary> Retourne le datum princal associé à un site carto </summary>
        /// <param name="SiteCarto"> site cartographique servant de base à une carte ou un regroupement </param>
        internal static Datums DatumPrincipal(SitesCartographiques Sitecarto)
        {
            return _Datums[(int)Sitecarto][0];
        }
        #endregion
        #region Données constantes
        /// <summary>le système suport peut avoir plusieurs type de coordonnées. info présente dans le fichier GeoRef. 
        /// Actuellement seulement 2 types de coordonnées différents par système. Tableau escaliers IndiceSytème et Indiceprojection </summary>
        /// <remarks> l'indice 0 des datums correspond au datum principal du site ou du regroupement</remarks>
        private static readonly Datums[][] _Datums = new[] {
            new Datums[] { Datums.WGS84, Datums.Lambert_93, Datums.Web_Mercator },
            new Datums[] { Datums.Grille_Suisse_LV03, Datums.Grille_Suisse },
            new Datums[] { Datums.WGS84, Datums.UTM_WGS84, Datums.Web_Mercator },
            new Datums[] { Datums.WGS84, Datums.Web_Mercator },
            new Datums[] { Datums.WGS84, Datums.Web_Mercator },
            new Datums[] { Datums.WGS84, Datums.Web_Mercator } };                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        // ES
        #endregion
        #region Functions et propriétés visibles
        /// <summary> renvoie la projection associé au sitecarto </summary>
        /// <param name="SiteCarto">site carto associé à la projection</param>
        /// <param name="LibelleProjection">libellé de la projection (datum) </param>
        internal ProjectionCartographique(SitesCartographiques SiteCarto, string LibelleProjection)
        {
            Datum = (Datums)Array.IndexOf(DatumsLibelles, LibelleProjection);
            Initialise(SiteCarto);
        }
        /// <summary> renvoie la projection associé au sitecarto </summary>
        /// <param name="SiteCarto">site carto associé à la projection</param>
        /// <param name="DatumProjection"> Datum de la projection </param>
        internal ProjectionCartographique(SitesCartographiques SiteCarto, Datums DatumProjection)
        {
            Datum = DatumProjection;
            Initialise(SiteCarto);
        }
        /// <summary>donne l'unité de coordonnées associée à un type de projection</summary>
        internal UnitesCoordonnees UniteCoordonnees
        {
            get
            {
                if (Datum == Datums.Aucun)
                {
                    return UnitesCoordonnees.Aucun;
                }
                if (Datum < Datums.RGF93)
                {
                    return UnitesCoordonnees.Mètre;
                }
                else
                {
                    return UnitesCoordonnees.DMS;
                }
            }
        }
        /// <summary>chaque type de projection est associé à un datum ou ellipsoïde</summary>
        internal Datums Datum { get; private set; }
        /// <summary>donne le libellé associé à un type de projection</summary>
        internal string Libelle
        {
            get
            {
                if (Datum == Datums.Aucun)
                {
                    return "Aucun";
                }
                return DatumsLibelles[(int)Datum];
            }
        }
        /// <summary>true si la création de la projection c'est bien passée</summary>
        internal bool IsOk { get; private set; }
        private void Initialise(SitesCartographiques SiteCarto)
        {
            if (Array.IndexOf(_Datums[(int)SiteCarto], Datum) > -1)
            {
                IsOk = true;
            }
            else
            {
                Datum = Datums.Aucun;
            }
        }
        #endregion
    }
}