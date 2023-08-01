using static FCGP.Enumerations;
using static FCGP.Proprietes.Resources;

namespace FCGP
{
    /// <summary> définit l'ensemble des curseurs de souris dont ont besoin FCGP_Capture et FCGP_Visue </summary>
    internal static class CurseurSouris
    {
        private static Cursor s_c_TraceDeplacerSouris;
        private static Cursor s_c_TraceDefault;
        private static Cursor s_c_TraceEditerTRK;
        private static Cursor s_c_TraceInsererPt;
        private static Cursor s_c_TraceDeplacerPt;
        private static Cursor s_c_TraceSupprimerTRK;
        private static Cursor s_c_TraceSupprimerPtSeg;
        private static Cursor s_c_TraceCoupeTRK;
        private static Cursor s_c_TraceCoupePtSeg;
        private static Cursor s_c_CarteDeplacerSouris;
        private static Cursor s_c_CarteDefault;
        private static Cursor s_c_CarteDeplacerHorizontal;
        private static Cursor s_c_CarteDeplacerVertical;
        private static Cursor s_c_CarteDeplacerVHS;
        private static Cursor s_c_CarteDeplacerVSH;
        private static Cursor s_c_FRefAgrandirHorizontal;
        private static Cursor s_c_FRefAgrandirVertical;
        private static Cursor s_c_FRefAgrandir_HD_BG;
        private static Cursor s_c_FRefAgrandir_HG_BD;
        private static Cursor s_c_FRefDeplacerVertical;
        private static Cursor s_c_FRefDeplacerHorizontal;
        private static Cursor s_c_FRefDeplacer;
        private static Cursor s_c_CurseurDefault;
        private static Cursor s_c_CurseurAttente;
        private static Cursor s_c_CurseurArrow;
        private static Cursor s_c_CurseurZoom;
        private static Enumerations.Curseurs _CurseurEncours;
        private static Cursor[] ListeCurseurs;
        private static PictureBox ZoneAffichage;
        internal static void InitialiserCurseurs(PictureBox Visue, Cursor Curseur)
        {
            ZoneAffichage = Visue;
            using (var ms = new MemoryStream(ModeTraceDefaut))
            {
                s_c_TraceDefault = new Cursor(ms);
            }
            using (var ms = new MemoryStream(ModeTraceDeplacement))
            {
                s_c_TraceDeplacerSouris = new Cursor(ms);
            }
            using (var ms = new MemoryStream(ModeTraceEditer))
            {
                s_c_TraceEditerTRK = new Cursor(ms);
            }
            using (var ms = new MemoryStream(ModeTraceInsererPtSeg))
            {
                s_c_TraceInsererPt = new Cursor(ms);
            }
            using (var ms = new MemoryStream(ModeTraceDeplacerPt))
            {
                s_c_TraceDeplacerPt = new Cursor(ms);
            }
            using (var ms = new MemoryStream(ModeTraceSupprimer))
            {
                s_c_TraceSupprimerTRK = new Cursor(ms);
            }
            using (var ms = new MemoryStream(ModeTraceSupprimerPtSeg))
            {
                s_c_TraceSupprimerPtSeg = new Cursor(ms);
            }
            using (var ms = new MemoryStream(ModeTraceCouper))
            {
                s_c_TraceCoupeTRK = new Cursor(ms);
            }
            using (var ms = new MemoryStream(ModeTraceCouperPtSeg))
            {
                s_c_TraceCoupePtSeg = new Cursor(ms);
            }
            using (var ms = new MemoryStream(ModeCarteDefaut))
            {
                s_c_CarteDefault = new Cursor(ms);
            }
            using (var ms = new MemoryStream(ModeCarteDeplacement))
            {
                s_c_CarteDeplacerSouris = new Cursor(ms);
            }
            using (var ms = new MemoryStream(ModeCarteDeplacementHorizontal))
            {
                s_c_CarteDeplacerVertical = new Cursor(ms);
            }
            using (var ms = new MemoryStream(ModeCarteDeplacementVertical))
            {
                s_c_CarteDeplacerHorizontal = new Cursor(ms);
            }
            using (var ms = new MemoryStream(ModeCarteDeplacementVHS))
            {
                s_c_CarteDeplacerVHS = new Cursor(ms);
            }
            using (var ms = new MemoryStream(ModeCarteDeplacementVSH))
            {
                s_c_CarteDeplacerVSH = new Cursor(ms);
            }
            using (var ms = new MemoryStream(FRefAgrandir_Horizontal))
            {
                s_c_FRefAgrandirHorizontal = new Cursor(ms);
            }
            using (var ms = new MemoryStream(FRefAgrandir_Vertical))
            {
                s_c_FRefAgrandirVertical = new Cursor(ms);
            }
            using (var ms = new MemoryStream(FRefAgrandir_HD_BG))
            {
                s_c_FRefAgrandir_HD_BG = new Cursor(ms);
            }
            using (var ms = new MemoryStream(FRefAgrandir_HG_BD))
            {
                s_c_FRefAgrandir_HG_BD = new Cursor(ms);
            }
            using (var ms = new MemoryStream(FRefDeplacer_Vertical))
            {
                s_c_FRefDeplacerVertical = new Cursor(ms);
            }
            using (var ms = new MemoryStream(FRefDeplacer_Horizontal))
            {
                s_c_FRefDeplacerHorizontal = new Cursor(ms);
            }
            using (var ms = new MemoryStream(FRefDeplacer))
            {
                s_c_FRefDeplacer = new Cursor(ms);
            }
            using (var ms = new MemoryStream(Zoom))
            {
                s_c_CurseurZoom = new Cursor(ms);
            }
            s_c_CurseurArrow = Cursors.Arrow;
            s_c_CurseurAttente = Cursors.WaitCursor;
            s_c_CurseurDefault = Curseur;
            ListeCurseurs = new Cursor[26];
            ListeCurseurs[(int)Curseurs.Defaut] = s_c_CurseurDefault;
            ListeCurseurs[(int)Curseurs.CarteDefaut] = s_c_CarteDefault;
            ListeCurseurs[(int)Curseurs.CarteDeplacement] = s_c_CarteDeplacerSouris;
            ListeCurseurs[(int)Curseurs.CarteDeplacementHorizontal] = s_c_CarteDeplacerHorizontal;
            ListeCurseurs[(int)Curseurs.CarteDeplacementVertical] = s_c_CarteDeplacerVertical;
            ListeCurseurs[(int)Curseurs.CarteDeplacementVHS] = s_c_CarteDeplacerVHS;
            ListeCurseurs[(int)Curseurs.CarteDeplacementVSH] = s_c_CarteDeplacerVSH;
            ListeCurseurs[(int)Curseurs.TraceDefaut] = s_c_TraceDefault;
            ListeCurseurs[(int)Curseurs.TraceDeplacement] = s_c_TraceDeplacerSouris;
            ListeCurseurs[(int)Curseurs.TraceEdite] = s_c_TraceEditerTRK;
            ListeCurseurs[(int)Curseurs.TraceInsertPt] = s_c_TraceInsererPt;
            ListeCurseurs[(int)Curseurs.TraceDeplacePt] = s_c_TraceDeplacerPt;
            ListeCurseurs[(int)Curseurs.TraceSupprime] = s_c_TraceSupprimerTRK;
            ListeCurseurs[(int)Curseurs.TraceSupprimePtSeg] = s_c_TraceSupprimerPtSeg;
            ListeCurseurs[(int)Curseurs.TraceCoupe] = s_c_TraceCoupeTRK;
            ListeCurseurs[(int)Curseurs.TraceCoupePtSeg] = s_c_TraceCoupePtSeg;
            ListeCurseurs[(int)Curseurs.FRefAgrandirHorizontal] = s_c_FRefAgrandirHorizontal;
            ListeCurseurs[(int)Curseurs.FRefAgrandirVertical] = s_c_FRefAgrandirVertical;
            ListeCurseurs[(int)Curseurs.FRefAgrandir_HD_BG] = s_c_FRefAgrandir_HD_BG;
            ListeCurseurs[(int)Curseurs.FRefAgrandir_HG_BD] = s_c_FRefAgrandir_HG_BD;
            ListeCurseurs[(int)Curseurs.FRefDeplacerVertical] = s_c_FRefDeplacerVertical;
            ListeCurseurs[(int)Curseurs.FRefDeplacerHorizontal] = s_c_FRefDeplacerHorizontal;
            ListeCurseurs[(int)Curseurs.FRefDeplacer] = s_c_FRefDeplacer;
            ListeCurseurs[(int)Curseurs.Attendre] = s_c_CurseurAttente;
            ListeCurseurs[(int)Curseurs.Arrow] = s_c_CurseurArrow;
            ListeCurseurs[(int)Curseurs.Zoom] = s_c_CurseurZoom;
        }
        internal static Curseurs CurseurEncours
        {
            set
            {
                _CurseurEncours = value;
                ZoneAffichage.Cursor = ListeCurseurs[(int)_CurseurEncours];
            }
        }

        internal static Cursor Curseur(Curseurs Curs)
        {
            return ListeCurseurs[(int)Curs];
        }
    }
}