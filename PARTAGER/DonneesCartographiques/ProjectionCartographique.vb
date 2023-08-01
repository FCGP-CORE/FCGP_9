''' <summary> définit une projection d'un système cartographique </summary>
Friend Class ProjectionCartographique
#Region "Données constantes"
    ''' <summary> Retourne le libellé du datum prinpical associé à un site carto </summary>
    ''' <param name="SiteCarto"> site cartographique servant de base à une carte ou un regroupement </param>
    Friend Shared ReadOnly Property DatumPrincipalLibelle(Sitecarto As SitesCartographiques) As String
        Get
            Return DatumsLibelles(_Datums(Sitecarto)(0))
        End Get
    End Property
    ''' <summary> Retourne le datum princal associé à un site carto </summary>
    ''' <param name="SiteCarto"> site cartographique servant de base à une carte ou un regroupement </param>
    Friend Shared ReadOnly Property DatumPrincipal(Sitecarto As SitesCartographiques) As Datums
        Get
            Return _Datums(Sitecarto)(0)
        End Get
    End Property
#End Region
#Region "Données constantes"
    ''' <summary>le système suport peut avoir plusieurs type de coordonnées. info présente dans le fichier GeoRef. 
    '''Actuellement seulement 2 types de coordonnées différents par système. Tableau escaliers IndiceSytème et Indiceprojection </summary>
    '''<remarks> l'indice 0 des datums correspond au datum principal du site ou du regroupement</remarks>
    Private Shared ReadOnly _Datums As Datums()() = {
            New Datums() {Datums.WGS84, Datums.Lambert_93, Datums.Web_Mercator},'GF
            New Datums() {Datums.Grille_Suisse_LV03, Datums.Grille_Suisse},     'SM
            New Datums() {Datums.WGS84, Datums.UTM_WGS84, Datums.Web_Mercator}, 'DT
            New Datums() {Datums.WGS84, Datums.Web_Mercator},                   'CY
            New Datums() {Datums.WGS84, Datums.Web_Mercator},                   'OT
            New Datums() {Datums.WGS84, Datums.Web_Mercator}}                   'ES
#End Region
#Region "Functions et propriétés visibles"
    ''' <summary> renvoie la projection associé au sitecarto </summary>
    ''' <param name="SiteCarto">site carto associé à la projection</param>
    ''' <param name="LibelleProjection">libellé de la projection (datum) </param>
    Friend Sub New(SiteCarto As SitesCartographiques, LibelleProjection As String)
        _Datum = CType(Array.IndexOf(DatumsLibelles, LibelleProjection), Datums)
        Initialise(SiteCarto)
    End Sub
    ''' <summary> renvoie la projection associé au sitecarto </summary>
    ''' <param name="SiteCarto">site carto associé à la projection</param>
    ''' <param name="DatumProjection"> Datum de la projection </param>
    Friend Sub New(SiteCarto As SitesCartographiques, DatumProjection As Datums)
        _Datum = DatumProjection
        Initialise(SiteCarto)
    End Sub
    ''' <summary>donne l'unité de coordonnées associée à un type de projection</summary>
    Friend ReadOnly Property UniteCoordonnees As UnitesCoordonnees
        Get
            If _Datum = Datums.Aucun Then
                Return UnitesCoordonnees.Aucun
            End If
            If _Datum < Datums.RGF93 Then
                Return UnitesCoordonnees.Mètre
            Else
                Return UnitesCoordonnees.DMS
            End If
        End Get
    End Property
    ''' <summary>chaque type de projection est associé à un datum ou ellipsoïde</summary>
    Friend ReadOnly Property Datum As Datums
    ''' <summary>donne le libellé associé à un type de projection</summary>
    Friend ReadOnly Property Libelle As String
        Get
            If _Datum = Datums.Aucun Then
                Return "Aucun"
            End If
            Return DatumsLibelles(_Datum)
        End Get
    End Property
    ''' <summary>true si la création de la projection c'est bien passée</summary>
    Friend ReadOnly Property IsOk As Boolean
    Private Sub Initialise(SiteCarto As SitesCartographiques)
        If Array.IndexOf(_Datums(SiteCarto), _Datum) > -1 Then
            _IsOk = True
        Else
            _Datum = Datums.Aucun
        End If
    End Sub
#End Region
End Class
