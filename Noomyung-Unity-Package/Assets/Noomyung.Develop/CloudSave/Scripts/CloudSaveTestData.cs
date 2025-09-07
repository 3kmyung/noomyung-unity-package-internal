using System;

namespace Noomyung.Develop.CloudSave
{
    /// <summary>
    /// CloudSave 테스트용 플레이어 데이터 클래스
    /// </summary>
    [Serializable]
    public class PlayerTestData
    {
        public string PlayerName { get; set; }
        public int Level { get; set; }
        public int Score { get; set; }
        public float Experience { get; set; }
        public DateTime LastPlayTime { get; set; }
        public bool IsPremium { get; set; }

        public PlayerTestData()
        {
            PlayerName = "TestPlayer";
            Level = 1;
            Score = 0;
            Experience = 0f;
            LastPlayTime = DateTime.Now;
            IsPremium = false;
        }

        public PlayerTestData(string name, int level, int score, float exp, bool premium)
        {
            PlayerName = name;
            Level = level;
            Score = score;
            Experience = exp;
            LastPlayTime = DateTime.Now;
            IsPremium = premium;
        }

        public override bool Equals(object obj)
        {
            if (obj is PlayerTestData other)
            {
                return PlayerName == other.PlayerName &&
                       Level == other.Level &&
                       Score == other.Score &&
                       Math.Abs(Experience - other.Experience) < 0.001f &&
                       IsPremium == other.IsPremium;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PlayerName, Level, Score, Experience, IsPremium);
        }

        public override string ToString()
        {
            return $"PlayerTestData(Name: {PlayerName}, Level: {Level}, Score: {Score}, Exp: {Experience}, Premium: {IsPremium})";
        }
    }

    /// <summary>
    /// CloudSave 테스트용 게임 설정 데이터 클래스
    /// </summary>
    [Serializable]
    public class GameSettingsData
    {
        public float MasterVolume { get; set; }
        public float MusicVolume { get; set; }
        public float SfxVolume { get; set; }
        public bool FullScreen { get; set; }
        public int QualityLevel { get; set; }
        public string Language { get; set; }

        public GameSettingsData()
        {
            MasterVolume = 1.0f;
            MusicVolume = 0.8f;
            SfxVolume = 0.9f;
            FullScreen = false;
            QualityLevel = 2;
            Language = "Korean";
        }

        public override bool Equals(object obj)
        {
            if (obj is GameSettingsData other)
            {
                return Math.Abs(MasterVolume - other.MasterVolume) < 0.001f &&
                       Math.Abs(MusicVolume - other.MusicVolume) < 0.001f &&
                       Math.Abs(SfxVolume - other.SfxVolume) < 0.001f &&
                       FullScreen == other.FullScreen &&
                       QualityLevel == other.QualityLevel &&
                       Language == other.Language;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MasterVolume, MusicVolume, SfxVolume, FullScreen, QualityLevel, Language);
        }

        public override string ToString()
        {
            return $"GameSettingsData(Master: {MasterVolume}, Music: {MusicVolume}, SFX: {SfxVolume}, FullScreen: {FullScreen}, Quality: {QualityLevel}, Lang: {Language})";
        }
    }
}
