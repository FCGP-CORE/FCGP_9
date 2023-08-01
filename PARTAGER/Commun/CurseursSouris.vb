''' <summary> définit l'ensemble des curseurs de souris dont ont besoin FCGP_Capture et FCGP_Visue </summary>
Friend Module CurseurSouris
    Private s_c_TraceDeplacerSouris As Cursor
    Private s_c_TraceDefault As Cursor
    Private s_c_TraceEditerTRK As Cursor
    Private s_c_TraceInsererPt As Cursor
    Private s_c_TraceDeplacerPt As Cursor
    Private s_c_TraceSupprimerTRK As Cursor
    Private s_c_TraceSupprimerPtSeg As Cursor
    Private s_c_TraceCoupeTRK As Cursor
    Private s_c_TraceCoupePtSeg As Cursor
    Private s_c_CarteDeplacerSouris As Cursor
    Private s_c_CarteDefault As Cursor
    Private s_c_CarteDeplacerHorizontal As Cursor
    Private s_c_CarteDeplacerVertical As Cursor
    Private s_c_CarteDeplacerVHS As Cursor
    Private s_c_CarteDeplacerVSH As Cursor
    Private s_c_FRefAgrandirHorizontal As Cursor
    Private s_c_FRefAgrandirVertical As Cursor
    Private s_c_FRefAgrandir_HD_BG As Cursor
    Private s_c_FRefAgrandir_HG_BD As Cursor
    Private s_c_FRefDeplacerVertical As Cursor
    Private s_c_FRefDeplacerHorizontal As Cursor
    Private s_c_FRefDeplacer As Cursor
    Private s_c_CurseurDefault As Cursor
    Private s_c_CurseurAttente As Cursor
    Private s_c_CurseurArrow As Cursor
    Private s_c_CurseurZoom As Cursor
    Private _CurseurEncours As Curseurs
    Private ListeCurseurs() As Cursor
    Private ZoneAffichage As PictureBox
    Friend Sub InitialiserCurseurs(Visue As PictureBox, Curseur As Cursor)
        ZoneAffichage = Visue
        Using ms As New MemoryStream(ModeTraceDefaut)
            s_c_TraceDefault = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(ModeTraceDeplacement)
            s_c_TraceDeplacerSouris = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(ModeTraceEditer)
            s_c_TraceEditerTRK = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(ModeTraceInsererPtSeg)
            s_c_TraceInsererPt = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(ModeTraceDeplacerPt)
            s_c_TraceDeplacerPt = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(ModeTraceSupprimer)
            s_c_TraceSupprimerTRK = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(ModeTraceSupprimerPtSeg)
            s_c_TraceSupprimerPtSeg = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(ModeTraceCouper)
            s_c_TraceCoupeTRK = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(ModeTraceCouperPtSeg)
            s_c_TraceCoupePtSeg = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(ModeCarteDefaut)
            s_c_CarteDefault = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(ModeCarteDeplacement)
            s_c_CarteDeplacerSouris = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(ModeCarteDeplacementHorizontal)
            s_c_CarteDeplacerVertical = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(ModeCarteDeplacementVertical)
            s_c_CarteDeplacerHorizontal = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(ModeCarteDeplacementVHS)
            s_c_CarteDeplacerVHS = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(ModeCarteDeplacementVSH)
            s_c_CarteDeplacerVSH = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(FRefAgrandir_Horizontal)
            s_c_FRefAgrandirHorizontal = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(FRefAgrandir_Vertical)
            s_c_FRefAgrandirVertical = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(FRefAgrandir_HD_BG)
            s_c_FRefAgrandir_HD_BG = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(FRefAgrandir_HG_BD)
            s_c_FRefAgrandir_HG_BD = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(FRefDeplacer_Vertical)
            s_c_FRefDeplacerVertical = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(FRefDeplacer_Horizontal)
            s_c_FRefDeplacerHorizontal = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(FRefDeplacer)
            s_c_FRefDeplacer = New Cursor(ms)
        End Using
        Using ms As New MemoryStream(Zoom)
            s_c_CurseurZoom = New Cursor(ms)
        End Using
        s_c_CurseurArrow = Cursors.Arrow
        s_c_CurseurAttente = Cursors.WaitCursor
        s_c_CurseurDefault = Curseur
        ReDim ListeCurseurs(25)
        ListeCurseurs(Curseurs.Defaut) = s_c_CurseurDefault
        ListeCurseurs(Curseurs.CarteDefaut) = s_c_CarteDefault
        ListeCurseurs(Curseurs.CarteDeplacement) = s_c_CarteDeplacerSouris
        ListeCurseurs(Curseurs.CarteDeplacementHorizontal) = s_c_CarteDeplacerHorizontal
        ListeCurseurs(Curseurs.CarteDeplacementVertical) = s_c_CarteDeplacerVertical
        ListeCurseurs(Curseurs.CarteDeplacementVHS) = s_c_CarteDeplacerVHS
        ListeCurseurs(Curseurs.CarteDeplacementVSH) = s_c_CarteDeplacerVSH
        ListeCurseurs(Curseurs.TraceDefaut) = s_c_TraceDefault
        ListeCurseurs(Curseurs.TraceDeplacement) = s_c_TraceDeplacerSouris
        ListeCurseurs(Curseurs.TraceEdite) = s_c_TraceEditerTRK
        ListeCurseurs(Curseurs.TraceInsertPt) = s_c_TraceInsererPt
        ListeCurseurs(Curseurs.TraceDeplacePt) = s_c_TraceDeplacerPt
        ListeCurseurs(Curseurs.TraceSupprime) = s_c_TraceSupprimerTRK
        ListeCurseurs(Curseurs.TraceSupprimePtSeg) = s_c_TraceSupprimerPtSeg
        ListeCurseurs(Curseurs.TraceCoupe) = s_c_TraceCoupeTRK
        ListeCurseurs(Curseurs.TraceCoupePtSeg) = s_c_TraceCoupePtSeg
        ListeCurseurs(Curseurs.FRefAgrandirHorizontal) = s_c_FRefAgrandirHorizontal
        ListeCurseurs(Curseurs.FRefAgrandirVertical) = s_c_FRefAgrandirVertical
        ListeCurseurs(Curseurs.FRefAgrandir_HD_BG) = s_c_FRefAgrandir_HD_BG
        ListeCurseurs(Curseurs.FRefAgrandir_HG_BD) = s_c_FRefAgrandir_HG_BD
        ListeCurseurs(Curseurs.FRefDeplacerVertical) = s_c_FRefDeplacerVertical
        ListeCurseurs(Curseurs.FRefDeplacerHorizontal) = s_c_FRefDeplacerHorizontal
        ListeCurseurs(Curseurs.FRefDeplacer) = s_c_FRefDeplacer
        ListeCurseurs(Curseurs.Attendre) = s_c_CurseurAttente
        ListeCurseurs(Curseurs.Arrow) = s_c_CurseurArrow
        ListeCurseurs(Curseurs.Zoom) = s_c_CurseurZoom
    End Sub
    Friend WriteOnly Property CurseurEncours() As Curseurs
        Set(value As Curseurs)
            _CurseurEncours = value
            ZoneAffichage.Cursor = ListeCurseurs(_CurseurEncours)
        End Set
    End Property

    Friend ReadOnly Property Curseur(Curs As Curseurs) As Cursor
        Get
            Return ListeCurseurs(Curs)
        End Get
    End Property
End Module