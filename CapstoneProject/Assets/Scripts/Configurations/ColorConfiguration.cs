using System;
using UnityEngine;

public static class ColorConfiguration
{
    public static TeamColorConfiguration Team1;
    public static TeamColorConfiguration Team2;

    static ColorConfiguration()
    {
        Team1 = new TeamColorConfiguration();
        Team2 = new TeamColorConfiguration();
        Team1.memberColors = new Color[8];
        Team2.memberColors = new Color[8];

        Team1.baseColor = new Color(1, 0, 0);
        Team1.memberColors[0] = new Color(1f, 0.47f, 0.47f);
        Team1.memberColors[1] = new Color(1f, 0.47f, 0.61f);
        Team1.memberColors[2] = new Color(1f, 0.56f, 0.43f);
        Team1.memberColors[3] = new Color(1f, 0.72f, 0.43f);
        Team1.memberColors[4] = new Color(1f, 0.72f, 0.43f);
        Team1.memberColors[5] = new Color(1f, 0.72f, 0.43f);
        Team1.memberColors[6] = new Color(1f, 0.72f, 0.43f);
        Team1.memberColors[7] = new Color(1f, 0.72f, 0.43f);

        Team2.baseColor = new Color(0, 0, 1);
        Team2.memberColors[0] = new Color(0.43f, 0.43f, 1f);
        Team2.memberColors[1] = new Color(0.43f, 0.55f, 1f);
        Team2.memberColors[2] = new Color(0.43f, 0.73f, 1f);
        Team2.memberColors[3] = new Color(0.43f, 0.97f, 1f);
        Team2.memberColors[4] = new Color(0.43f, 0.97f, 1f);
        Team2.memberColors[5] = new Color(0.43f, 0.97f, 1f);
        Team2.memberColors[6] = new Color(0.43f, 0.97f, 1f);
        Team2.memberColors[7] = new Color(0.43f, 0.97f, 1f);
    }
}