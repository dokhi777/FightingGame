using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class BattleSceneManager : MonoBehaviour
{
    public Transform[] spawnPoints; // SpawnPoint �迭
    public PlayableDirector timeline; // Timeline ��Ʈ�ѷ�
    public TimelineAsset timelineAsset; // Timeline Asset

    private void Start()
    {
        // Timeline Asset�� �ʱ�ȭ
        if (timelineAsset == null)
        {
            timelineAsset = (TimelineAsset)timeline.playableAsset;
        }

        // ���õ� ĳ���͸� Timeline�� ���ε�
        for (int i = 0; i < CharacterSelection.selectedCharacters.Length; i++)
        {
            if (CharacterSelection.selectedCharacters[i] != null)
            {
                GameObject character = CharacterSelection.selectedCharacters[i];
                character.SetActive(true);

                // SpawnPoint�� �������� �ʱ� ��ġ
                character.transform.position = spawnPoints[i].position;

                // ĳ���� ȸ���� ����
                Vector3 rotationOffset = Vector3.zero;

                if (i == 0) rotationOffset = new Vector3(0, 198, 0); // ù ��° ĳ���� ȸ����
                else if (i == 1) rotationOffset = new Vector3(0, 155, 0); // �� ��° ĳ���� ȸ����
                else rotationOffset = new Vector3(0, 180, 0); // �⺻ ȸ����

                character.transform.rotation = Quaternion.Euler(rotationOffset);

                // Timeline�� Track �߰� �� ���ε�
                AddAnimationTrackToTimeline(character, $"CharacterTrack_{i}", spawnPoints[i].position, rotationOffset);
            }
        }

        // Timeline ��� 
        if (timeline.state != PlayState.Playing)
        {
            Debug.Log("Starting Timeline...");
            timeline.Play(); // Timeline ���
        }
    }

    private void AddAnimationTrackToTimeline(GameObject character, string trackName, Vector3 initialPosition, Vector3 initialRotation)
    {
        // Timeline�� Animation Track �߰�
        var animationTrack = timelineAsset.CreateTrack<AnimationTrack>(null, trackName);

        // ���õ� ĳ���͸� Animation Track�� ���ε�
        timeline.SetGenericBinding(animationTrack, character);

        // "(Clone)" ����
        string characterName = character.name.Replace("(Clone)", "").Trim();

        // �ִϸ��̼� ��� ����Ʈ
        string[] animationClips = { "Walk", "Select" };
        float currentStartTime = 0; // Ŭ�� ���� �ð�

        Dictionary<int, Vector3> characterPositions = new Dictionary<int, Vector3>();
        characterPositions[0] = initialPosition; // ù ��° ĳ������ �ʱ� ��ġ
        characterPositions[1] = initialPosition; // �� ��° ĳ������ �ʱ� ��ġ

        foreach (string animationClipName in animationClips)
        {
            // �ִϸ��̼� ��� ����
            var animationPath = $"Animations/{characterName}/{characterName}{animationClipName}";
            var animClip = Resources.Load<AnimationClip>(animationPath);

            if (animClip != null)
            {
                var playableAsset = ScriptableObject.CreateInstance<AnimationPlayableAsset>();
                playableAsset.clip = animClip;

                var timelineClip = animationTrack.CreateDefaultClip();
                timelineClip.asset = playableAsset;

                timelineClip.start = currentStartTime; // ���� �ð� ����
                if (animationClipName == "Walk")
                {
                    timelineClip.duration = animClip.length * 8;  
                }
                else if (animationClipName == "Select")
                {
                    timelineClip.duration = animClip.length * 2;  
                }

                AnimationPlayableAsset animAsset = (AnimationPlayableAsset)timelineClip.asset;

                Vector3 currentPosition = characterPositions[Array.IndexOf(CharacterSelection.selectedCharacters, character)];

                if (animationClipName == "Select")
                {
                    int characterIndex = Array.IndexOf(CharacterSelection.selectedCharacters, character);

                    // ĳ���ͺ� Select ��ġ ����
                    if (characterIndex == 0)
                    {
                        if (characterName == "Bodybuilder")
                            currentPosition = new Vector3(-4.91018629f, 0.614367485f, -1.04243553f);
                        else if (characterName == "Rosales")
                            currentPosition = new Vector3(-4.88019609f, 0.567475796f, -0.869782567f);
                        else if (characterName == "Mutant")
                            currentPosition = new Vector3(-6.76947021f, 0.576784492f, 0.523667455f);
                    }
                    else if (characterIndex == 1)
                    {
                        if (characterName == "Bodybuilder")
                            currentPosition = new Vector3(6.53208923f, 0.614367485f, -0.533147156f);
                        else if (characterName == "Rosales")
                            currentPosition = new Vector3(6.70816708f, 0.560555577f, -0.262196302f);
                        else if (characterName == "Mutant")
                            currentPosition = new Vector3(4.10421276f, 0.576784492f, -0.655801833f);
                    }
                }

                if (animationClipName == "Walk")
                {
                    if (characterName == "Bodybuilder")
                    {
                        timelineClip.timeScale = 1.35f;
                    }
                    else if (characterName == "Rosales")
                    {
                        timelineClip.timeScale = 1.05f;
                    }
                    else if (characterName == "Mutant")
                    {
                        timelineClip.timeScale = 0.9f;
                    }
                }
                animAsset.position = currentPosition;
                animAsset.rotation = Quaternion.Euler(initialRotation);
                characterPositions[Array.IndexOf(CharacterSelection.selectedCharacters, character)] = currentPosition;
                // ���� Ŭ���� ���� �ð� ������Ʈ
                currentStartTime += (float)timelineClip.duration;
            }
        }
    }
}
