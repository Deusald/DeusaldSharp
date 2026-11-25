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

namespace DeusaldSharp
{
    public static class Glicko
    {
        public const double DEFAULT_RATING     = 1500; // The default/initial rating value
        public const double DEFAULT_DEVIATION  = 350;  // The default/initial deviation value
        public const double DEFAULT_VOLATILITY = 0.06; // The default/initial volatility value

        private const double _SCALE        = 173.7178; // The Glicko-1 to Glicko-2 scale factor
        private const double _SYSTEM_CONST = 0.5;      // The system constant (tau)
        private const double _CONVERGENCE  = 0.000001; // The convergence constant (epsilon)

        public static void Update(GlickoData playerA, GlickoData playerB, out GlickoData newPlayerAData, out GlickoData newPlayerBData, double playerAScore)
        {
            newPlayerAData = UpdatePlayer(playerA, playerB, playerAScore);
            newPlayerBData = UpdatePlayer(playerB, playerA, 1.0 - playerAScore);
        }

        public static GlickoData Update(GlickoData player, List<(GlickoData, double)> opponents)
        {
            double muPlayer    = (player.Rating - DEFAULT_RATING) / _SCALE;
            double phiPlayer   = player.Deviation / _SCALE;
            double sigmaPlayer = player.Volatility;

            double[] gTable = new double[opponents.Count];
            double[] eTable = new double[opponents.Count];
            double   invV   = 0.0;

            // Compute the g and e values for each opponent and accumulate 1/v
            for (int j = 0; j < opponents.Count; j++)
            {
                (GlickoData opponent, _) = opponents[j];

                double muOpponent  = (opponent.Rating - DEFAULT_RATING) / _SCALE;
                double phiOpponent = opponent.Deviation / _SCALE;

                double g = G(phiOpponent);
                double e = 1.0 / (1.0 + Math.Exp(-g * (muPlayer - muOpponent)));

                gTable[j] = g;
                eTable[j] = e;

                invV += g * g * e * (1.0 - e);
            }

            double v = 1.0 / invV;

            // Compute the delta value
            double dInner = 0.0;
            for (int j = 0; j < opponents.Count; j++)
            {
                double score = opponents[j].Item2;
                dInner += gTable[j] * (score - eTable[j]);
            }

            double d = v * dInner;

            // Compute new rating, deviation and volatility values
            double newSigma = Math.Exp(Convergence(d, v, phiPlayer, sigmaPlayer) / 2.0);
            double newPhi   = 1.0 / Math.Sqrt((1.0 / (phiPlayer * phiPlayer + newSigma * newSigma)) + invV);
            double newMu    = muPlayer + newPhi * newPhi * dInner;

            return new GlickoData
            {
                Rating     = newMu * _SCALE + DEFAULT_RATING,
                Deviation  = newPhi * _SCALE,
                Volatility = newSigma
            };
        }


