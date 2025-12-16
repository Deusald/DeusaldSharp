// MIT License

// DeusaldSharp:
// Copyright (c) 2020 Adam "Deusald" Orliński

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using DeusaldSharp;
using NUnit.Framework;

namespace DeusaldSharpTests
{
    public class GlickoTests
    {
        private static GlickoData CreateDefault()
        {
            return new GlickoData
            {
                Rating     = Glicko.DEFAULT_RATING,
                Deviation  = Glicko.DEFAULT_DEVIATION,
                Volatility = Glicko.DEFAULT_VOLATILITY
            };
        }

        [Test]
        [TestOf(nameof(Glicko.Update))]
        public void Glicko_Update_OneVsOne_Win_IncreasesWinnerDecreasesLoser()
        {
            // Arrange
            GlickoData a = CreateDefault();
            GlickoData b = CreateDefault();

            // Act
            Glicko.Update(a, b, out GlickoData newA, out GlickoData newB, playerAScore: 1.0);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.Greater(newA.Rating, a.Rating);
                Assert.Less(newB.Rating, b.Rating);

                Assert.Greater(newA.Volatility, 0.0);
                Assert.Greater(newB.Volatility, 0.0);

                // After an outcome, uncertainty should reduce (deviation typically decreases).
                Assert.Less(newA.Deviation, a.Deviation);
                Assert.Less(newB.Deviation, b.Deviation);
            });
        }

        [Test]
        [TestOf(nameof(Glicko.Update))]
        public void Glicko_Update_OneVsOne_Draw_EqualPlayers_RatingsStayClose()
        {
            // Arrange
            GlickoData a = CreateDefault();
            GlickoData b = CreateDefault();

            // Act
            Glicko.Update(a, b, out GlickoData newA, out GlickoData newB, playerAScore: 0.5);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.AreEqual(a.Rating, newA.Rating, 0.000001);
                Assert.AreEqual(b.Rating, newB.Rating, 0.000001);

                Assert.Greater(newA.Volatility, 0.0);
                Assert.Greater(newB.Volatility, 0.0);

                Assert.Less(newA.Deviation, a.Deviation);
                Assert.Less(newB.Deviation, b.Deviation);
            });
        }

        [Test]
        [TestOf(nameof(Glicko.Update))]
        public void Glicko_Update_PlayerVsMany_WinAll_IncreasesRating()
        {
            // Arrange
            GlickoData player = CreateDefault();

            List<(GlickoData, double)> opponents = new List<(GlickoData, double)>
            {
                (CreateDefault(), 1.0),
                (CreateDefault(), 1.0),
                (CreateDefault(), 1.0)
            };

            // Act
            GlickoData updated = Glicko.Update(player, opponents);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.Greater(updated.Rating, player.Rating);
                Assert.Greater(updated.Volatility, 0.0);
                Assert.Less(updated.Deviation, player.Deviation);
            });
        }

        [Test]
        [TestOf(nameof(Glicko.Update))]
        public void Glicko_Update_PlayerVsMany_LoseAll_DecreasesRating()
        {
            // Arrange
            GlickoData player = CreateDefault();

            List<(GlickoData, double)> opponents = new List<(GlickoData, double)>
            {
                (CreateDefault(), 0.0),
                (CreateDefault(), 0.0)
            };

            // Act
            GlickoData updated = Glicko.Update(player, opponents);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.Less(updated.Rating, player.Rating);
                Assert.Greater(updated.Volatility, 0.0);
                Assert.Less(updated.Deviation, player.Deviation);
            });
        }

        [Test]
        [TestOf(nameof(Glicko.GetWinProbability))]
        public void Glicko_GetWinProbability_EqualPlayers_IsHalf()
        {
            // Arrange
            GlickoData a = CreateDefault();
            GlickoData b = CreateDefault();

            // Act
            double p = Glicko.GetWinProbability(a, b);

            // Assert
            Assert.AreEqual(0.5, p, 0.000001);
        }

        [Test]
        [TestOf(nameof(Glicko.GetWinProbability))]
        public void Glicko_GetWinProbability_HigherRating_HasHigherProbability()
        {
            // Arrange
            GlickoData strong = CreateDefault();
            strong.Rating = 1700;

            GlickoData weak = CreateDefault();
            weak.Rating = 1300;

            // Act
            double pStrong = Glicko.GetWinProbability(strong, weak);
            double pWeak   = Glicko.GetWinProbability(weak, strong);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.Greater(pStrong, 0.5);
                Assert.Less(pWeak, 0.5);
                Assert.AreEqual(1.0, pStrong + pWeak, 0.000001);
            });
        }

        [Test]
        [TestOf(nameof(Glicko.GetWinProbability))]
        public void Glicko_GetWinProbability_HigherOpponentDeviation_PullsProbabilityTowardHalf()
        {
            // Arrange
            GlickoData player = CreateDefault();
            player.Rating = 1700;

            GlickoData lowDevOpponent = CreateDefault();
            lowDevOpponent.Rating    = 1300;
            lowDevOpponent.Deviation = 30;

            GlickoData highDevOpponent = lowDevOpponent;
            highDevOpponent.Deviation = 350;

            // Act
            double pLowDev  = Glicko.GetWinProbability(player, lowDevOpponent);
            double pHighDev = Glicko.GetWinProbability(player, highDevOpponent);

            // Assert
            // Higher opponent uncertainty should reduce confidence (closer to 0.5).
            Assert.Greater(pLowDev, pHighDev);
            Assert.Greater(pHighDev, 0.5);
        }

        [Test]
        [TestOf(nameof(Glicko.DecayPlayer))]
        public void Glicko_DecayPlayer_Recent_NoDecay()
        {
            // Arrange
            GlickoData player = CreateDefault();
            DateTime decayTime = DateTime.UtcNow.AddDays(-1);

            // Act
            GlickoData decayed = Glicko.DecayPlayer(player, decayTime, out bool didDecay);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsFalse(didDecay);
                Assert.AreEqual(player.Rating, decayed.Rating, 0.0);
                Assert.AreEqual(player.Deviation, decayed.Deviation, 0.0);
                Assert.AreEqual(player.Volatility, decayed.Volatility, 0.0);
            });
        }

        [Test]
        [TestOf(nameof(Glicko.DecayPlayer))]
        public void Glicko_DecayPlayer_Old_IncreasesDeviation_AndCapsAt350()
        {
            // Arrange
            GlickoData player = CreateDefault();
            player.Deviation = 200;

            DateTime decayTime = DateTime.UtcNow.AddDays(-30);

            // Act
            GlickoData decayed = Glicko.DecayPlayer(player, decayTime, out bool didDecay);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.IsTrue(didDecay);
                Assert.GreaterOrEqual(decayed.Deviation, player.Deviation);
                Assert.LessOrEqual(decayed.Deviation, 350.0);
                Assert.AreEqual(player.Rating, decayed.Rating, 0.0);
                Assert.AreEqual(player.Volatility, decayed.Volatility, 0.0);
            });
        }
    }
}