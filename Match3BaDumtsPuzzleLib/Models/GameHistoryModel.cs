using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Match3BaDumtsPuzzleLib.Models {
    public class GameHistoryModel {
    
        public int NumberGame { get; set; }
        public DateTime Date { get; set; }
        public int NumberOfPoints { get; set; }
        public bool IsWin { get; set; } = true;
        public String FormatLineForRecordsList { get; set; }

        public GameHistoryModel() { }
        public GameHistoryModel(int number, DateTime date, int numberOfPoints, bool isWin) {
            this.NumberGame = number;
            this.Date = date;
            this.NumberOfPoints = numberOfPoints;
            this.IsWin = isWin;

            this.FormatLineForRecordsList = string.Format("Игра №{0}  -  {1}  -  {2} - {3}", this.NumberGame, 
                this.Date.ToString("dd.MM.yy H:mm:ss"), 
                this.NumberOfPoints,
                this.IsWin ? "победа" : "поражение");
        }

    }
}
