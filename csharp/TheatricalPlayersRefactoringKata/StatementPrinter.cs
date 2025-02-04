﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace TheatricalPlayersRefactoringKata
{
    public class StatementPrinter
    {

        public string Print(Invoice invoice, Dictionary<string, Play> plays)
        {
            var totalAmount = 0;
            var volumeCredits = 0;
            var result = string.Format("Statement for {0}\n", invoice.Customer);
            CultureInfo cultureInfo = new CultureInfo("en-US");

            foreach(var perf in invoice.Performances)
            {
                var play = plays[perf.PlayID];
                var thisAmount = 0;
                thisAmount = EvaluateAmount(perf, play);
                // add volume credits
                volumeCredits = GetVolumeCredits(volumeCredits, perf, play);

                // print line for this order
                result += string.Format(cultureInfo, "  {0}: {1:C} ({2} seats)\n", play.Name, Convert.ToDecimal(thisAmount / 100), perf.Audience);
                totalAmount += thisAmount;
            }
            result += string.Format(cultureInfo, "Amount owed is {0:C}\n", Convert.ToDecimal(totalAmount / 100));
            result += string.Format("You earned {0} credits\n", volumeCredits);
            return result;
        }

        private static int GetVolumeCredits(int volumeCredits, Performance perf, Play play)
        {
            volumeCredits += Math.Max(perf.Audience - 30, 0);
            // add extra credit for every ten comedy attendees
            if ("comedy" == play.Type) volumeCredits += (int)Math.Floor((decimal)perf.Audience / 5);
            return volumeCredits;
        }

        private static int EvaluateAmount(Performance perf, Play play)
        {
            int thisAmount;
            switch (play.Type)
            {
                case "tragedy":
                    thisAmount = TragedyAmount(perf);
                    break;
                case "comedy":
                    thisAmount = ComedyAmount(perf);
                    break;
                default:
                    throw new Exception("unknown type: " + play.Type);
            }

            return thisAmount;
        }

        private static int ComedyAmount(Performance perf)
        {
            const int AUDIANCE_LIMIT = 20;
            const int BASE_AMOUNT = 30000;

            int thisAmount = BASE_AMOUNT;
            if (perf.Audience > AUDIANCE_LIMIT)
            {
                thisAmount += 10000 + 500 * (perf.Audience - AUDIANCE_LIMIT);
            }
            thisAmount += 300 * perf.Audience;
            return thisAmount;
        }

        private static int TragedyAmount(Performance perf)
        {
            const int AUDIANCE_LIMIT = 30;
            const int BASE_AMOUNT = 40000;

            int thisAmount = BASE_AMOUNT;
            if (perf.Audience > AUDIANCE_LIMIT)
            {
                thisAmount += 1000 * (perf.Audience - AUDIANCE_LIMIT);
            }

            return thisAmount;
        }

        public string PrintAsHtml(Invoice invoice, Dictionary<string, Play> plays)
        {
            var totalAmount = 0;
            var volumeCredits = 0;
            var result = "<html>\n";
            result += string.Format("  <h1>Statement for {0}</h1>\n", invoice.Customer);

            CultureInfo cultureInfo = new CultureInfo("en-US");

            result += "  <table>\n";
            result += "    <tr><th>play</th><th>seats</th><th>cost</th></tr>\n";
            foreach (var perf in invoice.Performances)
            {
                var play = plays[perf.PlayID];
                var thisAmount = 0;
                thisAmount = EvaluateAmount(perf, play);
                // add volume credits
                volumeCredits = GetVolumeCredits(volumeCredits, perf, play);

                // print line for this order
                result += string.Format(cultureInfo, "    <tr><td>{0}</td><td>{1}</td><td>{2:C}</td></tr>\n", play.Name, perf.Audience, Convert.ToDecimal(thisAmount / 100));
                totalAmount += thisAmount;
            }

            result += "  </table>\n";
            result += string.Format(cultureInfo, "  <p>Amount owed is <em>{0:C}</em></p>\n", Convert.ToDecimal(totalAmount / 100));
            result += string.Format("  <p>You earned <em>{0}</em> credits</p>\n", volumeCredits);
            result += "</html>";
            return result;
        }
    }
}
