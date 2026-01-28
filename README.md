# 🛡️ RiskHunter VR - Formation Sécurité & Tri

**RiskHunter VR** est une simulation immersive en Réalité Virtuelle destinée à former les agents de sécurité et le personnel industriel. Le projet met l'accent sur la gestion du stress, le tri des déchets dangereux et l'identification des risques environnementaux.

---

## 🚀 Installation

1.  Cloner le dépôt :
    ```bash
    git clone [https://github.com/ismaelel/RiskHunterVR.git](https://github.com/ismaelel/RiskHunterVR.git)
    ```
2.  Ouvrir le projet avec **Unity Hub**.
3.  Lancer la scène `MenuPrincipal` situé dans `Assets/Scenes`.
4.  *Note : La base de données se créera automatiquement au premier lancement.*

---

## 🕹️ Contrôles

* **Déplacement :** Joystick Gauche (Teleportation).
* **Interaction :** Gâchette Latérale (Grip) pour attraper.
* **UI :** Index (Trigger) pour valider.

---
## 🎮 Fonctionnalités de Gameplay

### 1. Tri Sélectif & Physique Réaliste
Le joueur doit trier des déchets (Cartons, Produits inflammables) dans les bennes appropriées.
* **Mécanique de Fragilité (Physics-Based) :** Les objets possèdent un script `ObjetFragile` qui surveille leur vélocité (`Rigidbody.linearVelocity`) et la force des impacts (`Collision.relativeVelocity`).
* **Pénalités :** Si un objet fragile est secoué violemment ou tombe de haut, il se brise. Cela déclenche un feedback sonore et une **pénalité immédiate de points** (Score négatif possible).

### 2. Système de Score Dynamique
Le score n'est pas une simple addition, c'est un calcul d'efficacité :
> **Formule :** `(Objets Triés + Risques Identifiés + Bonus Temps) - Pénalités de Casse = SCORE FINAL`
* **Chronometre :** Le temps défile. À la fin du niveau, chaque seconde restante est convertie en points bonus pour récompenser la rapidité.
* **Feedback UI :** Le score s'affiche en temps réel. Il passe en **Rouge** si le joueur est en négatif (malus trop importants) et en **Blanc/Jaune** sinon.

### 3. Progression & Sauvegarde
* **Affichage dans le Menu :** Grâce à un système de requêtes SQL au chargement (`Awake`), chaque porte de niveau affiche dynamiquement le **Meilleur Score Personnel** du joueur connecté sur un panneau 3D.
* **Classement :** Un Leaderboard (Top 3) est généré à la fin de chaque session.

---

## 🛠️ Architecture Technique

### Stack Technologique
* **Moteur :** Unity 2022 LTS
* **Langage :** C#
* **VR Framework :** XR Interaction Toolkit
* **Données :** SQLite (`Mono.Data.Sqlite`)
* **UI :** TextMeshPro

### Choix d'Architecture : Pourquoi SQLite et pas de Web Service ?

Pour la gestion des données, nous avons opté pour une architecture **locale (Standalone)** utilisant SQLite, plutôt que de développer une API REST (Web Service) connectée à un serveur distant.



**Justification de ce choix technique :**

1.  **Philosophie "Offline-First" :** Le dispositif est conçu pour être utilisé dans des zones industrielles, des sous-sols ou des salles de formation où la connexion Wi-Fi est instable ou inexistante. SQLite garantit un fonctionnement 100% autonome.
2.  **Performance & Latence :** En VR, l'immersion est critique. L'accès direct au fichier `.db` local élimine la latence réseau (Ping) qu'imposerait un appel HTTP vers une API externe. L'affichage des scores sur les portes est instantané.
3.  **Simplicité de déploiement :** Pas de maintenance serveur. La base de données est un fichier unique stocké dans le `Application.persistentDataPath` du casque.

### Solutions Techniques Spécifiques

* **UI "Always On Top" (Shader Overlay) :** Problème rencontré : Les interfaces de chargement ou de score passaient parfois à travers les murs ou étaient cachées par la géométrie 3D.  
    Solution : Création d'un **Shader personnalisé** utilisant la propriété `ZTest Always`. Cela force le rendu de l'interface par-dessus tous les autres objets de la scène, simulant un affichage HUD (Head-Up Display).

---

## 📂 Structure du Code (Scripts Clés)

* **`ManagerNiveauTri.cs` :** Orchestre la boucle de jeu. Il gère le timer, réceptionne les événements de casse (Pénalités), calcule le score final et déclenche la sauvegarde.
* **`DatabaseManager.cs` :** Couche d'abstraction SQL. Gère la connexion, la création des tables (`IF NOT EXISTS`) et les méthodes CRUD (Create, Read, Update, Delete) pour les joueurs et les scores.
* **`ObjetFragile.cs` :** Script attaché aux prefabs interactifs. Il calcule la magnitude des vecteurs de force pour déterminer si l'objet doit casser.
* **`ScorePorte.cs` :** Script UI autonome qui interroge la BDD pour mettre à jour l'environnement du menu principal selon la progression du joueur.



## 👨‍💻 Auteur

**[EL KASBAOUI ISMAËL]**
Projet Étudiant / Formation VR