/*
 * Created by SharpDevelop.
 * User: d.barboutie
 * Date: 20/11/2023
 * Time: 17:00
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;



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
			
		// jeu de 5 cartes
		public static carte[] unJeu = new carte[5];
			
		// Test de la combinaison
		switch (cherche_combinaison(unJeu))
		{
		case combinaison.RIEN :
			afficher_message("rien du tout... desole!", couleur.ROUGE, new coordonnees { x = 24, y = 15 }); 
			break;
			
			
			
		// Génère aléatoirement une carte : {valeur;famille}
		// Retourne une expression de type "carte"
		private static carte tirage(){
			Random rnd = new Random();
			new carte = {valeur = valeurs[rnd], famille = familles[rnd]};
			return carte;
		}
			
		
		
		// Indique si une carte est déjà présente dans le jeu
		// Paramètres : une carte, le jeu 5 cartes, le numéro de la carte dans le jeu
		// Retourne un booléen
		private static bool carteUnique(carte uneCarte, carte[] unJeu, int numero)
		{
			foreach(carte element in unJeu){
				if (element.valeur == uneCarte.valeur && element.famille == uneCarte.famille) {
					return true;
				}
			}
			return false;
		}
		
		// Affiche à l'écran une carte {valeur;famille}
		private static void affichageCarte(carte uneCarte){
			
			
			
			// Choix de la couleur
			// * couleur.ROUGE pour coeur et carreau
			// * couleur.NOIRE pour trèfle et pique
			/////int colonne = Console.GetCursorPosition();
			couleur lacouleur;
			if (uneCarte.famille == 3 || uneCarte.famille == 4){
				lacouleur = couleur.ROUGESURBLANC;}
			else
			{
				lacouleur = couleur.NOIRESURBLANC;
				// Positionnement et affichage
				// REMARQUE : dans la fenêtre exécution, choisir "Police Roster" pour afficher les symboles...
				Console.WriteLine
				(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n",'*','-','-','-','-','-','-','-','-','-','*'), lacouleur, new coordonnees {x= 1 * 15, y=1 });
				Console.WriteLine (
				string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n",'|',
				(char)uneCarte.famille,' ', (char)uneCarte.famille,' ', (char)uneCarte.famille,' ',
				(char)uneCarte.famille,' ',
				(char)uneCarte.famille, '|'), lacouleur, new coordonnees { x = 1 * 15, y = 2 }
				);
			}
		}
		
		
		// Tirage d'un jeu de 5 cartes
		// Paramètre : le tableau de 5 cartes à remplir
		private static void tirageDuJeu(carte[] unJeu){
		for (int i = 0; i < 5; i++) {
				unJeu[i] = tirage();
		}
		
		
		
		// Echange des cartes
		// Paramètres : le tableau de 5 cartes et le tableau des numéros des cartes à échanger
		private static void echangeDeCartes(carte[] unJeu, int[] e){
			//
		}
		
		
		
		// Calcule et retourne la combinaison (paire, double-paire…) pour un jeu complet de 5 cartes
		// La valeur retournée est un élement de l'énumération 'combinaison'
		private static combinaison chercheCombinaison(carte[] unJeu){
			
			int cpt = 0;
			int paire = 0;
			int triple = 0;
			
			for (int i = 0; i < 5; i++) {
				carte carte = unJeu[i];
				for (int j = 0; j < end; i++) {
					if (carte != unJeu[j]) {
						if (carte == unJeu[j]){
							cpt+=1
						}
					}
				}
				if (cpt == 2){
					paire += 1;	
				}
				else{
					if (cpt == 3) {
						triple += 1;
					}
					else{
						if (cpt == 4) {
							//cas du carré
							return combinaison.CARRE
						}
					}
				}
			}
			if (paire >= 1) {
				//cas d'une seule paire
				if (paire == 2) {
					return combinaison.DOUBLE_PAIRE;
				}
				else{
					if (triple == 0) {
						return combinaison.PAIRE;
					}
					else{
						return combinaison.BRELAN;
					}
				}
					
				}
			}
		//cas ou rien n'est présent
		return combinaison.RIEN;
		}
		
		// Calcul et affichage du résultat
		// Paramètre : le tableau de 5 cartes
		private static void afficheResultat(carte [] unJeu){}
		
		
		
		// Enregistrer le jeu dans un fichier
		private static void enregistrerJeu(carte [] unJeu){}
		
		
		// Voir les scores depuis le fichier 
		private static void voirScores(){}
		
		
		//jouer au poker
		public static void jouerAuPoker(carte [] unJeu){}
		
		
		// Affiche le menu du jeu et retourne l'option choisie 
		public static char afficherMenu(){}
	}
}