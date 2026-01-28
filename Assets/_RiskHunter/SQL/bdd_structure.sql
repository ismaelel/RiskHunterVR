-- Structure de la base de données SQLite pour Space Operators

-- Table des Joueurs (Profils)
CREATE TABLE IF NOT EXISTS Joueur (
                                      id INTEGER PRIMARY KEY,
                                      nom TEXT NOT NULL,
                                      niveau INT DEFAULT 1,
                                      score INT DEFAULT 0
);

-- Table des Scores (Highscores par niveau)
CREATE TABLE IF NOT EXISTS Scores (
                                      player_id INTEGER,
                                      level_id INTEGER,
                                      high_score INTEGER,
                                      PRIMARY KEY (player_id, level_id),
    FOREIGN KEY (player_id) REFERENCES Joueur(id)
    );

