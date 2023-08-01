Imports System.Reflection
''' <summary> une seule sub en remplacement de l'initialisation de l'application par VB.
''' Correspondance directe avec C# </summary>
Friend Module FCGP_Regrouper
    ''' <summary> Point d'entrée principal de l'application. </summary>
    <STAThread>
    Friend Sub Main()
        Dim GuidID = "FCGP_REGROUPER_VB_CORE"
        'retrouve le Guid associé à l'assembly pour pouvoir rendre l'instance unique
        Dim AttributsAssembly = GetType(FCGP_Regrouper).Assembly.CustomAttributes
        For Each I As CustomAttributeData In AttributsAssembly
            If I.AttributeType.Name = "GuidAttribute" Then
                GuidID = CStr(I.ConstructorArguments(0).Value)
                Exit For
            End If
        Next
        Application.SetHighDpiMode(HighDpiMode.DpiUnaware)
        Application.SetDefaultFont(New Font("Segoe UI", 14.0!, FontStyle.Regular, GraphicsUnit.Pixel))
        'émulation de la propriété visualbasic Application.SingleInstance=true
        Using mutex As New Mutex(False, GuidID)
            If mutex.WaitOne(0) Then
                Application.EnableVisualStyles()
                Application.SetCompatibleTextRenderingDefault(False)
                Application.Run(New Regrouper)
            End If
        End Using
    End Sub
End Module
