using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(menuName = "Custom Assets/Dialogue")]
    public class DialogueSO : ScriptableObject
    {
        public DialogueClass Dialogue;
        public bool HasChoices = false;

        public List<ChoiceClass> Choices;

        public bool HasSceneChange = false;

        public String TargetSceneName;


        public void ResetParts()
        {
            Dialogue.DialogueParts.Clear();
            Dialogue.DialogueCSV = null;
        }

        /// <summary>
        /// Ensures that data in the Dialogue object remains consistent and initializes necessary fields.
        /// </summary>
        private void OnValidate()
        {
            // Ensure the Dialogue contains parts; initialize if empty.
            if (Dialogue.DialogueParts.Count == 0) TextElaboration();

            // Sync choices with the Dialogue object.
            Dialogue.HasChoices = HasChoices;
            Dialogue.DialogueChoices = Choices;

            // Sync the sceneChange with the Dialogue object
            Dialogue.HasSceneChange = HasSceneChange;
            Dialogue.TargetSceneName = TargetSceneName;
        }

        /// <summary>
        /// Converts text from a CSV file into Dialogue data.
        /// </summary>
        private void TextElaboration()
        {
            if (Dialogue.DialogueCSV == null) return;

            TextAsset asset = Dialogue.DialogueCSV;
            string[] rows = asset.text.Split('\n'); // split all rows 

            List<Line> listLines;

            // Parse the CSV file into a list of lines.
            PrepareListLine(out listLines, rows);

            // Create Dialogue objects from the parsed lines.
            CreateDialogue(listLines);
        }

        /// <summary>
        /// Parses lines from the CSV file into structured data.
        /// </summary>
        /// <param name="listLines">Output list of parsed lines.</param>
        /// <param name="lines">Array of CSV file lines.</param>
        private void PrepareListLine(out List<Line> listLines, string[] lines)
        {
            listLines = new List<Line>();

            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split('_');

                Line line = new Line();

                line.SpriteAnimMap = new();

                line.OstName = parts[(int)Field.OSTName];
                line.OstSecondStartingLoop = parts[(int)Field.OSTStartLoop];
                line.Audio = parts[(int)Field.SFX];
                line.SpeakingPG = parts[(int)Field.PGName];
                line.Sentence = parts[(int)Field.Sentence];
                for (int x = 5; x < parts.Length; x++)
                {
                    string[] spriteAnim = parts[x].Split('!'); // ex: Sprites!Animations
                    line.SpriteAnimMap.Add(spriteAnim[0], spriteAnim[1]);
                }

                listLines.Add(line);
            }
        }

        /// <summary>
        /// Converts parsed lines into Dialogue objects.
        /// </summary>
        /// <param name="listLines">List of parsed lines.</param>
        private void CreateDialogue(List<Line> listLines)
        {
            bool musicFound = false;

            for (int i = 0; i < listLines.Count; i++)
            {
                List<SentenceClass> strSentences = new List<SentenceClass>();

                Line line = listLines[i];
                string currentlySpeakingPg = line.SpeakingPG;

                // gets date in csv line
                SentenceClass strSentence = new SentenceClass(line.Sentence, TextToSpriteAnimationMap(line.SpriteAnimMap), TextToAudioClip(line.Audio, false));

                // adds data to the monologue
                strSentences.Add(strSentence);

                if (!musicFound) // if background music not found
                {
                    // search for music
                    String OSTName = line.OstName;
                    if (!IsStringNull(OSTName))
                    {
                        float seconds = float.Parse(line.OstSecondStartingLoop, NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("en-US"));
                        Dialogue.DialogueMusicBackground = TextToAudioClip(OSTName, true);
                        Dialogue.StartingLoopPoint = seconds;
                        musicFound = true;
                    }
                }

                int j = i + 1;
                while (j < listLines.Count) // used to add all the lines from the same Character 
                {
                    if (!IsStringNull(listLines[j].SpeakingPG) && listLines[j].SpeakingPG != currentlySpeakingPg) break;
                    strSentence = new SentenceClass(listLines[j].Sentence, TextToSpriteAnimationMap(listLines[j].SpriteAnimMap), TextToAudioClip(line.Audio, false));
                    strSentences.Add(strSentence);
                    j++;
                }

                if (j - 1 != i)
                    i = --j;

                MonologueClass monologue = new MonologueClass(currentlySpeakingPg, strSentences);
                Dialogue.DialogueParts.Add(monologue);
            }
        }


        private SerializedDictionary<Sprite, List<AnimationClip>> TextToSpriteAnimationMap(SerializedDictionary<string, string> spriteAnimMap)
        {
            SerializedDictionary<Sprite, List<AnimationClip>> map = new();
            foreach (var spriteAnim in spriteAnimMap)
            {
                var sprite = TextToSprite(new string[] { spriteAnim.Key });
                var anims = TextToAnimation(new string[] { spriteAnim.Value });
                if (sprite.Count <= 0)
                    continue;
                for (int i = 0; i< anims.Count; i++)
                {
                    var anim = anims[i];
                    if(anim == null)
                        anims.RemoveAt(i);
                }

                map.Add(sprite[0], anims);
            }


            return map;
        }


        /// <summary>
        /// Converts string to animations
        /// </summary>
        /// <param name="strAnimations"></param>
        /// <returns></returns>
        private List<AnimationClip> TextToAnimation(string[] strAnimations)
        {
            List<AnimationClip> animations = new();

            for (int i = 0; i < strAnimations.Length; i++)
            {
                if (IsStringNull(strAnimations[i])) continue;

                string animPath;

                string[] strSpriteParts = strAnimations[i].Split('-'); // Splits each animation

                foreach (string animName in strSpriteParts)
                {
                    animPath = "2D/Character Animations/";
                    animPath += animName; // 2D/Character Animations/*AnimationName*

                    AnimationClip anim = Resources.Load<AnimationClip>(animPath);
                    animations.Add(anim);
                }
            }

            return animations;
        }

        /// <summary>
        /// Converts sprite references in text form to Sprite objects.
        /// </summary>
        /// <param name="texts">Array of sprite paths as strings.</param>
        /// <returns>Array of Sprite objects.</returns>
        private List<Sprite> TextToSprite(string[] texts)
        {
            List<Sprite> sprites = new();

            for (int i = 0; i < texts.Length; i++)
            {
                if (IsStringNull(texts[i])) continue;

                string spritePath = "2D/Character Sprites/";

                string[] separatedText = texts[i].Split('-'); // Split sprite string into character name and state.
                
                spritePath += separatedText[0] + "/"; // add character's name to the path . (2D/Character Sprites/*CharacterName*/)

                spritePath += separatedText[1]; // (2D/Character Sprites/*CharacterName*/*CharacterState*)

                // Load sprite from the Resources folder.
                Sprite sprite = Resources.Load<Sprite>(spritePath); // 2D/Character Sprites/*CharacterName*/*CharacterState* (ex: 2D/Character Sprites/Pippo/Happy)

                sprites.Add(sprite);
            }

            return sprites;
        }

        // Supporting structs and enums

        /// <summary>
        /// Represents a line parsed from the CSV file.
        /// </summary>
        private struct Line
        {
            public string OstName;
            public string OstSecondStartingLoop;
            public string Audio;

            /// <summary>
            /// dictionary with PG's sprite + PG's Animation
            /// </summary>
            public SerializedDictionary<string, string> SpriteAnimMap;

            public string SpeakingPG;
            public string Sentence;
        }

        /// <summary>
        /// Enum for indexing fields in the CSV file.
        /// </summary>
        private enum Field
        {
            OSTName = 0,
            OSTStartLoop = 1,
            SFX = 2,
            PGName = 3,
            Sentence = 4
        }

        private bool IsStringNull(string str)
        {
            return String.IsNullOrEmpty(str) || "aaa".Equals(str);
        }

        /// <summary>
        /// when isOST = false the sound will be searched in the SFX folder
        /// </summary>
        private AudioClip TextToAudioClip(string fileName, bool isOst)
        {
            if (!IsStringNull(fileName))
            {
                string strAudioPath = "Audio/";
                strAudioPath += isOst ? "OST/OST_" : "SFX/SFX_";
                strAudioPath += fileName;

                return Resources.Load(strAudioPath) as AudioClip;
            }

            return null;
        }
    }
}