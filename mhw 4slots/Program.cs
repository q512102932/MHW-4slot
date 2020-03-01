/*
    This program is created by Sirius on 1/3/2020
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace mhw_4slots
{
    class Program
    {
        private static List<Decoration> _DecorationList;
        static void Main(string[] args)
        {
            ReadDecoration();
            Set set = new Set(
                new Slot[]{
                    new Slot(){Level=1},
                    new Slot(){Level=1},
                    new Slot(){Level=1},
                    new Slot(){Level=2},
                    new Slot(){Level=2},
                    new Slot(){Level=4},
                    new Slot(){Level=4},
                    new Slot(){Level=4},
                    new Slot(){Level=4},
                    new Slot(){Level=4},
                    new Slot(){Level=4}
                },
                new Skill[]{
                    new Skill(){Name="攻击",Level=4},
                    new Skill(){Name="体力",Level=3},
                    new Skill(){Name="纳刀",Level=2},
                    new Skill(){Name="精灵加护",Level=1},
                    new Skill(){Name="超会心",Level=3},
                    new Skill(){Name="拔刀术【技】",Level=1},
                    new Skill(){Name="耐震",Level=1},
                    new Skill(){Name="集中",Level=1},
                    new Skill(){Name="匠",Level=1}
                },
                _DecorationList
            );
            Calculation(set);
            Console.WriteLine(set.ShowSlots());
            Console.WriteLine(set.ToString());
            Console.ReadLine();
        }

        static void ReadDecoration()
        {
            _DecorationList = new List<Decoration>();
            using (StreamReader streamReader = new StreamReader("data.csv"))
            {
                string data;
                while ((data = streamReader.ReadLine()) != null)
                {
                    string[] splitData = data.Split(',');
                    Decoration decoration = new Decoration()
                    {
                        Name = splitData[0],
                        Level = Convert.ToInt32(splitData[1])
                    };
                    if (decoration.Level == 4)
                    {
                        if (splitData[3] == "null")
                        {
                            decoration.Skills = new Skill[] {
                                new Skill() { Name = splitData[2], Level = 2 }
                            };
                        }
                        else
                        {
                            decoration.Skills = new Skill[] {
                                new Skill() { Name = splitData[2], Level = 1 },
                                new Skill() { Name = splitData[3], Level = 1 }
                            };
                        }
                    }
                    else
                    {
                        decoration.Skills = new Skill[] {
                                new Skill() { Name = splitData[2], Level = 1 }
                            };
                    }
                    _DecorationList.Add(decoration);
                }
            }
            _DecorationList = _DecorationList.OrderByDescending(dec => dec.Level).ToList();
        }
        static void Calculation(Set set)
        {
            //Sort the skill expectation in ascending order
            //Start with the lowest skills
            int remainingExpectedSkillIndex = 0;
            while (set.RemainingExpectation.Where(r => r.Level > 0).Count() > remainingExpectedSkillIndex)
            {
                Skill expectedSkill = set.RemainingExpectation.Where(r => r.Level > 0).First();
                //Find 4 slots decoration if possible
                if (set.RemainingSlots.Any(slot => slot.Level == 4 && slot.Decoration == null))
                {

                    do
                    {
                        bool fourSlotsflag = false;
                        //Find 4 slots decoration with itself first
                        {
                            Decoration tempDecoration = _DecorationList
                                                        .FirstOrDefault(dec => dec.Skills.Select(s => s.Name)
                                                                                    .SequenceEqual(new string[] { expectedSkill.Name, "null" }) &&
                                                                                    dec.Level == 4);
                            if (tempDecoration != null)
                            {
                                set.AddDecoration(tempDecoration);
                                fourSlotsflag = true;
                                if (set.RemainingExpectation.First(r => r.Name == expectedSkill.Name).Level <= 0)
                                    break;
                            }
                        }

                        //Find 4 slots decoration with other skills
                        //Search from higher level to lower level
                        foreach (Skill subExpectedSkill in
                                                    _DecorationList.Where(dec => dec.Level < 4 &&
                                                                                set.RemainingExpectation.Select(re => re.Name)
                                                                                    .Intersect(dec.Skills.Select(s => s.Name))
                                                                                    .Any()).Select(dec => dec.Skills[0]))
                        {
                            Decoration tempDecoration = _DecorationList
                                                    .FirstOrDefault(dec => dec.Skills.Select(s => s.Name)
                                                                        .SequenceEqual(new string[] { expectedSkill.Name, subExpectedSkill.Name }) ||
                                                                    dec.Skills.Select(s => s.Name).Reverse()
                                                                        .SequenceEqual(new string[] { expectedSkill.Name, subExpectedSkill.Name }));
                            if (tempDecoration != null)
                            {
                                set.AddDecoration(tempDecoration);
                                fourSlotsflag = true;
                                if (set.RemainingExpectation.First(r => r.Name == expectedSkill.Name).Level <= 0)
                                    break;
                            }
                        }
                        //If decoration cannot be found within 4 slot 
                        //Find with <3 slot
                        if (!fourSlotsflag)
                        {
                            Decoration tempDecoration = _DecorationList
                                                        .FirstOrDefault(dec => dec.Skills[0].Name == expectedSkill.Name &&
                                                        dec.Level < 4);
                            if (tempDecoration != null)
                            {
                                set.AddDecoration(tempDecoration);
                                fourSlotsflag = true;
                                if (set.RemainingExpectation.First(r => r.Name == expectedSkill.Name).Level <= 0)
                                    break;
                            }
                        }

                        if (!fourSlotsflag)
                            remainingExpectedSkillIndex++;
                        break;
                    } while (set.IsSkillRequired(expectedSkill.Name) > 0);
                }
                else
                {
                    //Find with <3 slot
                    Decoration tempDecoration = _DecorationList
                                                    .FirstOrDefault(dec => dec.Skills[0].Name == expectedSkill.Name &&
                                                    dec.Level < 4);
                    if (tempDecoration != null)
                    {
                        set.AddDecoration(tempDecoration);
                    }
                }
            }
        }
    }
}