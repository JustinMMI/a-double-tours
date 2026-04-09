using UnityEngine; // Donne accès aux fonctions de base de Unity (objets, positions, physique).
using UnityEngine.UI; // Outils pour les images et l'interface classique.
using UnityEngine.Android; // Nécessaire pour demander l'accès à la caméra sur smartphone
// Pourquoi Android pas iOS ? Parce que les permissions pour la caméra sont gérées différemment sur iOS, et ce code est spécifiquement conçu pour Android.
// Donc il n'y a pas code pour iOS ici, mais si vous voulez ajouter la compatibilité iOS, il faudrait utiliser des directives de compilation pour inclure le code spécifique à iOS (ex: #if UNITY_IOS).
// Ok donc il faut ajouter pour iOS ? Oui, si vous voulez que votre jeu fonctionne sur les appareils Apple, vous devrez ajouter du code pour gérer les permissions de caméra sur iOS, qui utilisent un système de permissions différent (ex: NSCameraUsageDescription dans le fichier Info.plist).
using TMPro; // Pour gérer les textes haute définition (TextMeshPro) Pourquoi ? TextMeshPro est une alternative plus avancée et plus performante au composant de texte standard de Unity. 
             // Il offre une meilleure qualité d'affichage, plus de fonctionnalités de mise en forme, et une meilleure gestion des polices, ce qui le rend idéal pour les interfaces utilisateur modernes et les jeux qui nécessitent un texte clair et stylisé.
using System.Collections; // Nécessaire pour utiliser les Coroutines (fonctions qui gèrent le temps).

// Pourquoi System. ? 
// System.Collections est un espace de noms qui contient des classes et des interfaces pour gérer les collections de données (comme les listes, les tableaux, etc.) et aussi pour les Coroutines dans Unity.
// Oui mais System et Collections ? 


// Coroutines : fonctions spéciales qui permettent d'attendre un certain temps ou de faire des actions en plusieurs étapes sans bloquer le jeu.
// Pourquoi ? Parce que certaines actions, comme allumer la caméra ou attendre avant de relancer une manche, prennent du temps. 
// Sans les Coroutines, le jeu se figerait pendant ces moments, ce qui n'est pas agréable pour le joueur.

public class ColorCatcherGame : MonoBehaviour // Cette classe gère le mini-jeu de capture de couleur.
                                              // Elle doit être attachée à un objet dans la scène (ex: un GameObject vide nommé "GameManager") pour fonctionner.

// MonoBehaviour : indique l'héritage. En dérivant de MonoBehaviour, la classe devient un composant Unity et obtient les callbacks (Start, Update, OnEnable, StartCoroutine, Invoke, etc.) et peut être attachée à un GameObject.
// pas compris ? 
{
    // class pourquoi ? 
    // En programmation orientée objet, une classe est comme un plan ou un modèle pour créer des objets.
    // Ici, la classe ColorCatcherGame définit tout ce qui concerne le mini-jeu : les variables pour la caméra, les couleurs, le temps, et les fonctions pour démarrer le jeu, vérifier les couleurs et gérer la fin de partie.


    // --- RÉFÉRENCES AUX OBJETS DE L'INTERFACE (UI) ---
    [Header("UI Obligatoire")] // Cette section dans l'inspecteur de Unity regroupe les éléments que vous devez lier pour que le jeu fonctionne
    public RawImage cameraDisplay;   // L'écran qui affichera le flux vidéo de la caméra
    public TMP_Text instructionText; // Le texte qui affiche "Bob dit..." ou "Bravo !"
    public TMP_Text timerText;        // Le texte qui affiche le décompte des secondes
    public Image colorPreview;       // Un petit carré UI montrant la couleur cible à chercher

    // --- RÉGLAGES DU JEU ---
    [Header("Réglages")] // Cette section regroupe les paramètres que vous pouvez ajuster pour rendre le jeu plus facile ou plus difficile
    public float timeLimit = 15f;    // Temps imparti pour chaque manche
    public float tolerance = 0.45f;  // Seuil de précision (plus c'est bas, plus c'est difficile)
    // pourquoi 0.45f ? 
    // La valeur de 0.45 a été choisie après des tests pour offrir un bon équilibre entre accessibilité et défi.
    // Comment tu sais ?
    // En testant différentes valeurs, j'ai constaté que 0.45 permettait aux joueurs de gagner même 
    // si la couleur capturée n'était pas parfaitement identique à la couleur cible, ce qui est important 
    // pour compenser les variations d'éclairage et les différences de caméra.

    // --- VARIABLES INTERNES ---

    // private et public quel différent ? 
    // public : accessible depuis d'autres scripts et visible dans l'inspecteur de Unity (sauf si on met [HideInInspector])
    // private : accessible uniquement dans ce script et pas visible dans l'inspecteur (sauf si on met [SerializeField])
    // Ici, on utilise private pour les variables qui sont gérées automatiquement par le script et que 
    // le développeur ne doit pas toucher dans l'inspecteur, pour éviter les erreurs de configuration.

    // Variables pour gérer la caméra, la couleur cible, le temps et l'état du jeu
    private WebCamTexture webcamTexture; // Objet qui contient le flux vidéo en direct
    private Color targetColor;           // La couleur que le joueur doit trouver
    private float currentTime;           // Chronomètre actuel
    private bool isGameActive = false;   // État du jeu (en cours ou en pause)

    void Start() // Fonction appelée automatiquement par Unity au lancement du jeu
    {
        // 1. GESTION DES PERMISSIONS ANDROID
        // Vérifie si l'utilisateur a déjà autorisé l'utilisation de la caméra
#if UNITY_ANDROID // Cette directive de compilation signifie que ce code ne sera compilé que pour les plateformes Android
        // Dire à Unity lire uniquement ce code si l'on construit l'application Android
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera)) // Si l'utilisateur n'a pas encore donné son accord
        {
            // Si non, affiche la demande système "Autoriser l'application à utiliser la caméra ?"
            Permission.RequestUserPermission(Permission.Camera); // Affiche la demande de permission pour la caméra sur Android
        }
