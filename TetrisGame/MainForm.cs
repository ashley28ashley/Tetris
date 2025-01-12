using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TetrisGame
{
    public partial class MainForm : Form
    {
        private const int CELL_SIZE = 30;
        private GameState gameState = null!;
        private System.Windows.Forms.Timer gameTimer = null!;
        private Panel gamePanel = null!;
        private Panel nextPiecePanel = null!;
        private Label scoreLabel = null!;
        private int gameSpeed = 1000;

        public MainForm()
        {
            InitializeComponent();
            SetupMainMenu();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Name = "MainForm";
            this.Text = "Tetris";
            this.ClientSize = new Size(600, 800);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;
            this.ResumeLayout(false);
        }

        private void SetupMainMenu()
        {
            Controls.Clear();

            var menuPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 100, 0, 50) // Ajoute de l'espace en haut et en bas
            };

            menuPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 40)); // Pour le titre
            menuPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20)); // Pour le bouton "JOUER"
            menuPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20)); // Pour le bouton "OPTIONS"
            menuPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

            var titleLabel = new Label
            {
                Text = "TETRIS ",
                Font = new Font("Arial", 48, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Margin = new Padding(10, 10, 20, 20) // Espacement en bas
            };

            // Bouton "JOUER" (grande taille, centré)
            var playButton = new Button
            {
                Text = "JOUER",
                Font = new Font("Arial", 28, FontStyle.Bold),
                Size = new Size(300, 100),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.None
            };
            playButton.FlatAppearance.BorderSize = 0;
            playButton.Margin = new Padding((Width - 300) / 2, 10, (Width - 300) / 2, 10); // Centré horizontalement
            playButton.Click += (s, e) => StartGame();

            // Bouton "OPTIONS" (plus petit, positionné sous "JOUER")
            var optionsButton = new Button
            {
                Text = "OPTIONS",
                Font = new Font("Arial", 18, FontStyle.Bold),
                Size = new Size(200, 50),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.None
            };
            optionsButton.FlatAppearance.BorderSize = 0;
            optionsButton.Margin = new Padding((Width - 200) / 2, 10, (Width - 200) / 2, 10); // Centré horizontalement
            optionsButton.Click += (s, e) => ShowOptions();

            // Bouton "QUITTER" (plus compact, en bas)
            var quitButton = new Button
            {
                Text = "QUITTER",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Size = new Size(150, 40),
                BackColor = Color.FromArgb(100, 30, 30),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.None
            };
            quitButton.FlatAppearance.BorderSize = 0;
            quitButton.Margin = new Padding((Width - 150) / 2, 10, (Width - 150) / 2, 10); // Centré horizontalement
            quitButton.Click += (s, e) => Application.Exit();

            // Ajout des contrôles au menu
            menuPanel.Controls.Add(titleLabel);
            menuPanel.Controls.Add(playButton);
            menuPanel.Controls.Add(optionsButton);
            menuPanel.Controls.Add(quitButton);

            Controls.Add(menuPanel);
        }

        private Button CreateMenuButton(string text)
        {
            return new Button
            {
                Text = text,
                Font = new Font("Arial", 20, FontStyle.Bold),
                Dock = DockStyle.Fill,
                Margin = new Padding(100, 10, 100, 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White
            };
        }

        private void StartGame()
        {
            Controls.Clear();
            ClientSize = new Size(600, 800);

            gameState = new GameState();
            gameState.ScoreChanged += OnScoreChanged;
            gameState.GameOver += OnGameOver;
            gameState.LineCleared += OnLineCleared;

            SetupGameControls();
            InitializeGameTimer();

            this.KeyPreview = true;
            this.KeyDown += OnKeyDown; 
        }

        private void SetupGameControls()
        {
            gamePanel = new Panel
            {
                Size = new Size(CELL_SIZE * 10, CELL_SIZE * 20),
                Location = new Point(50, 50),
                BorderStyle = BorderStyle.Fixed3D,
                BackColor = Color.Black
            };
            gamePanel.Paint += OnGamePanelPaint;

            nextPiecePanel = new Panel
            {
                Size = new Size(150, 150),
                Location = new Point(400, 100),
                BorderStyle = BorderStyle.Fixed3D,
                BackColor = Color.FromArgb(40, 40, 40)
            };
            nextPiecePanel.Paint += OnNextPiecePaint;

            scoreLabel = new Label
            {
                Location = new Point(400, 50),
                Size = new Size(150, 30),
                Font = new Font("Arial", 16, FontStyle.Bold),
                Text = "Score: 0",
                ForeColor = Color.White
            };

            var pauseButton = new Button
            {
                Text = "PAUSE",
                Location = new Point(400, 300),
                Size = new Size(150, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            pauseButton.Click += (s, e) => PauseGame();

            var menuButton = new Button
            {
                Text = "MENU",
                Location = new Point(400, 350),
                Size = new Size(150, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            menuButton.Click += (s, e) => ReturnToMenu();

            Controls.AddRange(new Control[] {
                gamePanel,
                nextPiecePanel,
                scoreLabel,
                pauseButton,
                menuButton
            });
        }

        private void ReturnToMenu()
        {
            if (gameTimer != null)
            {
                gameTimer.Stop();
                gameTimer = null!;
            }
            SetupMainMenu();
        }

        private void ShowOptions()
        {
            using (var optionsForm = new OptionsForm(gameSpeed))
            {
                optionsForm.StartPosition = FormStartPosition.CenterParent;
                if (optionsForm.ShowDialog() == DialogResult.OK)
                {
                    gameSpeed = optionsForm.GameSpeed;
                    if (gameTimer != null)
                    {
                        gameTimer.Interval = gameSpeed;
                    }
                }
            }
        }

        private void InitializeGameTimer()
        {
            gameTimer = new System.Windows.Forms.Timer
            {
                Interval = gameSpeed
            };
            gameTimer.Tick += GameTick;
            gameTimer.Start();
        }

        private void GameTick(object? sender, EventArgs e)
        {
            if (gameState.MoveDown())
            {
                gamePanel.Invalidate();
            }
            else
            {
                gameState.LockPiece();
                gameState.ClearLines();

                if (gameState.IsGameOver)
                {
                    gameTimer.Stop();
                    OnGameOver(this, EventArgs.Empty);
                }
            }
        }

        private void OnGamePanelPaint(object? sender, PaintEventArgs e)
        {
            gameState.Draw(e.Graphics, CELL_SIZE);
        }

        private void OnNextPiecePaint(object? sender, PaintEventArgs e)
        {
            gameState.DrawNextPiece(e.Graphics, 25, new Point(25, 25));
        }

        private void OnScoreChanged(object? sender, int newScore)
        {
            scoreLabel.Text = $"Score: {newScore}";
            if (gameTimer != null)
            {
                int newInterval = Math.Max(100, gameSpeed - (newScore / 1000) * 100);
                gameTimer.Interval = newInterval;
            }
        }

        private void OnLineCleared(object? sender, EventArgs e)
        {
            
        }

        private void PauseGame()
        {
            if (gameTimer != null && gameTimer.Enabled)
            {
                gameTimer.Stop();
                MessageBox.Show("Jeu en pause\nAppuyez sur OK pour continuer",
                              "Pause",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
                gameTimer.Start();
            }
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (gameState == null || gameState.IsGameOver) return;

            bool moved = false;
            switch (e.KeyCode)
            {
                case Keys.Left:
                    moved = gameState.MoveLeft();
                    break;
                case Keys.Right:
                    moved = gameState.MoveRight();
                    break;
                case Keys.Up:
                    gameState.Rotate();
                    moved = true;
                    break;
                case Keys.Down:
                    moved = gameState.MoveDown();
                    break;
                case Keys.Space:
                    gameState.Drop();
                    moved = true;
                    break;
            }

            if (moved)
            {
                gamePanel.Invalidate();
            }
        }

        private void OnGameOver(object? sender, EventArgs e)
        {
            gameTimer?.Stop();
            MessageBox.Show($"Game Over!\nScore final: {gameState.Score}",
                          "Game Over",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Information);
            SetupMainMenu();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }
    }


public class OptionsForm : Form
    {
        private NumericUpDown speedInput; // Contrôle pour ajuster la vitesse du jeu
        private GameSettings settings; // Instance des paramètres du jeu

        [Serializable]
        public class GameSettings
        {
            public int GameSpeed { get; set; } // Propriété pour la vitesse du jeu
        }

        public OptionsForm(int initialSpeed)
        {
            // Initialisation des paramètres avec la vitesse initiale
            settings = new GameSettings { GameSpeed = initialSpeed };

            // Méthode d'initialisation des composants
            InitializeComponents(initialSpeed);
        }

        // Propriété exposant la vitesse du jeu
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public int GameSpeed
        {
            get { return settings.GameSpeed; }
            private set
            {
                settings.GameSpeed = value; // Mise à jour de la vitesse
            }
        }

        private void InitializeComponents(int initialSpeed)
        {
            // Configuration de la fenêtre
            this.Text = "Options";
            this.Width = 300;
            this.Height = 200;

            // Création du contrôle NumericUpDown pour la vitesse
            speedInput = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 100,
                Value = initialSpeed,
                Location = new System.Drawing.Point(50, 50),
                Width = 100
            };
            speedInput.ValueChanged += (sender, args) =>
            {
                GameSpeed = (int)speedInput.Value; // Mise à jour de la vitesse dans les paramètres
            };

            // Ajout du contrôle au formulaire
            this.Controls.Add(speedInput);

            // Ajout d'un bouton pour fermer le formulaire
            Button saveButton = new Button
            {
                Text = "Enregistrer",
                Location = new System.Drawing.Point(50, 100),
                Width = 100
            };
            saveButton.Click += (sender, args) => this.Close();

            this.Controls.Add(saveButton);
        }
    }
}