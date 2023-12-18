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

namespace Poker
{
    class Program
    {
        // -----------------------
        // DECLARATION DES DONNEES
        // -----------------------
        // Importation des DL (librairies de code) permettant de gérer les couleurs en mode console
        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleTextAttribute(IntPtr hConsoleOutput, int wAttributes);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(uint nStdHandle);
        static uint STD_OUTPUT_HANDLE = 0xfffffff5;
        static IntPtr hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
        // Pour utiliser la fonction C 'getchar()' : sasie d'un caractère
        [DllImport("msvcrt")]
        static extern int _getche();

        //-------------------
        // TYPES DE DONNEES
        //-------------------

        // Fin du jeu
        public static bool fin = false;

        // Codes COULEUR
        public enum couleur { VERT = 10, ROUGE = 12, JAUNE = 14, BLANC = 15, NOIRE = 0, ROUGESURBLANC = 252, NOIRESURBLANC = 240 };

        // Coordonnées pour l'affichage
        public struct coordonnees
        {
            public int x;
            public int y;
        }

        // Une carte
        public struct carte
        {
            public char valeur;
            public int famille;
        };

        // Liste des combinaisons possibles
        public enum combinaison { RIEN, PAIRE, DOUBLE_PAIRE, BRELAN, QUINTE, FULL, COULEUR, CARRE, QUINTE_FLUSH };

        // Valeurs des cartes : As, Roi,...
        public static char[] valeurs = { 'A', 'R', 'D', 'V', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };

        // Codes ASCII (3 : coeur, 4 : carreau, 5 : trèfle, 6 : pique)
        public static int[] familles = { 3, 4, 5, 6 };

        // Numéros des cartes à échanger
        public static int[] echange = { 0, 0, 0, 0 };

        // Jeu de 5 cartes
        public static carte[] MonJeu = new carte[5];
        
        public static Random rnd = new Random();

        //----------
        // FONCTIONS
        //----------

        // Génère aléatoirement une carte : {valeur;famille}
        // Retourne une expression de type "structure carte"
        public static carte tirage()
        {
            int x = rnd.Next(0,12);
            int y = rnd.Next(0,3);
            carte unecarte = new carte{};
			unecarte.famille = familles[y];
			unecarte.valeur = valeurs[x];
			return unecarte;
        }
  

        // Indique si une carte est déjà présente dans le jeu
        // Paramètres : une carte, le jeu 5 cartes, le numéro de la carte dans le jeu
        // Retourne un entier (booléen)
        public static bool carteUnique(carte uneCarte, carte[] unJeu)
        {
			for (int i = 0; i < 5; i++) {
        		if (uneCarte.valeur.ToString() == unJeu[i].valeur.ToString() && uneCarte.famille.ToString() == unJeu[i].famille.ToString()) {
					return false;
				}
			}
        	return true;
        }

        // Calcule et retourne la COMBINAISON (paire, double-paire... , quinte-flush)
        // pour un jeu complet de 5 cartes.
        // La valeur retournée est un élement de l'énumération 'combinaison' (=constante)
        public static combinaison chercheCombinaison(ref carte[] unJeu)
        {
        	
			int [] similaire = {0,0,0,0,0};
			int [] meme_famille = {0,0,0,0,0};
			char [,] quintes = {{'X','V','D','R','A'},{'9','X','V','D','R'},{'8','9','X','V','D'},{'7','8','9','X','V'}};
			bool couleur = false;
			int cpt_brelan = 0;
			int cpt_paires = 0;
			int cpt_un = 0;
			int cpt_quinte;
			
			//completion du tableau similaire
			for (int i = 0; i < 5; i++) {
				for (int j = 0; j < 5; j++) {
					if (unJeu[i].valeur == unJeu[j].valeur) {
						similaire[i] = similaire[i] + 1;
					}
				}
			}
			
			//completion du tableau meme_famille
			for (int i = 0; i < 5; i++) {
				for (int j = 0; j < 5; j++) {
					if (unJeu[i].famille == unJeu[j].famille) {
						meme_famille[i] = meme_famille[i] + 1;
					}
				}
			}
			
			//verifie si couleur
			for (int i = 0; i < 5; i++) {
				if (meme_famille[i] == 5) {
					couleur = true;
				}
			}
			
			for (int i = 0; i < 5; i++) {
				Console.WriteLine(similaire[i]);
				//verifie si carre
				if (similaire[i] == 4) {
					return combinaison.CARRE;
				}
				
				//verifie si brelan
				if (similaire[i] == 3) {
					cpt_brelan++;
				}
				
				//verifie si paires
				if (similaire[i] == 2) {
					cpt_paires++;
				}
				
				//verifie si toutes les cartes sont differentes
				if (similaire[i] == 1) {
					cpt_un++;
				}
			}
			
			//verifie si quinte
			for (int i = 0; i < 4; i++) {
				cpt_quinte = 0;
				for (int j = 0; j < 5; j++) {
					for (int k = 0; k < 5; k++) {
						if (unJeu[k].valeur == quintes[i,j]) {
							cpt_quinte++;
						}
					}
					if (cpt_quinte == 5 && couleur == true && cpt_paires == 0 && cpt_brelan == 0) {
						return combinaison.QUINTE_FLUSH;
					}
					if (cpt_quinte == 5 && cpt_paires == 0 && cpt_brelan == 0 && couleur == false) {
						return combinaison.QUINTE;
					}
				}
			}
			
			//couleur
			if (couleur == true) {
				return combinaison.COULEUR;
			}
			
			//verifie si full
			if (cpt_paires == 2 && cpt_brelan == 3) {
				return combinaison.FULL;
			}
			//verifie si brelan
			if (cpt_brelan == 3) {
				return combinaison.BRELAN;
			}
			
			//verifie double paires
			if (cpt_paires / 2 == 2) {
				return combinaison.DOUBLE_PAIRE;
			}
			
			//verifie paire
			if (cpt_paires/2==1) {
				return combinaison.PAIRE;
			}
			//cas ou rien n'est présent
			return combinaison.RIEN;
        }

