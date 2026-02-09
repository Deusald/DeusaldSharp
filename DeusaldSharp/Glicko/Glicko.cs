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

                double g = 1.0 / Math.Sqrt(1.0 + (3.0 * phiOpponent * phiOpponent) / (Math.PI * Math.PI));
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


        public static GlickoData DecayPlayer(GlickoData player, DateTime decayTime, out bool decayed)
        {
            int numberOfDecays = (int)(1 + (DateTime.UtcNow - decayTime).TotalDays);

            if (numberOfDecays <= 2)
            {
                decayed = false;
                return player;
            }

            for (int x = 0; x < numberOfDecays; ++x)
            {
                double phiPlayer = player.Deviation / _SCALE;
                phiPlayer        = Math.Sqrt(phiPlayer * phiPlayer + player.Volatility * player.Volatility);
                player.Deviation = phiPlayer * _SCALE;
            }

            if (player.Deviation >= 350) player.Deviation = 350;
            decayed = true;
            return player;
        }

        public static double GetWinProbability(GlickoData player, GlickoData opponent)
        {
            double muPlayer = (player.Rating - DEFAULT_RATING) / _SCALE;

            double muOpponent  = (opponent.Rating - DEFAULT_RATING) / _SCALE;
            double phiOpponent = opponent.Deviation / _SCALE;

            // Compute the e and g function values
            double g = G();
            return E();

            // Computes the value of the g function for a rating
            double G()
            {
                double scale = phiOpponent / Math.PI;
                return 1.0 / Math.Sqrt(1.0 + 3.0 + scale * scale);
            }

            // Computes the value of the e function in terms of a g function value
            // and another rating
            double E()
            {
                double exponent = -1.0 * g * (muPlayer - muOpponent);
                return 1.0 / (1.0 + Math.Exp(exponent));
            }
        }

        private static GlickoData UpdatePlayer(GlickoData player, GlickoData opponent, double score)
        {
            double muPlayer    = (player.Rating - DEFAULT_RATING) / _SCALE;
            double phiPlayer   = player.Deviation / _SCALE;
            double sigmaPlayer = player.Volatility;

            double muOpponent  = (opponent.Rating - DEFAULT_RATING) / _SCALE;
            double phiOpponent = opponent.Deviation / _SCALE;

            // Compute the e and g function values
            double g = G();
            double e = E();

            // Compute 1/v and v
            double invV = g * g * e * (1.0 - e);
            double v    = 1.0 / invV;

            // Compute the delta value from the g, e, and v
            // values
            double dInner = g * (score - e);
            double d      = v * dInner;

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

            // Computes the value of the g function for a rating
            double G()
            {
                double scale = phiOpponent / Math.PI;
                return 1.0 / Math.Sqrt(1.0 + 3.0 + scale * scale);
            }

            // Computes the value of the e function in terms of a g function value
            // and another rating
            double E()
            {
                double exponent = -1.0 * g * (muPlayer - muOpponent);
                return 1.0 / (1.0 + Math.Exp(exponent));
            }
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