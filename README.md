# 🎈 Le Pendu de Bob - Mode Duel 🎈

Bienvenue dans le projet **PenduBob**, un jeu de pendu classique revisité pour deux joueurs avec des mécaniques de bonus uniques !

## 📝 Description du Projet
Ce projet est un jeu de lettres développé sous **Unity 2026**. Deux joueurs s'affrontent sur le même écran pour deviner un mot choisi aléatoirement.

## 🚀 Fonctionnalités clés
* **Mode Duel :** Chaque joueur possède son propre compteur de **5 vies**.
* **La Règle de Bob :** Si un joueur tente de deviner le mot complet et échoue, l'autre joueur reçoit **2 essais bonus** (Bob le protège et il ne perd pas de vies sur ses prochaines erreurs).
* **Sécurité Anti-Doublon :** Le jeu détecte si une lettre a déjà été jouée et empêche le joueur de perdre une vie par inattention.
* **Visuel clair :** Les lettres fausses s'affichent barrées en bas de l'écran.
* **Multi-plateforme :** Conçu pour PC et Android.

## 🛠 Structure du projet (voir image)
* **Assets/** : Contient tous les scripts C#, les scènes et les éléments UI.
* **Packages/** : Dépendances du projet (inclut TextMeshPro pour les textes).
* **ProjectSettings/** : Configuration spécifique à Unity (entrées, tags, couches).
* **AAA Builds/Android/** : Emplacement des fichiers exportés pour téléphones Android.
* **A Double Tours.slnx** : Fichier de solution pour le code dans Visual Studio.

## 🎮 Comment jouer ?
1.  Lancez la scène `PenduBob.unity`.
2.  Le Joueur 1 commence par proposer une lettre ou un mot complet.
3.  Si la lettre est bonne, il continue de jouer.
4.  Si la lettre est fausse, son compteur de vies baisse et c'est au tour du Joueur 2.
5.  Le premier joueur qui tombe à 0 vie a perdu !

## 🏗 Installation
1.  Cloner ce dépôt GitHub.
2.  Ouvrir le dossier avec **Unity Hub**.
3.  S'assurer que **TextMeshPro** est bien importé si Unity le demande au premier lancement.
