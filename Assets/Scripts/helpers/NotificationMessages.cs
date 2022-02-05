using System.Collections;
using System.Collections.Generic;

namespace helpers
{
    public class NotificationMessages
    {
        public static SortedDictionary<int,string> AbilityNotifications = new SortedDictionary<int,string>
        {
            {1,"{0} Can be felt."},
            {2,"{0} Hp restored."},
            {3,"{0} Mp restored."},
            {4,"{0} Cp restored."},
            {5,"You use {0}"},
            {6,"{0} Suceeded."},
            {7,"{0} Failed."},
            {8,"Missed."},
            {9,"Effect {0} Removed."},
            {10,"Not enough HP."},
            {11,"Not enough MP."},
        };
    }
}