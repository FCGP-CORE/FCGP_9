# FCGP_Core_9.200

## Principe
Fcgp est un ensemble de 3 programmes destinés aux applications de cartographie, aux GPS Garmin et à l'application ORUX Map sur smartphone Android, 
en produisant des cartes raster géoréférencées et des fichiers tuiles afin d'être autonome (off ligne). Voir le [site FCGP](http://fcgp.e-monsite.com/ "aller sur le site") pour plus de détail
- ***Format des cartes*** 
  - Georef : format texte propriétaire. Contient toutes les informations de géoréférencement
  de la carte.
  - Imp : format pour le logiciel CompeLand (Gps TwoNav).
  - Map : format pour le logiciel OziExploreur.
  - Tab : format pour le logiciel MapInfo.
  - Bpw, Pgw, Jgw : format pour le logiciel QGIS.
- ***Format des fichiers tuiles***
  - KMZ : fichier kml compressé pour Google Earth, BaseCamp et les GPS Garmin supportant les CustomMap.
  - JNX : fichier propriétaire Garmin pour BaseCamp et les GPS Garmin supportant les BirdsEye.
  - ORUX : Répertoire contenant 2 fichiers pour l'application Orux Map sur smartphone Android.

## Particularité du code VB
- ***Implémentation des programmes*** : Le programme démarre à partir d'une procédure Main qui lance le formulaire principal. Voir dans les propriétés du projet  la partie Application. Cela permet d'établir une similitude avec un programme C# et ainsi de pouvoir comparer les 2 langages.
- ***Implémentation des formulaires WindowsForms*** : Les formulaires WindowsForms n'utilisent pas le modificateur de variable `WithEvents` ni la clause `Handles` spécifique à VB mais ajoutent explicitement le constructeur `New`. Les évènements du formulaire et de ces contrôles sont ajoutés dans la `Sub InitialiserEvenements`.
Cette implémentation de formulaire n'empêche pas l'utilisation du concepteur de formulaire qui intervient sur la `Sub InitializeComponent()` et facilite la traduction VB --> C# du formulaire en enlevant beaucoup de code inutile. L'utilisation du concepteur de formulaire détruira tout ou partie de la mise en forme actuelle du code du designer. 
```VB
	'Dans le designer du formulaire
	'Suppression par rapport au désigner VB de la déclaration d'une variable avec le modificateur WithEvents
	'Friend WithEvents Button1 As Button
	'Ajout par rapport au designer VB de la déclaration normale d'une variable au lieu du modificateur WithEvents
	Friend Button1 As Button

	'Ajout du constructeur du formulaire
	Friend Sub New()
	   InitializeComponent()
	   InitialiserEvenements()
	End Sub

	'Ajout d'une procédure qui centralise l'ajout des évènements liés au formulaire
	Private Sub InitialiserEvenements()
	   AddHandler Me.Button1.Click, New EventHandler(AddressOf Button1_Click)
	End Sub

	'Dans le code du formulaire
	'Suppression de la clause Handles. Si on ne supprime pas la clause il y aura 2 passages sur la Sub
	Private Sub Button1_Click(sender As Object, e As EventArgs) 'Handles Button1.Click
	  'action à réaliser
	End Sub
```
- ***Autres particularités***
  - D'une manière générale il n'est pas fait appel aux procédures, fonctions et constantes spécifiques de VB disponibles avec l'assembly `Microsoft.VisualBasic`. On retrouve presque tout dans le framework. Seules les fonctions de conversion telque Cint, Ctype, et DirectCast sont utilisées. 
  - Les classes ou modules principaux contiennent toutes des structures ou des classes privées qui les concernent y compris des formulaires. Ces formulaires peuvent être modifiés avec le concepteur si besoin. 

## Conversion du code VB en C#
- ***Utilitaires de conversion*** : Le plus gros de la traduction a été faite avec l'extension Visual Studio de Code Converter. Quelques passages ont été comparés avec la traduction de Instant C# de Tangible Software (version gratuite) ou avec dnSpy principalement pour voir si il y avait une meilleure traduction. La finalisation du code C# a été faite par la comparaison des 2 codes pour enlever les noms de domaine superflus inclus au niveau du projet ou les noms de classe reportés dans les directives `using static` et par des tests servant à comparer la similitude des fichiers de sortie. 
  - Les fichiers Sqlite (.db) diffèrent car le texte de création des tables est sauvegardé dans les métadonnées de la base et que le texte de création diffère entre les 2 codes. Cela est dû à l'emploi d'une `String multilignes`. Il y a 5 espaces par ligne en défaveur de C#, 4 pour une indentation et un pour le caractère de chaine textuelle `@`.
  - Les fichiers JNX diffèrent car le Guid est aléatoire.
  - Les fichiers KMZ diffèrent car les tags des entrées d'un fichier ZIP sont aléatoires.