        /// <summary>
        /// Increases the rating deviation of an inactive player by one Glicko-2 rating period for every <paramref name="period"/>
        /// that elapsed since <paramref name="decayDeadline"/>. Reaching the deadline applies the first period.
        /// Rating and volatility are not changed; deviation is capped at <see cref="DEFAULT_DEVIATION"/>.
        /// </summary>
        /// <param name="player"> Player data to decay. </param>
        /// <param name="decayDeadline"> Moment the first decay period applies, e.g. last game time + grace period. </param>
        /// <param name="now"> Current time. Pass <see cref="DateTime.UtcNow"/>; it is a parameter to keep the method deterministic. </param>
        /// <param name="period"> Length of one decay period, e.g. one day. </param>
        /// <param name="periodsApplied"> Number of periods applied; 0 when the deadline has not been reached yet. </param>
        /// <param name="nextDecayDeadline">
        /// Deadline to store for the next call: <paramref name="decayDeadline"/> moved by the applied periods (always after <paramref name="now"/>
        /// when periods were applied), so partially elapsed periods are not lost. Equals <paramref name="decayDeadline"/> when nothing was applied.
        /// </param>
        /// <remarks> Local times are converted to UTC; <see cref="DateTimeKind.Unspecified"/> times are treated as UTC. </remarks>
        public static GlickoData DecayPlayer(GlickoData   player,
                                             DateTime     decayDeadline,
                                             DateTime     now,
                                             TimeSpan     period,
                                             out long     periodsApplied,
                                             out DateTime nextDecayDeadline)
        {
            if (period <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(period), period, "Decay period must be positive.");

            decayDeadline = ToUtc(decayDeadline);
            now           = ToUtc(now);

            if (now < decayDeadline)
            {
                periodsApplied    = 0;
                nextDecayDeadline = decayDeadline;
                return player;
            }

            periodsApplied    = (now - decayDeadline).Ticks / period.Ticks + 1;
            nextDecayDeadline = decayDeadline + TimeSpan.FromTicks(period.Ticks * periodsApplied);

            // Applying k periods one by one (phi^2 + sigma^2 each time) equals a single step of phi^2 + k * sigma^2.
            double phiPlayer = player.Deviation / _SCALE;
            double phiDecay  = Math.Sqrt(phiPlayer * phiPlayer + periodsApplied * player.Volatility * player.Volatility);
            player.Deviation = Math.Min(phiDecay * _SCALE, DEFAULT_DEVIATION);
            return player;

            static DateTime ToUtc(DateTime time)
            {
                return time.Kind switch
                {
                    DateTimeKind.Local       => time.ToUniversalTime(),
                    DateTimeKind.Unspecified => DateTime.SpecifyKind(time, DateTimeKind.Utc),
                    _                        => time
                };
            }
        }

        public static double GetWinProbability(GlickoData player, GlickoData opponent)
        {
            double muPlayer    = (player.Rating - DEFAULT_RATING) / _SCALE;
            double phiPlayer   = player.Deviation / _SCALE;
            double muOpponent  = (opponent.Rating - DEFAULT_RATING) / _SCALE;
            double phiOpponent = opponent.Deviation / _SCALE;

            // Both ratings are uncertain, so the combined deviation is used in the g function
            double g = G(Math.Sqrt(phiPlayer * phiPlayer + phiOpponent * phiOpponent));
            return 1.0 / (1.0 + Math.Exp(-g * (muPlayer - muOpponent)));
        }

        private static GlickoData UpdatePlayer(GlickoData player, GlickoData opponent, double score)
        {
            return Update(player, new List<(GlickoData, double)> { (opponent, score) });
        }

        // Computes the value of the g function for a deviation in the Glicko-2 scale
        private static double G(double phi)
        {
            return 1.0 / Math.Sqrt(1.0 + 3.0 * phi * phi / (Math.PI * Math.PI));
        }

        private static double Convergence(double d, double v, double p, double s)
        {
            // Initialize function values for iteration procedure
            double dS = d * d;
            double pS = p * p;
            double tS = _SYSTEM_CONST * _SYSTEM_CONST;
            double a  = Math.Log(s * s);

            // Select the upper and lower iteration ranges
            double A = a;
            double B;
            double bTest = dS - pS - v;

            if (bTest > 0.0)
            {
                B = Math.Log(bTest);
            }
            else
            {
                B = a - _SYSTEM_CONST;
                while (F(B, dS, pS, v, a, tS) < 0.0)
                {
                    B -= _SYSTEM_CONST;
                }
            }

            // Perform the iteration
            double fA = F(A, dS, pS, v, a, tS);
            double fB = F(B, dS, pS, v, a, tS);
            while (Math.Abs(B - A) > _CONVERGENCE)
            {
                double C  = A + (A - B) * fA / (fB - fA);
                double fC = F(C, dS, pS, v, a, tS);

                if (fC * fB < 0.0)
                {
                    A  = B;
                    fA = fB;
                }
                else
                {
                    fA /= 2.0;
                }

                B  = C;
                fB = fC;
            }

            return A;
        }

        /// Computes the value of the f function in terms of x, delta^2, phi^2,
        /// v, a and tau^2.
        private static double F(double x, double dS, double pS, double v, double a, double tS)
        {
            double eX  = Math.Exp(x);
            double num = eX * (dS - pS - v - eX);
            double den = pS + v + eX;
            return num / (2.0 * den * den) - (x - a) / tS;
        }
    }
}