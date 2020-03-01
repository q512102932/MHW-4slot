/*
    This program is created by Sirius on 1/3/2020
 */
using System;


namespace mhw_4slots
{
    public class Slot
    {
        private Decoration _Decoration;

        public int Level;
        public Decoration Decoration
        {
            get { return _Decoration; }
            set
            {
                if (value.Level <= Level)
                {
                    _Decoration = value;
                }
                else
                {
                    throw new Exception("Cannot Add Decoration Bigger Than The Slot.");
                }
            }
        }

    }
}