/*
    This program is created by Sirius on 1/3/2020
 */
using System;
using System.Text;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace mhw_4slots
{
    public class Set
    {
        private List<Slot> _Slots;

        private Skill[] _RemainingExpectation;

        public ReadOnlyCollection<Slot> Slots
        {
            get { return _Slots.AsReadOnly(); }
        }

        public Skill[] Expectation;

        private Dictionary<string, int> _Skills
        {
            get
            {
                Dictionary<string, int> skillList = new Dictionary<string, int>();
                foreach (Skill[] skills in _Slots.Where(s => s.Decoration != null).
                                            Select(s => s.Decoration).Select(d => d.Skills))
                {
                    foreach (Skill skill in skills)
                    {
                        if (skillList.Any(s => s.Key == skill.Name))
                        {
                            skillList[skill.Name] += skill.Level;
                        }
                        else
                        {
                            skillList.Add(skill.Name, skill.Level);
                        }
                    }
                }
                return skillList;
            }
        }

        public Slot[] RemainingSlots
        {
            get { return _Slots.Where(s => s.Decoration == null).ToArray(); }
        }

        public Skill[] RemainingExpectation
        {
            get
            {
                if (_RemainingExpectation == null)
                    _RemainingExpectation = Expectation;
                Skill[] remainingExpectation = JsonConvert.DeserializeObject<Skill[]>(JsonConvert.SerializeObject(_RemainingExpectation));
                foreach (KeyValuePair<string, int> skill in _Skills)
                {
                    remainingExpectation.First(s => s.Name == skill.Key).Level -= skill.Value;
                }
                return remainingExpectation.ToArray();
            }
            private set
            {
                _RemainingExpectation = value;
            }
        }
        public Set(Slot[] slots, Skill[] expectation, List<Decoration> decorations)
        {
            _Slots = new List<Slot>(slots);
            Expectation = expectation;

            RemainingExpectation = RemainingExpectation.ToList()
                                            .OrderBy(skill => decorations
                                            .First(dec => dec.Skills.Select(s => s.Name).Contains(skill.Name)
                                            )).ToArray();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, int> s in _Skills)
            {
                sb.Append(string.Format("Skill {0} Level {1}\r\n", s.Key, s.Value));
            }
            return sb.ToString();
        }

        [Obsolete]
        public string ShowSlots()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Slot slot in _Slots)
            {
                sb.Append(string.Format("Slot Level: {0}, Slot Decoration: {1} \r\n",
                                            slot.Level, slot.Decoration == null ? "null" : slot.Decoration.Name));
            }
            return sb.ToString();
        }
        public void AddDecoration(Decoration decoration)
        {
            if (_Slots.Any(s => s.Level <= decoration.Level && s.Decoration == null))
            {
                _Slots[_Slots.FindIndex(s => s.Level >= decoration.Level && s.Decoration == null)]
                    .Decoration = decoration;
            }
        }

        public int IsSkillRequired(string skillName)
        {
            if (!RemainingExpectation.Any(ex => ex.Name != skillName))
            {
                throw new Exception("Cannot ifnd Skill in the expectation");
            }
            return RemainingExpectation.First(s => s.Name == skillName).Level;
        }

    }
}