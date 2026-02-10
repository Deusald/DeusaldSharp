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
                Assert.That(newA.Rating, Is.GreaterThan(a.Rating));
                Assert.That(newB.Rating, Is.LessThan(b.Rating));

                Assert.That(newA.Volatility, Is.GreaterThan(0.0));
                Assert.That(newB.Volatility, Is.GreaterThan(0.0));

                // After an outcome, uncertainty should reduce (deviation typically decreases).
                Assert.That(newA.Deviation, Is.LessThan(a.Deviation));
                Assert.That(newB.Deviation, Is.LessThan(b.Deviation));
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
                Assert.That(newA.Rating, Is.EqualTo(a.Rating).Within(0.000001));
                Assert.That(newB.Rating, Is.EqualTo(b.Rating).Within(0.000001));

                Assert.That(newA.Volatility, Is.GreaterThan(0.0));
                Assert.That(newB.Volatility, Is.GreaterThan(0.0));

                Assert.That(newA.Deviation, Is.LessThan(a.Deviation));
                Assert.That(newB.Deviation, Is.LessThan(b.Deviation));
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
                Assert.That(updated.Rating, Is.GreaterThan(player.Rating));
                Assert.That(updated.Volatility, Is.GreaterThan(0.0));
                Assert.That(updated.Deviation, Is.LessThan(player.Deviation));
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
                Assert.That(updated.Rating, Is.LessThan(player.Rating));
                Assert.That(updated.Volatility, Is.GreaterThan(0.0));
                Assert.That(updated.Deviation, Is.LessThan(player.Deviation));
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
            Assert.That(p, Is.EqualTo(0.5).Within(0.000001));
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
                Assert.That(pStrong, Is.GreaterThan(0.5));
                Assert.That(pWeak, Is.LessThan(0.5));
                Assert.That(pStrong + pWeak, Is.EqualTo(1.0).Within(0.000001));
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
            Assert.That(pLowDev, Is.GreaterThan(pHighDev));
            Assert.That(pHighDev, Is.GreaterThan(0.5));
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
                Assert.That(didDecay, Is.False);
                Assert.That(decayed.Rating, Is.EqualTo(player.Rating).Within(0.0));
                Assert.That(decayed.Deviation, Is.EqualTo(player.Deviation).Within(0.0));
                Assert.That(decayed.Volatility, Is.EqualTo(player.Volatility).Within(0.0));
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
                Assert.That(didDecay, Is.True);
                Assert.That(decayed.Deviation, Is.GreaterThanOrEqualTo(player.Deviation));
                Assert.That(decayed.Deviation, Is.LessThanOrEqualTo(350.0));
                Assert.That(decayed.Rating, Is.EqualTo(player.Rating).Within(0.0));
                Assert.That(decayed.Volatility, Is.EqualTo(player.Volatility).Within(0.0));
            });
        }
    }
}