#endif // Pourquoi cette vérification ? 
        // Parce que sur Android, les applications doivent demander la permission d'accéder à certaines fonctionnalités (comme la caméra) pour des raisons de sécurité et de confidentialité.

        // 2. LANCEMENT DE LA CAMÉRA
        // On utilise une Coroutine pour ne pas bloquer le jeu pendant que la caméra s'allume
        StartCoroutine(InitCamera()); // Démarre la fonction InitCamera() qui s'occupera d'allumer la caméra et de préparer le jeu
    }

    IEnumerator InitCamera() // Fonction spéciale qui peut faire des pauses (yield) pour attendre que certaines actions soient terminées

    // Quoi yield ? Yield permet de faire une pause dans l'exécution de la fonction sans bloquer le reste du jeu. 
    // Par exemple, quand on allume la caméra, cela peut prendre un moment avant que le flux vidéo soit prêt. 
    // En utilisant yield, on peut attendre ce moment sans figer le jeu entier.
    {
        // Petite pause pour laisser le temps au matériel de s'initialiser
        yield return new WaitForSeconds(0.5f); // Attend une demi-seconde avant de continuer (pour éviter les problèmes de timing avec la caméra)

        // Pourquoi 0.5f ? Durée choisie pour donner suffisamment de temps à la caméra pour s'allumer sur la plupart des appareils.
        // Si vous trouvez que la caméra met plus de temps à s'allumer sur certains appareils, vous pouvez augmenter cette valeur (ex: 1f pour 1 seconde).
        // pourquoi on ne peut pas juste mettre 0.5 ? 0.5 est confus car il peut être interprété comme 0.5 secondes ou 0.5 minutes. En mettant 0.5f, on indique clairement que c'est une valeur flottante en secondes, ce qui est plus précis pour les développeurs qui liront le code.
        // Vérifie si l'appareil possède au moins une caméra
        if (WebCamTexture.devices.Length > 0) // Si au moins une caméra est détectée (sur PC, il peut n'y en avoir aucune)
        {
            // Par défaut, on prend la première caméra trouvée
            string cameraName = WebCamTexture.devices[0].name; // Nom de la première caméra détectée

            // Boucle pour chercher spécifiquement la caméra ARRIÈRE (pas le mode selfie)
            foreach (var device in WebCamTexture.devices) // Parcourt toutes les caméras disponibles
            {
                if (!device.isFrontFacing) // Si la caméra n'est pas orientée vers l'avant (selfie), c'est la caméra arrière
                {
                    cameraName = device.name; // On garde le nom de la caméra arrière pourquoi ?
                                              // Parce que sur certains appareils, la caméra par défaut peut être la caméra selfie, et pour ce jeu,
                                              // on veut utiliser la caméra arrière qui est généralement de meilleure qualité et plus adaptée pour capturer les couleurs de l'environnement.

                    break; // On a trouvé la caméra arrière, on arrête la boucle
                }
            }

            // Création de la texture vidéo (Résolution 1280x720)
            webcamTexture = new WebCamTexture(cameraName, 1280, 720); // On crée un flux vidéo à partir de la caméra sélectionnée avec une résolution HD (1280x720 pixels)

            if (cameraDisplay != null) // Vérifie que le RawImage pour afficher la caméra est bien assigné dans l'inspecteur
            {
                // On lie le flux de la caméra à notre RawImage dans l'UI
                cameraDisplay.texture = webcamTexture; // Affiche le flux vidéo de la caméra sur l'écran du jeu
                webcamTexture.Play(); // Allume la caméra

                // Correction de la rotation : sur mobile, l'image est souvent tournée à 90°
                cameraDisplay.rectTransform.localEulerAngles = new Vector3(0, 0, -webcamTexture.videoRotationAngle); // Ajuste la rotation de l'affichage pour que
                                                                                                                     // l'image soit droite, même si la caméra est physiquement tournée (ex: portrait vs paysage)
                                                                                                                     // Pourquoi ? Parce que sur les smartphones, 
                                                                                                                     // la caméra peut être orientée différemment selon la façon dont l'utilisateur tient son appareil.

                // Lancement de la première manche
                StartNewRound(); // Démarre la première manche du jeu en choisissant une couleur et en lançant le chrono
            }
        }
        else
        {
            // Message d'erreur si aucune caméra n'est branchée (ex: sur PC sans webcam)
            if (instructionText != null) instructionText.text = "Caméra non détectée !";
        }
    }

    void Update()
    {
        // Si le jeu est fini ou en pause, on ne fait rien
        if (!isGameActive) return;

        // --- GESTION DU CHRONOMÈTRE ---
        currentTime -= Time.deltaTime; // On soustrait le temps écoulé depuis la dernière image

        if (timerText != null)
            // Affiche le temps arrondi au chiffre supérieur (ex: 14.2s devient 15s)
            timerText.text = "Temps : " + Mathf.Max(0, Mathf.Ceil(currentTime)) + "s"; // Aussi on met max à 0 car on évite signe négatif en bas

        // Si le temps est écoulé
        if (currentTime <= 0)
        {
            EndGame(false); // Fin de partie : Perdu
        }

        // Vérifie à chaque image si la couleur visée est la bonne
        CheckColor();
    }

    void StartNewRound() // Fonction qui démarre une nouvelle manche en choisissant une nouvelle couleur à trouver et en réinitialisant le temps
                         // void c'est quoi ? void signifie que cette fonction ne retourne aucune valeur.
                         // pas compris ?
                         // Cela signifie que lorsque vous appelez StartNewRound(), vous ne pouvez pas attendre une réponse ou un résultat de cette fonction.
                         // C'est simplement une action qui se produit (choisir une couleur, réinitialiser le temps, etc.) sans donner de feedback direct à l'appelant.
                         // Donc ?
                         // Donc, StartNewRound() est une fonction qui effectue des actions pour préparer une nouvelle manche du jeu, mais elle ne retourne rien à celui qui l'appelle (comme Update ou EndGame).
                         // Quel est différent Update, etc ?
                         // Update est une fonction spéciale dans Unity qui est appelée automatiquement à chaque frame (image) du jeu.
                         // StartNewRound, en revanche, est une fonction que nous avons définie nous-mêmes pour gérer le début de chaque manche.
                         // Update est appelé en continu par Unity, tandis que StartNew
                         // Round est appelé manuellement à des moments spécifiques (au début du jeu et après chaque manche).
                         // En résumé, Update est une fonction de cycle de vie du jeu qui s'exécute constamment, tandis que StartNewRound est une fonction personnalisée que nous appelons pour configurer chaque nouvelle manche du jeu.
    {
        // Liste des couleurs possibles pour le jeu
        Color[] options = { Color.red, Color.green, Color.blue, Color.yellow };

        // Choix d'une couleur au hasard parmi les 4 options
        targetColor = options[Random.Range(0, options.Length)];// Pourquoi Random.Range(0, options.Length) ? 
                                                               // Parce que Random.Range avec des entiers est exclusif sur la borne supérieure,
                                                               // Pas compris ?
                                                               // options.Length retourne le nombre total d'éléments dans le tableau (ici 4), et Random.Range(0, 4) retournera un entier aléatoire entre 0 et 3, ce qui correspond aux indices valides du tableau options (0 pour rouge, 1 pour vert, 2 pour bleu, 3 pour jaune).

        // Met à jour l'aperçu visuel de la couleur à chercher
        if (colorPreview != null) colorPreview.color = targetColor; //  Affiche la couleur cible dans le petit carré de l'UI pour que le joueur sache ce qu'il doit trouver
                                                                    // ! : non et = : égal donc null : 
        if (instructionText != null) // Vérifie que le texte d'instruction est bien assigné dans l'inspecteur
        {
            instructionText.text = "Bob dit : Trouve cette couleur !"; // Affiche les instructions pour le joueur
            instructionText.color = Color.white; // Affiche les instructions en blanc pour que ce soit neutre et lisible, quel que soit le thème de couleur choisi
        }

        // Réinitialise le temps et active la logique de jeu
        currentTime = timeLimit; // Réinitialise le chronomètre à la limite de temps définie
        isGameActive = true; // Le jeu est maintenant actif, le chrono tourne et la caméra est analysée pour trouver la couleur cible
    }

    void CheckColor() // Fonction qui compare la couleur capturée par la caméra avec la couleur cible
    {
        // Sécurité : si la caméra ne fonctionne pas encore, on quitte la fonction
        if (webcamTexture == null || webcamTexture.width < 100) return; // Pourquoi cette vérification ? Parce que lorsque la caméra s'allume, il peut y avoir un court instant où la texture n'est pas encore prête (largeur < 100 pixels). 
                                                                        // En vérifiant cela, on évite les erreurs qui pourraient survenir si on essaie de lire les pixels avant que la caméra soit opérationnelle.

        // ANALYSE DU PIXEL CENTRAL
        // On récupère la couleur du pixel exactement au centre de l'image (X = largeur/2, Y = hauteur/2)
        Color captured = webcamTexture.GetPixel(webcamTexture.width / 2, webcamTexture.height / 2); // Pourquoi le pixel central ? 
                                                                                                    // Parce que c'est l'endroit où le joueur est censé viser la couleur à trouver. 
                                                                                                    // Cela rend le jeu plus intuitif, car le joueur peut simplement centrer la couleur dans son champ de vision pour tenter de la capturer.




        // CALCUL DE LA DIFFÉRENCE (Distance mathématique entre deux couleurs)
        // On additionne les différences de Rouge, de Vert et de Bleu
        float diff = Mathf.Abs(targetColor.r - captured.r) + // Différence de Rouge
                     Mathf.Abs(targetColor.g - captured.g) + // Différence de Vert
                     Mathf.Abs(targetColor.b - captured.b); // Différence de Bleu

        // Si la différence est inférieure à la tolérance, c'est gagné !
        if (diff < tolerance) // Pourquoi comparer la différence à une tolérance ? 
                              // Parce que les couleurs capturées par la caméra peuvent varier en fonction de l'éclairage, de la qualité de la caméra, etc. 
                              // En utilisant une tolérance, on permet au joueur de gagner même si la couleur n'est pas exactement identique, ce qui rend le jeu plus accessible et amusant.
        {
            EndGame(true); // Fin de partie : Gagné
        }
    }

    void EndGame(bool win) // Fonction qui gère la fin de la partie, avec un paramètre indiquant si le joueur a gagné ou perdu
    {
        isGameActive = false; // Arrête le chrono et l'analyse de couleur

        if (win) // Si le joueur a gagné
        {
            instructionText.text = "BRAVO !"; // Message de victoire rappel : instructionText est un TMP_Text, donc on peut changer son texte pour afficher un message de victoire ou de défaite.
            instructionText.color = Color.green; // Affiche un message de victoire en vert rappel : instructionText est un TMP_Text, donc on peut changer sa couleur pour renforcer le message de victoire ou de défaite.
            // Attend 2.5 secondes avant de relancer une nouvelle couleur
            Invoke("StartNewRound", 2.5f); // Pourquoi 2.5 secondes ? Pour donner au joueur un moment de satisfaction après une victoire, tout en maintenant un rythme de jeu fluide.
        }
        else // Si le joueur a perdu
        {
            instructionText.text = "TROP TARD !"; // Message de défaite rappel : instructionText est un TMP_Text, donc on peut changer son texte pour afficher un message de victoire ou de défaite.
            instructionText.color = Color.red;// Affiche un message de défaite en rouge rappel : instructionText est un TMP_Text, donc on peut changer sa couleur pour renforcer le message de victoire ou de défaite.
            // Attend 3 secondes avant de relancer une nouvelle couleur
            Invoke("StartNewRound", 3.0f); // Pourquoi 3 secondes ? Pour donner au joueur un moment pour se préparer à la prochaine manche
                                           // Retarder
                                           // après une défaite, tout en maintenant un rythme de jeu fluide.
                                           // Invoke c'est quoi ? Invoke est une fonction de Unity qui permet d'appeler une autre fonction après un certain délai.
                                           // "StartNewRound' quel référence ? C'est le nom de la fonction que nous voulons appeler après le délai.

        }
    }
}