        // Echange des cartes
        // Paramètres : le tableau de 5 cartes et le tableau des numéros des cartes à échanger
        private static void echangeCarte(carte[] unJeu, int[] e)
        {
        	for (int i = 0; i < e.Length; i++) {
        	unJeu[e[i]] = tirage();
        	}
        }

        // Pour afficher le Menu pricipale
        private static void afficheMenu(){
        	
        	Console.WriteLine("*-------------*");
			for (int i = 1; i < 7; i++) {
        		if (i == 2) {
        			Console.WriteLine("|    POKER    |");
        		}
        		if (i == 4) {
        			Console.WriteLine("|   1 JOUER   |");
        		}
        		if (i == 5) {
        			Console.WriteLine("|   2 score   |");
        		}
        		if (i == 6) {
        			Console.WriteLine("|   3 Fin     |");
        		}
        		if (i == 1 || i == 3 || i == 7) {
        			Console.WriteLine("|             |");
        		} 
			}
        	Console.WriteLine("*-------------*");
        }

        // Jouer au Poker
		// Ici que vous appellez toutes les fonction permettant de joueur au poker
        private static void jouerAuPoker()
        {
        	//clear la console
        	Console.Clear();
        	
        	//creer un deck
        	tirageDuJeu2(MonJeu);
        	
        	//affiche le deck
        	affichageCarte();
        	
        	//combien de cartes a echanger
        	Console.WriteLine("nombre de cartes a echanger <1-5>");
        	char saisies = (char)_getche();
        	int saisie = int.Parse(saisies.ToString());
        	int[] e = new int[saisie];
        	
        	//quelles cartes a echanger
        	for (int i = 0; i < saisie; i++) {
        		Console.WriteLine("\n nombre de cartes a echanger <0-4>");
        		char entrer = (char)_getche();
        		e[i] = int.Parse(entrer.ToString());
        	}
        	
			Console.Clear();
			
			//echange les cartes designer
        	echangeCarte(MonJeu, e);
        	
        	//affiche les nouvelles cartes
        	affichageCarte();
        	
        	//affiche les resultats
        	afficheResultat(MonJeu);
        	
 			//permet de ne pas reafficher le menu avant d'avoir appuyer sur une touche quelconque 
 			enregistrerJeu(MonJeu);
        	Console.ReadKey();
        	Console.Clear();
        }
        // Tirage d'un jeu de 5 cartes
        // Paramètre : le tableau de 5 cartes à remplir
        private static void tirageDuJeu(carte[] unJeu)
        {
        	int I = 0;
        	
        	while (I < 5) {
        		//tire une carte aléatoire
				carte c = tirage();

				//verifie si la carte se trouve dans le deck
				if (carteUnique(c,unJeu) == true){
					
					//ajoute la carte au deck
					unJeu[I] = c;
					I++;
				}
			}
        }
		
