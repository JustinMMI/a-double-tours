using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LogiquePendu : MonoBehaviour
{
    [Header("Configuration")]
    public string[] listeDeMots =
    {
        "BANANE", "POMME", "SOLEIL", "ORDINATEUR", "BOB", "PENDU", "PROJET", "ANANAS", "POIRE", "CERISE",
        "FRAISE", "FRAMBOISE", "MANGUE", "KIWI", "RAISIN", "PASTEQUE", "MELON", "ABRICOT", "PRUNEAU", "PECHE",
        "CAROTTE", "TOMATE", "PATATE", "HARICOT", "POIVRON", "AUBERGINE", "CONCOMBRE", "SALADE", "RADIS", "EPINARD",
        "CHOU", "OIGNON", "AIL", "PERSIL", "BASILIC", "THYM", "ROMARIN", "MENTHE", "GINGEMBRE", "CITRON",
        "ORANGE", "PAMPLEMOUSSE", "MANDARINE", "NOISETTE", "AMANDE", "NOIX", "COCO", "CAFE", "CHOCOLAT", "VANILLE",
        "SUCRE", "SEL", "POIVRE", "PAIN", "FROMAGE", "BEURRE", "YAOURT", "BISCUIT", "GATEAU", "BONBON",
        "MONTAGNE", "RIVIERE", "OCEAN", "PLAGE", "DESERT", "FORET", "PRAIRIE", "COLLINE", "VALLEE", "CAVERNE",
        "NUAGE", "ORAGE", "PLUIE", "NEIGE", "VENT", "BROUILLARD", "ECLAIR", "TONNERRE", "LUNE", "ETOILE",
        "PLANETE", "COMETE", "GALAXIE", "FUSEE", "SATELLITE", "ASTRONAUTE", "UNIVERS", "METEORE", "HORIZON", "AURORE",
        "MAISON", "JARDIN", "FENETRE", "PORTE", "ESCALIER", "COULOIR", "SALON", "CUISINE", "CHAMBRE", "GARAGE",
        "BUREAU", "LIVRE", "CAHIER", "STYLO", "CRAYON", "GOMME", "TABLEAU", "LAMPE", "HORLOGE", "MIROIR",
        "VOITURE", "MOTO", "VELO", "TRAIN", "AVION", "BATEAU", "BUS", "ROUTE", "PONT", "TUNNEL",
        "VILLAGE", "VILLE", "CHATEAU", "TOUR", "FORTERESSE", "DONJON", "PALAIS", "TEMPLE", "MARCHE", "PLACE",
        "SOLDAT", "CHEVALIER", "MAGICIEN", "DRAGON", "MONSTRE", "TROLL", "OGRE", "GUERRIER", "ARCHER", "ASSASSIN",
        "BOUCLIER", "EPEE", "DAGUE", "MARTEAU", "ARC", "FLECHE", "LANCE", "CASQUE", "ARMURE", "GANTELET",
        "POTION", "ELIXIR", "CRISTAL", "RUNE", "TALISMAN", "AMULETTE", "GRIMOIRE", "PARCHEMIN", "SORTILEGE", "ENCHANTEMENT",
        "AVENTURE", "MISSION", "QUETE", "ENIGME", "SECRET", "MYSTERE", "LABYRINTHE", "PASSAGE", "PORTAIL", "SANCTUAIRE",
        "ROBOT", "CYBORG", "LASER", "CIRCUIT", "SERVEUR", "CLAVIER", "SOURIS", "ECRAN", "TABLETTE", "SMARTPHONE",
        "LOGICIEL", "DONNEE", "RESEAU", "CODE", "PROGRAMME", "SCRIPT", "MOTEUR", "PIXEL", "SPRITE", "NIVEAU",
        "BONUS", "MALUS", "SCORE", "VICTOIRE", "DEFAITE", "PARTIE", "DUEL", "ARCADE", "MINIJEU", "CHALLENGE",
        "PION", "DES", "CARTE", "PLATEAU", "STRATEGIE", "TACTIQUE", "ALLIANCE", "RIVAL", "COMPAGNON", "CAPITAINE",
        "ANCRE", "VOILE", "PIRATE", "TRESOR", "CARTEPOSTALE", "BOTTINE", "TORCHE", "CAMP", "BIVOUAC", "SACADOS",
        "MUSIQUE", "MELODIE", "RYTHME", "GUITARE", "PIANO", "VIOLON", "TAMBOUR", "CHANT", "THEATRE", "SPECTACLE",
        "PEINTURE", "DESSIN", "SCULPTURE", "MUSEE", "GALERIE", "PHOTO", "IMAGE", "COULEUR", "FORME", "MOTIF",
        "LAPIN", "RENARD", "LOUP", "OURS", "TIGRE", "LION", "PANTHERE", "AIGLE", "HIBOU", "CORBEAU",
        "DAUPHIN", "REQUIN", "BALEINE", "POULPE", "CRABE", "TORTUE", "SERPENT", "LEZARD", "PAPILLON", "ABEILLE",
        "FOURMI", "SCARABEE", "CHENILLE", "COCCINELLE", "ESCARGOT", "GRENOUILLE", "HERISSON", "ECUREUIL", "CASTOR", "SANGLIER",
        "RUBIS", "SAPHIR", "EMERAUDE", "DIAMANT", "OPALE", "JADE", "ONYX", "TOPAZE", "QUARTZ", "PERLE",
        "COFFRE", "CLE", "SERRURE", "CHAINE", "BARRIERE", "PASSERELLE", "RAMPE", "TRAPPE", "ASCENSEUR", "MECANISME"
    };

    [Header("Interface UI")]
    public TMP_Text affichageMot;
    public TMP_Text texteMessage;
    public TMP_Text texteLettresFausses;

    [Header("Clavier")]
    public List<Button> boutonsClavier = new List<Button>();

    private static readonly Color CouleurBonne    = new Color(0.1f, 0.75f, 0.2f);
    private static readonly Color CouleurMauvaise = new Color(0.85f, 0.15f, 0.1f);
    private static readonly Color CouleurDefaut   = Color.white;

    private string motA_Deviner;
    private string motCache = "";
    private int viesJ1 = 5;
    private int viesJ2 = 5;
    private int boucliersActifs = 0;
    private int joueurActuel = 1;
    private List<string> lettresRatees = new List<string>();
    private bool partieTerminee = false;
    private int lastWordIndex = -1;

    void Start()
    {
        NouvellePartie();
    }

    void NouvellePartie()
    {
        int indexAleatoire = Random.Range(0, listeDeMots.Length);
        if (listeDeMots.Length > 1)
        {
            while (indexAleatoire == lastWordIndex)
            {
                indexAleatoire = Random.Range(0, listeDeMots.Length);
            }
        }

        lastWordIndex = indexAleatoire;
        motA_Deviner = listeDeMots[indexAleatoire].ToUpper();

        motCache = "";
        for (int i = 0; i < motA_Deviner.Length; i++) motCache += "_ ";

        affichageMot.text = motCache;
        lettresRatees.Clear();
        viesJ1 = 5;
        viesJ2 = 5;
        boucliersActifs = 0;
        joueurActuel = 1;
        partieTerminee = false;
        texteLettresFausses.text = "Ratees : ";

        ActiverClavier(true);
        ReinitialiseCouleursBoutons();
        MettreAJourMessage("--- DEBUT DU DUEL ---");
    }

    /// <summary>Appelée par chaque bouton lettre du clavier.</summary>
    public void AppuyerLettre(string lettre)
    {
        if (partieTerminee) return;

        string proposition = lettre.ToUpper().Trim();
        if (string.IsNullOrEmpty(proposition)) return;

        char caractere = proposition[0];

        // Lettre déjà utilisée
        if (lettresRatees.Contains("<s>" + caractere + "</s>") || motCache.Contains(caractere.ToString()))
        {
            MettreAJourMessage("[!] Lettre deja utilisee");
            return;
        }

        if (motA_Deviner.Contains(caractere.ToString()))
        {
            ColorerBouton(caractere, CouleurBonne);
            ActualiserAffichageMot(caractere);
            if (!motCache.Contains("_"))
                Gagner();
            else
                MettreAJourMessage("BIEN JOUE ! Tu rejoues.");
        }
        else
        {
            ColorerBouton(caractere, CouleurMauvaise);
            VerifierEchec(caractere);
        }
    }

    void VerifierEchec(char lettre)
    {
        AjouterLettreRatee(lettre.ToString());

        if (boucliersActifs > 0)
        {
            boucliersActifs--;
            MettreAJourMessage("RATE ! (Bouclier Bob utilise)");
        }
        else
        {
            if (joueurActuel == 1) viesJ1--;
            else viesJ2--;

            if (viesJ1 <= 0 || viesJ2 <= 0)
                Perdre();
            else
            {
                joueurActuel = (joueurActuel == 1) ? 2 : 1;
                MettreAJourMessage("MAUVAISE LETTRE !");
            }
        }
    }

    void ActiverRegleBob()
    {
        boucliersActifs = 1;
    }

    void ChangerDeJoueur()
    {
        joueurActuel = (joueurActuel == 1) ? 2 : 1;
    }

    void MettreAJourMessage(string info)
    {
        string tourCouleur = (joueurActuel == 1) ? "#3399FF" : "#FF3333";
        string bouclierInfo = (boucliersActifs > 0) ? "\n[BOUCLIER ACTIF]" : "";

        texteMessage.text = "<b>" + info + "</b>\n" +
                            "TOUR : <color=" + tourCouleur + ">JOUEUR " + joueurActuel + "</color>" +
                            "\n<color=#3399FF>J1: " + viesJ1 + " HP</color> | <color=#FF3333>J2: " + viesJ2 + " HP</color>" +
                            "<b><color=yellow>" + bouclierInfo + "</color></b>";
    }

    void AjouterLettreRatee(string l)
    {
        lettresRatees.Add("<s>" + l + "</s>");
        texteLettresFausses.text = "Lettres absentes du mot : " + string.Join(" ", lettresRatees);
    }

    void ActualiserAffichageMot(char lettre)
    {
        string nouveauRendu = "";
        for (int i = 0; i < motA_Deviner.Length; i++)
        {
            if (motA_Deviner[i] == lettre || (motCache.Length > i * 2 && motCache[i * 2] != '_'))
                nouveauRendu += motA_Deviner[i] + " ";
            else nouveauRendu += "_ ";
        }
        motCache = nouveauRendu;
        affichageMot.text = motCache;
    }

    void Gagner()
    {
        partieTerminee = true;
        affichageMot.text = motA_Deviner;
        texteMessage.text = "--- VICTOIRE JOUEUR " + joueurActuel + " ---";
        ActiverClavier(false);
    }

    void Perdre()
    {
        partieTerminee = true;
        int gagnant = (viesJ1 <= 0) ? 2 : 1;
        texteMessage.text = "--- FIN DE PARTIE ---\nLE JOUEUR " + gagnant + " GAGNE !\nLe mot : " + motA_Deviner;
        ActiverClavier(false);
    }

    /// <summary>Active ou désactive tous les boutons du clavier.</summary>
    void ActiverClavier(bool actif)
    {
        foreach (Button btn in boutonsClavier)
        {
            if (btn != null) btn.interactable = actif;
        }
    }

    /// <summary>Colore le bouton correspondant à la lettre donnée.</summary>
    void ColorerBouton(char lettre, Color couleur)
    {
        string nomBouton = $"Btn_{lettre}";
        foreach (Button btn in boutonsClavier)
        {
            if (btn == null || btn.gameObject.name != nomBouton) continue;
            Image img = btn.GetComponent<Image>();
            if (img != null) img.color = couleur;
            break;
        }
    }

    /// <summary>Remet tous les boutons du clavier à leur couleur par défaut.</summary>
    void ReinitialiseCouleursBoutons()
    {
        foreach (Button btn in boutonsClavier)
        {
            if (btn == null) continue;
            Image img = btn.GetComponent<Image>();
            if (img != null) img.color = CouleurDefaut;
        }
    }
}