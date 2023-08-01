using static FCGP.AfficheInformation;
using static FCGP.Commun;
using static FCGP.Enumerations;

namespace FCGP
{
    internal partial class Carte
    {
        #region Fichiers de géoréférencement
        /// <summary> Ecrit le fichier de géoréférencement spécifique à FCGP </summary>
        private bool EcrireFichierGeoref()
        {
            bool EcrireFichierGeorefRet = false;
            try
            {
                {
                    ref var WB = ref SystemCartographique;
                    int Cpt;
                    var S = new StringBuilder();   // pour écrire le fichier des structures georef
                    S.AppendLine($"\"Carte\" Nom : {NomComplet + "." + Format.ToString()}, Site : {WB.LibelleSiteCarto}" + (WB.SiteCarto == SitesCartographiques.DomTom ? "-" + WB.LibelleDomtom : ""));
                    S.AppendLine($"\"Carte\" Echelle : {WB.Niveau.Clef}, Echelle Impression : {WB.Niveau.ImpressionClef}, Coordonnées : {WB.Projection.Libelle}" + (WB.Projection.Libelle[0] == 'U' ? $" {WB.HemisphereUtmReferencement} {WB.ZoneUtmReferencement:00}" : ""));
                    S.AppendLine($"\"Carte\" Retail : {(WB.IsInterpol == true ? "Oui" : "Non")}, Grille : {(GrilleIsAffiche == true ? "Oui" : "Non")}" + $", DPI_X : {DblToStr(DPIImpressionX, "000.00")}, DPI_Y : {DblToStr(DPIImpressionY, "000.00")}");
                    // Pour chaque coin de la carte
                    for (Cpt = 0; Cpt <= 3; Cpt++)
                    {
                        // écrit les coordonnées capturées sur le site internet
                        S.AppendLine($"\"Pt{Cpt:0}\" Coordonnées capturées : {Coins[Cpt].CoordonneesCaptureEcran}");
                        // écrit la longitude et la latitude en degrés décimaux avec le signe plus ou moins
                        S.AppendLine($"\"Pt{Cpt:0}\" " + (Coins[Cpt].Longitude >= 0d ? "Longitude : +" : "Longitude : ") + DblToStr(Coins[Cpt].Longitude, "000.00000000") + ", " + (Coins[Cpt].Latitude >= 0d ? "Latitude : +" : "Latitude : ") + DblToStr(Coins[Cpt].Latitude, "00.00000000"));
                        // écrit les X et Y en pixel
                        S.AppendLine($"\"Pt{Cpt:0}\" X image : {Coins[Cpt].X_Pixel:000000}, Y image : {Coins[Cpt].Y_Pixel:000000}");
                    }
                    // écrit le nb de zones UTM de la carte
                    S.AppendLine("\"UTM WGS84\" NB Zones : " + NbZonesUTM);
                    // Pour chaque coin de la carte et pour chaque zone UTM
                    for (int Boucle = 0, loopTo = NbZonesUTM - 1; Boucle <= loopTo; Boucle++)
                    {
                        for (Cpt = 0; Cpt <= 3; Cpt++)
                            // écrit les coordonnées UTM, l'hémisphère et la zone
                            S.AppendLine($"\"Pt{Cpt:0}\" " + (Coins[Cpt].UTMs[Boucle].X < 0d ? "X : " : "X : +") + DblToStr(Coins[Cpt].UTMs[Boucle].X, "00000000.0") + ", " + (Coins[Cpt].UTMs[Boucle].Y < 0d ? "Y : " : "Y : +") + DblToStr(Coins[Cpt].UTMs[Boucle].Y, "00000000.0") + ", " + $"Hemisphère : {Coins[0].Hemisphere_UTM}, Zone : {Coins[0].NumZone_UTM + Boucle:00}");
                    }

                    File.WriteAllText(CheminCarte + @"\" + NomComplet + ".georef", S.ToString(), Encoding_FCGP);
                }
                EcrireFichierGeorefRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "E6A4");
            }

            return EcrireFichierGeorefRet;
        }
        /// <summary> Ecrit le fichier de géoréférencement spécifique à CompeGPSLand en système WGS84 (latitude et longitude)
        /// pour 2 coins  de la carte</summary>
        private bool EcrireCompeLandMercator()
        {
            bool EcrireCompeLandMercatorRet = false;
            try
            {
                int Cpt;
                var S = new StringBuilder();   // pour écrire le fichier des structures georef
                                               // l'entête est  toujours le même
                S.AppendLine("CompeGPS MAP File" + CrLf + "<Header>" + CrLf + "Version=2" + CrLf + "VerCompeGPS=7.7.2" + CrLf + "Projection=2,Mercator," + CrLf + "Coordinates=1" + CrLf + "Datum=WGS 84" + CrLf + "</Header>");

                S.AppendLine("<Map>" + CrLf + "Bitmap=" + NomComplet + "." + Format.ToString() + CrLf + "BitsPerPixel=0" + CrLf + "BitmapWidth=" + Coins[2].X_Pixel.ToString() + CrLf + "BitmapHeight=" + Coins[2].Y_Pixel.ToString() + CrLf + "Type=10" + CrLf + "</Map>" + CrLf + "<Calibration>");
                for (Cpt = 0; Cpt <= 2; Cpt += 2)
                    S.AppendLine("P" + Cpt / 2 + "=" + Coins[Cpt].X_Pixel.ToString() + "," + Coins[Cpt].Y_Pixel.ToString() + ",A," + DblToStr(Coins[Cpt].Longitude, "0.00000000") + "," + DblToStr(Coins[Cpt].Latitude, "0.00000000"));
                S.AppendLine("</Calibration>" + CrLf + "<MainPolygonBitmap>");
                for (Cpt = 0; Cpt <= 3; Cpt++)
                    S.AppendLine("M" + Cpt + "=" + DblToStr(Coins[Cpt].X_Pixel, "0.0") + "," + DblToStr(Coins[Cpt].Y_Pixel, "0.0"));
                S.Append("</MainPolygonBitmap>");

                File.WriteAllText(CheminCarte + @"\" + NomComplet + ".Imp", S.ToString(), Encoding_FCGP);
                EcrireCompeLandMercatorRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "W2T9");
            }

            return EcrireCompeLandMercatorRet;
        }
        /// <summary> Ecrit le fichier de géoréférencement spécifique à CompeGPSLand en MN03 (projection des cartes suisses) associé à CH1903 (système Suisse)
        /// pour 2 coins  de la carte</summary>
        private bool EcrireCompeLandLV03()
        {
            bool EcrireCompeLandLV03Ret = false;
            try
            {
                int Cpt;
                var S = new StringBuilder();   // pour écrire le fichier des structures georef
                                               // l'entête est  toujours le même
                S.AppendLine("CompeGPS MAP File" + CrLf + "<Header>" + CrLf + "Version=2" + CrLf + "VerCompeGPS=7.7.2" + CrLf + "Projection=14,Swiss Grid," + CrLf + "Coordinates=1" + CrLf + "Datum=CH-1903" + CrLf + "</Header>");

                S.AppendLine("<Map>" + CrLf + "Bitmap=" + NomComplet + "." + Format.ToString() + CrLf + "BitsPerPixel=0" + CrLf + "BitmapWidth=" + Coins[2].X_Pixel.ToString() + CrLf + "BitmapHeight=" + Coins[2].Y_Pixel.ToString() + CrLf + "Type=10" + CrLf + "</Map>" + CrLf + "<Calibration>");
                for (Cpt = 0; Cpt <= 2; Cpt += 2)
                {
                    var PGR = Coins[Cpt].Grille;
                    ProjectionSuisse.LV95ToLV03(ref PGR);
                    var PDD = ProjectionSuisse.ConvertLV03ToCH1903(PGR, true);
                    S.AppendLine("P" + Cpt / 2 + "=" + Coins[Cpt].X_Pixel.ToString() + "," + Coins[Cpt].Y_Pixel.ToString() + ",A," + DblToStr(PDD.Lon, "0.00000000") + "," + DblToStr(PDD.Lat, "0.00000000"));
                }
                S.AppendLine("</Calibration>" + CrLf + "<MainPolygonBitmap>");
                for (Cpt = 0; Cpt <= 3; Cpt++)
                    S.AppendLine("M" + Cpt + "=" + DblToStr(Coins[Cpt].X_Pixel, "0.0") + "," + DblToStr(Coins[Cpt].Y_Pixel, "0.0"));
                S.Append("</MainPolygonBitmap>");

                File.WriteAllText(CheminCarte + @"\" + NomComplet + ".Imp", S.ToString(), Encoding_FCGP);
                EcrireCompeLandLV03Ret = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "W2K7");
            }

            return EcrireCompeLandLV03Ret;
        }
        /// <summary> Ecrit le fichier de géoréférencement spécifique à CompeGPSLand en système WGS84 (latitude et longitude)
        /// pour 2 coins  de la carte</summary>
        private bool EcrireCompeLandWGS84()
        {
            bool EcrireCompeLandWGS84Ret = false;
            try
            {
                int Cpt;
                var S = new StringBuilder();   // pour écrire le fichier des structures georef
                                               // l'entête est  toujours le même
                S.AppendLine("CompeGPS MAP File" + CrLf + "<Header>" + CrLf + "Version=2" + CrLf + "VerCompeGPS=7.7.2" + CrLf + "Projection=1,Lat/Long," + CrLf + "Coordinates=1" + CrLf + "Datum=WGS 84" + CrLf + "</Header>");
                S.AppendLine("<Map>" + CrLf + "Bitmap=" + NomComplet + "." + Format.ToString() + CrLf + "BitsPerPixel=0" + CrLf + "BitmapWidth=" + Coins[2].X_Pixel.ToString() + CrLf + "BitmapHeight=" + Coins[2].Y_Pixel.ToString() + CrLf + "Type=10" + CrLf + "</Map>" + CrLf + "<Calibration>");
                for (Cpt = 0; Cpt <= 2; Cpt += 2)
                    S.AppendLine("P" + Cpt / 2 + "=" + Coins[Cpt].X_Pixel.ToString() + "," + Coins[Cpt].Y_Pixel.ToString() + ",A," + DblToStr(Coins[Cpt].Longitude, "0.00000000") + "," + DblToStr(Coins[Cpt].Latitude, "0.00000000"));
                S.AppendLine("</Calibration>" + CrLf + "<MainPolygonBitmap>");
                for (Cpt = 0; Cpt <= 3; Cpt++)
                    S.AppendLine("M" + Cpt + "=" + DblToStr(Coins[Cpt].X_Pixel, "0.0") + "," + DblToStr(Coins[Cpt].Y_Pixel, "0.0"));
                S.Append("</MainPolygonBitmap>");

                File.WriteAllText(CheminCarte + @"\" + NomComplet + ".Imp", S.ToString(), Encoding_FCGP);
                EcrireCompeLandWGS84Ret = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "B4C0");
            }

            return EcrireCompeLandWGS84Ret;
        }
        /// <summary> Ecrit le fichier de géoréférencement spécifique à MAPINFO en MN03 (projection des cartes suisses) associé à CH1903 (système Suisse)
        /// pour chaque coin  de la carte ou de la capture on écrit les X, Y geographique et les pixels correspondants sous la forme
        /// (Longitude, Latidude) (Ximg, Yimg) Label "PTn",. Il n'y a pas de virgule pour le dernier coin </summary>
        /// <remarks>Si le numéro d'écran est vide, il s'agit d'une carte et non une capture écran</remarks>
        private bool EcrireMapInfoLV03()
        {
            bool EcrireMapInfoLV03Ret = false;
            try
            {
                int Cpt;
                var S = new StringBuilder();   // pour écrire le fichier des structures georef
                                               // l'entête est  toujours le même
                S.AppendLine("!table" + CrLf + "!version 300" + CrLf + "!charset WindowsLatin1" + CrLf + CrLf + "Definition Table" + CrLf + "  File \"" + NomComplet + "." + Format.ToString() + "\"" + CrLf + "  Type \"RASTER\"");
                // Il n'y a pas de virgule pour le dernier coin 
                for (Cpt = 0; Cpt <= 3; Cpt++)
                {
                    var PGR = Coins[Cpt].Grille;
                    ProjectionSuisse.LV95ToLV03(ref PGR);
                    S.AppendLine((Coins[Cpt].X_Grid >= 0d ? "  (+" : "  (") + DblToStr(PGR.X, "00000000.0") + (Coins[Cpt].Y_Grid >= 0d ? ",+" : ",") + DblToStr(PGR.Y, "00000000.0") + ") (" + Coins[Cpt].X_Pixel.ToString("000000") + "," + Coins[Cpt].Y_Pixel.ToString("000000") + ") Label \"Pt" + Cpt.ToString("0") + "\"" + (Cpt != 3 ? "," : ""));
                }
                // projection utm associée à l'ellipsoïde wgs84. elle change avec la zone UTM
                S.Append("  CoordSys Earth Projection 25, 1003, \"m\", 7.4395833333, 46.9524055555, 600000, 200000" + CrLf + "  Units \"m\"" + CrLf + "  RasterStyle 7 0");

                File.WriteAllText(CheminCarte + @"\" + NomComplet + ".Tab", S.ToString(), Encoding_FCGP);
                EcrireMapInfoLV03Ret = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "Q3I6");
            }

            return EcrireMapInfoLV03Ret;
        }
        /// <summary> Ecrit le fichier de géoréférencement spécifique à MAPINFO en UTM Mercator associé à wgs84
        /// pour chaque coin  de la carte ou de la capture on écrit les X, Y geographique et les pixels correspondants sous la forme
        /// (Longitude, Latidude) (Ximg, Yimg) Label "PTn",. Il n'y a pas de virgule pour le dernier coin </summary>
        /// <remarks>Si le numéro d'écran est vide, il s'agit d'une carte et non une capture écran</remarks>
        private bool EcrireMapInfoUTM()
        {
            bool EcrireMapInfoUTMRet = false;
            try
            {
                int Cpt;
                var S = new StringBuilder();   // pour écrire le fichier des structures georef
                                               // l'entête est  toujours le même
                S.AppendLine("!table" + CrLf + "!version 300" + CrLf + "!charset WindowsLatin1" + CrLf + CrLf + "Definition Table" + CrLf + "  File \"" + NomComplet + "." + Format.ToString() + "\"" + CrLf + "  Type \"RASTER\"");
                // Il n'y a pas de virgule pour le dernier coin 
                for (Cpt = 0; Cpt <= 3; Cpt++)
                    S.AppendLine((Coins[Cpt].UTMs[0].X >= 0d ? "  (+" : "  (") + DblToStr(Coins[Cpt].UTMs[0].X, "00000000.0") + (Coins[Cpt].UTMs[0].Y >= 0d ? ",+" : ",") + DblToStr(Coins[Cpt].UTMs[0].Y, "00000000.0") + ") (" + Coins[Cpt].X_Pixel.ToString("000000") + "," + Coins[Cpt].Y_Pixel.ToString("000000") + ") Label \"Pt" + Cpt.ToString("0") + "\"" + (Cpt != 3 ? "," : ""));
                // projection utm associée à l'ellipsoïde wgs84. elle change avec la zone UTM
                S.Append("  CoordSys Earth Projection 8, 104, \"m\", " + (Coins[0].NumZone_UTM * 6 - 183).ToString("0") + ", 0, 0.9996, 500000, 0" + CrLf + "  Units \"m\"" + CrLf + "  RasterStyle 7 0");

                File.WriteAllText(CheminCarte + @"\" + NomComplet + "_UTM.Tab", S.ToString(), Encoding_FCGP);
                EcrireMapInfoUTMRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "T5J2");
            }

            return EcrireMapInfoUTMRet;
        }
        /// <summary> Ecrit le fichier de géoréférencement spécifique à MAPINFO en longitude, latitude WGS84 pour Carte DT ou carte interpolée SM ou GF
        /// pour chaque coin  de la carte ou de la capture on écrit les X, Y geographique et les pixels correspondants sous la forme
        /// (Longitude, Latidude) (Ximg, Yimg) Label "PTn",. 6 caractères pour les X, Y à compléter avec des 0 si besoin est </summary>
        private bool EcrireMapInfoWGS84()
        {
            bool EcrireMapInfoWGS84Ret = false;
            try
            {
                int Cpt;
                var S = new StringBuilder();   // pour écrire le fichier des structures georef
                                               // l'entête est  toujours le même
                S.AppendLine("!table" + CrLf + "!version 300" + CrLf + "!charset WindowsLatin1" + CrLf + CrLf + "Definition Table" + CrLf + "  File \"" + NomComplet + "." + Format.ToString() + "\"" + CrLf + "  Type \"RASTER\"");
                // Il n'y a pas de virgule pour le dernier coin 
                for (Cpt = 0; Cpt <= 3; Cpt++)
                    S.AppendLine((Coins[Cpt].Longitude >= 0d ? "  (+" : "  (") + DblToStr(Coins[Cpt].Longitude, "000.0000000") + (Coins[Cpt].Latitude >= 0d ? ",+" : ",") + DblToStr(Coins[Cpt].Latitude, "00.0000000") + ") (" + Coins[Cpt].X_Pixel.ToString("000000") + "," + Coins[Cpt].Y_Pixel.ToString("000000") + ") Label \"Pt" + Cpt.ToString("0") + "\"" + (Cpt != 3 ? "," : ""));
                // la fin est toujours la même : projection lat/lon et ellipsoïde wgs84
                S.Append("  CoordSys Earth Projection 1, 104" + CrLf + "  Units \"Degree\"" + CrLf + "  RasterStyle 7 0");

                File.WriteAllText(CheminCarte + @"\" + NomComplet + ".Tab", S.ToString(), Encoding_FCGP);
                EcrireMapInfoWGS84Ret = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "F8W1");
            }

            return EcrireMapInfoWGS84Ret;
        }
        /// <summary> Ecrit le fichier de géoréférencement spécifique à MAPINFO en longitude, latitude WGS84 pour Carte DT ou carte interpolée SM ou GF
        /// pour chaque coin  de la carte ou de la capture on écrit les X, Y geographique et les pixels correspondants sous la forme
        /// (Longitude, Latidude) (Ximg, Yimg) Label "PTn",. 6 caractères pour les X, Y à compléter avec des 0 si besoin est </summary>
        private bool EcrireMapInfoMercator()
        {
            bool EcrireMapInfoMercatorRet = false;
            try
            {
                int Cpt;
                var S = new StringBuilder();   // pour écrire le fichier des structures georef
                                               // l'entête est  toujours le même
                S.AppendLine("!table" + CrLf + "!version 300" + CrLf + "!charset WindowsLatin1" + CrLf + CrLf + "Definition Table" + CrLf + "  File \"" + NomComplet + "." + Format.ToString() + "\"" + CrLf + "  Type \"RASTER\"");
                // Il n'y a pas de virgule pour le dernier coin 
                for (Cpt = 0; Cpt <= 3; Cpt++)
                    S.AppendLine((Coins[Cpt].Longitude >= 0d ? "  (+" : "  (") + DblToStr(Coins[Cpt].Longitude, "000.0000000") + (Coins[Cpt].Latitude >= 0d ? ",+" : ",") + DblToStr(Coins[Cpt].Latitude, "00.0000000") + ") (" + Coins[Cpt].X_Pixel.ToString("000000") + "," + Coins[Cpt].Y_Pixel.ToString("000000") + ") Label \"Pt" + Cpt.ToString("0") + "\"" + (Cpt != 3 ? "," : ""));
                // la fin est toujours la même : projection mercator et ellipsoïde wgs84
                S.Append("  CoordSys Earth Projection 10, 104, \"m\", 0" + CrLf + "  Units \"Degree\"" + CrLf + "  RasterStyle 7 0");

                File.WriteAllText(CheminCarte + @"\" + NomComplet + ".Tab", S.ToString(), Encoding_FCGP);
                EcrireMapInfoMercatorRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "A9P8");
            }

            return EcrireMapInfoMercatorRet;
        }
        /// <summary> Ecrit le fichier de géoréférencement spécifique à OziExploreur en système Mercator (grille ) 
        /// valable pour le site SM ou GF interpolé ou DT. Voir la description du format sur le net</summary>
        private bool EcrireOziExploreurMercator()
        {
            bool EcrireOziExploreurMercatorRet = false;
            try
            {
                var S = new StringBuilder();   // pour écrire le fichier
                                               // l'entête est  toujours le même
                S.AppendLine("OziExplorer Map Data File Version 2.2" + CrLf + NomComplet + "." + Format.ToString() + CrLf + CheminCarte + @"\" + NomComplet + "." + Format.ToString() + CrLf + "1 ,Map Code," + CrLf + "WGS 84,WGS 84,   0.0000,   0.0000,WGS 84" + CrLf + "Reserved 1" + CrLf + "Reserved 2" + CrLf + "Magnetic Variation,,,E" + CrLf + "Map Projection,Mercator,PolyCal,No,AutoCalOnly,No,BSBUseWPX,No");

                int NumPoint = 0;
                for (int Cpt = 0; Cpt <= 2; Cpt += 2)
                {
                    NumPoint += 1;
                    double Longitude = Math.Abs(Coins[Cpt].Longitude);
                    double Latitude = Math.Abs(Coins[Cpt].Latitude);
                    bool IsPositifLatitude = Coins[Cpt].Latitude >= 0d;
                    bool IsPositifLongitude = Coins[Cpt].Longitude >= 0d;
                    S.AppendLine("Point" + NumPoint.ToString("00") + ",xy," + Coins[Cpt].X_Pixel.ToString("000000") + "," + Coins[Cpt].Y_Pixel.ToString("000000") + ",in, deg," + Math.Floor(Latitude).ToString("00") + "," + DblToStr((Latitude - Math.Floor(Latitude)) * 60d, "00.000000") + "," + (IsPositifLatitude ? "N" : "S") + "," + Math.Floor(Longitude).ToString("000") + "," + DblToStr((Longitude - Math.Floor(Longitude)) * 60d, "00.000000") + "," + (IsPositifLongitude ? "E" : "W") + ",grid,,,,N");
                }

                for (int Cpt = 3; Cpt <= 9; Cpt++)
                    S.AppendLine("Point" + Cpt.ToString("00") + ",xy,,,in,deg,,,N,,,,grid,,,,N");
                for (int Cpt = 10; Cpt <= 30; Cpt++)
                    S.AppendLine("Point" + Cpt.ToString("00") + ",xy,,,in,deg,,,,,,,grid,,,,");

                S.AppendLine("Projection Setup,,,,,,,,,," + CrLf + "Map Feature = MF ; Map Comment = MC     These follow if they exist" + CrLf + "Track File = TF      These follow if they exist" + CrLf + "MM0,Yes" + CrLf + "MMPNUM,4");

                for (int Cpt = 0; Cpt <= 3; Cpt++)
                    S.AppendLine("MMPXY," + (Cpt + 1).ToString("0") + "," + Coins[Cpt].X_Pixel.ToString("000000") + "," + Coins[Cpt].Y_Pixel.ToString("000000"));
                for (int Cpt = 0; Cpt <= 3; Cpt++)
                    S.AppendLine("MMPLL," + (Cpt + 1).ToString("0") + "," + (Coins[Cpt].Longitude >= 0d ? "+" : "") + DblToStr(Coins[Cpt].Longitude, "000.0000000") + (Coins[Cpt].Latitude >= 0d ? ",+" : ",") + DblToStr(Coins[Cpt].Latitude, "00.0000000"));

                S.AppendLine("MM1B," + DblToStr(Echelle_M_PixelX, "0.000000") + CrLf + "LL Grid Setup" + CrLf + "LLGRID,No,No Grid,Yes,255,16711680,0,No Labels,0,16777215,7,1,Yes,x" + CrLf + "Other Grid Setup" + CrLf + "GRGRID,Yes," + SystemCartographique.Niveau.PasGrilleTexte + ",Yes,16711935,255," + SystemCartographique.Niveau.PasGrilleTexte + ",0,16777215,8,1,Yes,No,No,x" + CrLf + "MOP,Map Open Position,0,0");

                S.Append("IWH,Map Image Width/Height," + Coins[2].X_Pixel.ToString("000000") + "," + Coins[2].Y_Pixel.ToString("000000"));

                File.WriteAllText(CheminCarte + @"\" + NomComplet + ".map", S.ToString(), Encoding_FCGP);
                EcrireOziExploreurMercatorRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "A4T8");
            }

            return EcrireOziExploreurMercatorRet;
        }
        /// <summary> Ecrit le fichier de géoréférencement spécifique à OziExploreur pour SM non interpolé en grille Suisse associée au système CH1903 
        /// Voir la description du format sur le net</summary>
        private bool EcrireOziExploreurLV03()
        {
            bool EcrireOziExploreurLV03Ret = false;
            try
            {
                var S = new StringBuilder();   // pour écrire le fichier
                                               // l'entête est  toujours le même
                S.AppendLine("OziExplorer Map Data File Version 2.2" + CrLf + NomComplet + "." + Format.ToString() + CrLf + CheminCarte + @"\" + NomComplet + "." + Format.ToString() + CrLf + "1 ,Map Code," + CrLf + "CH-1903,WGS 84,   0.0000,   0.0000,WGS 84" + CrLf + "Reserved 1" + CrLf + "Reserved 2" + CrLf + "Magnetic Variation,,,E" + CrLf + "Map Projection,(SUI) Swiss Grid,PolyCal,No,AutoCalOnly,No,BSBUseWPX,No");

                int NumPoint = 0;
                for (int Cpt = 0; Cpt <= 2; Cpt += 2)
                {
                    NumPoint += 1;
                    var PGR = Coins[Cpt].Grille;
                    ProjectionSuisse.LV95ToLV03(ref PGR);
                    S.AppendLine("Point" + NumPoint.ToString("00") + ",xy," + Coins[Cpt].X_Pixel.ToString("000000") + "," + Coins[Cpt].Y_Pixel.ToString("000000") + ",in, deg,,,N,,,E,grid,," + (PGR.X >= 0d ? "+" : "") + DblToStr(PGR.X, "00000000.0") + (PGR.Y >= 0d ? ",+" : ",") + DblToStr(PGR.Y, "00000000.0") + ",N");
                }

                for (int Cpt = 3; Cpt <= 30; Cpt++)
                    S.AppendLine("Point" + Cpt.ToString("00") + ",xy,,,in,deg,,,,,,,grid,,,,");

                S.AppendLine("Projection Setup,,,,,,,,,," + CrLf + "Map Feature = MF ; Map Comment = MC     These follow if they exist" + CrLf + "Track File = TF      These follow if they exist" + CrLf + "MM0,Yes" + CrLf + "MMPNUM,4");

                for (int Cpt = 0; Cpt <= 3; Cpt++)
                    S.AppendLine("MMPXY," + (Cpt + 1).ToString("0") + "," + Coins[Cpt].X_Pixel.ToString("000000") + "," + Coins[Cpt].Y_Pixel.ToString("000000"));

                for (int Cpt = 0; Cpt <= 3; Cpt++)
                {
                    var PGR = Coins[Cpt].Grille;
                    ProjectionSuisse.LV95ToLV03(ref PGR);
                    var PDD = ProjectionSuisse.ConvertLV03ToCH1903(PGR, true);
                    S.AppendLine("MMPLL," + (Cpt + 1).ToString("0") + "," + (PDD.Lon >= 0d ? "+" : "") + DblToStr(PDD.Lon, "000.0000000") + (PDD.Lat >= 0d ? ",+" : ",") + DblToStr(PDD.Lat, "00.0000000"));
                }

                S.Append("MM1B," + DblToStr(Echelle_M_PixelX, "0.000000") + CrLf + "LL Grid Setup" + CrLf + "LLGRID,No,No Grid,Yes,255,16711680,0,No Labels,0,16777215,7,1,Yes,x" + CrLf + "Other Grid Setup" + CrLf + "GRGRID,Yes," + SystemCartographique.Niveau.PasGrilleTexte + ",Yes,16711935,255," + SystemCartographique.Niveau.PasGrilleTexte + ",0,16777215,8,1,Yes,No,No,x" + CrLf + "MOP,Map Open Position," + Coins[0].X_Pixel.ToString("000000") + "," + Coins[0].Y_Pixel.ToString("000000") + CrLf + "IWH,Map Image Width/Height," + Coins[2].X_Pixel.ToString("000000") + "," + Coins[2].Y_Pixel.ToString("000000"));

                File.WriteAllText(CheminCarte + @"\" + NomComplet + ".map", S.ToString(), Encoding_FCGP);
                EcrireOziExploreurLV03Ret = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "U3D5");
            }

            return EcrireOziExploreurLV03Ret;
        }
        /// <summary> Ecrit le fichier de géoréférencement spécifique à OziExploreur en système WGS84 (latitude et longitude) 
        /// valable pour le site SM ou GF interpolé ou DT. Voir la description du format sur le net</summary>
        private bool EcrireOziExploreurWGS84()
        {
            bool EcrireOziExploreurWGS84Ret = false;
            try
            {
                var S = new StringBuilder();   // pour écrire le fichier
                                               // l'entête est  toujours le même
                S.AppendLine("OziExplorer Map Data File Version 2.2" + CrLf + NomComplet + "." + Format.ToString() + CrLf + CheminCarte + @"\" + NomComplet + "." + Format.ToString() + CrLf + "1 ,Map Code," + CrLf + "WGS 84,WGS 84,   0.0000,   0.0000,WGS 84" + CrLf + "Reserved 1" + CrLf + "Reserved 2" + CrLf + "Magnetic Variation,,,E" + CrLf + "Map Projection,Latitude/Longitude,PolyCal,No,AutoCalOnly,No,BSBUseWPX,No");

                int NumPoint = 0;
                for (int Cpt = 0; Cpt <= 2; Cpt += 2)
                {
                    NumPoint += 1;
                    double Longitude = Math.Abs(Coins[Cpt].Longitude);
                    double Latitude = Math.Abs(Coins[Cpt].Latitude);
                    bool IsPositifLatitude = Coins[Cpt].Latitude >= 0d;
                    bool IsPositifLongitude = Coins[Cpt].Longitude >= 0d;
                    S.AppendLine("Point" + NumPoint.ToString("00") + ",xy," + Coins[Cpt].X_Pixel.ToString("000000") + "," + Coins[Cpt].Y_Pixel.ToString("000000") + ",in, deg," + Math.Floor(Latitude).ToString("00") + "," + DblToStr((Latitude - Math.Floor(Latitude)) * 60d, "00.000000") + "," + (IsPositifLatitude ? "N" : "S") + "," + Math.Floor(Longitude).ToString("000") + "," + DblToStr((Longitude - Math.Floor(Longitude)) * 60d, "00.000000") + "," + (IsPositifLongitude ? "E" : "W") + ",grid,,,,N");
                }

                for (int Cpt = 3; Cpt <= 9; Cpt++)
                    S.AppendLine("Point" + Cpt.ToString("00") + ",xy,,,in,deg,,,N,,,,grid,,,,N");
                for (int Cpt = 10; Cpt <= 30; Cpt++)
                    S.AppendLine("Point" + Cpt.ToString("00") + ",xy,,,in,deg,,,,,,,grid,,,,");

                S.AppendLine("Projection Setup,,,,,,,,,," + CrLf + "Map Feature = MF ; Map Comment = MC     These follow if they exist" + CrLf + "Track File = TF      These follow if they exist" + CrLf + "MM0,Yes" + CrLf + "MMPNUM,4");

                for (int Cpt = 0; Cpt <= 3; Cpt++)
                    S.AppendLine("MMPXY," + (Cpt + 1).ToString("0") + "," + Coins[Cpt].X_Pixel.ToString("000000") + "," + Coins[Cpt].Y_Pixel.ToString("000000"));
                for (int Cpt = 0; Cpt <= 3; Cpt++)
                    S.AppendLine("MMPLL," + (Cpt + 1).ToString("0") + "," + (Coins[Cpt].Longitude >= 0d ? "+" : "") + DblToStr(Coins[Cpt].Longitude, "000.0000000") + (Coins[Cpt].Latitude >= 0d ? ",+" : ",") + DblToStr(Coins[Cpt].Latitude, "00.0000000"));

                S.AppendLine("MM1B," + DblToStr(Echelle_M_PixelX, "0.000000") + CrLf + "LL Grid Setup" + CrLf + "LLGRID,No,No Grid,Yes,255,16711680,0,No Labels,0,16777215,7,1,Yes,x" + CrLf + "Other Grid Setup" + CrLf + "GRGRID,Yes," + SystemCartographique.Niveau.PasGrilleTexte + ",Yes,16711935,255," + SystemCartographique.Niveau.PasGrilleTexte + ",0,16777215,8,1,Yes,No,No,x" + CrLf + "MOP,Map Open Position,0,0");

                S.Append("IWH,Map Image Width/Height," + Coins[2].X_Pixel.ToString("000000") + "," + Coins[2].Y_Pixel.ToString("000000"));

                File.WriteAllText(CheminCarte + @"\" + NomComplet + ".map", S.ToString(), Encoding_FCGP);
                EcrireOziExploreurWGS84Ret = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "E3Z5");
            }

            return EcrireOziExploreurWGS84Ret;
        }
        /// <summary> Ecrit le fichier de géoréférencement spécifique à QGIS en système WebMercator (EPSG:3857) 
        /// valable pour le site GF non interpolé . Voir la description du format sur le net</summary>
        private bool EcrireQgisMercator()
        {
            bool EcrireQgisMercatorRet = false;
            try
            {
                var S = new StringBuilder();   // pour écrire le fichier
                                               // Dim X0 As Double, Y0 As Double, X2 As Double, Y2 As Double
                var Pt0 = ConvertirCoordonnees.ConvertWGS84ToProjection(Coins[0].LatLon, Datums.Web_Mercator);
                var Pt2 = ConvertirCoordonnees.ConvertWGS84ToProjection(Coins[2].LatLon, Datums.Web_Mercator);
                double DeltaX = (Pt2.X - Pt0.X) / Coins[2].X_Pixel;
                double DeltaY = (Pt2.Y - Pt0.Y) / Coins[2].Y_Pixel;
                string Extension = "." + Format.ToString()[0] + Format.ToString()[^1] + "w";
                S.AppendLine(DblToStr(DeltaX, "0.0000000000000000"));
                S.AppendLine(Coins[0].X_Pixel.ToString("0"));
                S.AppendLine(Coins[0].Y_Pixel.ToString("0"));
                S.AppendLine(DblToStr(DeltaY, "0.00000000000000000"));
                // géo-référencement au centre du pixel alors que pour FCGP le géo-référencement est au coin haut-gauche du pixel
                S.AppendLine(DblToStr(Pt0.X + DeltaX / 2d, "0.0000000000000000"));
                S.AppendLine(DblToStr(Pt0.Y + DeltaY / 2d, "0.0000000000000000"));
                S.Append("EPSG:3857");
                File.WriteAllText(CheminCarte + @"\" + NomComplet + Extension, S.ToString(), Encoding_FCGP);
                EcrireQgisMercatorRet = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "P8W7");
            }

            return EcrireQgisMercatorRet;
        }
        /// <summary> Ecrit le fichier de géoréférencement spécifique à QGIS en système CH1903 / LV03 (EPSG:21781) 
        /// valable pour le site SM non interpolé . Voir la description du format sur le net</summary>
        private bool EcrireQgisLV03()
        {
            bool EcrireQgisLV03Ret = false;
            try
            {
                var S = new StringBuilder();   // pour écrire le fichier
                double DeltaX = (Coins[2].X_Grid - Coins[0].X_Grid) / Coins[2].X_Pixel;
                double DeltaY = (Coins[2].Y_Grid - Coins[0].Y_Grid) / Coins[2].Y_Pixel;
                string Extension = "." + Format.ToString()[0] + Format.ToString()[^1] + "w";
                S.AppendLine(DblToStr(DeltaX, "0.0000000000000000"));
                S.AppendLine(Coins[0].X_Pixel.ToString("0"));
                S.AppendLine(Coins[0].Y_Pixel.ToString("0"));
                S.AppendLine(DblToStr(DeltaY, "0.00000000000000000"));
                // géo-référencement au centre du pixel alors que pour FCGP le géo-référencement est au coin haut-gauche du pixel
                var PGR = new PointD(Coins[0].X_Grid, Coins[0].Y_Grid);
                ProjectionSuisse.LV95ToLV03(ref PGR);
                S.AppendLine(DblToStr(PGR.X + DeltaX / 2d, "0.0000000000000000"));
                S.AppendLine(DblToStr(PGR.Y + DeltaY / 2d, "0.0000000000000000"));
                S.Append("EPSG:21781");

                File.WriteAllText(CheminCarte + @"\" + NomComplet + Extension, S.ToString(), Encoding_FCGP);
                EcrireQgisLV03Ret = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "O7T5");
            }

            return EcrireQgisLV03Ret;
        }
        /// <summary> Ecrit le fichier de géoréférencement spécifique à QGIS en système CH1903 / LV03 (EPSG:21781) 
        /// valable pour le site SM non interpolé . Voir la description du format sur le net</summary>
        private bool EcrireQgisWGS84()
        {
            bool EcrireQgisWGS84Ret = false;
            try
            {
                var S = new StringBuilder();   // pour écrire le fichier
                double DeltaLon = (Coins[2].Longitude - Coins[0].Longitude) / Coins[2].X_Pixel;
                double DeltaLat = (Coins[2].Latitude - Coins[0].Latitude) / Coins[2].Y_Pixel;
                string Extension = "." + Format.ToString()[0] + Format.ToString()[^1] + "w";
                S.AppendLine(DblToStr(DeltaLon, "0.0000000000000000"));
                S.AppendLine(Coins[0].X_Pixel.ToString("0"));
                S.AppendLine(Coins[0].Y_Pixel.ToString("0"));
                S.AppendLine(DblToStr(DeltaLat, "0.00000000000000000"));
                // géo-référencement au centre du pixel alors que pour FCGP le géo-référencement est au coin haut-gauche du pixel
                S.AppendLine(DblToStr(Coins[0].Longitude + DeltaLon / 2d, "0.0000000000000000"));
                S.AppendLine(DblToStr(Coins[0].Latitude + DeltaLat / 2d, "0.0000000000000000"));
                S.Append("EPSG:4326");
                File.WriteAllText(CheminCarte + @"\" + NomComplet + Extension, S.ToString(), Encoding_FCGP);
                EcrireQgisWGS84Ret = true;
            }
            catch (Exception Ex)
            {
                AfficherErreur(Ex, "O4J4");
            }

            return EcrireQgisWGS84Ret;
        }
        #endregion
    }
}