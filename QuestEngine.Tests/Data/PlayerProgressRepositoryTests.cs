using System;
using System.Threading.Tasks;
using FluentAssertions;
using QuestEngine.Configuration;
using QuestEngine.Data;
using QuestEngine.Data.Models;
using QuestEngine.Data.Repositories;
using Xunit;

namespace QuestEngine.Tests.Data
{
    public class PlayerProgressRepositoryTests : IDisposable
    {
        private readonly QuestEngineContextFactory _contextFactory;
        private readonly QuestEngineContext _context;

        public PlayerProgressRepositoryTests()
        {
            _contextFactory = new QuestEngineContextFactory(
                new ContextConfiguration
                {
                    InMemoryDb = true
                });

            _context = _contextFactory.Create();

            _context.PlayerProgresses.AddRange(new[]
            {
                new PlayerProgress
                {
                    QuestId = 1,
                    PlayerId = "player1",
                    QuestPointsEarned = 1,
                },
                new PlayerProgress
                {
                    QuestId = 2,
                    PlayerId = "player1",
                    QuestPointsEarned = 2
                },
                new PlayerProgress
                {
                    QuestId = 1,
                    PlayerId = "player2",
                    QuestPointsEarned = 3
                }
            });

            _context.SaveChanges();
        }

        [Fact]
        public async Task GetPlayerProgress_WithExistingPlayerProgress_GetCorrectOne()
        {
            var repository = new PlayerProgressRepository(_context);
            var playerProgress = await repository.GetPlayerProgress("player1", 1);

            playerProgress.Should().NotBeNull();
            playerProgress.PlayerId.Should().Be("player1");
            playerProgress.QuestId.Should().Be(1);
            playerProgress.QuestPointsEarned.Should().Be(1);
        }

        [Fact]
        public async Task GetPlayerProgress_WithNoExistingPlayerProgress_GetEmptyNewOne()
        {
            var repository = new PlayerProgressRepository(_context);
            var playerProgress = await repository.GetPlayerProgress("player2", 2);

            playerProgress.Should().NotBeNull();
            playerProgress.PlayerId.Should().Be("player2");
            playerProgress.QuestId.Should().Be(2);
            playerProgress.QuestPointsEarned.Should().Be(0);
        }

        [Fact]
        public async Task GetPlayerProgress_WithEmptyContext_GetEmptyNewOne()
        {
            _context.PlayerProgresses.RemoveRange(_context.PlayerProgresses);
            _context.SaveChanges();

            var repository = new PlayerProgressRepository(_context);
            var playerProgress = await repository.GetPlayerProgress("player1", 1);

            playerProgress.Should().NotBeNull();
            playerProgress.PlayerId.Should().Be("player1");
            playerProgress.QuestId.Should().Be(1);
            playerProgress.QuestPointsEarned.Should().Be(0);
        }

        [Fact]
        public async Task SavePlayerProgress_UpdateExistingPlayerProgress()
        {
            var playerProgress = _context.PlayerProgresses.Find("player1", 1);
            
            playerProgress.Should().NotBeNull();
            playerProgress.PlayerId.Should().Be("player1");
            playerProgress.QuestId.Should().Be(1);
            playerProgress.QuestPointsEarned.Should().Be(1);
            playerProgress.LastMilestoneCompletedId.Should().BeNull();

            
            var repository = new PlayerProgressRepository(_context);

            await repository.SavePlayerProgress("player1", 1, 10, 1);

            playerProgress = _context.PlayerProgresses.Find("player1", 1);
            
            playerProgress.Should().NotBeNull();
            playerProgress.PlayerId.Should().Be("player1");
            playerProgress.QuestId.Should().Be(1);
            playerProgress.QuestPointsEarned.Should().Be(11);
            playerProgress.LastMilestoneCompletedId.Should().Be(1);
        }

        [Fact]
        public async Task SavePlayerProgress_CreatingNewPlayerProgress()
        {
            var playerProgress = _context.PlayerProgresses.Find("player2", 2);
            
            playerProgress.Should().BeNull();

            
            var repository = new PlayerProgressRepository(_context);

            await repository.SavePlayerProgress("player2", 2, 10, 1);

            playerProgress = _context.PlayerProgresses.Find("player2", 2);
            
            playerProgress.Should().NotBeNull();
            playerProgress.PlayerId.Should().Be("player2");
            playerProgress.QuestId.Should().Be(2);
            playerProgress.QuestPointsEarned.Should().Be(10);
            playerProgress.LastMilestoneCompletedId.Should().Be(1);
        }

        public void Dispose()
        {
            _context?.Database?.EnsureDeleted();
        }
    }
}
