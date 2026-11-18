using MauiApp3.Data;
using MauiApp3.Model;

namespace MauiApp3.Commands
{
    public class AddPlayerCommandHandler
    {
        private readonly PlayerRepository _playerRepository;

        public AddPlayerCommandHandler(
            PlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public void Handle(AddPlayerCommand command)
        {
            int newId = _playerRepository.Players.Count == 0
                ? 1
                : _playerRepository.Players.Max(p => p.Id) + 1;

            Player player = new Player
            {
                Id = newId,
                Name = command.Name,
                Level = command.Level
            };

            _playerRepository.Players.Add(player);
        }
    }
}