using System;
using System.Collections.Generic;

namespace VNMaker.EventBuss
{
    /// <summary>
    /// Classic observer pattern.<br></br>
    /// Has a Dictionary that uses a strings to get a list of actions that use an array of objects as parameters
    /// </summary>
    public class EventManager
    {
        // Dictionary used to store Events
        private Dictionary<string, Action<object[]>> _eventDictionary = new();

        /// <summary>
        /// Registers Action inside the Dictionary
        /// </summary>
        public void Register(string eventName, Action<object[]> action)
        {
            if (!IsEventDataValid(eventName, action))
                return;

            if (_eventDictionary.ContainsKey(eventName))
                _eventDictionary[eventName] += action;
            else
            {
                _eventDictionary.Add(eventName, null);
                _eventDictionary[eventName] += action;
            }
        }

        /// <summary>
        /// Unregisters Action inside the Dictionary
        /// </summary>
        public void Unregister(string eventName, Action<object[]> action)
        {
            if (!IsEventDataValid(eventName, action))
                return;

            if (_eventDictionary.ContainsKey(eventName))
                _eventDictionary[eventName] -= action;
        }


        /// <summary>
        /// Trigger the actions associated with the given eventName using the given parameters
        /// </summary>
        public void TriggerEvent(string eventName, params object[] parameters)
        {
            if (_eventDictionary.TryGetValue(eventName, out var action))
                action.Invoke(parameters);
        }

        /// <summary>
        /// Checks if the given parameters are not null
        /// </summary>
        /// <returns>Returns true when eventName and action are not null</returns>
        private bool IsEventDataValid(string eventName, Action<object[]> action)
        {
            return action != null && !string.IsNullOrEmpty(eventName);
        }
    }
}