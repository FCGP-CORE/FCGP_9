Namespace Coordonnees
    ''' <summary> Définit les champs nécessaires à un système géodésique à 3 paramètres (sans la rotation autour des axes) </summary>
    Module PARAM_DATUMS
#Region "Données"
        Friend ReadOnly ParametresDatums As (Projection As Projections, EPSG As String, LibellesEPSG As String, Ellipsoide As Ellipsoides)() =
            New(Projections, String, String, Ellipsoides)() {
                (Projections.Aucun, "", "", Ellipsoides.Aucun),
                (Projections.UTM, "230ZZ", "ED50 / UTM zone ZZN", Ellipsoides.HAY09),                           'UTM ED50
                (Projections.Lambert, "27561", "NTF (Paris) / Nord", Ellipsoides.C_1880_M),                     'LAMBERT I
                (Projections.Lambert, "27562", "NTF (Paris) / Centre", Ellipsoides.C_1880_M),                   'LAMBERT II
                (Projections.Lambert, "27572", "NTF (Paris) / zone II étendue", Ellipsoides.C_1880_M),          'LAMBERT II Etendu
                (Projections.Lambert, "27563", "NTF (Paris) / Sud", Ellipsoides.C_1880_M),                      'LAMBERT III
                (Projections.Lambert, "27564", "NTF (Paris) / Corse", Ellipsoides.C_1880_M),                    'LAMBERT IV
                (Projections.Lambert, "2154", "RGF93 / Lambert-93 -- France", Ellipsoides.GRS80),               'LAMBERT 93
                (Projections.Suisse, "21781", "Swiss CH1903 / LV03", Ellipsoides.BESSEL_1841),                  'SUISSE
                (Projections.Suisse, "21781", "Swiss CH1903 / LV03", Ellipsoides.BESSEL_1841),                  'SUISSE LV03
                (Projections.Suisse, "2056", "Swiss CH1903+ / LV95", Ellipsoides.BESSEL_1841),                  'SUISSE LV95
                (Projections.UTM, "326ZZ", "WGS 84 / UTM zone ZZN", Ellipsoides.WGS84),                         'UTM WGS84 
                (Projections.LatLon, "32662", "WGS 84 / Plate Carree", Ellipsoides.WGS84),                      'LATITUDE, LONGITUDE
                (Projections.WMercator, "3857", "WGS 84 / Pseudo-Mercator -- Spherical", Ellipsoides.WGS84),    'WEB MERCATOR 
                (Projections.Datum, "4171", "Reseau Geodesique Francais 1993", Ellipsoides.GRS80),              'RGF93
                (Projections.Datum, "4326", "World Geodetic System 1984", Ellipsoides.WGS84),                   'WGS84
                (Projections.Datum, "4230", "European Datum 1950", Ellipsoides.HAY09),                          'ED50
                (Projections.Datum, "4275", "Nouvelle Triangulation Francaise", Ellipsoides.C_1880_M),          'NTF
                (Projections.Datum, "4149", "CH 1903", Ellipsoides.BESSEL_1841)}                                'CH1903
        Friend ParametresEllipsoides As (DX As Double, DY As Double, DZ As Double, A As Double, F As Double, EPSG As String)() =
            New(Double, Double, Double, Double, Double, String)() {
                (0.0R, 0.0R, 0.0R, 0.0R, 0.0R, ""),                                         'Aucun
                (-168.0R, -60.0R, 320.0R, 6378249.2R, 1.0R / 293.466021293625R, "7011"),    'C_1880_M
                (0.0R, 0.0R, 0.0R, RayonWGS84, 1.0R / 298.257222101R, "7019"),              'GRS80
                (-89.5R, -93.8R, -123.1R, 6378388.0R, 1.0R / 297.0R, "7022"),               'HAY09
                (0.0R, 0.0R, 0.0R, RayonWGS84, F_WGS84, "7030"),                            'WGS84
                (674.374R, 15.056R, 405.346R, 6377397.155R, 1.0R / 299.15281285R, "7004")}  'BESSEL_1841
#End Region
#Region "propriétés"
        Friend ReadOnly Property Datum As Datums
        Friend ReadOnly Property Ellipsoide As Ellipsoides
        Friend ReadOnly Property LibelleDatums As String
            Get
                Return ParametresDatums(_Datum).LibellesEPSG
            End Get
        End Property
        Friend ReadOnly Property EPSGDatum() As String
            Get
                Return ParametresDatums(_Datum).EPSG
            End Get
        End Property
        Friend ReadOnly Property EPSGEllipsoide As String
            Get
                Return ParametresEllipsoides(_Ellipsoide).EPSG
            End Get
        End Property
        Friend ReadOnly Property LibelleEllipsoide() As String
            Get
                Return LibellesEllipsoides(_Ellipsoide)
            End Get
        End Property
        Friend ReadOnly Property Projection As Projections
            Get
                Return ParametresDatums(_Datum).Projection
            End Get
        End Property
        Friend ReadOnly Property DX As Double
        Friend ReadOnly Property DY As Double
        Friend ReadOnly Property DZ As Double
        Friend ReadOnly Property A As Double
        Friend ReadOnly Property F As Double
        Friend ReadOnly Property E2 As Double
        Friend ReadOnly Property EP2 As Double
        Friend ReadOnly Property B As Double
#End Region
        'met à jour les paramètres de conversion des coordonnées cartésiennes du datum vers
        'les coordonnées cartésiennes de WGS84 qui est le système pivot
        Friend Sub SetParametresDatum(Datum As Datums)
            _Datum = Datum
            _Ellipsoide = ParametresDatums(_Datum).Ellipsoide
            _DX = ParametresEllipsoides(_Ellipsoide).DX
            _DY = ParametresEllipsoides(_Ellipsoide).DY
            _DZ = ParametresEllipsoides(_Ellipsoide).DZ
            _A = ParametresEllipsoides(_Ellipsoide).A
            _F = ParametresEllipsoides(_Ellipsoide).F
            _E2 = 2.0R * _F - _F * _F
            _EP2 = (1.0R / (1.0R - _E2)) - 1.0R
            _B = _A * (1.0R - _F)
        End Sub
    End Module
End Namespace