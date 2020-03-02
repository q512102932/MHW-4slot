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
        private static Set Set;


        static void Main(string[] args)
        {
            ReadDecoration();
            ReSetSet();
            Calculation();
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
        static void Calculation()
        {
            //Search with HH
            SearchWithCondition(Set, SetSearchCriteria.HH);
            Console.WriteLine("1. Search 4 slots with larger slot + larger slot Finished.\r\n Result:");
            Console.WriteLine(Set.ShowSlots());
            Console.WriteLine(Set.ToString());
            Console.WriteLine("This is a {0} result.", Set.ValidateSkillWithExpectation()==true?"valid":"invalid");
            ReSetSet();

            //Search with HL
            SearchWithCondition(Set, SetSearchCriteria.HL);
            Console.WriteLine("2. Search 4 slots with larger slot + smaller slot Finished.\r\n Result:");
            Console.WriteLine(Set.ShowSlots());
            Console.WriteLine(Set.ToString());
            Console.WriteLine("This is a {0} result.", Set.ValidateSkillWithExpectation()==true?"valid":"invalid");
            ReSetSet();

            //Search with LH
            SearchWithCondition(Set, SetSearchCriteria.LH);
            Console.WriteLine("3. Search 4 slots with smaller slot + larger slot Finished.\r\n Result:");
            Console.WriteLine(Set.ShowSlots());
            Console.WriteLine(Set.ToString());
            Console.WriteLine("This is a {0} result.", Set.ValidateSkillWithExpectation()==true?"valid":"invalid");
            ReSetSet();

            //Search with LL
            SearchWithCondition(Set, SetSearchCriteria.LL);
            Console.WriteLine("4. Search 4 slots with smaller slot + smaller slot Finished.\r\n Result:");
            Console.WriteLine(Set.ShowSlots());
            Console.WriteLine(Set.ToString());
            Console.WriteLine("This is a {0} result.", Set.ValidateSkillWithExpectation()==true?"valid":"invalid");
            ReSetSet();
        }

        static void SearchWithCondition(Set Set, SetSearchCriteria SetSearchCriteria)
        {
            //Sort the skill expectation in ascending order
            //Start with the lowest skills
            //Initial an attempting cache dictionary
            Dictionary<Skill, bool> remainingExpectationDic = new Dictionary<Skill, bool>();
            RefreshRemainingExpectationDictionary(remainingExpectationDic, Set.RemainingExpectation);
            while (remainingExpectationDic.Any(re => re.Key.Level > 0))
            {
                bool fourSlotsflag = false;
                RefreshRemainingExpectationDictionary(remainingExpectationDic, Set.RemainingExpectation);
                //Skill expectedSkill = Set.RemainingExpectation.Where(r => r.Level > 0).First();  
                IEnumerable<Skill> searchSkillList = new List<Skill>();
                Skill expectedSkill = new Skill();
                List<Skill> orderedRemainingSkillList = new List<Skill>();
                switch (SetSearchCriteria)
                {
                    case SetSearchCriteria.HH:
                        expectedSkill = remainingExpectationDic.OrderBy(re => re.Value)
                                                                .ThenBy(re => _DecorationList.Where(dec => dec.Level != 4)
                                                                                            .Select(dec => dec.Skills[0].Name)
                                                                                            .ToList().IndexOf(re.Key.Name))
                                                                                            .FirstOrDefault(re => re.Key.Level > 0).Key;

                        orderedRemainingSkillList = Set.RemainingExpectation
                                                        .OrderBy(skill => _DecorationList.Where(dec => dec.Level != 4)
                                                                                        .Select(dec => dec.Skills[0].Name)
                                                                                        .ToList().IndexOf(skill.Name)).ToList();
                        searchSkillList = _DecorationList.Where(dec => dec.Level < 4 &&
                                                                    Set.RemainingExpectation.Where(re => re.Level > 0)
                                                                        .Select(re => re.Name)
                                                                        .Intersect(dec.Skills.Select(s => s.Name))
                                                                        .Any()).OrderBy(skill => _DecorationList.Where(dec => dec.Level != 4)
                                                                                        .Select(dec => dec.Skills[0].Name)
                                                                                        .ToList().IndexOf(skill.Name)).Select(dec => dec.Skills[0]);
                        break;
                    case SetSearchCriteria.HL:
                        expectedSkill = remainingExpectationDic.OrderBy(re => re.Value)
                                                                .ThenBy(re => _DecorationList.Where(dec => dec.Level != 4)
                                                                                            .Select(dec => dec.Skills[0].Name)
                                                                                            .ToList().IndexOf(re.Key.Name))
                                                                                            .FirstOrDefault(re => re.Key.Level > 0).Key;
                        searchSkillList = _DecorationList.Where(dec => dec.Level < 4 &&
                                                                    Set.RemainingExpectation.Where(re => re.Level > 0)
                                                                        .Select(re => re.Name)
                                                                        .Intersect(dec.Skills.Select(s => s.Name))
                                                                        .Any()).OrderByDescending(skill => _DecorationList.Where(dec => dec.Level != 4)
                                                                                        .Select(dec => dec.Skills[0].Name)
                                                                                        .ToList().IndexOf(skill.Name)).Select(dec => dec.Skills[0]);
                        break;
                    case SetSearchCriteria.LH:
                        expectedSkill = remainingExpectationDic.OrderBy(re => re.Value)
                                                                .ThenByDescending(re => _DecorationList.Where(dec => dec.Level != 4)
                                                                                            .Select(dec => dec.Skills[0].Name)
                                                                                            .ToList().IndexOf(re.Key.Name))
                                                                                            .FirstOrDefault(re => re.Key.Level > 0).Key;
                        searchSkillList = _DecorationList.Where(dec => dec.Level < 4 &&
                                                                    Set.RemainingExpectation.Where(re => re.Level > 0)
                                                                        .Select(re => re.Name)
                                                                        .Intersect(dec.Skills.Select(s => s.Name))
                                                                        .Any()).OrderBy(skill => _DecorationList.Where(dec => dec.Level != 4)
                                                                                        .Select(dec => dec.Skills[0].Name)
                                                                                        .ToList().IndexOf(skill.Name)).Select(dec => dec.Skills[0]);
                        break;
                    case SetSearchCriteria.LL:
                        expectedSkill = remainingExpectationDic.OrderBy(re => re.Value)
                                                                .ThenByDescending(re => _DecorationList.Where(dec => dec.Level != 4)
                                                                                            .Select(dec => dec.Skills[0].Name)
                                                                                            .ToList().IndexOf(re.Key.Name))
                                                                                            .FirstOrDefault(re => re.Key.Level > 0).Key;

                        orderedRemainingSkillList = Set.RemainingExpectation
                                                        .OrderByDescending(skill => _DecorationList.Where(dec => dec.Level != 4)
                                                                                        .Select(dec => dec.Skills[0].Name)
                                                                                        .ToList().IndexOf(skill.Name)).ToList();
                        searchSkillList = _DecorationList.Where(dec => dec.Level < 4 &&
                                                                    Set.RemainingExpectation.Where(re => re.Level > 0)
                                                                        .Select(re => re.Name)
                                                                        .Intersect(dec.Skills.Select(s => s.Name))
                                                                        .Any()).OrderByDescending(skill => _DecorationList.Where(dec => dec.Level != 4)
                                                                                        .Select(dec => dec.Skills[0].Name)
                                                                                        .ToList().IndexOf(skill.Name)).Select(dec => dec.Skills[0]);
                        break;
                }
                //Find 4 slots decoration if possible
                if (Set.RemainingSlots.Any(slot => slot.Level == 4 && slot.Decoration == null))
                {
                    //Find 4 slots decoration with itself first if 2 or more skill points required
                    if (expectedSkill.Level >= 2)
                    {
                        Decoration tempDecoration = _DecorationList
                                                    .FirstOrDefault(dec => dec.Level == 4 &&
                                                                                dec.Skills.Count() == 1 &&
                                                                                dec.Skills[0].Name == expectedSkill.Name);
                        if (tempDecoration != null)
                        {
                            Set.AddDecoration(tempDecoration);
                            fourSlotsflag = true;
                            continue;
                        }
                    }

                    //Find 4 slots decoration with other skills
                    foreach (Skill subExpectedSkill in searchSkillList)
                    {
                        Decoration tempDecoration = _DecorationList
                                                .FirstOrDefault(dec => dec.Skills.Select(s => s.Name)
                                                                    .SequenceEqual(new string[] { expectedSkill.Name, subExpectedSkill.Name }) ||
                                                                dec.Skills.Select(s => s.Name).Reverse()
                                                                    .SequenceEqual(new string[] { expectedSkill.Name, subExpectedSkill.Name }));
                        if (tempDecoration != null)
                        {
                            Set.AddDecoration(tempDecoration);
                            fourSlotsflag = true;
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
                            Set.AddDecoration(tempDecoration);
                            fourSlotsflag = true;
                        }
                    }

                    if (!fourSlotsflag)
                    {
                        remainingExpectationDic[expectedSkill] = false;
                        fourSlotsflag = true;
                    }
                }
                else if (Set.RemainingSlots.Count() > 0)
                {
                    //Find with <3 slot
                    Decoration tempDecoration = _DecorationList
                                                    .FirstOrDefault(dec => dec.Skills[0].Name == expectedSkill.Name &&
                                                    dec.Level < 4);
                    if (tempDecoration != null&&Set.RemainingSlots.Any(re=>re.Level>=tempDecoration.Level))
                    {
                        Set.AddDecoration(tempDecoration);
                        fourSlotsflag = true;
                    }
                }
                if (!fourSlotsflag)
                {
                    break;
                }
            }
        }

        static void RefreshRemainingExpectationDictionary(Dictionary<Skill, bool> dictionary, Skill[] skillArray)
        {
            foreach (Skill skill in skillArray)
            {
                if (!dictionary.Any(dic => dic.Key.Name == skill.Name))
                {
                    dictionary.Add(skill, false);
                }
                else
                {
                    dictionary.Keys.First(v => v.Name == skill.Name).Level = skill.Level;
                }
            }
        }

        static void ReSetSet()
        {
            Set = new Set(
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
                }
            );
        }
    }
    enum SetSearchCriteria
    {
        HH,//Higher + Higher
        HL,//Higher + Lower
        LH,//Lower + Higher
        LL,//Lower + Lower
    }
}