        //fonction temporaire pour tester facilement les combinaisons en creant artificiellement un deck
        private static void tirageDuJeu2(carte[] unJeu)
        {
				carte c1 = new carte{};
				carte c2 = new carte{};
				carte c3 = new carte{};
				carte c4 = new carte{};
				carte c5 = new carte{};
				
				c1.famille = familles[0];
				c1.valeur = valeurs[6];
				
				c2.famille = familles[1];
				c2.valeur = valeurs[1];
				
				c3.famille = familles[0];
				c3.valeur = valeurs[2];
				
				c4.famille = familles[1];
				c4.valeur = valeurs[2];
				
				c5.famille = familles[2];
				c5.valeur = valeurs[2];
				
				unJeu[0] = c1;
				unJeu[1] = c2;
				unJeu[2] = c3;
				unJeu[3] = c4;
				unJeu[4] = c5;
		}
		
        
        // Affiche à l'écran une carte {valeur;famille} 
        private static void affichageCarte()
        {
            //----------------------------
            // TIRAGE D'UN JEU DE 5 CARTES
            //----------------------------
            int left = 0;
            int c = 1;
            // Tirage aléatoire de 5 cartes
            for (int i = 0; i < 5; i++)
            {
                // Tirage de la carte n°i (le jeu doit être sans doublons !)

                // Affichage de la carte
                if (MonJeu[i].famille == 3 || MonJeu[i].famille == 4)
                    SetConsoleTextAttribute(hConsole, 252);
                else
                    SetConsoleTextAttribute(hConsole, 240);
                Console.SetCursorPosition(left, 5);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '*', '-', '-', '-', '-', '-', '-', '-', '-', '-', '*');
                Console.SetCursorPosition(left, 6);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, 7);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|');
                Console.SetCursorPosition(left, 8);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', ' ', ' ', ' ', ' ', ' ', ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, 9);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, ' ', ' ', ' ', '|');
                Console.SetCursorPosition(left, 10);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', ' ', (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, ' ', ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, 11);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, ' ', ' ', ' ', '|');
                Console.SetCursorPosition(left, 12);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', ' ', ' ', ' ', ' ', ' ', ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, 13);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|');
                Console.SetCursorPosition(left, 14);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, 15);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '*', '-', '-', '-', '-', '-', '-', '-', '-', '-', '*');
                Console.SetCursorPosition(left, 16);
                SetConsoleTextAttribute(hConsole, 10);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", ' ', ' ', ' ', ' ', ' ', c, ' ', ' ', ' ', ' ', ' ');
                left = left + 15;
                c++;
            }

        }

        // Enregistre le score dans le txt
        private static void enregistrerJeu(carte[] unJeu)
        {
        	SetConsoleTextAttribute(hConsole, 10);
        	string nom, ligne;
			BinaryWriter f; // Variable FICHIER
			// Ouverture du fichier en AJOUT
			// Si le fichier EXISTE : ajout à la fin sinon création du fichier
			
			
        	Console.WriteLine("Enregistrer le jeu ? <O/N> ");
        	char choix = (char)_getche();
        	if (choix.ToString() == "n" || choix.ToString() == "N") {
        		Console.WriteLine("vous avez choisi de ne pas enregistrer le jeu");
        	}
        	if (choix.ToString() == "o" || choix.ToString() == "O") {
        		Console.WriteLine("quel est votre PSEUDO ?");
        		char pseudo = (char)_getche();

        	}
        }

        // Affiche le Scores
        private static void voirScores()
        {
           //a faire
        }

        // Affiche résultat
        private static void afficheResultat(carte[] unJeu)
        {
            SetConsoleTextAttribute(hConsole, 012);
            Console.Write("RESULTAT - Vous avez : ");
            try
            {
                // Test de la combinaison
                switch (chercheCombinaison(ref unJeu))
                {
                    case combinaison.RIEN:
                        Console.WriteLine("rien du tout... desole!"); break;
                    case combinaison.PAIRE:
                        Console.WriteLine("une simple paire..."); break;
                    case combinaison.DOUBLE_PAIRE:
                        Console.WriteLine("une double paire; on peut esperer..."); break;
                    case combinaison.BRELAN:
                        Console.WriteLine("un brelan; pas mal..."); break;
                    case combinaison.QUINTE:
                        Console.WriteLine("une quinte; bien!"); break;
                    case combinaison.FULL:
                        Console.WriteLine("un full; ouahh!"); break;
                    case combinaison.COULEUR:
                        Console.WriteLine("une couleur; bravo!"); break;
                    case combinaison.CARRE:
                        Console.WriteLine("un carre; champion!"); break;
                    case combinaison.QUINTE_FLUSH:
                        Console.WriteLine("une quinte-flush; royal!"); break;
                };
            }
            catch { }
        }


        //--------------------
        // Fonction PRINCIPALE
        //--------------------
        static void Main(string[] args)
        {
            //---------------
            // BOUCLE DU JEU
            //---------------
            char reponse;
            while (true)
            {
                afficheMenu();
                reponse = (char)_getche();
                if (reponse != '1' && reponse != '2' && reponse != '3')
                {
                    Console.Clear();
                }
                else
                {
                SetConsoleTextAttribute(hConsole, 015);
                // Jouer au Poker
                if (reponse == '1')
                {
                    int i = 0;
                    jouerAuPoker();
                    
                }

                if (reponse == '2')
                    voirScores();

                if (reponse == '3')
                    break;
            }
            }
            Console.Clear();
        }
    }
}



