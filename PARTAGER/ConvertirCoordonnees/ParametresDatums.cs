using static FCGP.ConvertirCoordonnees;
using static FCGP.Enumerations;

namespace FCGP.Coordonnees
{
    /// <summary> Définit les champs nécessaires à un système géodésique à 3 paramètres (sans la rotation autour des axes) </summary>
    static class PARAM_DATUMS
    {
        #region Données
        internal readonly static (Projections Projection, string EPSG, string LibellesEPSG, Ellipsoides Ellipsoide)[] ParametresDatums =
            new (Projections, string, string, Ellipsoides)[] {
                (Projections.Aucun, "", "", Ellipsoides.Aucun),
                (Projections.UTM, "230ZZ", "ED50 / UTM zone ZZN", Ellipsoides.HAY09),
                (Projections.Lambert, "27561", "NTF (Paris) / Nord", Ellipsoides.C_1880_M),
                (Projections.Lambert, "27562", "NTF (Paris) / Centre", Ellipsoides.C_1880_M),
                (Projections.Lambert, "27572", "NTF (Paris) / zone II étendue", Ellipsoides.C_1880_M),
                (Projections.Lambert, "27563", "NTF (Paris) / Sud", Ellipsoides.C_1880_M),
                (Projections.Lambert, "27564", "NTF (Paris) / Corse", Ellipsoides.C_1880_M),
                (Projections.Lambert, "2154", "RGF93 / Lambert-93 -- France", Ellipsoides.GRS80),
                (Projections.Suisse, "21781", "Swiss CH1903 / LV03", Ellipsoides.BESSEL_1841),
                (Projections.Suisse, "21781", "Swiss CH1903 / LV03", Ellipsoides.BESSEL_1841),
                (Projections.Suisse, "2056", "Swiss CH1903+ / LV95", Ellipsoides.BESSEL_1841),
                (Projections.UTM, "326ZZ", "WGS 84 / UTM zone ZZN", Ellipsoides.WGS84),
                (Projections.LatLon, "32662", "WGS 84 / Plate Carree", Ellipsoides.WGS84),
                (Projections.WMercator, "3857", "WGS 84 / Pseudo-Mercator -- Spherical", Ellipsoides.WGS84),
                (Projections.Datum, "4171", "Reseau Geodesique Francais 1993", Ellipsoides.GRS80),
                (Projections.Datum, "4326", "World Geodetic System 1984", Ellipsoides.WGS84),
                (Projections.Datum, "4230", "European Datum 1950", Ellipsoides.HAY09),
                (Projections.Datum, "4275", "Nouvelle Triangulation Francaise", Ellipsoides.C_1880_M),
                (Projections.Datum, "4149", "CH 1903", Ellipsoides.BESSEL_1841) };                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         // CH1903
        internal static (double DX, double DY, double DZ, double A, double F, string EPSG)[] ParametresEllipsoides =
            new (double, double, double, double, double, string)[] {
                (0.0d, 0.0d, 0.0d, 0.0d, 0.0d, ""),
                (-168.0d, -60.0d, 320.0d, 6378249.2d, 1.0d / 293.466021293625d, "7011"),
                (0.0d, 0.0d, 0.0d, RayonWGS84, 1.0d / 298.257222101d, "7019"),
                (-89.5d, -93.8d, -123.1d, 6378388.0d, 1.0d / 297.0d, "7022"),
                (0.0d, 0.0d, 0.0d, RayonWGS84, F_WGS84, "7030"),
                (674.374d, 15.056d, 405.346d, 6377397.155d, 1.0d / 299.15281285d, "7004") };                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              // BESSEL_1841
        #endregion
        #region propriétés
        internal static Datums Datum { get; private set; }
        internal static Ellipsoides Ellipsoide { get; private set; }
        internal static string LibelleDatums
        {
            get
            {
                return ParametresDatums[(int)Datum].LibellesEPSG;
            }
        }
        internal static string EPSGDatum
        {
            get
            {
                return ParametresDatums[(int)Datum].EPSG;
            }
        }
        internal static string EPSGEllipsoide
        {
            get
            {
                return ParametresEllipsoides[(int)Ellipsoide].EPSG;
            }
        }
        internal static string LibelleEllipsoide
        {
            get
            {
                return LibellesEllipsoides[(int)Ellipsoide];
            }
        }
        internal static Projections Projection
        {
            get
            {
                return ParametresDatums[(int)Datum].Projection;
            }
        }
        internal static double DX { get; private set; }
        internal static double DY { get; private set; }
        internal static double DZ { get; private set; }
        internal static double A { get; private set; }
        internal static double F { get; private set; }
        internal static double E2 { get; private set; }
        internal static double EP2 { get; private set; }
        internal static double B { get; private set; }
        #endregion
        // met à jour les paramètres de conversion des coordonnées cartésiennes du datum vers
        // les coordonnées cartésiennes de WGS84 qui est le système pivot
        internal static void SetParametresDatum(Datums Datum)
        {
            PARAM_DATUMS.Datum = Datum;
            Ellipsoide = ParametresDatums[(int)PARAM_DATUMS.Datum].Ellipsoide;
            DX = ParametresEllipsoides[(int)Ellipsoide].DX;
            DY = ParametresEllipsoides[(int)Ellipsoide].DY;
            DZ = ParametresEllipsoides[(int)Ellipsoide].DZ;
            A = ParametresEllipsoides[(int)Ellipsoide].A;
            F = ParametresEllipsoides[(int)Ellipsoide].F;
            E2 = 2.0d * F - F * F;
            EP2 = 1.0d / (1.0d - E2) - 1.0d;
            B = A * (1.0d - F);
        }
    }
}