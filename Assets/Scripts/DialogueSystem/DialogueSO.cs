using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// scriptable object with the data of the dialogue <br></br>
    /// uses a cvs file, separated by underscores, to populate the data<br></br>
    /// </summary>
    [CreateAssetMenu(menuName = "Custom Assets/Dialogue")]
    public class DialogueSO : ScriptableObject
    {
        public DialogueClass Dialogue;

        [ContextMenu("ResetParts")]
        public void ResetParts()
        {
            Dialogue.DialogueParts.Clear();
        }

        [ContextMenu("ElaborateTextFile")]
        public void ElaborateTextFile()
        {
            TextElaboration();
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
        /// splits all the lines of the cvs file then populates a Line struct variable
        /// </summary>
        /// <param name="listLines">Output list of parsed lines.</param>
        /// <param name="lines">Array of CSV file lines.</param>
        private void PrepareListLine(out List<Line> listLines, string[] lines)
        {
            listLines = new List<Line>();

            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split('_'); // OstName_Audio_SpeakingPG_Sentence_Character1Sprite!Animation_Character2Sprite!Animation_Character3!Animation...

                Line line = new Line();

                line.SpriteAnimMap = new();

                line.OstName = parts[(int)Field.OSTName];
                line.Audio = parts[(int)Field.SFX];
                line.SpeakingCharacter = parts[(int)Field.CharacterName];
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
        /// sends the strings inside the list listLines to be converted to the correct Data format  <br></br>
        /// then it populates a SentenceClass variable <br></br>
        /// then 
        /// </summary>
        /// <param name="listLines">List of parsed lines.</param>
        private void CreateDialogue(List<Line> listLines)
        {
            bool musicFound = false;

            for (int i = 0; i < listLines.Count; i++)
            {
                List<SentenceClass> strSentences = new List<SentenceClass>();

                Line line = listLines[i];
                string currentlySpeakingCharacter = line.SpeakingCharacter;
                //                                                     Gets sprites and animations inside resource folder  Gets audio clips inside resource folder
                SentenceClass strSentence = new SentenceClass(line.Sentence, TextToSpriteAnimationMap(line.SpriteAnimMap), TextToAudioClip(line.Audio, false));

                strSentences.Add(strSentence);

                if (!musicFound) // if background music not found
                {
                    // search for music
                    String OSTName = line.OstName;
                    if (!IsStringNull(OSTName))
                    {
                        Dialogue.DialogueMusicBackground = TextToAudioClip(OSTName, true);
                        musicFound = true;
                    }
                }

                int j = i + 1;
                while (j < listLines.Count) // used to add all the lines from the same Character 
                {
                    if (!IsStringNull(listLines[j].SpeakingCharacter) && listLines[j].SpeakingCharacter != currentlySpeakingCharacter) break;
                    strSentence = new SentenceClass(listLines[j].Sentence, TextToSpriteAnimationMap(listLines[j].SpriteAnimMap), TextToAudioClip(line.Audio, false));
                    strSentences.Add(strSentence);
                    j++;
                }

                if (j - 1 != i)
                    i = --j;

                MonologueClass monologue = new MonologueClass(currentlySpeakingCharacter, strSentences);
                Dialogue.DialogueParts.Add(monologue);
            }
        }


        /// <summary>
        /// creates a dictionary of Sprites to List[AnimationClip] so that it can be used during the dialogue to play animations on the correct character <br></br>
        /// then for each key value pair in the given spriteAnimMap it gets the sprite and animation and uses them to populate the dictionary
        /// </summary>
        /// <param name="spriteAnimMap"></param>
        /// <returns></returns>
        private SerializedDictionary<Sprite, List<AnimationClip>> TextToSpriteAnimationMap(SerializedDictionary<string, string> spriteAnimMap)
        {
            SerializedDictionary<Sprite, List<AnimationClip>> map = new();
            foreach (var spriteAnim in spriteAnimMap)
            {
                var sprite = TextToSprite(new string[] { spriteAnim.Key });
                var anims = TextToAnimation(new string[] { spriteAnim.Value });

                if (sprite.Count <= 0)
                    continue;

                for (int i = 0; i < anims.Count; i++)
                {
                    var anim = anims[i];
                    if (anim == null)
                        anims.RemoveAt(i);
                }

                map.Add(sprite[0], anims);
            }


            return map;
        }


        /// <summary>
        /// Gets the Animations inside the Resources/2D/Character Animations/
        /// </summary>
        /// <param name="strAnimations">array of string names</param>
        /// <returns>a list of animation clips</returns>
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
        /// Gets the sprites inside Resources/2D/Character Sprites/*GivenCharName* folder
        /// </summary>
        /// <param name="texts">Array of sprite paths as strings.</param>
        /// <returns>List of Sprite objects.</returns>
        private List<Sprite> TextToSprite(string[] texts)
        {
            List<Sprite> sprites = new();

            for (int i = 0; i < texts.Length; i++)
            {
                if (IsStringNull(texts[i])) continue;

                string spritePath = "2D/Character Sprites/";

                string[] separatedText = texts[i].Split('-'); // Split sprite string into character name and state.

                if (separatedText.Length < 2)
                    continue;

                spritePath += separatedText[0] + "/"; // add character's name to the path . (2D/Character Sprites/*CharacterName*/)

                spritePath += separatedText[1]; // (2D/Character Sprites/*CharacterName*/*CharacterState*)

                // Load sprite from the Resources folder.
                Sprite sprite = Resources.Load<Sprite>(spritePath); // 2D/Character Sprites/*CharacterName*/*CharacterState* (ex: 2D/Character Sprites/Pippo/Happy or 2D/Character Sprites/Gino/GinoDead)

                sprites.Add(sprite);
            }

            return sprites;
        }

        /// <summary>
        /// Represents a line parsed from the CSV file.
        /// </summary>
        private struct Line
        {
            public string OstName;
            public string Audio;
            /// <summary>
            /// dictionary with Character's sprite + Character's Animation
            /// </summary>
            public SerializedDictionary<string, string> SpriteAnimMap;
            public string SpeakingCharacter;
            public string Sentence;
        }

        /// <summary>
        /// Enum for indexing fields in the CSV file.
        /// </summary>
        private enum Field
        {
            OSTName = 0,
            SFX = 1,
            CharacterName = 2,
            Sentence = 3
        }

        private bool IsStringNull(string str)
        {
            return String.IsNullOrEmpty(str) || "aaa".Equals(str);
        }

        /// <summary>
        /// gets the audio clip in Resources/Audio/<br></br>
        /// when isOST = false the sound will be searched in the SFX folder
        /// </summary>
        private AudioClip TextToAudioClip(string fileName, bool isOst)
        {
            if (!IsStringNull(fileName))
            {
                string strAudioPath = "Audio/";
                strAudioPath += isOst ? "OST/" : "SFX/";
                strAudioPath += fileName;

                return Resources.Load(strAudioPath) as AudioClip;
            }

            return null;
        }
    }
}