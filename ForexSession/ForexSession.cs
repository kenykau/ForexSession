using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cAlgo.API;
using cAlgo.API.Collections;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo
{
    [Indicator(AccessRights = AccessRights.None)]
    public class ForexSession : Indicator
    {
        [Output("Asia", PlotType = PlotType.Points, LineColor = "DarkBlue", LineStyle = LineStyle.Solid, Thickness = 10)]
        public IndicatorDataSeries Japan { get; set; }
        [Output("London", PlotType = PlotType.Points, LineColor = "Pink", LineStyle = LineStyle.Solid, Thickness = 10)]
        public IndicatorDataSeries London { get; set; }
        [Output("US", PlotType = PlotType.Points, LineColor = "Yellow", LineStyle = LineStyle.Solid, Thickness = 10)]
        public IndicatorDataSeries US { get; set; }
        [Output("Sydney", PlotType = PlotType.Points, LineColor = "Green", LineStyle = LineStyle.Solid, Thickness = 10)]
        public IndicatorDataSeries Sydney { get; set; }

        record Session{
            public TimeOnly Start;
            public TimeOnly End;
        }
        Dictionary<string, Session> Sessions = new();
        Dictionary<string, double> Values = new();
        int lastIdx = -1;
        protected override void Initialize()
        {
            Sessions.Add("Japan", new(){Start = new TimeOnly(0, 0),End = new TimeOnly(9, 0)});
            Sessions.Add("London", new(){Start = new TimeOnly(7, 0),End = new TimeOnly(16, 0)});
            Sessions.Add("US", new(){Start = new TimeOnly(12, 0),End = new TimeOnly(21, 0)});
            Sessions.Add("Sydney", new(){Start = new TimeOnly(20, 0),End = new TimeOnly(5, 0)});
            Values.Add("Japan", -1);
            Values.Add("London", -3);
            Values.Add("US", -5);
            Values.Add("Sydney", -7);
            

        }

        public override void Calculate(int index)
        {
            if(lastIdx<index){
                var bar = Bars[index];
                TimeOnly time = new(bar.OpenTime.Hour, bar.OpenTime.Minute);
                foreach (var session in Sessions) {
                    bool isInSession = false;

                    // Check if the session crosses midnight
                    if (session.Value.Start > session.Value.End)
                    {
                        // Session spans across two days, so check both sides of midnight
                        isInSession = time >= session.Value.Start || time < session.Value.End;
                    }
                    else
                    {
                        // Session is within the same day
                        isInSession = time >= session.Value.Start && time < session.Value.End;
                    }

                    if (isInSession)
                    {
                        if (session.Key == "Japan") Japan[index] = Values[session.Key];
                        if (session.Key == "London") London[index] = Values[session.Key];
                        if (session.Key == "US") US[index] = Values[session.Key];
                        if (session.Key == "Sydney") Sydney[index] = Values[session.Key];

                    }
                }

                lastIdx = index;
            }
        }
    }
}