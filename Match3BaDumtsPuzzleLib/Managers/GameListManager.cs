using Match3BaDumtsPuzzleLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3BaDumtsPuzzleLib.Managers {
    public static class GameListManager {

        public static List<GameHistoryModel> readCsvFile(string path) {
            try {
                var lst = new List<GameHistoryModel>();
                var count = 0;

                using (var sr = new StreamReader(path, Encoding.UTF8)) {
                    while (!sr.EndOfStream) {
                        count++;
                        var str = sr.ReadLine();
                        var arr = str.Split(';');
                        lst.Add(new GameHistoryModel(count, DateTime.Parse(arr[0]), int.Parse(arr[1]), true));
                    }
                }

                return lst;
            } catch (Exception ex) {
                return null;
            }
        }

        public static void writeLine(GameHistoryModel mdl, string path) {
            File.WriteAllText(path, mdl.FormatLineForRecordsList);
        }

    }
}
