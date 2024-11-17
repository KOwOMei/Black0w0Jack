using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Black0w0Jack
{
    public class Deck
    {
        private List<Card> cards;
        private Random random;

        public Deck()
        {
            cards = new List<Card>();
            random = new Random();
            InitializeDeck();
        }

        private void InitializeDeck()
        {
            string[] suits = { "Червей", "Бубнов", "Трефов", "Пиков" };
            string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Валет", "Дама", "Король", "Туз" };
            int[] points = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 10, 10, 11 };

            foreach (var suit in suits)
            {
                for (int i = 0; i < ranks.Length; i++)
                {
                    string rank = ranks[i];
                    int point = points[i];
                    bool isAce = (rank == "Туз");
                    string imagePath = $"{rank}_{suit}.png";
                    cards.Add(new Card($"{rank} {suit}", point, isAce, false, imagePath));
                }
            }
        }

        public void Shuffle()
        {
            cards = cards.OrderBy(c => random.Next()).ToList();
        }

        public Card DrawCard()
        {
            if (cards.Count == 0)
            {
                throw new InvalidOperationException("Чё, карты кончились? КАК БЛЯТЬ?!?!?!?!.");
            }

            Card card = cards[0];
            cards.RemoveAt(0);
            return card;
        }
    }
    public class Card
    {

        public string Name { get; set; }
        public int Points { get; set; }
        public Image Image { get; set; }
        public bool IsAce { get; set; }
        public bool IsHidden { get; set; }
        public ImageSource HiddenImage { get; set; }

        public Card(string name, int points, bool isace, bool ishidden, string imagePath, ImageSource hiddenImage = null)
        {
            Name = name;
            Points = points;
            IsAce = isace;
            IsHidden = ishidden;
            string fullPath;
            if (!IsHidden)  
            {
                fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\CardsImages", imagePath ?? "black_joker.png");
                HiddenImage = null;
            }
            else
            {
                fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\CardsImages", "Зад.png");
                HiddenImage = hiddenImage;
            }
            Image = new Image
            {
                Source = new BitmapImage(new Uri(fullPath, UriKind.Absolute)),
                Width = 125,
                Height = 182
            };
        }
    }

    public partial class MainWindow : Window
    {
        private Deck deck;
        private ObservableCollection<Card> playerCards;
        private ObservableCollection<Card> dealerCards;

        public int playerScore;
        public int dealerScore;

        public MainWindow()
        {
            InitializeComponent();
            playerCards = new ObservableCollection<Card>();
            dealerCards = new ObservableCollection<Card>();

            playerScore = 0;
            dealerScore = 0;

            PlayerCardsPanel.ItemsSource = playerCards;
            DealerCardsPanel.ItemsSource = dealerCards;
            StartGame();

        }

        private void StartGame()
        {
            deck = new Deck();
            deck.Shuffle();
            playerCards.Add(deck.DrawCard());
            playerCards.Add(deck.DrawCard());
            dealerCards.Add(deck.DrawCard());
            Card hiddenCard = deck.DrawCard();
            dealerCards.Add(new Card(hiddenCard.Name, hiddenCard.Points, hiddenCard.IsAce, true, "pass", hiddenCard.Image.Source)); // Скрытая карта

            PlayerScore.Text = "Счет: " + CalculatePoints(playerCards);
            DealerScore.Text = "Счет: " + CalculatePoints(dealerCards);

            CheckForBlackJack(playerCards, int.Parse(CalculatePoints(playerCards)));
            CheckForBlackJack(dealerCards, int.Parse(CalculatePoints(dealerCards)));
        }

        private void RestartGame()
        {
            playerCards.Clear();
            dealerCards.Clear();
            StartGame();
        }

        private void CheckForBlackJack(ObservableCollection<Card> cards, int points)
        {
            foreach (var card in cards)
            {
                if (card.IsAce && points == 21)
                {
                    MessageBox.Show("BlackJack! Поздравляю! Вы выиграли!");
                    RestartGame();
                }
            }
        }

        private string CalculatePoints(ObservableCollection<Card> cards)
        {
            int points = 0;
            int aceCount = 0;
            int hiddenCardIndex = 0;


            // Делаем подсчет очков по картам
            foreach (var card in cards)
            {
                if (card.IsHidden)
                {
                    hiddenCardIndex++;
                }
                else
                {
                    points += card.Points;
                    if (card.IsAce)
                    {
                        aceCount++;
                    }
                }
                
            }

            // Если сумма больше 21 и есть тузы, уменьшаем их ценность до 1
            while (points > 21 && aceCount > 0)
            {
                points -= 10;
                aceCount--;
            }

            if (hiddenCardIndex > 0)
            {
                return points.ToString() + " + ?";
            }
            else
            {
                return points.ToString();
            }
        }

        private void TakeButton_Click(object sender, RoutedEventArgs e)
        {
            playerCards.Add(deck.DrawCard());

            int playerScore = int.Parse(CalculatePoints(playerCards));
            PlayerScore.Text = "Счет: " + playerScore.ToString();

            if (playerScore > 21)
            {
                MessageBox.Show("Игрок проиграл! Сумма очков больше 21.");
                RestartGame();
            }
        }

        private void StopTakeButton_Click(object sender, RoutedEventArgs e)
        {
            dealerCards[1].IsHidden = false;
            dealerCards[1].Image.Source = dealerCards[1].HiddenImage;     

            int playerPoints = int.Parse(CalculatePoints(playerCards));
            int dealerPoints = int.Parse(CalculatePoints(dealerCards));
            
            DealerScore.Text = "Счет: " + dealerPoints.ToString();

            while (dealerPoints < 17 || (dealerPoints < playerPoints && dealerPoints < 21))
            {
                dealerCards.Add(deck.DrawCard());
                dealerPoints = int.Parse(CalculatePoints(dealerCards));
                DealerScore.Text = "Счет: " + dealerPoints.ToString();
            }

            if (dealerPoints > 21)
            {
                MessageBox.Show("Дилер проиграл! Ценность его карт превысила 21 очко.");
                playerScore++;
            }
            else if (dealerPoints > playerPoints)
            {
                MessageBox.Show("Дилер выиграл! Ценность его карт выше, чем у Игрока.");
                dealerScore++;
            }
            else if (dealerPoints == playerPoints)
            {
                MessageBox.Show("Ничья! Ценность карт у Дилера и Игрока равны!");
            }
            else
            {
                MessageBox.Show("Игрок выиграл! Ценность его карт выше, чем у Дилера.");
                playerScore++;
            }
            MessageBox.Show($"Счет: Дилер - {dealerScore}, Игрок - {playerScore}");
            RestartGame();
        }
    }
}
