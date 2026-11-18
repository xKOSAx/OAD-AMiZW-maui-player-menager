using MauiApp3.Commands;
using MauiApp3.Model;
using MauiApp3.Queries;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MauiApp3.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly GetPlayerByIdQueryHandler _getPlayerByIdQueryHandler;
        private readonly GetPlayersQueryHandler _getPlayersQueryHandler;
        private readonly AddPlayerCommandHandler _addPlayerCommandHandler;
        private readonly DeletePlayerCommandHandler _deletePlayerCommandHandler;
        private readonly EditPlayerCommandHandler _editPlayerCommandHandler;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Player> Players { get; } = new();

        private string _playerName = "";
        public string PlayerName
        {
            get => _playerName;
            set
            {
                _playerName = value;
                OnPropertyChanged();
            }
        }

        private string _playerLevel = "";
        public string PlayerLevel
        {
            get => _playerLevel;
            set
            {
                _playerLevel = value;
                OnPropertyChanged();
            }
        }

        private string _playerId = "";
        public string PlayerId
        {
            get => _playerId;
            set
            {
                _playerId = value;
                OnPropertyChanged();
            }
        }

        private string _deletePlayerId = "";
        public string DeletePlayerId
        {
            get => _deletePlayerId;
            set
            {
                _deletePlayerId = value;
                OnPropertyChanged();
            }
        }

        private string _editPlayerId = "";
        public string EditPlayerId
        {
            get => _editPlayerId;
            set
            {
                _editPlayerId = value;
                OnPropertyChanged();
            }
        }

        private string _editPlayerName = "";
        public string EditPlayerName
        {
            get => _editPlayerName;
            set
            {
                _editPlayerName = value;
                OnPropertyChanged();
            }
        }

        private string _editPlayerLevel = "";
        public string EditPlayerLevel
        {
            get => _editPlayerLevel;
            set
            {
                _editPlayerLevel = value;
                OnPropertyChanged();
            }
        }

        private string _searchResult = "";
        public string SearchResult
        {
            get => _searchResult;
            set
            {
                _searchResult = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage = "";
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddPlayerCommandButton { get; }
        public ICommand FindPlayerCommandButton { get; }
        public ICommand DeletePlayerCommandButton { get; }
        public ICommand EditPlayerCommandButton { get; }

        public MainViewModel(
            GetPlayerByIdQueryHandler getPlayerByIdQueryHandler,
            GetPlayersQueryHandler getPlayersQueryHandler,
            AddPlayerCommandHandler addPlayerCommandHandler,
            DeletePlayerCommandHandler deletePlayerCommandHandler,
            EditPlayerCommandHandler editPlayerCommandHandler)
        {
            _getPlayerByIdQueryHandler = getPlayerByIdQueryHandler;
            _getPlayersQueryHandler = getPlayersQueryHandler;
            _addPlayerCommandHandler = addPlayerCommandHandler;
            _deletePlayerCommandHandler = deletePlayerCommandHandler;
            _editPlayerCommandHandler = editPlayerCommandHandler;

            AddPlayerCommandButton = new Command(AddPlayer);
            FindPlayerCommandButton = new Command(FindPlayer);
            DeletePlayerCommandButton = new Command(DeletePlayer);
            EditPlayerCommandButton = new Command(EditPlayer);

            LoadPlayers();
        }

        private void AddPlayer()
        {
            if (string.IsNullOrWhiteSpace(PlayerName))
            {
                StatusMessage = "Podaj nazwę gracza.";
                return;
            }

            if (!int.TryParse(PlayerLevel, out int level) || level < 0)
            {
                StatusMessage = "Podaj poprawny poziom gracza.";
                return;
            }

            AddPlayerCommand command =
                new AddPlayerCommand(PlayerName.Trim(), level);

            _addPlayerCommandHandler.Handle(command);

            LoadPlayers();

            PlayerName = "";
            PlayerLevel = "";

            StatusMessage = "Gracz został dodany.";
        }

        private void FindPlayer()
        {
            if (!int.TryParse(PlayerId, out int id) || id <= 0)
            {
                SearchResult = "Niepoprawne ID.";
                return;
            }

            GetPlayerByIdQuery query =
                new GetPlayerByIdQuery(id);

            Player? player =
                _getPlayerByIdQueryHandler.Handle(query);

            if (player == null)
            {
                SearchResult = "Nie znaleziono gracza.";
                return;
            }

            SearchResult =
                $"{player.Name} - poziom {player.Level}";
        }

        private void DeletePlayer()
        {
            if (!int.TryParse(DeletePlayerId, out int id) || id <= 0)
            {
                StatusMessage = "Podaj poprawne ID gracza.";
                return;
            }

            DeletePlayerCommand command =
                new DeletePlayerCommand(id);

            bool deleted =
                _deletePlayerCommandHandler.Handle(command);

            if (!deleted)
            {
                StatusMessage = "Nie znaleziono gracza o takim ID.";
                return;
            }

            LoadPlayers();

            DeletePlayerId = "";

            StatusMessage = "Gracz został usunięty.";
        }

        private void EditPlayer()
        {
            if (!int.TryParse(EditPlayerId, out int id) || id <= 0)
            {
                StatusMessage = "Podaj poprawne ID gracza.";
                return;
            }

            if (string.IsNullOrWhiteSpace(EditPlayerName))
            {
                StatusMessage = "Podaj nową nazwę gracza.";
                return;
            }

            if (!int.TryParse(EditPlayerLevel, out int level) || level < 0)
            {
                StatusMessage = "Podaj poprawny poziom.";
                return;
            }

            EditPlayerCommand command =
                new EditPlayerCommand(
                    id,
                    EditPlayerName.Trim(),
                    level
                );

            bool edited =
                _editPlayerCommandHandler.Handle(command);

            if (!edited)
            {
                StatusMessage = "Nie znaleziono gracza o takim ID.";
                return;
            }

            LoadPlayers();

            EditPlayerId = "";
            EditPlayerName = "";
            EditPlayerLevel = "";

            StatusMessage = "Dane gracza zostały zmienione.";
        }

        private void LoadPlayers()
        {
            GetPlayersQuery query = new GetPlayersQuery();

            List<Player> players =
                _getPlayersQueryHandler.Handle(query);

            Players.Clear();

            foreach (Player player in players)
            {
                Players.Add(player);
            }
        }

        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}