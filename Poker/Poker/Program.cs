/*
 * Created by SharpDevelop.
 * User: d.barboutie
 * Date: 20/11/2023
 * Time: 17:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace poker
{
	class Program
	{
			//
			// DECLARATION DES DONNEES
			//
			// Importation des DLL (librairies de code) permettant de gérer les couleurs en mode console
			[DllImport("kernel32.dll")]
			public static extern bool SetConsoleTextAttribute(IntPtr hConsoleOutput, int wAttributes);
			[DllImport("kernel32.dll")]
			public static extern IntPtr GetStdHandle(uint nStdHandle);
			
			static uint STD_OUTPUT_HANDLE = 0xfffffff5;
			static IntPtr hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
			
			// Pour utiliser la fonction C 'getchar()' : sasie d'un caractère 
			[DllImport("msvcrt")]
			static extern int _getch();
			
			// *** TYPES DE DONNEES
			
			// Codes COULEUR
			public enum couleur {VERT=10,ROUGE=12,JAUNE=14,BLANC=15,NOIRE=0,ROUGESURBLANC=252,NOIRESURBLANC= 240};
			
			public struct coordonnees
				{
					public int x; 
					public int y;
				}
			
			public struct carte
				{
					public char valeur; 
					public int famille;
				}
			
			public enum combinaison {RIEN,PAIRE,DOUBLE_PAIRE,BRELAN,QUINTE,FULL,COULEUR,CARRE,QUINTE_FLUSH};
			
			
			
			// *** VARIABLES
			// Coordonnées de départ pour l'affichage
			public static coordonnees depart;
			
			// Fin du jeu
			public static bool fin = false;
			
			// Valeurs des cartes : As, Roi,...
			public static char [] valeurs = {'A','R','D','V','X','9','8','7'};
			
			// Codes ASCII (3 : coeur, 4 : carreau, 5 : trèfle, 6 : pique) 
			public static int [] familles = {3,4,5,6};
			
			// Numéros des cartes à échanger
			public static int [] echange = {0,0,0,0};
			
			// Jeu de 5 cartes
			public static carte[] MonJeu = new carte[5];
			
			// Test de la combinaison
			switch (chercheCombinaison(unJeu))
			{
			case combinaison.RIEN :
				afficher_message("rien du tout... desole!", couleur.ROUGE, new coordonnees { x = 24, y = 15 });
				break;
			case combinaison.PAIRE:
				...}
			
		// Génère aléatoirement une carte : {valeur;famille}
		// Retourne une expression de type "carte"
		private static carte tirage(){
		
		}
			
		// Indique si une carte est déjà présente dans le jeu
		// Paramètres : une carte, le jeu 5 cartes, le numéro de la carte dans le jeu
		// Retourne un booléen
		
		// Affiche à l'écran une carte {valeur;famille}
		private static bool carteUnique(carte uneCarte, carte[] unJeu, int numero)
		{
			foreach(carte element in unJeu){
				if (element == uneCarte) {
					return true;
				}
			}
			return false;
		}
		

		private static void affichageCarte(carte uneCarte)
		// Tirage d'un jeu de 5 cartes
		// Paramètre : le tableau de 5 cartes à remplir private static void tirageDuJeu(carte[] unJeu)
		// Echange des cartes
		// Paramètres : le tableau de 5 cartes et le tableau des numéros des cartes à échanger private static void echangeDeCartes(carte[] unJeu, int[] e)
		// Calcule et retourne la combinaison (paire, double-paire…) pour un jeu complet de 5 cartes
		// La valeur retournée est un élement de l'énumération 'combinaison' private static combinaison chercheCombinaison(carte[] unJeu)
		// Calcul et affichage du résultat
		// Paramètre : le tableau de 5 cartes
		
		// Choix de la couleur
		// * couleur.ROUGE pour coeur et carreau
		// * couleur.NOIRE pour trèfle et pique couleur lacouleur;
		if (uneCarte.famille == 3 || uneCarte.famille == 4) lacouleur = couleur.ROUGESURBLANC;
		Affichage de la carte depuis le haut :
		else
		lacouleur = couleur.NOIRESURBLANC;
		// Positionnement et affichage
		// REMARQUE : dans la fenêtre exécution, choisir "Police Roster" pour afficher les symboles... afficher_message
		(
		string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n",'*','-','-','-','-','-','-','-','-','-','*'),
		lacouleur,
		new coordonnees {x= colonne * 15, y=1 }
		);
		afficher_message (
		string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n",'|',
		(char)uneCarte.famille,' ', (char)uneCarte.famille,' ', (char)uneCarte.famille,' ',
		(char)uneCarte.famille,' ',
		(char)uneCarte.famille, '|'), lacouleur, new coordonnees { x = colonne * 15, y = 2 }
		);
			
		// Tirage d'un jeu de 5 cartes
		// Paramètre : le tableau de 5 cartes à remplir
		private static void tirageDuJeu(carte[] unJeu)
		// Echange des cartes
		// Paramètres : le tableau de 5 cartes et le tableau des numéros des cartes à échanger
		private static void echangeDeCartes(carte[] unJeu, int[] e)
		// Calcule et retourne la combinaison (paire, double-paire…) pour un jeu complet de 5 cartes
		// La valeur retournée est un élement de l'énumération 'combinaison'
		private static combinaison chercheCombinaison(carte[] unJeu)
		
		private static void afficheResultat(carte [] unJeu)
		// Enregistrer le jeu dans un fichier
			
		private static void enregistrerJeu(carte [] unJeu)
		// Voir les scores depuis le fichier private static void voirScores()
		// Jouer au poker
		
		// Voir les scores depuis le fichier 
		private static void voirScores()
		
		public static void jouerAuPoker(carte [] leJeu)
		// Affiche le menu du jeu et retourne l'option choisie public static char afficherMenu()
		
		// Affiche le menu du jeu et retourne l'option choisie 
		public static char afficherMenu()
	}
}