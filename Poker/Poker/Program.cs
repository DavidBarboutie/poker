﻿/*
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
        
        //variable de hauteur des cartes dans l'affichage
        public static int hauteur = 12;

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
        	tirageDuJeu(MonJeu);
        	
        	//affiche le deck
        	affichageCarte();
        	
        	//combien de cartes a echanger
        	Console.WriteLine("nombre de cartes a echanger <0-5>");
        	char saisies = (char)_getche();
        	int saisie = int.Parse(saisies.ToString());
        	
        	//si la saisie est entre 1 et 5
        	if (saisie > 0) {
        		int[] e = new int[saisie];

	        	//quelles cartes a echanger
	        	for (int i = 0; i < saisie; i++) {
	        		Console.WriteLine("\n quelles cartes a echanger <1-5>");
	        		char entrer = (char)_getche();
	        		int valeur_a_modifier = int.Parse(entrer.ToString())-1;
	        		e[i] = valeur_a_modifier;
	        	}
	        
	        	Console.Clear();
			
				//echange les cartes designer
	        	echangeCarte(MonJeu, e);
	        	
	        	//affiche les nouvelles cartes
	        	affichageCarte();
        	}
        	else{
        		Console.Clear();
        		
        		affichageCarte();
        	}
        	
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
                Console.SetCursorPosition(left, hauteur);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '*', '-', '-', '-', '-', '-', '-', '-', '-', '-', '*');
                Console.SetCursorPosition(left, hauteur+1);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, hauteur+2);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|');
                Console.SetCursorPosition(left, hauteur+3);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', ' ', ' ', ' ', ' ', ' ', ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, hauteur+4);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, ' ', ' ', ' ', '|');
                Console.SetCursorPosition(left, hauteur+5);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', ' ', (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, ' ', ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, hauteur+6);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, ' ', ' ', ' ', '|');
                Console.SetCursorPosition(left, hauteur+7);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', ' ', ' ', ' ', ' ', ' ', ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, hauteur+8);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|');
                Console.SetCursorPosition(left, hauteur+9);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, hauteur+10);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '*', '-', '-', '-', '-', '-', '-', '-', '-', '-', '*');
                Console.SetCursorPosition(left, hauteur+11);
                SetConsoleTextAttribute(hConsole, 10);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", ' ', ' ', ' ', ' ', ' ', c, ' ', ' ', ' ', ' ', ' ');
                left = left + 15;
                c++;
                
            }
            hauteur += 16;

        }

        // Enregistre le score dans le txt
        private static void enregistrerJeu(carte[] unJeu)
        {
        	SetConsoleTextAttribute(hConsole, 10);
        	string nom   = "";
       		string ligne = "";
			BinaryWriter f; // Variable FICHIER
			// Ouverture du fichier en AJOUT
			// Si le fichier EXISTE : ajout à la fin sinon création du fichier
			f = new BinaryWriter(new FileStream("scores.txt", FileMode.Append, FileAccess.Write));
			
			//demande si enrgistrement
        	Console.WriteLine("Enregistrer le jeu ? <O/N> ");
        	char choix = (char)_getche();
        	
        	//si non
        	if (choix.ToString() == "n" || choix.ToString() == "N") {
        		Console.WriteLine("vous avez choisi de ne pas enregistrer le jeu");
        	}
        	
        	//si oui
        	if (choix.ToString() == "o" || choix.ToString() == "O") {
        		
        		//entrer du pseudo
        		Console.WriteLine("\nquel est votre PSEUDO ?");
        		nom = Console.ReadLine();
        		
        		//recuperation du jeu
        		for (int i = 0; i < 5; i++) {
        			char carte = unJeu[i].valeur;
        			int fam = unJeu[i].famille;
        			ligne = ligne + carte.ToString() + fam.ToString();
        		}
        		//mettre l'ensemble de la ligne dans une seule variable pour faciliter le chiffrement
        		ligne = nom +":"+ ligne;
        		
        		//chiffrement en césar
        		ligne = chiffre(ligne);
        		
        		//ajout au fichier
        		f.Write(ligne + ";");
        	}
        	f.Close();
        }
        
        //fonction de chiffrement
        private static string chiffre(string chaine){
        	
        	//chaine chiffrer
        	string chaine_chiffre = "";
        	
        	//distinguer pseudo et cartes
        	string[] split = chaine.Split(Convert.ToChar(":"));
        	string nom = split[0];
        	string jeu = split[1];
        	
        	//alphabets
        	string[] alpha = {"a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z"};
        	string[] ALPHA = {"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};
        	
        	//val
        	char[] valeurs = { 'A', 'R', 'D', 'V', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };
			int[] familles = { 3, 4, 5, 6 };
			
        	//chiffrement jeu
        	string[] val = {"&","ç","@","%","$","^","_","`","|","[","{","#","~"};
        	string[] famm = {"ù","û","î","ô"};
        	
        	//parcours du pseudo
        	for (int i = 0; i < nom.Length; i++) {
        		//parcours de l'alphabet pour chaque caracete du pseudo
        		for (int j = 0; j < alpha.Length; j++) {

        			//PSEUDO
        			//si un caractere est egal
        			if (nom[i].ToString() == alpha[j] || nom[i].ToString() == ALPHA[j]){
        				//si le caractere de la chaine est z
        				if (nom[i].ToString() == "z" || nom[i].ToString() == "Z") {
        					chaine_chiffre += "a";
        				}
        				//si le caractere n'est pas z
        				else{
        					chaine_chiffre += alpha[j+1];
        					
        				}
        			}
        		}
        	}
        	chaine_chiffre += ":";
        	//JEU
        	for (int i = 0; i < jeu.Length; i++) {
        		if (i%2 == 0) {
        			for (int j = 0; j < valeurs.Length; j++){
       					if (jeu[i].ToString() == valeurs[j].ToString()) {
      							chaine_chiffre += val[j];
      						}
       				}
        		}
       			else{
        			for (int j = 0; j < familles.Length; j++) {
        				if (jeu[i].ToString() == familles[j].ToString()) {
        					chaine_chiffre += famm[j];
        				}
        			}
       			}
      		}
       	return chaine_chiffre;
        }
        
        
        //fonction de déchiffrement
		private static string dechiffre(string chaine){
        	
        	//chaine chiffrer
        	string chaine_dechiffre = "";
        	
        	//distinguer pseudo et cartes
        	string[] split = chaine.Split(Convert.ToChar(":"));
        	if (split.Length > 1) {
        		string nom = split[0];
	        	string jeu = split[1];
	        	//alphabets
	        	string[] alpha = {"a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z"};
	        	string[] ALPHA = {"A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};
	        	
	        	//val
	        	char[] valeurs = { 'A', 'R', 'D', 'V', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };
				int[] familles = { 3, 4, 5, 6 };
				
	        	//déchiffrement jeu
	        	string[] val = {"&","ç","@","%","$","^","_","`","|","[","{","#","~"};
	        	string[] famm = {"ù","û","î","ô"};
	        	
	        	//PSEUDO
	        	for (int i = 0; i < nom.Length; i++) {
	        		//parcours de l'alphabet pour chaque caracete du pseudo
	        		for (int j = 0; j < alpha.Length; j++) {
	
	        			//si un caractere est egal
	        			if (nom[i].ToString() == alpha[j] || nom[i].ToString() == ALPHA[j]){
	        				//si le caractere de la chaine est a
	        				if (nom[i].ToString() == "a" || nom[i].ToString() == "A") {
	        					chaine_dechiffre += "z";
	        				}
	        				//si le caractere n'est pas z
	        				else{
	        					chaine_dechiffre += alpha[j-1];
	        				}
	        			}
	        		}
	        	}
	        	chaine_dechiffre += " ";
	        	//JEU
	        	for (int i = 0; i < jeu.Length; i++) {
	        		if (i%2 == 0) {
	        			for (int j = 0; j < valeurs.Length; j++){
	       					if (jeu[i].ToString() == val[j].ToString()) {
	      							chaine_dechiffre += valeurs[j];
	      						}
	       				}
	        		}
	       			else{
	        			for (int j = 0; j < familles.Length; j++) {
	        				if (jeu[i].ToString() == famm[j].ToString()) {
	        					chaine_dechiffre += familles[j];
	        				}
	        			}
	       			}
	      		}
        	}
        	else{}	
       	return chaine_dechiffre;
        }

        // Affiche le Scores
        private static void voirScores()
        {

        	BinaryReader f; // Variable FICHIER
        	
			// Caractères délimiteurs des champs de l'article
			char [] délimiteurs = {';'};
			
			// Ouverture en LECTURE
			f = new BinaryReader(new FileStream("scores.txt", FileMode.Open, FileAccess.Read));
			
			string temp = "";
			string chaine ="";
			//lecture du fichier en entier
			Console.WriteLine("\n");
			while (f.BaseStream.Position != f.BaseStream.Length)
			{
				temp += f.ReadString();
			}
			
			//decoupage du fichier selon les joueurs
			string[] fichier = temp.Split(';');
			//dechiffrement et affichage de toutes les parties enregistrées
			for (int i = 0; i < fichier.Length-1; i++) {
				chaine = dechiffre(fichier[i]);
				
				//separe nom et jeu
				string[] card = chaine.Split(' ');
				//affiche le nom du joueur
				Console.WriteLine(card[0]);
				//stock le jeu
				string game = card[1];
				
				//instancie les nouvelles cartes
				carte c = new carte();
				
				//compteur permettant dajouter les cartes au bon indices de MonJeu
				int cpt = 1;
				
				//parcours de la chaine de caractere du fichier representant le jeu
				for (int k = 0; k < 10; k++) {
					
					//si l'indicde est pair, le caractere est une valeur de carte
					if (k % 2 == 0) {
						c.valeur = game[k];
					}
					//si l'indice est impaire le caractere est une famille de carte
					else{
						//conversion permettant l'affichage correcte des symboles
						c.famille = int.Parse(Convert.ToChar(Convert.ToByte(game[k])).ToString());
						//ajout de la carte dans le jeu
						MonJeu[k-cpt] = c;
						cpt++;
					}
				}
				//affiche les jeux enregistrer sous forme de carte
				affichageCarte();
			}
			f.Close();
			Console.ReadKey();
			Console.Clear();
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