- ***Particularités de conversion et différences*** : 
  - Nombres Entiers : Les fonctions de conversion de VB vers des nombres entiers, `Byte, Short, Integer, Long, UShort, UInteger, ULong`, ne tronquent pas la valeur d'origine mais appliquent un arrondi dit 'Financier'. Si l'on veut obtenir exactement les mêmes résultats entre VB et C#, il faut utiliser par exemple pour `Cint(Valeur)` `(int)Math.Round(Valeur)` sauf pour les valeurs qui sont connues et considérées comme des entiers tel que les valeurs retournées par les fonctions `Math.Floor`, `Math.Ceiling` et `Math.Truncate` ou la valeur décimale de certains contrôles de formulaires en fonction de leur configuration.
  - Propriétés : 
    - VB déclare automatiquement une variable privée `_NomPropriété` associée à chaque Propriété implémentée automatiquement ou readOnly. Cela permet d'utiliser la variable plutôt que la propriété à l'intérieur du bloc de la déclaration de la propriété et d'éviter ainsi des appels inutiles aux fonctions sous-jacentes `get` et `set`. C# ne propose pas ce genre de facilité mais on peut le faire manuellement si besoin.
	- Les propriétés avec paramètres ne sont pas acceptées en C#, elles sont traduites en fonction `get_NomPropriété` et `set_NomPropriété`. On peut renommer éventuellement une des deux en `NomPropriété`.
  - Opérateur \ : VB a un opérateur spécifique pour la division entière. Les opérandes sont d'abord arrondies avant de procéder à la division. C# choisit le type du retour de la division en fonction du type des opérandes.
  - Variable static : VB permet de déclarer des variables `static` éventuellement initialisées à l'intérieur d'une Sub ou Function. C# ne le permet pas. Cela oblige à déclarer un champ privé au niveau de la classe. 
  - Fonction : VB déclare automatiquement une variable ayant le même nom que celui de la fonction. Cela permet d'affecter une valeur de retour sans obligation d'utiliser le `Return`. Lors de la traduction en C# une variable sera créée et le `return` sera utilisé pour retourner la valeur.
  - Exit Try : VB n'est pas avare de mots clés pour sortir d'une boucle ou d'une structure. `Exit try` n'existe pas en C# mais peut être remplacé par `return`. Dans le cas d'une fonction `return` sera suivi d'une valeur correspondante au type de retour de la fonction. D'autres traductions sont possibles notamment avec un `label` et le mot clé `goto`. Attention si il y a du code après le bloc `Try Catch Finally`
  - Autres : 
    - Les variables doivent être déclarées avant de servir de paramètre pour une procédure en VB. C# permet de faire la déclaration au niveau de l'appel de la procédure dans le cas d'une valeur `out`
	- VB oblige l'affectation d'une variable lors de la déclaration d'un objet `Dim Info = New TuileAffichage(Col, Row, Me)`, C# permet d'ignorer l'affectation de la création de l'object `_ = new TuileAffichage(Col, Row, this);`
    - L'échange des valeurs de 2 variables nécessite une variable intermédiaire en VB `Dim Tempo As Integer = Futur : Futur = Encours : Encours = Tempo`. En C# l'utilisation des tuples permet de le faire plus naturellement `(Encours, Futur) = (Futur, Encours);`
    - VB n'a pas d'opérateur de plage alors que celui-ci est systématiquement employé dans le code C# lorsque cela est possible.
	- La forme simplifiée du `using` C# n'est pas utilisée car elle n'existe pas en VB. Son emploi ne facile pas la comparaison des 2 codes étant donné qu'on ne sait pas où fini le bloc `using`.
  - Remarques : 
    - Les 2 codes sont très similaires à la lectutre. Cela est du au fait que le code VB par du postulat qu'il faut utiliser le moins possible les particularités spécifiques de VB. 
	- Le framework .Net intègre des nouveaux types qui sont inacessibles à VB, cela sous entend qu'un code spécifique à C# pourrait améliorer la réactivité des programmes FCGP. 

## Type de projet 
- Il faut une version de Visual Studio qui supporte le `.Net 6` et la dernière version du langage VB et C#. Les solutions sont des versions de Visual Studio 2017 mais vous pouvez utiliser Visual Studio 2022 Community. Attention les versions 2022 17.1 à 2022 17.4 ne peuvent plus afficher les propriétés de certains projets VB. La version 2022 17.5 n'a plus ce problème.
- Le projet PARTAGER est commun aux 3 programmes FCGP. Vous pouvez soit l'intégrer comme référence dans les autres projets ou laisser les liens sur les différents fichiers qui le composent. Cela facilite les modifications du code. 
- Il est fait appel à la DLL de Sqlite sous forme de package Nugets pour la base de données des Settings ou les fichiers Orux. Voir le [site SQLite.org](https://www.sqlite.org/index.html)
- Il est fait appel à la DLL de ScottPlot sous forme de package Nugets et au code pour le contôle formulaire qui permet l'affichage des graphiques liés aux traces. Voir le [site ScottPlot.Net ](https://scottplot.net/)
- Le module FormatJNX est basé sur la description du format JNX et le code FreePascal de Whiter Brinkster. Voir le [blog JNX Raster Maps](http://whiter.brinkster.net/en/JNX.shtml). Télécharger le code de la [Library Pascal](http://whiter.brinkster.net/JNXLib.rar)
- Les encodeurs d'image Jpeg et Png du .Net sont limités à des dimensions de 65500 pixels environ. On peut trouver des codes pour encodeur et les modifier pour qu'ils acceptent des dimensions plus importantes. Il n'est pas certain que les programmes de lecture d'images Open Source soient capables de lire les grandes images de ce type et pour les programmes du commerce je ne sais pas. Cependant cela permettrai d'extraire directement les tuiles Jpeg sans passer par le découpage de l'image au format Raw.
- Code sous license GNU GENERAL PUBLIC LICENSE Version 3.
