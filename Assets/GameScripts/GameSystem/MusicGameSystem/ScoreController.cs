
namespace Softstar
{
    public class ScoreController
    {
        private double m_noteBaseScoreTotal     = 100000.0;
        private double m_comboBaseScoreTotal    = 100000.0;
        private double m_geniusScore            = 1000000.0;
        private double m_sScore                 = 900000.0;
        private double m_aScore                 = 800000.0;
        private double m_bScore                 = 700000.0;
        private double m_cScore                 = 600000.0;
        private int m_iTotalNotes               = 1;
        private double m_noteBaseScore          = 1.0;
        private double m_comboBaseScore         = 1.0;

        public int TotalNotes
        {
            set
            {
                m_iTotalNotes = value;
                m_noteBaseScore = m_noteBaseScoreTotal / (double)m_iTotalNotes;
                m_comboBaseScore = m_comboBaseScoreTotal / (((double)(m_iTotalNotes + 1) * (double)m_iTotalNotes) / 2.0);
                NoteBaseScore = m_noteBaseScore;
                ComboBaseScore = m_comboBaseScore;
            }
            get { return m_iTotalNotes; }
        }
        public double NoteBaseScore { private set; get; }
        public double ComboBaseScore { private set; get; }

        public int MaxCombo { set; get; }
        public int ComboCount { set; get; }
        public int FantasicCount { set; get; }
        public int GreatCount { set; get; }
        public int WeakCount { set; get; }
        public int LostCount { set; get; }

        public double NoteBaseScoreTotal { set { m_noteBaseScoreTotal = value; } get { return m_noteBaseScoreTotal; } }
        public double ComboBaseScoreTotal { set { m_comboBaseScoreTotal = value; } get { return m_comboBaseScoreTotal; } }
        public double ScoreGenius { set { m_geniusScore = value; } get { return m_geniusScore; } }
        public double ScoreS { set { m_sScore = value; } get { return m_sScore; } }
        public double ScoreA { set { m_aScore = value; } get { return m_aScore; } }
        public double ScoreB { set { m_bScore = value; } get { return m_bScore; } }
        public double ScoreC { set { m_cScore = value; } get { return m_cScore; } }

        public Enum_SongRank Rank
        {
            get
            {
                if (Score >= m_geniusScore) return Enum_SongRank.G;
                else if (Score >= m_sScore) return Enum_SongRank.S;
                else if (Score >= m_aScore) return Enum_SongRank.A;
                else if (Score >= m_bScore) return Enum_SongRank.B;
                else if (Score >= m_cScore) return Enum_SongRank.C;
                else    return Enum_SongRank.D;
            }
        }

        public double Score { set; get; }

        private static ScoreType[] m_scoreType = { ScoreType.Fantastic, ScoreType.Great, ScoreType.Weak, ScoreType.Lost };
        private static float[] m_scoreThreshold = { 0.8f, 0.55f, 0.2f, -1.0f };

        public ScoreController()
        {
        }

        /// <summary>
        /// 每首歌開始前初始化
        /// </summary>
        public void initialize()
        {
            MaxCombo = 0;
            ComboCount = 0;
            FantasicCount = 0;
            GreatCount = 0;
            WeakCount = 0;
            LostCount = 0;

            Score = 0.0;
            m_iTotalNotes = 1;
        }

        public ScoreType getScoreType(float score)
        {
            ScoreType targetType = ScoreType.Lost;
            for (int i = 0; i < ScoreController.m_scoreThreshold.Length; i++)
            {
                //must be smaller than (Weak/Lost issue)
                if (score > ScoreController.m_scoreThreshold[i])
                {
                    targetType = ScoreController.m_scoreType[i];
                    break;
                }
            }
            return targetType;
        }

        public void CountComboScore(int ComboCount)
        {
            if (ComboCount > 1)
            {
                Score += (m_noteBaseScore * (((1 + ComboCount) * ComboCount) / 2)) / GameDefine.DEFAULT_COMBO_PARAMETER;
            }
        }

        public void addHitScore(AbstractNote note)
        {
            if (note.HitType == HitScoreType.NoScore)
                return;

            ScoreType scoreType = getScoreType(note.NoteScore);
            note.NodeScoreType = scoreType;

            // Combo
            switch(scoreType)
            {
                case ScoreType.Fantastic:
                case ScoreType.Great:
                    ComboCount++;
                    if(ComboCount > MaxCombo)
                    {
                        MaxCombo = ComboCount;
                    }
                    break;
                default:
                    {
                        //combo斷掉 計算分數
                        CountComboScore(ComboCount);
                        ComboCount = 0;
                    }                    
                    break;
            }

            // Stats
            switch(scoreType)
            {
                case ScoreType.Fantastic:   FantasicCount++;    break;
                case ScoreType.Great:       GreatCount++;       break;
                case ScoreType.Weak:        WeakCount++;        break;
                case ScoreType.Lost:        LostCount++;        break;
                default:                                        break;
            }

            // 總分
            Score += m_noteBaseScore * scoreType.ToScorePercentage();
            //Score += m_comboBaseScore * ComboCount;
        }
    }
}
