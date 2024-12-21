#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResultObject.Samples.BasicUsage.Scripts
{
    #region Game state classes

    public class PlayerData
    {
        public string? Name { get; set; }
        public int Level { get; set; }
        public Vector2? Position { get; set; }
        public int Health { get; set; }
        public List<string>? Inventory { get; set; }
    }

    public class Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    public class GameState
    {
        public PlayerData? Player { get; set; }
        public string? CurrentLevel { get; set; }
        public DateTime LastSaved { get; set; }
        public int PlayTime { get; set; }
        public Dictionary<string, bool>? Achievements { get; set; }
    }

    #endregion

    public class SaveGameSystem : MonoBehaviour
    {
        private const string SaveFileExtension = ".sav";

        // Main save game operation
        public Result<Unit> SaveGame(GameState currentState, string saveName)
        {
            return ValidateCheckpoint(currentState)
                .Bind(SerializeGameData)
                .Bind(data => WriteToFile(data, saveName))
                .OnSuccess(_ => Console.WriteLine("Game saved successfully!"))
                .OnFailure(error => Console.WriteLine($"Save failed: {error}"));
        }

        // Step 1: Validate the checkpoint/save point
        private static Result<GameState> ValidateCheckpoint(GameState currentState)
        {
            // Check if we're in a valid save location
            if (IsInCombat(currentState))
                return Result.Failure<GameState>("Cannot save during combat", "COMBAT_ACTIVE");

            if (IsInCutscene(currentState))
                return Result.Failure<GameState>("Cannot save during cutscene", "CUTSCENE_ACTIVE");

            // Validate essential game state data
            if (currentState.Player == null)
                return Result.Failure<GameState>("Invalid player data", "INVALID_STATE");

            if (string.IsNullOrEmpty(currentState.CurrentLevel))
                return Result.Failure<GameState>("Invalid level data", "INVALID_STATE");

            // Update last saved timestamp
            currentState.LastSaved = DateTime.UtcNow;
            return Result.Success(currentState);
        }

        // Step 2: Serialize the game data
        private static Result<string> SerializeGameData(GameState state)
        {
            try
            {
                // In a real implementation, you'd stringify the game state
                var json = state.ToString();

                return Result.Success(json);
            }
            catch (Exception ex)
            {
                return Result.Failure<string>(
                    new ResultError("Failed to serialize game data", "SERIALIZATION_ERROR", ex));
            }
        }

        // Step 3: Write to file
        private static Result<Unit> WriteToFile(string serializedData, string saveName)
        {
            try
            {
                // Validate save name
                if (string.IsNullOrWhiteSpace(saveName))
                    return Result.Failure<Unit>("Invalid save name", "INVALID_FILENAME");

                var fileName = $"{saveName}{SaveFileExtension}";

                // Simulate file writing (replace with actual file I/O)
                Console.WriteLine($"Writing save data to: {fileName}");
                Console.WriteLine($"Data size: {serializedData.Length} bytes");

                // In a real implementation, you'd write to file here
                // File.WriteAllText(fileName, serializedData);

                return Result.Unit();
            }
            catch (Exception ex)
            {
                return Result.Unit(new ResultError("Failed to write save file", "FILE_WRITE_ERROR", ex));
            }
        }

        // Helper methods to simulate game conditions
        private static bool IsInCombat(GameState state)
        {
            // Simulate combat check
            return false;
        }

        private static bool IsInCutscene(GameState state)
        {
            // Simulate cutscene check
            return false;
        }
    }
}