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

        private static readonly DateTime _DecayDeadline = new DateTime(2026, 1, 10, 12, 0, 0, DateTimeKind.Utc);
        private static readonly TimeSpan _DecayPeriod   = TimeSpan.FromDays(1);

        /// <summary> Reference implementation: one Glicko-2 rating period of inactivity applied period by period. </summary>
        private static double DecayDeviationIteratively(GlickoData player, long periods)
        {
            const double scale = 173.7178;
            double       phi   = player.Deviation / scale;
            for (long x = 0; x < periods; ++x) phi = Math.Sqrt(phi * phi + player.Volatility * player.Volatility);
            return Math.Min(phi * scale, Glicko.DEFAULT_DEVIATION);
        }

        [Test]
        [TestOf(nameof(Glicko.DecayPlayer))]
        public void Glicko_DecayPlayer_BeforeDeadline_NoDecay()
        {
            // Arrange
            GlickoData player = CreateDefault();
            player.Deviation  = 80;

            // Act
            GlickoData decayed = Glicko.DecayPlayer(player, _DecayDeadline, _DecayDeadline.AddTicks(-1), _DecayPeriod,
                out long periods, out DateTime nextDeadline);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(periods,      Is.EqualTo(0));
                Assert.That(nextDeadline, Is.EqualTo(_DecayDeadline));
                Assert.That(decayed,      Is.EqualTo(player));
            });
        }

        [TestCase(0.0,  1)] // reaching the deadline applies the first period - no hidden extra grace days
        [TestCase(0.5,  1)]
        [TestCase(1.0,  2)]
        [TestCase(2.9,  3)]
        [TestCase(30.2, 31)]
        [TestOf(nameof(Glicko.DecayPlayer))]
        public void Glicko_DecayPlayer_AppliesOnePeriodPerElapsedPeriod(double daysAfterDeadline, long expectedPeriods)
        {
            // Arrange
            GlickoData player = CreateDefault();
            player.Deviation  = 60;
            DateTime now      = _DecayDeadline.AddDays(daysAfterDeadline);

            // Act
            GlickoData decayed = Glicko.DecayPlayer(player, _DecayDeadline, now, _DecayPeriod, out long periods, out DateTime nextDeadline);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(periods,           Is.EqualTo(expectedPeriods));
                Assert.That(nextDeadline,      Is.EqualTo(_DecayDeadline.AddDays(expectedPeriods)));
                Assert.That(nextDeadline,      Is.GreaterThan(now));
                Assert.That(decayed.Deviation, Is.EqualTo(DecayDeviationIteratively(player, expectedPeriods)).Within(1e-9));
                Assert.That(decayed.Rating,    Is.EqualTo(player.Rating).Within(0.0));
                Assert.That(decayed.Volatility, Is.EqualTo(player.Volatility).Within(0.0));
            });
        }

        [Test]
        [TestOf(nameof(Glicko.DecayPlayer))]
        public void Glicko_DecayPlayer_DailyLogins_DecayGraduallyAndLoseNoTime()
        {
            // Arrange - an inactive player logs in every day at a different hour; the stored deadline is moved forward each time
            GlickoData player   = CreateDefault();
            player.Deviation    = 60;
            GlickoData expected = player;
            DateTime   deadline = _DecayDeadline;
            long       total    = 0;

            // Act
            for (int x = 0; x < 10; ++x)
            {
                DateTime now = _DecayDeadline.AddDays(x).AddHours(x % 3 * 7);
                player = Glicko.DecayPlayer(player, deadline, now, _DecayPeriod, out long periods, out deadline);
                Assert.That(periods, Is.LessThanOrEqualTo(1), $"login {x} applied a burst of {periods} periods");
                total += periods;
            }

            // Assert - same result as a single login after the same time
            GlickoData single = Glicko.DecayPlayer(expected, _DecayDeadline, _DecayDeadline.AddDays(9).AddHours(0), _DecayPeriod,
                out long singlePeriods, out _);
            Assert.Multiple(() =>
            {
                Assert.That(total,            Is.EqualTo(singlePeriods));
                Assert.That(player.Deviation, Is.EqualTo(single.Deviation).Within(1e-9));
                Assert.That(player.Deviation, Is.GreaterThan(expected.Deviation));
            });
        }

        [Test]
        [TestOf(nameof(Glicko.DecayPlayer))]
        public void Glicko_DecayPlayer_LongInactivity_CapsAtDefaultDeviation()
        {
            // Arrange
            GlickoData player = CreateDefault();
            player.Deviation  = 50;

            // Act - 20 years of daily periods: closed form, no per-period loop
            GlickoData decayed = Glicko.DecayPlayer(player, _DecayDeadline, _DecayDeadline.AddYears(20), _DecayPeriod, out long periods, out _);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(periods,           Is.GreaterThan(7000));
                Assert.That(decayed.Deviation, Is.EqualTo(Glicko.DEFAULT_DEVIATION).Within(0.0));
            });
        }

        [Test]
        [TestOf(nameof(Glicko.DecayPlayer))]
        public void Glicko_DecayPlayer_DateTimeKinds_AreComparedInUtc()
        {
            // Arrange - 30 minutes before the deadline, so any UTC offset of 1 hour or more would flip the result
            GlickoData player      = CreateDefault();
            DateTime   now         = _DecayDeadline.AddMinutes(-30);
            DateTime   localNow    = now.ToLocalTime();
            DateTime   unspecified = DateTime.SpecifyKind(_DecayDeadline, DateTimeKind.Unspecified);

            // Act
            Glicko.DecayPlayer(player, _DecayDeadline, localNow, _DecayPeriod, out long localPeriods,       out _);
            Glicko.DecayPlayer(player, unspecified,    now,      _DecayPeriod, out long unspecifiedPeriods, out _);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(localPeriods,       Is.EqualTo(0));
                Assert.That(unspecifiedPeriods, Is.EqualTo(0));
            });
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestOf(nameof(Glicko.DecayPlayer))]
        public void Glicko_DecayPlayer_NonPositivePeriod_Throws(int periodTicks)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Glicko.DecayPlayer(CreateDefault(), _DecayDeadline, _DecayDeadline.AddDays(1),
                TimeSpan.FromTicks(periodTicks), out _, out _));
        }

        [Test]
        [TestOf(nameof(Glicko.Update))]
        public void Glicko_Update_PaperExample()
        {
            // Arrange - example from Glickman's "Example of the Glicko-2 system"
            GlickoData player = new GlickoData { Rating = 1500, Deviation = 200, Volatility = 0.06 };

            List<(GlickoData, double)> opponents = new List<(GlickoData, double)>
            {
                (new GlickoData { Rating = 1400, Deviation = 30,  Volatility = 0.06 }, 1.0),
                (new GlickoData { Rating = 1550, Deviation = 100, Volatility = 0.06 }, 0.0),
                (new GlickoData { Rating = 1700, Deviation = 300, Volatility = 0.06 }, 0.0)
            };

            // Act
            GlickoData updated = Glicko.Update(player, opponents);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(updated.Rating,     Is.EqualTo(1464.06).Within(0.05));
                Assert.That(updated.Deviation,  Is.EqualTo(151.52).Within(0.05));
                Assert.That(updated.Volatility, Is.EqualTo(0.05999).Within(0.00001));
            });
        }

        [Test]
        [TestOf(nameof(Glicko.Update))]
        public void Glicko_Update_OneVsOne_SameAsListUpdate()
        {
            // Arrange
            GlickoData a = new GlickoData { Rating = 1600, Deviation = 120, Volatility = 0.06 };
            GlickoData b = new GlickoData { Rating = 1450, Deviation = 250, Volatility = 0.05 };

            // Act
            Glicko.Update(a, b, out GlickoData newA, out GlickoData newB, playerAScore: 1.0);
            GlickoData listA = Glicko.Update(a, new List<(GlickoData, double)> { (b, 1.0) });
            GlickoData listB = Glicko.Update(b, new List<(GlickoData, double)> { (a, 0.0) });

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(newA.Rating,     Is.EqualTo(listA.Rating).Within(0.000001));
                Assert.That(newA.Deviation,  Is.EqualTo(listA.Deviation).Within(0.000001));
                Assert.That(newA.Volatility, Is.EqualTo(listA.Volatility).Within(0.000001));
                Assert.That(newB.Rating,     Is.EqualTo(listB.Rating).Within(0.000001));
                Assert.That(newB.Deviation,  Is.EqualTo(listB.Deviation).Within(0.000001));
                Assert.That(newB.Volatility, Is.EqualTo(listB.Volatility).Within(0.000001));
            });
        }

        [Test]
        [TestOf(nameof(Glicko.GetWinProbability))]
        public void Glicko_GetWinProbability_MatchesGlicko2Formula()
        {
            // Arrange
            const double scale = 173.7178;

            GlickoData strong = new GlickoData { Rating = 1800, Deviation = 30, Volatility = 0.06 };
            GlickoData weak   = new GlickoData { Rating = 1500, Deviation = 30, Volatility = 0.06 };

            double phiStrong = strong.Deviation / scale;
            double phiWeak   = weak.Deviation / scale;
            double phi       = Math.Sqrt(phiStrong * phiStrong + phiWeak * phiWeak);
            double g         = 1.0 / Math.Sqrt(1.0 + 3.0 * phi * phi / (Math.PI * Math.PI));
            double expected  = 1.0 / (1.0 + Math.Exp(-g * (strong.Rating - weak.Rating) / scale));

            // Act
            double p = Glicko.GetWinProbability(strong, weak);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(p, Is.EqualTo(expected).Within(0.001));
                Assert.That(p, Is.EqualTo(0.849).Within(0.005));
            });
        }
    }
}