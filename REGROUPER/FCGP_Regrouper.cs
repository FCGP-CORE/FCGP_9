using System.Reflection;

namespace FCGP
{
    /// <summary> une seule sub en remplacement de l'initialisation de l'application par VB.
    /// Correspondance directe avec C# </summary>
    internal static class FCGP_Regrouper
    {
        /// <summary> Point d'entrée principal de l'application. </summary>
        [STAThread]
        internal static void Main()
        {
            string GuidID = "FCGP_REGROUPER_CS_CORE";
            // retrouve le Guid associé à l'assembly pour pouvoir rendre l'instance unique
            var AttributsAssembly = typeof(FCGP_Regrouper).Assembly.CustomAttributes;
            foreach (CustomAttributeData I in AttributsAssembly)
            {
                if (I.AttributeType.Name == "GuidAttribute")
                {
                    GuidID = Convert.ToString(I.ConstructorArguments[0].Value);
                    break;
                }
            }
            Application.SetHighDpiMode(HighDpiMode.DpiUnaware);
            Application.SetDefaultFont(new Font("Segoe UI", 14.0f, FontStyle.Regular, GraphicsUnit.Pixel));
            // émulation de la propriété visualbasic Application.SingleInstance=true
            using (var mutex = new Mutex(false, GuidID))
            {
                if (mutex.WaitOne(0))
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new Regrouper());
                }
            }
        }
    }
}