# 🛡️ RiskHunter VR - Formation Sécurité Incendie & Tri

**RiskHunter VR** est une simulation en Réalité Virtuelle (VR) destinée à former les agents de sécurité et les employés d'usine. Le but est d'identifier des risques environnementaux et d'effectuer un tri sélectif de déchets dangereux sous la pression du temps.

---

## 🎮 Fonctionnalités Principales

### 1. Gameplay Immersif
* **Tri Sélectif Physique :** Manipulation d'objets (Cartons, Bidons) avec physique réaliste.
* **Gestion de la Fragilité :** Système de détection de chocs. Si un objet "Fragile" est secoué trop fort ou tombe de haut, il casse (Feedback sonore + Pénalité de score).
* **Chasse aux Risques :** Identification visuelle de dangers dans l'environnement (Flaques, Câbles dénudés, etc.).

### 2. Système de Score Avancé
Le score n'est pas linéaire, il récompense l'efficacité et la prudence :
* **Points de base :** +100 pts par objet trié, +50 pts par risque détecté.
* **Bonus de Temps :** Le temps restant au chrono est converti en points bonus à la fin.
* **Pénalités (Malus) :** Des points sont retirés si le joueur casse du matériel.
* **Formule :** `(Tri + Risques + Bonus Temps) - Pénalités = Score Final`.

### 3. Persistance des Données (SQL)
Le jeu intègre une base de données locale complète (**SQLite**) :
* **Sauvegarde automatique :** Les scores sont enregistrés localement.
* **Classement (Leaderboard) :** Affichage du TOP 3 des meilleurs agents sur l'écran de fin.
* **Progression Visuelle :** Dans le menu, chaque porte de niveau affiche dynamiquement le **Meilleur Score Personnel** du joueur grâce à des requêtes SQL personnalisées.

### 4. Interface Utilisateur (UI) Technique
* **Shader "Always Visible" :** Développement d'un shader personnalisé (`ZTest Always`) pour que les interfaces critiques (Chargement, Alertes) soient visibles à travers les murs et les objets 3D.
* **Feedback Visuel :** Textes flottants et indicateurs de couleur (Rouge = Pénalité/Urgence, Jaune = Or, Vert = Validation).

---

## 🛠️ Stack Technique

* **Moteur :** Unity 2022 (LTS)
* **Langage :** C#
* **VR Framework :** XR Interaction Toolkit
* **Base de Données :** SQLite (`Mono.Data.Sqlite` & `System.Data`)
* **UI :** TextMeshPro (TMP)

---

## 📂 Architecture du Code

Voici les scripts clés qui pilotent la simulation :

* **`ManagerNiveauTri.cs` :** Le "Cerveau" du niveau.
    * Gère la boucle de jeu (Start -> Play -> End).
    * Calcule le score en temps réel (incluant les malus et le timer).
    * Communique avec la BDD pour sauvegarder le résultat.
* **`DatabaseManager.cs` :** Gestionnaire SQL.
    * Connexion à la BDD `RiskhunterSave.db`.
    * Exécution des requêtes (INSERT, SELECT, UPDATE).
    * Gestion des profils joueurs.
* **`ObjetFragile.cs` :** Script de physique.
    * Surveille la vélocité (`Rigidbody.velocity`) et les collisions.
    * Déclenche les pénalités si les seuils de tolérance sont dépassés.
* **`ScorePorte.cs` :**
    * Script UI placé dans le Menu Principal.
    * Récupère le record du joueur connecté dès le chargement (`Awake`) pour l'afficher sur la porte.

---

## 🚀 Installation & Lancement

1.  Cloner ce dépôt :
    ```bash
    git clone [https://github.com/VOTRE_NOM/RiskHunterVR.git](https://github.com/VOTRE_NOM/RiskHunterVR.git)
    ```
2.  Ouvrir le projet avec **Unity Hub** (Version recommandée : 2022.x).
3.  Ouvrir la scène de démarrage : `Assets/Scenes/MenuPrincipal.unity`.
4.  Lancer le mode **Play** (avec un casque VR connecté ou en mode simulation).

---

## 🕹️ Contrôles VR

* **Grip (Gâchette latérale) :** Attraper / Relâcher les objets.
* **Trigger (Gâchette index) :** Valider les menus / Interagir.
* **Thumbstick (Joystick) :** Se déplacer (Téléportation ou Continu).

---

## 👨‍💻 Auteur

**[TON NOM / PRÉNOM]**
*Projet de fin de formation / module Unity.*
