using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scenarios
{
    public class Scenario
    {
        public String Name { get; internal set; }

        public String Description { get; internal set; }

        public String ThumbnailFile { get; internal set; }

        internal String ScenarioFolder { get; set; }

        public ScenarioShip Ship { get; internal set; }

        public bool Validate()
        {
            if (Ship == null)
                throw new Exception("Scenario is missing Ship data.");

            Ship.FixProblems(this);
            return true;
        }

        public class ScenarioShip
        {
            public String Name { get; internal set; }

            public StarshipTheory.ModLib.Util.TileData[] Design { get; internal set; }

            public int MaxCrewPoints { get; internal set; }

            public CrewMember[] Crew { get; internal set; }

            public ShipCargo Cargo { get; internal set; }

            //TODO: Add scenario research

            public void FixProblems(Scenario scenario)
            {
                if (String.IsNullOrEmpty(Name))
                    Name = "Default";

                if (Design == null || Design.Length == 0)
                    throw new Exception("Scenario is missing Ship Design.");

                if (MaxCrewPoints < 0)
                    MaxCrewPoints = 0;

                if (Crew == null || Crew.Length == 0)
                {
                    Crew = new CrewMember[4];
                    for(int i = 0; i < Crew.Length; i++)
                        Crew[i] = new CrewMember();
                }

                foreach(CrewMember member in Crew)
                    member.FixProblems(this);

                if (Cargo == null)
                    Cargo = new ShipCargo();

                Cargo.FixProblems(this);
            }
        }

        public class ShipCargo
        {
            public int CargoSpace { get; internal set; }
            public int Metal { get; internal set; }
            public int Gold { get; internal set; }
            public int Silicon { get; internal set; }
            public int Food { get; internal set; }
            public int Water { get; internal set; }
            public int Credits { get; internal set; }

            public int CurrentCargo
            {
                get
                {
                    return Metal + Gold + Silicon + Food + Water;
                }
            }

            public void FixProblems(ScenarioShip ship)
            {
                if(Metal + Gold + Silicon + Food + Water >= CargoSpace)
                {
                    int i = 0;
                    while(Metal + Gold + Silicon + Food + Water >= CargoSpace)
                    {
                        switch(i)
                        {
                            case 0:
                                Metal--;
                                break;
                            case 1:
                                Gold--;
                                break;
                            case 2:
                                Silicon--;
                                break;
                            case 3:
                                Food--;
                                break;
                            case 4:
                                Water--;
                                break;
                        }

                        i++;
                        if (i > 4)
                            i = 0;
                    }
                }

                if (Credits < 0)
                    Credits = 0;
            }

        }

        public class CrewMember
        {
            private static readonly Color emptyColor = new Color(0f, 0f, 0f, 0f);
            private static String[] maleNameList = null;
            private static String[] femaleNameList = null;

            public String Name { get; internal set; }

            public CrewGender Gender { get; internal set; }

            public CrewRole Role { get; internal set; }

            public int Agility { get; internal set; }
            public int Endurance { get; internal set; }
            public int Engineering { get; internal set; }
            public int Intelligence { get; internal set; }
            public int Combat { get; internal set; }

            public Color HairColor { get; internal set; }
            public Color HeadColor { get; internal set; }

            public int HairType { get; internal set; }
            public int HeadType { get; internal set; }
            public int BodyType { get; internal set; }
            public int FaceType { get; internal set; }

            public enum CrewGender : int
            {
                Random = 0,
                Male = 1,
                Female = 2
            }

            public enum CrewRole : int
            {
                Untrained = 0,
                Engineer = 1,
                Science = 2,
                Caption = 3,
                Military = 4,
                Systems = 5
            }

            public void FixProblems(ScenarioShip ship)
            {
                if (GameObject.Find("Manager").GetComponent<ManagerOptions>().namesMaleList == null || GameObject.Find("Manager").GetComponent<ManagerOptions>().namesMaleList.Count == 0)
                {
                    GameObject.Find("Manager").GetComponent<ManagerOptions>().buildGameDictionary();
                    GameObject.Find("Manager").GetComponent<ManagerOptions>().buildGameNames();
                }

                if (maleNameList == null)
                    maleNameList = GameObject.Find("Manager").GetComponent<ManagerOptions>().namesMaleList.ToArray();

                if (femaleNameList == null)
                    femaleNameList = GameObject.Find("Manager").GetComponent<ManagerOptions>().namesFemaleList.ToArray();
                

                if (Gender == CrewGender.Random)
                    Gender = (CrewGender)UnityEngine.Random.Range(1, 2);

                if(String.IsNullOrEmpty(Name))
                {
                    if (Gender == CrewGender.Male)
                        Name = maleNameList[UnityEngine.Random.Range(0, maleNameList.Length - 1)];
                    else
                        Name = femaleNameList[UnityEngine.Random.Range(0, femaleNameList.Length - 1)];
                }

                if(Agility + Endurance + Engineering + Intelligence + Combat == 0)
                {
                    Agility = Endurance = Engineering = Intelligence = Combat = 1;
                    int pointsToDistribute = ship.MaxCrewPoints;
                    while(pointsToDistribute > 0)
                    {
                        int skill = UnityEngine.Random.Range(0, 5);

                        if (skill == 0 && Agility < 3)
                        {
                            Agility++;
                            pointsToDistribute--;
                        }
                        else if (skill == 1 && Endurance < 3)
                        {
                            Endurance++;
                            pointsToDistribute--;
                        }
                        else if (skill == 2 && Engineering < 3)
                        {
                            Engineering++;
                            pointsToDistribute--;
                        }
                        else if (skill == 3 && Intelligence < 3)
                        {
                            Intelligence++;
                            pointsToDistribute--;
                        }
                        else if (skill == 4 && Combat < 3)
                        {
                            Combat++;
                            pointsToDistribute--;
                        }
                    }
                }

                Agility = FixPoints(Agility);
                Endurance = FixPoints(Endurance);
                Engineering = FixPoints(Engineering);
                Intelligence = FixPoints(Intelligence);
                Combat = FixPoints(Combat);

                if (HairColor == null || HairColor == emptyColor)
                    HairColor = GameObject.Find("TileMap").GetComponent<Crew>().returnRandomHairColor();

                if (HeadColor == null || HeadColor == emptyColor)
                    HeadColor = GameObject.Find("TileMap").GetComponent<Crew>().returnRandomHeadColor();

                if (HairType < 1 || HairType > 32)
                    HairType = UnityEngine.Random.Range(1, 32);

                if (HeadType < 1 || HeadType > 32)
                    HeadType = UnityEngine.Random.Range(1, 32);

                if (BodyType < 1 || BodyType > 32)
                    BodyType = UnityEngine.Random.Range(2, 32);

                if (FaceType < 1 || FaceType > 32)
                    FaceType = UnityEngine.Random.Range(17, 25);
            }

            private int FixPoints(int skill)
            {
                if (skill < 1 || skill > 4)
                    skill = UnityEngine.Random.Range(1, 4);

                return skill;
            }
        }
    }
}
